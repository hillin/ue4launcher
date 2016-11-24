using System.ComponentModel;
using System.Timers;
using System.Windows;

namespace UE4Launcher
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        private readonly Timer _statusTimeoutTimer;

        public MainWindow()
        {
            App.CurrentMainWindow = this;

            this.InitializeComponent();
            this.DataContext = _viewModel = new MainWindowViewModel();

            _statusTimeoutTimer = new Timer {AutoReset = false};
            _statusTimeoutTimer.Elapsed += this.StatusTimeoutTimer_Elapsed;

            this.ResetStatusText();
        }

        private void StatusTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.ResetStatusText();
        }

        private void ResetStatusText()
        {
            _viewModel.StatusText = "Ready";
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_viewModel.ConfirmSaveBeforeExit())
                e.Cancel = true;
        }


        public void ReportStatus(string status, double? timeout)
        {
            if (_viewModel == null)
                return;

            _viewModel.StatusText = status;
            _statusTimeoutTimer.Stop();
            if (timeout != null)
            {
                _statusTimeoutTimer.Interval = timeout.Value;
                _statusTimeoutTimer.Start();
            }
        }
    }
}
