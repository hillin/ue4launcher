using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UE4Launcher.Places
{
    abstract class LocationViewModelBase : NotificationObject
    {
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
        
    }
}
