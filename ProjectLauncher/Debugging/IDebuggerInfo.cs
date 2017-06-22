using System.Diagnostics;

namespace UE4Launcher.Debugging
{
	internal interface IDebuggerInfo
	{
		string DisplayName { get; }
		string Description { get; }
		bool AttachProcess(Process process);
	}
}
