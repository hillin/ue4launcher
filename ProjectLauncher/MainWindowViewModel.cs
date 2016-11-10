using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using ProjectLauncher.Debugging;
using ProjectLauncher.Launcher;
using ProjectLauncher.Processes;

namespace ProjectLauncher
{
    class MainWindowViewModel : NotificationObject
    {

        public bool DeveloperMode => ((App)Application.Current).DeveloperMode;
        public bool EditMode => ((App)Application.Current).EditMode;


        public string Title
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("Launcher");

                if (this.DeveloperMode)
                {
                    builder.Append(this.EditMode ? " (Developer Edit Mode)" : " (Developer Mode)");
                }

                builder.Append(" ").Append(Assembly.GetExecutingAssembly().GetName().Version);

                return builder.ToString();
            }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                this.RaisePropertyChanged(nameof(this.StatusText));
            }
        }

        public ProjectLauncherViewModel ProjectLauncher { get; }
        public ProcessPageViewModel Processes { get; }

        public MainWindowViewModel()
        {
            this.ProjectLauncher = new ProjectLauncherViewModel(this);
            this.Processes = new ProcessPageViewModel(this);
        }

        public bool ConfirmSaveBeforeExit()
        {
            if (!this.DeveloperMode || !this.EditMode)
                return true;

            if (!this.ProjectLauncher.ConfirmSaveBeforeExit())
                return false;

            return true;
        }

    }
}
