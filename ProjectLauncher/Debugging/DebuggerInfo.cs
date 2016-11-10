using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProjectLauncher.Debugging
{
    class DebuggerInfo
    {
        public static readonly DebuggerInfo[] Debuggers;

        static DebuggerInfo()
        {
            Debuggers = new[]
            {
                new DebuggerInfo(VisualStudioVersions.Automatic, "Automaticlly Choose Version", true),
                new DebuggerInfo(VisualStudioVersions.VS2015, "Visual Studio 2015"),
                new DebuggerInfo(VisualStudioVersions.VS2013, "Visual Studio 2013"),
                new DebuggerInfo(VisualStudioVersions.VS2012, "Visual Studio 2012"),
                new DebuggerInfo(VisualStudioVersions.VS2010, "Visual Studio 2010")
            };
        }

        public VisualStudioVersions VSVersion { get; }
        public string Name { get; }
        public bool IsAvailable { get; }

        public DebuggerInfo(VisualStudioVersions vsVersion, string name, bool alwaysAvailable = false)
        {
            this.VSVersion = vsVersion;
            this.Name = name;
            this.IsAvailable = alwaysAvailable || DebuggerLauncher.GetDebuggerAvailability(vsVersion);
        }

        public void AttachProcess(Process process)
        {
            if (DebuggerLauncher.Attach(process, this.VSVersion))
                App.ReportStatus("Debugger attached successfully.");
            else
                App.ReportStatus("Failed to attach a debugger.");
        }

    }
}
