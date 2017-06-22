using Microsoft.Win32;

namespace UE4Launcher.Utilities
{
	internal class ConsoleCommandViewModel : LaunchProcessMenuItemBase
	{
		public override string Name => _usePowershell ? "Powershell" : "Command Prompt";

		public override string Description => _usePowershell
			? "Open Powershell at project folder"
			: "Open Command Prompt at project folder";

		public override string Path => Helpers.WhereSearch(_usePowershell ? "powershell.exe" : "cmd.exe");
		

		private readonly bool _usePowershell;

		public ConsoleCommandViewModel()
		{
			var subKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
			if (subKey == null)
			{
				_usePowershell = false;
			}
			else
			{
				_usePowershell = (int)subKey.GetValue("DontUsePowerShellOnWinX", 1) != 1;
			}
			
		}
	}
}
