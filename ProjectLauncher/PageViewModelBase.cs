using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UE4Launcher.Root;

namespace UE4Launcher
{
    abstract class PageViewModelBase : NotificationObject
    {
        public MainWindowViewModel Owner { get; }

        public bool DeveloperMode => this.Owner.DeveloperMode;
        public bool EditMode => this.Owner.EditMode;


        protected PageViewModelBase(MainWindowViewModel owner)
        {
            this.Owner = owner;
        }
    }
}
