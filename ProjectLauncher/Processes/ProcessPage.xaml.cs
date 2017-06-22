using System.Windows.Controls;

namespace UE4Launcher.Processes
{
    /// <summary>
    /// Interaction logic for ProcessPage.xaml
    /// </summary>
    public partial class ProcessPage : UserControl
    {
        private ProcessPageViewModel ViewModel => this.DataContext as ProcessPageViewModel;
        public ProcessPage()
        {
            this.InitializeComponent();
        }

        private void KillAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ViewModel.KillAllProcesses();
        }

        private void RefreshButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ViewModel.RefreshProcesses();
        }

        private void AttachButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ViewModel.AttachDebuggerToSelectedProcess();
        }

        private void KillButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ViewModel.KillSelectedProcess();
        }

        private void ProcessList_DeleteCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.ViewModel.KillSelectedProcess();
        }

        private void ProcessListItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Helpers.IsAltDown)
                this.ViewModel.KillSelectedProcess();
            else if (Helpers.IsCtrlDown)
                this.ViewModel.AttachDebuggerToSelectedProcess();
            else
                this.ViewModel.BringSelectedProcessToFront();
        }

        private void ActivateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ViewModel.BringSelectedProcessToFront();
        }

		private void RefreshDebuggersButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.ViewModel.RefreshProcesses();
		}
	}
}
