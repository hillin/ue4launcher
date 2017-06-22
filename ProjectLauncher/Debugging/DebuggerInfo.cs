using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace UE4Launcher.Debugging
{
	internal class DebuggerInfo : IDebuggerInfo
	{
		[DllImport("ole32.dll")]
		private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

		public static DebuggerInfo[] Instances { get; private set; }

		public static void RefreshInstances()
		{
			var instances = new List<DebuggerInfo>();
			var retVal = DebuggerInfo.GetRunningObjectTable(0, out IRunningObjectTable rot);

			if (retVal == 0)
			{
				rot.EnumRunning(out IEnumMoniker enumMoniker);

				var fetched = IntPtr.Zero;
				var monikers = new IMoniker[1];
				while (enumMoniker.Next(1, monikers, fetched) == 0)
				{
					var moniker = monikers[0];
					DebuggerInfo.CreateBindCtx(0, out IBindCtx bindCtx);
					moniker.GetDisplayName(bindCtx, null, out string displayName);
					moniker.GetClassID(out var classId);
					if (displayName.StartsWith("!VisualStudio.DTE"))
					{
						try
						{
							rot.GetObject(monikers[0], out object obj);
							instances.Add(new DebuggerInfo(displayName, obj));
						}
						catch (COMException)
						{
							// the debugger might not be ready at this time
						}
					}
				}
			}

			DebuggerInfo.Instances = instances.ToArray();
		}

		private static string VisualStudioVersionToProductVersion(string version)
		{
			switch (version)
			{
				case "15.0":
					return "2017";
				case "14.0":
					return "2015";
				case "13.0":
					return "2013";
				case "12.0":
					return "2012";
				case "11.0":
					return "2010";
				default:
					return version;
			}
		}

		public static DebuggerInfo PickDebugger(string solutionFile, bool forceRefresh)
		{
			if (forceRefresh || DebuggerInfo.Instances == null || DebuggerInfo.Instances.Length == 0)
			{
				DebuggerInfo.RefreshInstances();
			}

			if (DebuggerInfo.Instances == null || DebuggerInfo.Instances.Length == 0)
			{
				return null;
			}

			solutionFile = Path.GetFullPath(solutionFile);
			var debugger = DebuggerInfo.Instances.FirstOrDefault(
				d => d.SolutionFileName?.Equals(solutionFile, StringComparison.OrdinalIgnoreCase) ?? false);
			return debugger ?? DebuggerInfo.Instances.First();
		}

		private static void TryLaunchDebugger()
		{

		}

		public string MonikerName { get; }
		public dynamic DteObject { get; }
		public int ProcessId { get; }

		public string Name => this.DteObject.Name;
		public string Description => Path.GetFileName(this.SolutionFileName);
		public string Version => this.DteObject.Version;
		public string Edition => this.DteObject.Edition;
		public string SolutionFileName { get; }

		public string SolutionDisplayName => string.IsNullOrEmpty(this.SolutionFileName)
			? "new solution"
			: Path.GetFileName(this.SolutionFileName);

		public string DisplayName => $"VS {DebuggerInfo.VisualStudioVersionToProductVersion(this.Version)} - {this.SolutionDisplayName}";

		public DebuggerInfo(string monikerName, dynamic dteObject)
		{
			this.MonikerName = monikerName;
			this.DteObject = dteObject;
			this.SolutionFileName = this.DteObject.Solution?.FileName;  // could be slow and prone to exception, cache it

			var match = Regex.Match(this.MonikerName, @"\:(\d+)$");
			this.ProcessId = match.Success ? int.Parse(match.Groups[1].Value) : -1;
		}

		public bool AttachProcess(Process process)
		{
			MessageFilter.Register();

			try
			{
				var processes = ((IEnumerable)this.DteObject.Debugger.LocalProcesses).OfType<dynamic>();
				var dteProcess = processes.SingleOrDefault(x => x.ProcessID == process.Id);
				if (dteProcess == null)
					throw new Exception("Process not found");

				dteProcess.Attach();

				this.DteObject.MainWindow.Activate();
				return true;
			}
			catch (Exception exception)
			{
				MessageBox.Show(
					$"Unable to launch and attach to debugger, make sure you have Visual Studio installed correctly.\n\nError: {exception.Message}",
					"Launch and Debug", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return false;
			}
			finally
			{
				MessageFilter.Revoke();
			}
		}
	}
}
