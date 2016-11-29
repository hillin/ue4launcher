using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace UE4Launcher.Launcher
{

    public partial class ProfileEditor : UserControl
    {
        private LaunchProfileViewModel ViewModel => this.DataContext as LaunchProfileViewModel;

        public ProfileEditor()
        {
            this.InitializeComponent();
        }

        private void IPSectionTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(char.IsDigit))
                e.Handled = true;

            if (e.Text == ".")
            {
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                ((Control)sender).MoveFocus(request);
            }
        }

        private void NavigateDefaultEngineFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(this.ViewModel.DefaultEngineFilename);
        }

        private void NavigateFile(string file)
        {
            if (!Path.IsPathRooted(file))
            {
                file = Path.GetFullPath(Path.Combine(App.CurrentRootPath,
                                                     this.ViewModel.Profile.ProjectName,
                                                     "Config",
                                                     file));
            }

            Utilities.NavigateFile(file);
        }

        private void NavigateDefaultGameFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(this.ViewModel.DefaultGameFilename);
        }

        private void NavigateDefaultEditorFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(this.ViewModel.DefaultEditorFilename);
        }

        private void NavigateDefaultInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(this.ViewModel.DefaultInputFilename);
        }

        private void NavigateLogFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(Path.Combine(this.ViewModel.SelectedProject.Path, "Saved", "Logs", this.ViewModel.LogFilename));
        }

        private void NavigateExecutableFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(this.ViewModel.SelectedExecutableFile.Path);
        }

        private void NavigateProfileFileButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((ProfileStorage)this.ProfileStorageList.SelectedItem)
            {
                case ProfileStorage.Public:
                    this.NavigateFile(Path.Combine(App.CurrentRootPath, Constants.PublicProfileFilename));
                    break;
                case ProfileStorage.Personal:
                    this.NavigateFile(Path.Combine(App.CurrentRootPath, Constants.PersonalProfileFilename));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NavigateProjectFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateFile(this.ViewModel.SelectedProject.Path);
        }
    }
}
