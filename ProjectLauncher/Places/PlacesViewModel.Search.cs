using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using System.Xml.Serialization;
using Timer = System.Timers.Timer;

namespace UE4Launcher.Places
{
    partial class PlacesViewModel
    {
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                var wasSearching = this.IsSearching;
                _searchText = value;

                if (wasSearching && !this.IsSearching)
                    this.ShowFavorites();

                if (this.IsSearching)
                {
                    _searchInitiationDelayTimer.Stop();
                    _searchInitiationDelayTimer.Start();
                }

                this.RaisePropertyChanged(nameof(this.SearchText));
                this.RaisePropertyChanged(nameof(this.IsSearching));
                this.RaisePropertyChanged(nameof(this.IsSearchResultPartiallyShown));   
            }
        }

        
        public bool IsSearching => !string.IsNullOrWhiteSpace(this.SearchText);

        private readonly Timer _searchInitiationDelayTimer;
        private CancellationTokenSource _searchTaskCancellationTokenSource;
        private readonly SortedSet<SearchResultViewModelBase> _searchResults;
        private readonly ReaderWriterLockSlim _searchResultLock;


        private List<string> _fileIndices;
        private bool _isFileIndicesReady;

        public bool IsFileIndicesReady
        {
            get { return _isFileIndicesReady; }
            private set
            {
                _isFileIndicesReady = value;
                this.RaisePropertyChanged(nameof(this.IsFileIndicesReady));
            }
        }


        public bool IsSearchResultPartiallyShown => this.IsSearching && !this.IsAllSearchResultShown && this.SearchResultCount > 20;

        private int _searchResultCount;

        public int SearchResultCount
        {
            get { return _searchResultCount; }
            private set
            {
                _searchResultCount = value;
                this.RaisePropertyChanged(nameof(this.SearchResultCount));
                this.RaisePropertyChanged(nameof(this.IsSearchResultPartiallyShown));
            }
        }

        private bool _isAllSearchResultShown;
        public bool IsAllSearchResultShown
        {
            get { return _isAllSearchResultShown; }
            private set
            {
                _isAllSearchResultShown = value;
                this.RaisePropertyChanged(nameof(this.IsAllSearchResultShown));
                this.RaisePropertyChanged(nameof(this.IsSearchResultPartiallyShown));
            }
        }

        private void InitializeSearch()
        {
            var updateSearchResultTimer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            updateSearchResultTimer.Tick += this.UpdateSearchResultTimer_Tick;
            updateSearchResultTimer.Start();

            this.PrepareFileIndices();
        }

        private void PrepareFileIndices()
        {
            var cacheFilePath = Path.Combine(App.CurrentRootPath, Constants.FileIndicesFile);
            if (File.Exists(cacheFilePath))
            {
                using (var indexFile = File.OpenRead(cacheFilePath))
                    _fileIndices = (List<string>)new XmlSerializer(typeof(List<string>)).Deserialize(indexFile);

                this.IsFileIndicesReady = true;
            }
            else
            {
                _fileIndices = new List<string>();
                this.IsFileIndicesReady = false;
            }

            Task.Factory.StartNew(this.BuildFileIndices);
        }

        private void BuildFileIndices()
        {
            var files = new List<string>();
            this.BuildFileIndicesRecursive(App.CurrentRootPath, files);
            this.IsFileIndicesReady = true;
            _fileIndices = files;

            using (var cacheFile = File.Create(Path.Combine(App.CurrentRootPath, Constants.FileIndicesFile)))
                new XmlSerializer(_fileIndices.GetType()).Serialize(cacheFile, _fileIndices);
        }

        private void BuildFileIndicesRecursive(string path, List<string> files)
        {
            foreach (var file in Directory.EnumerateFileSystemEntries(path))
            {
                var fileName = Path.GetFileName(file);
                if (fileName == null)
                    continue;

                if (fileName.StartsWith("."))
                    continue;

                files.Add(file);
                if (Directory.Exists(file))
                    this.BuildFileIndicesRecursive(file, files);
            }
        }

        private void UpdateSearchResultTimer_Tick(object sender, EventArgs e)
        {
            if (!this.IsSearching)
                return;

            this.Locations.Clear();
            _searchResultLock.EnterReadLock();

            foreach (var element in this.IsAllSearchResultShown ? _searchResults : _searchResults.Take(20))
                this.Locations.Add(element);

            this.SearchResultCount = _searchResults.Count;
            _searchResultLock.ExitReadLock();
        }

        private void SearchInitiationDelayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _searchInitiationDelayTimer.Stop();
            this.BeginSearch();
        }

        private void BeginSearch()
        {
            _searchTaskCancellationTokenSource?.Cancel();

            this.IsAllSearchResultShown = false;
            _searchTaskCancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => this.BeginSearchTask(_searchTaskCancellationTokenSource),
                                  _searchTaskCancellationTokenSource.Token);
        }

        private void BeginSearchTask(CancellationTokenSource cancellation)
        {
            if (cancellation.IsCancellationRequested)
                return;

            _searchResultLock.EnterWriteLock();
            _searchResults.Clear();
            _searchResultLock.ExitWriteLock();

            var fileCount = 0;
            var keyword = this.SearchText.Trim();
            foreach (var file in _fileIndices)
            {
                if (cancellation.IsCancellationRequested)
                    return;

                ++fileCount;

                if (fileCount % 100 == 0)
                    App.ReportStatus($"{fileCount} files and folders searched ({(double)fileCount / _fileIndices.Count:P})", 100);

                var relevancy = FileSearchResultViewModel.CalculateRelevancyRating(file, keyword);
                if (relevancy > 0)
                {
                    _searchResultLock.EnterWriteLock();
                    _searchResults.Add(new FileSearchResultViewModel(file, relevancy));
                    _searchResultLock.ExitWriteLock();
                }
            }
        }

        public void ShowAllSearchResult()
        {
            this.IsAllSearchResultShown = true;
        }
    }
}
