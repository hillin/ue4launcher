using System.Collections.ObjectModel;
using System.IO;
using UE4Launcher.Root;

namespace UE4Launcher.Utilities
{
	internal class UtilitiesViewModel : NotificationObject
	{
		public  ObservableCollection<ITrayContextMenuItem> MenuItems { get; }

		public UtilitiesViewModel()
		{
			this.MenuItems = new ObservableCollection<ITrayContextMenuItem>();
			this.AddLaunchProcessUtility<ConsoleCommandViewModel>();
			this.AddLaunchProcessUtility<UnrealFrontendViewModel>();
			this.AddLaunchProcessUtility<SwarmAgentViewModel>();
		}

		private void AddLaunchProcessUtility<T>()
			where T : LaunchProcessMenuItemBase, new()
		{
			var utility = new T();
			if (File.Exists(utility.Path))
			{
				this.MenuItems.Add(utility);
			}
		}
	}
}
