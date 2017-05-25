using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UE4Launcher.Debugging
{
	internal class AutomaticDebuggerInfo : IDebuggerInfo
	{

		public static readonly AutomaticDebuggerInfo Instance =
			new AutomaticDebuggerInfo(Path.Combine(App.CurrentRootPath, "UE4.sln"));

		private readonly string _solutionFileName;
		public string DisplayName => $"Automatic ({this.Debugger?.DisplayName ?? "not found"})";
		public string Description => this.Debugger?.SolutionFileName ?? "Visual Studio instance not found";

		private DebuggerInfo Debugger => DebuggerInfo.PickDebugger(_solutionFileName);

		private AutomaticDebuggerInfo(string solutionFileName)
		{
			_solutionFileName = solutionFileName;
		}

		public bool AttachProcess(Process process)
		{
			return this.Debugger.AttachProcess(process);
		}
	}
}
