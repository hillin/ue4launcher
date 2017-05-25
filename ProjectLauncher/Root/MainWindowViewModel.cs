using System.Reflection;
using System.Text;
using System.Windows;
using UE4Launcher.Launcher;
using UE4Launcher.Places;
using UE4Launcher.Processes;

namespace UE4Launcher.Root
{
	internal class MainWindowViewModel : NotificationObject
    {
        public bool EditMode => ((App)Application.Current).EditMode;


        public string Title
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("Launcher");

	            if (this.EditMode)
	            {
		            builder.Append(" (Edit Mode)");
	            }

	            builder.Append(" ").Append(Assembly.GetExecutingAssembly().GetName().Version);

                return builder.ToString();
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
	        set
            {
                _statusText = value;
                this.RaisePropertyChanged(nameof(this.StatusText));
            }
        }

        public bool StartWithWindows
        {
            get => Preferences.Default.StartWithWindows;
	        set
            {
                Preferences.Default.StartWithWindows = value;
                Preferences.Default.Save();

                ((App) Application.Current).SetStartupWithWindows(value);

                this.RaisePropertyChanged(nameof(this.StartWithWindows));
            }
        }

        public bool CloseToSystemTray
        {
            get => Preferences.Default.CloseToSystemTray;
	        set
            {
                Preferences.Default.CloseToSystemTray = value;
                Preferences.Default.Save();

                this.RaisePropertyChanged(nameof(this.CloseToSystemTray));
            }
        }

        public ProjectLauncherViewModel ProjectLauncher { get; }
        public ProcessPageViewModel Processes { get; }
        public PlacesViewModel Places { get; }

        public MainWindowViewModel()
        {
            this.ProjectLauncher = new ProjectLauncherViewModel(this);
            this.Processes = new ProcessPageViewModel(this);
            this.Places = new PlacesViewModel(this);
        }

        public bool ConfirmSaveBeforeExit()
        {
            if (!this.EditMode)
                return true;

            if (!this.ProjectLauncher.ConfirmSaveBeforeExit())
                return false;

            return true;
        }

    }
}
