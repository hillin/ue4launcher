using System.Collections.Generic;

namespace UE4Launcher.Debugging
{
	interface IDebuggerSupportedViewModel
	{
		ICollection<IDebuggerInfo> Debuggers { get; }
		IDebuggerInfo SelectedDebugger { set; }
	}

	internal static class DebuggerSupportedViewModelExtensions
	{
		public static void RefreshDebuggers(this IDebuggerSupportedViewModel @this)
		{
			@this.Debuggers.Clear();
			DebuggerInfo.RefreshInstances();
			@this.Debuggers.Add(AutomaticDebuggerInfo.Instance);
			foreach (var debugger in DebuggerInfo.Instances)
			{
				@this.Debuggers.Add(debugger);
			}

			@this.SelectedDebugger = AutomaticDebuggerInfo.Instance;
		}
	}
}
