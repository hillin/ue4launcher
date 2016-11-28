using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;
using UE4Launcher.Launcher;
using UE4Launcher.Root;
using Timer = System.Timers.Timer;

namespace UE4Launcher.Places
{
    partial class PlacesViewModel : PageViewModelBase
    {
        public ObservableCollection<LocationViewModelBase> Locations { get; }
        public ObservableCollection<FavoriteLocationViewModel> Favorites { get; }

        private LocationViewModelBase _selectedLocation;


        private string _publicPlacesStorage;
        private string _personalPlacesStorage;

        public LocationViewModelBase SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                if (_selectedLocation == value)
                    return;

                if (_selectedLocation != null)
                    _selectedLocation.IsSelected = false;

                _selectedLocation = value;

                if (_selectedLocation != null)
                    _selectedLocation.IsSelected = true;

                this.RaisePropertyChanged(nameof(this.SelectedLocation));
            }
        }


        public PlacesViewModel(MainWindowViewModel owner)
            : base(owner)
        {
            this.Locations = new ObservableCollection<LocationViewModelBase>();
            this.Favorites = new ObservableCollection<FavoriteLocationViewModel>();
            this.LoadFavorites();
            this.ShowFavorites();

            _searchResults = new SortedSet<SearchResultViewModelBase>(SearchResultViewModelBase.Comparer);

            _searchInitiationDelayTimer = new Timer(500);
            _searchInitiationDelayTimer.Elapsed += this.SearchInitiationDelayTimerElapsed;
            _searchResultLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            this.InitializeSearch();
        }

        private void ShowFavorites()
        {
            this.Locations.Clear();
            foreach (var favorite in this.Favorites)
                this.Locations.Add(favorite);
        }

        private void LoadFavorites()
        {
            _publicPlacesStorage = Path.Combine(App.CurrentRootPath, Constants.PublicPlacesFilename);
            _personalPlacesStorage = Path.Combine(App.CurrentRootPath, Constants.PersonalPlacesFilename);

            if (File.Exists(_publicPlacesStorage))
                this.LoadFavorites(_publicPlacesStorage, true);

            if (File.Exists(_personalPlacesStorage))
                this.LoadFavorites(_personalPlacesStorage, false);
        }

        private void LoadFavorites(string filename, bool isPublic)
        {
            var rootPath = App.CurrentRootPath;
            using (var file = File.OpenRead(filename))
            {
                foreach (var location in (Location[])new XmlSerializer(typeof(Location[])).Deserialize(file))
                {
                    this.Favorites.Add(new FavoriteLocationViewModel(location, rootPath, isPublic));
                }
            }
        }

        public void RemoveSelectedFavorite()
        {
            var favorite = this.SelectedLocation as FavoriteLocationViewModel;
            if (favorite != null)
            {
                this.Favorites.Remove(favorite);
                this.Locations.Remove(favorite);
                this.SaveFavorites();
            }
        }

        public void SaveFavorites()
        {
            this.SaveFavorites(_personalPlacesStorage, false);
            this.SaveFavorites(_publicPlacesStorage, true);

            App.ReportStatus("Favorite places saved.");
        }

        private void SaveFavorites(string filename, bool isPublic)
        {
            var profiles = this.Favorites.Where(p => p.IsPublic == isPublic)
                               .Select(p => p.Location)
                               .ToArray();
            if (profiles.Length > 0)
            {
                using (var file = File.Create(filename))
                {
                    new XmlSerializer(typeof(Location[]))
                        .Serialize(file, profiles);
                }
            }
            else if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        public void TogglePublicity(FavoriteLocationViewModel favorite)
        {
            if (!this.EditMode)
                return;

            favorite.TogglePublicity();
        }
    }
}
