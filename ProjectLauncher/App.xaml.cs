using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using UE4Launcher.Launcher;

namespace UE4Launcher
{
    public partial class App : Application
    {
        public string RootPath { get; set; }
        public bool DeveloperMode { get; set; }
        public bool EditMode { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Contains("-dev"))
                this.DeveloperMode = true;

            if (this.DeveloperMode && e.Args.Contains("-edit"))
                this.EditMode = true;

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
        }

        public static void ReportStatus(string status, double? timeOut = 10000)
        {
            ((MainWindow)((App)Application.Current).MainWindow).ReportStatus(status, timeOut);
        }
    }
}
