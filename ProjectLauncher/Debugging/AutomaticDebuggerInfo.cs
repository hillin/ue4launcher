using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace UE4Launcher.Debugging
{
	internal class AutomaticDebuggerInfo : IDebuggerInfo
	{

		public static readonly AutomaticDebuggerInfo Instance =
			new AutomaticDebuggerInfo(Path.Combine(App.CurrentRootPath, "UE4.sln"));

		private readonly string _solutionFileName;
		public string DisplayName => $"Automatic ({this.Debugger?.DisplayName ?? "not found"})";
		public string Description => this.Debugger?.SolutionFileName ?? "Visual Studio instance not found";

		private DebuggerInfo Debugger => DebuggerInfo.PickDebugger(_solutionFileName, true);

		private AutomaticDebuggerInfo(string solutionFileName)
		{
			_solutionFileName = solutionFileName;
		}

		public bool AttachProcess(Process process)
		{
			var debugger = this.Debugger;

			if (debugger == null)
			{
				var result = MessageBox.Show(
					"No debugger instance was found, would you like to try to open UE4.sln and attach the debugger?", "Debug",
					MessageBoxButton.YesNo, MessageBoxImage.Question);

				return result != MessageBoxResult.No && this.LaunchDebuggerAndAttachProcess(process);
			}

			return debugger.AttachProcess(process);
		}

		private bool LaunchDebuggerAndAttachProcess(Process process)
		{
			var debuggerProcess = Process.Start(_solutionFileName);

			if (!SpinWait.SpinUntil(() => debuggerProcess == null || this.Debugger != null, 30000) || debuggerProcess == null)
			{
				MessageBox.Show(
					"Failed to launch Visual Studio, timed out after 30 seconds. You can try to launch a Visual Studio instance manually and attach again.", "Debug",
					MessageBoxButton.OK, MessageBoxImage.Exclamation);

				return false;
			}

			return this.Debugger.AttachProcess(process);
		}
	}
}
