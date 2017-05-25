using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using UE4Launcher.Root;

namespace UE4Launcher.Places
{
	internal abstract class LocationViewModelBase : NotificationObject, ITrayContextMenuItem
    {

        private string _displayName;

        public virtual string DisplayName
        {
            get => _displayName;
	        set
            {
                _displayName = value;
                this.RaisePropertyChanged(nameof(this.DisplayName));
            }
        }

        public string Path { get; protected set; }


        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
	        set
            {
                _isSelected = value;
                this.RaisePropertyChanged(nameof(this.IsSelected));
            }
        }

        string ITrayContextMenuItem.Name => this.DisplayName;

        public ImageSource Icon => Utilities.GetFileSystemIcon(this.Path, false);

        bool ITrayContextMenuItem.IsEnabled => true;

        ImageSource ITrayContextMenuItem.Icon => Utilities.GetFileSystemIcon(this.Path, true);

        private readonly ICommand _trayContextMenuCommand;
        ICommand ITrayContextMenuItem.Command => _trayContextMenuCommand;

        string ITrayContextMenuItem.Description
            => $"{this.Path}\nClick to reveal in Explorer, Ctrl-click to open directly";

        protected LocationViewModelBase()
        {
            _trayContextMenuCommand = new SimpleCommand(this.ExecuteTrayContextMenuCommand);
        }

        private void ExecuteTrayContextMenuCommand(object obj)
        {
            this.Navigate(Utilities.IsCtrlDown);
        }

        public void Navigate(bool openDirectly)
        {
            if (openDirectly)
                Process.Start(this.Path);
            else
                Utilities.NavigateFile(this.Path);
        }


    }
}
