using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using UE4Launcher.Root;

namespace UE4Launcher.Utilities
{
	abstract class LaunchProcessMenuItemBase : NotificationObject, ITrayContextMenuItem
	{
		public abstract string Name { get; }

		public abstract string Description { get; }
		public abstract string Path { get; }

		public ImageSource Icon => Helpers.GetFileSystemIcon(this.Path, true);
		public bool IsEnabled => true;
		public ICommand Command { get; }

		protected LaunchProcessMenuItemBase()
		{
			this.Command = new SimpleCommand(this.Execute);
		}

		protected virtual void Execute(object obj)
		{
			var startInfo = this.CreateProcessStartInfo();
			Process.Start(startInfo);
		}

		protected virtual ProcessStartInfo CreateProcessStartInfo()
		{
			return new ProcessStartInfo(this.Path)
			{
				WorkingDirectory = App.CurrentRootPath,
			};
		}
	}
}
