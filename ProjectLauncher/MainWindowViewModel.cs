using System.Reflection;
using System.Text;
using System.Windows;
using UE4Launcher.Launcher;
using UE4Launcher.Places;
using UE4Launcher.Processes;

namespace UE4Launcher
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
        public PlacesViewModel Places { get; }

        public MainWindowViewModel()
        {
            this.ProjectLauncher = new ProjectLauncherViewModel(this);
            this.Processes = new ProcessPageViewModel(this);
            this.Places = new PlacesViewModel(this);
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
