using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace ProjectLauncher.Launcher
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
                file = Path.GetFullPath(Path.Combine(((App)Application.Current).RootPath,
                                                     this.ViewModel.Profile.ProjectName,
                                                     "Config",
                                                     file));
            }

            if (!File.Exists(file))
                return;

            Process.Start("explorer.exe", $"/select, \"{file}\"");
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
    }
}
