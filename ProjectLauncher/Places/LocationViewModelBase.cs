using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UE4Launcher.Places
{
    abstract class LocationViewModelBase : NotificationObject
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
        private bool _isSelected;

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

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged(nameof(this.IsSelected));
            }
        }

        public ImageSource Icon => LocationViewModelBase.GetIcon(this.Path, false);

        public void Navigate(bool openDirectly)
        {
            if (openDirectly)
                Process.Start(this.Path);
            else
                Utilities.NavigateFile(this.Path);
        }


    }
}
