using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UE4Launcher.Debugging
{
	internal interface IDebuggerInfo
	{
		string DisplayName { get; }
		string Description { get; }
		bool AttachProcess(Process process);
	}
}
