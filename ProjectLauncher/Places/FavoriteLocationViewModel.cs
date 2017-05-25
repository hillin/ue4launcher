using UE4Launcher.Root;
using IOPath = System.IO.Path;

namespace UE4Launcher.Places
{
	internal class FavoriteLocationViewModel : LocationViewModelBase, ITrayContextMenuItem
    {
        private bool _isPublic;

        public bool IsPublic
        {
            get => _isPublic;
	        set
            {
                _isPublic = value;
                this.RaisePropertyChanged(nameof(this.IsPublic));
            }
        }

        public string RelativePath
        {
            get => this.Location.RelativePath;
	        set => this.Location.RelativePath = value;
        }

        public override string DisplayName
        {
            get => this.Location.DisplayName;

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
            this.Path = IOPath.Combine(rootPath, location.RelativePath).Replace(IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar);
        }

        public void TogglePublicity()
        {
            this.IsPublic = !this.IsPublic;
        }
    }
}
