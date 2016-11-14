using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UE4Launcher.Launcher
{
    /// <summary>
    /// Interaction logic for ProjectLauncher.xaml
    /// </summary>
    public partial class ProjectLauncherPage : UserControl
    {
        private ProjectLauncherViewModel ViewModel => this.DataContext as ProjectLauncherViewModel;

        public ProjectLauncherPage()
        {
            this.InitializeComponent();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SaveProfiles();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LaunchProfile();
        }

        private void NewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.AddNewProfile();
        }

        private void RemoveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.SelectedProfile == null)
                return;

            if (MessageBox.Show(
                    $"Confirm to remove \"{this.ViewModel.SelectedProfile.ProfileName}\"? This cannot be undone.", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.ViewModel.RemoveSelectedProfile();
            }
        }

        private void ProfileListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ViewModel.LaunchProfile(((ListBoxItem)sender)?.DataContext as LaunchProfileViewModel);
        }

        private void DuplicateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.DuplicateProfile();
        }

        private void ProfileNotSavedMessageBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                this.ViewModel.SaveProfiles();
            }
        }
    }
}
