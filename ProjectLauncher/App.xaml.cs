using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Shell;
using Microsoft.Win32;
using UE4Launcher.Launcher;
using UE4Launcher.Root;

namespace UE4Launcher
{
	public partial class App : ISingleInstanceApp
	{
		public static string CurrentRootPath => ((App)Application.Current).RootPath;
		public static MainWindow CurrentMainWindow { get; set; }

		private const string FallbackSingleInstanceIdentifier = "ue4launcher";

		[STAThread]
		public static void Main()
		{
			var identifier = Assembly.GetEntryAssembly().Location?.Replace('\\', '_') ?? FallbackSingleInstanceIdentifier;

			if (SingleInstance<App>.InitializeAsFirstInstance(identifier))
			{
				var application = new App();

				application.InitializeComponent();
				application.Run();

				// Allow single instance code to perform cleanup operations
				SingleInstance<App>.Cleanup();
			}
		}


		public string RootPath { get; set; }
		public bool EditMode { get; set; }
		public bool StartMinimized { get; private set; }


		private string GetRestoredStartArgs()
		{
			var builder = new StringBuilder();

			if (this.EditMode)
				builder.Append(" -edit");

			return builder.ToString();
		}

		public bool SetStartupWithWindows(bool startUp)
		{
			var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
			var entryAssembly = Assembly.GetEntryAssembly();
			if (key != null && !string.IsNullOrEmpty(entryAssembly.Location))
			{
				if (startUp)
					key.SetValue(entryAssembly.GetName().Name, $"\"{entryAssembly.Location}\"{this.GetRestoredStartArgs()} -minimized");
				else
					key.DeleteValue(entryAssembly.GetName().Name, false);

				return true;
			}

			return false;
		}

		private string FindProjectRoot(string location, HashSet<string> searchedLocations)
		{
			var projectFound = false;
			while (!string.IsNullOrEmpty(location) && Directory.Exists(location))
			{
				searchedLocations.Add(location);
				if (ProjectUtilities.IsValidRootPath(location))
				{
					projectFound = true;
					break;
				}
				location = Directory.GetParent(location)?.FullName;
			}

			return projectFound ? location : null;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Contains("-edit"))
				this.EditMode = true;

			if (e.Args.Contains("-minimized"))
				this.StartMinimized = true;

			var searchedLocation = new HashSet<string>();
			var projectRoot = this.FindProjectRoot(Environment.CurrentDirectory, searchedLocation);
			if (string.IsNullOrEmpty(projectRoot))
				projectRoot = this.FindProjectRoot(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), searchedLocation);

			if (string.IsNullOrEmpty(projectRoot))
			{
				MessageBox.Show(
					$"No valid project found under these folders:\n\n{string.Join("\n", searchedLocation)}\n\nMake sure this launcher is placed under (any level of) your unreal project folder",
					"Project not Found",
					MessageBoxButton.OK,
					MessageBoxImage.Exclamation);

				Environment.Exit(-1);
			}

			this.RootPath = projectRoot;

			// only start with windows if we are in developer mode
			this.SetStartupWithWindows(Preferences.Default.StartWithWindows);

			PluginManager.Instance.Initialize();
		}

		public static void ReportStatus(string status, double? timeOut = 10000)
		{
			App.CurrentMainWindow.Dispatcher.BeginInvoke(
				   new Action(() => App.CurrentMainWindow.ReportStatus(status, timeOut)),
				   DispatcherPriority.Background);
		}

		public bool SignalExternalCommandLineArgs(IList<string> args)
		{
			App.CurrentMainWindow.Show();
			App.CurrentMainWindow.Activate();
			return true;
		}
	}
}
