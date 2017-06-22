using IOPath = System.IO.Path;

namespace UE4Launcher.Utilities
{
	internal class UnrealFrontendViewModel : LaunchProcessMenuItemBase
	{
		public override string Name => "Unreal Frontend";
		public override string Description => "Launch Unreal Frontend";
		public override string Path => IOPath.Combine(App.CurrentRootPath, @"Engine\Binaries\Win64\UnrealFrontend.exe");
	}
}
