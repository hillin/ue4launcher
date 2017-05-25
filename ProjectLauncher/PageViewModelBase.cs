using UE4Launcher.Root;

namespace UE4Launcher
{
	internal abstract class PageViewModelBase : NotificationObject
    {
        public MainWindowViewModel Owner { get; }
		
        public bool EditMode => this.Owner.EditMode;


        protected PageViewModelBase(MainWindowViewModel owner)
        {
            this.Owner = owner;
        }
    }
}
