using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Win32;
using UE4Launcher.Launcher;
using UE4Launcher.Root;

namespace UE4Launcher
{
    public partial class App : Application
    {
        public static string CurrentRootPath => ((App)Application.Current).RootPath;
        public static MainWindow CurrentMainWindow { get; set; }

        public string RootPath { get; set; }
        public bool DeveloperMode { get; set; }
        public bool EditMode { get; set; }
        public bool StartMinimized { get; private set; }

        private string GetRestoredStartArgs()
        {
            var builder = new StringBuilder();
            if (this.DeveloperMode)
            {
                builder.Append(" -dev");
                if (this.EditMode)
                    builder.Append(" -edit");
            }

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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Contains("-dev"))
                this.DeveloperMode = true;

            if (this.DeveloperMode && e.Args.Contains("-edit"))
                this.EditMode = true;

            if (e.Args.Contains("-minimized"))
                this.StartMinimized = true;

            var location = Environment.CurrentDirectory;
            var searchedLocation = new List<string>();
            var projectFound = false;
            while (!string.IsNullOrEmpty(location) && Directory.Exists(location))
            {
                searchedLocation.Add(location);
                if (ProjectUtilities.IsValidRootPath(location))
                {
                    projectFound = true;
                    break;
                }
                location = Directory.GetParent(location)?.FullName;
            }

            if (!projectFound)
            {
                MessageBox.Show(
                    $"No valid project found under these folders:\n\n{string.Join("\n", searchedLocation)}\n\nMake sure this launcher is placed under (any level of) your unreal project folder",
                    "Project not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                Environment.Exit(-1);
            }

            this.RootPath = location;

            this.SetStartupWithWindows(Preferences.Default.StartWithWindows);
        }

        public static void ReportStatus(string status, double? timeOut = 10000)
        {
            App.CurrentMainWindow.Dispatcher.BeginInvoke(
                   new Action(() => CurrentMainWindow.ReportStatus(status, timeOut)),
                   DispatcherPriority.Background);
        }
    }
}
