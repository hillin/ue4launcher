using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UE4Launcher.Root;

namespace UE4Launcher.Places
{
    abstract class LocationViewModelBase : NotificationObject, ITrayContextMenuItem
    {

        private static ImageSource GetIcon(string path, bool bSmall)
        {
            var info = new Interop.SHFILEINFO(true);
            var cbFileInfo = Marshal.SizeOf(info);

            var flags = Interop.SHGFI.Icon
                        | Interop.SHGFI.UseFileAttributes
                        | (bSmall ? Interop.SHGFI.SmallIcon : Interop.SHGFI.LargeIcon);

            var attribute = Directory.Exists(path)
                ? Interop.FileAttributes.Directory
                : Interop.FileAttributes.File;
            try
            {
                Interop.SHGetFileInfo(path, attribute, out info, (uint) cbFileInfo, flags);


                var icon = Imaging.CreateBitmapSourceFromHIcon(info.hIcon,
                                                               Int32Rect.Empty,
                                                               BitmapSizeOptions.FromEmptyOptions());
                Interop.DestroyIcon(info.hIcon);
                return icon;
            }
            catch
            {
                return null;
            }
        }

        private string _displayName;

        public virtual string DisplayName
        {
            get { return _displayName; }
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
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged(nameof(this.IsSelected));
            }
        }

        string ITrayContextMenuItem.Name => this.DisplayName;

        public ImageSource Icon => LocationViewModelBase.GetIcon(this.Path, false);

        bool ITrayContextMenuItem.IsEnabled => true;

        ImageSource ITrayContextMenuItem.Icon => LocationViewModelBase.GetIcon(this.Path, true);

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
