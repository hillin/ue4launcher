using IOPath = System.IO.Path;

namespace UE4Launcher.Utilities
{
	internal class SwarmAgentViewModel : LaunchProcessMenuItemBase
	{
		public override string Name => "Swarm Agent";
		public override string Description => "Launch Swarm Agent";
		public override string Path => IOPath.Combine(App.CurrentRootPath, @"Engine\Binaries\DotNET\SwarmAgent.exe");
	}
}
