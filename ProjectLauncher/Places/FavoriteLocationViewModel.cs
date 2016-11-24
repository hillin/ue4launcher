using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOPath = System.IO.Path;

namespace UE4Launcher.Places
{
    class FavoriteLocationViewModel : LocationViewModelBase
    {
        private bool _isPublic;

        public bool IsPublic
        {
            get { return _isPublic; }
            set
            {
                _isPublic = value;
                this.RaisePropertyChanged(nameof(this.IsPublic));
            }
        }

        public string RelativePath
        {
            get { return this.Location.RelativePath; }
            set { this.Location.RelativePath = value; }
        }

        public override string DisplayName
        {
            get { return this.Location.DisplayName; }

            set
            {
                this.Location.DisplayName = value;
                base.DisplayName = value;
            }
        }

        public Location Location { get; }
        public FavoriteLocationViewModel(Location location, string rootPath, bool isPublic)
        {
            this.Location = location;
            this.IsPublic = isPublic;
            this.Path = IOPath.Combine(rootPath, location.RelativePath);
        }

        public void TogglePublicity()
        {
            this.IsPublic = !this.IsPublic;
        }
    }
}
