using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
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
        private bool _searchResultInvalidated;

        public bool IsFileIndicesReady
        {
            get { return _isFileIndicesReady; }
            private set
            {
                _isFileIndicesReady = value;
                this.RaisePropertyChanged(nameof(this.IsFileIndicesReady));
            }
        }


        public bool IsSearchResultPartiallyShown
            => this.IsSearching && !this.IsAllSearchResultShown && this.SearchResultCount > 20;

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


        private double _searchProgress;

        public double SearchProgress
        {
            get { return _searchProgress; }
            private set
            {
                _searchProgress = value;
                this.RaisePropertyChanged(nameof(this.SearchProgress));
                this.RaisePropertyChanged(nameof(this.ShouldShowSearchProgress));
            }
        }

        public bool ShouldShowSearchProgress => this.IsSearching && this.SearchProgress < 1;

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
                try
                {
                    using (var indexFile = File.OpenRead(cacheFilePath))
                        _fileIndices = (List<string>) new XmlSerializer(typeof(List<string>)).Deserialize(indexFile);

                    this.IsFileIndicesReady = true;
                }
                catch
                {
                    this.IsFileIndicesReady = false;
                }
            }
            
            if(!this.IsFileIndicesReady)
            { 
                _fileIndices = new List<string>();
                Task.Factory.StartNew(this.BuildFileIndices);
            }
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

            if (!_searchResultInvalidated)
                return;

            _searchResultLock.EnterReadLock();

            this.UpdateSearchResultDisplay(this.IsAllSearchResultShown
                                               ? _searchResults
                                               : _searchResults.Take(20));


            this.SearchResultCount = _searchResults.Count;
            _searchResultLock.ExitReadLock();

            _searchResultInvalidated = false;
        }

        private void UpdateSearchResultDisplay(IEnumerable<SearchResultViewModelBase> items)
        {
            var itemsArray = items.ToArray();

            this.Locations.Clear();
            for (var i = 0; i < itemsArray.Length; ++i)
            {
                var item = itemsArray[i];
                var currentIndex = this.Locations.IndexOf(item);

                if (currentIndex < 0)
                    this.Locations.Insert(i, item);
                else if (currentIndex >= 0 && currentIndex != i)
                    this.Locations.Move(currentIndex, i);
            }

            for (var i = this.Locations.Count - 1; i >= itemsArray.Length; --i)
                this.Locations.RemoveAt(i);

            this.SelectedLocation = this.Locations.FirstOrDefault();
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
            this.SearchProgress = 0;
            Parallel.ForEach(_fileIndices, (file, loop) =>
                             {
                                 if (cancellation.IsCancellationRequested)
                                 {
                                     loop.Break();
                                     return;
                                 }

                                 Interlocked.Increment(ref fileCount);

                                 if (fileCount % 1000 == 0)
                                 {
                                     this.SearchProgress = (double)fileCount / _fileIndices.Count;
                                     App.ReportStatus($"Searching ({this.SearchProgress:P0})", 100);
                                 }

                                 var relevancy = FileSearchResultViewModel.CalculateRelevancyRating(file, keyword);
                                 if (relevancy > 0)
                                 {
                                     _searchResultLock.EnterWriteLock();
                                     _searchResults.Add(new FileSearchResultViewModel(file, relevancy));
                                     _searchResultLock.ExitWriteLock();
                                     _searchResultInvalidated = true;
                                 }
                             });

            this.SearchResultCount = _searchResults.Count;
            this.SearchProgress = 1;
            App.ReportStatus($"Search complete, {this.SearchResultCount} result(s) found");
        }

        public void ShowAllSearchResult()
        {
            if (this.SearchResultCount > 10000)
            {
                if (
                    MessageBox.Show(
                        $"You are about to show {this.SearchResultCount} search results, which will be very slow. Would you like to continue?",
                        "Show All", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }
            this.IsAllSearchResultShown = true;
        }

        public void SelectPreviousLocation()
        {
            this.MoveSelectedLocation(-1);
        }

        private void MoveSelectedLocation(int i)
        {
            if (this.SelectedLocation == null)
            {
                this.SelectedLocation = i > 0 ? this.Locations.FirstOrDefault() : this.Locations.LastOrDefault();
                return;
            }
            var index = this.Locations.IndexOf(this.SelectedLocation) + i;
            index = (index + this.Locations.Count) % this.Locations.Count;
            this.SelectedLocation = this.Locations[index];
        }

        public void SelectNextLocation()
        {
            this.MoveSelectedLocation(1);
        }

        public void NavigateToSelectedLocation(bool openDirectly)
        {
            this.SelectedLocation?.Navigate(openDirectly);
        }

        public void AddToFavorite(FileSearchResultViewModel searchResult)
        {
            var location = new Location
            {
                DisplayName = searchResult.DisplayName,
                RelativePath = searchResult.Path.Substring(App.CurrentRootPath.Length + 1)
            };

            this.Favorites.Add(new FavoriteLocationViewModel(location, App.CurrentRootPath, false));

            this.SaveFavorites();
        }
    }
}
