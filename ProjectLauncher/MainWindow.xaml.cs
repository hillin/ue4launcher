using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace ProjectLauncher
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        private readonly Timer _statusTimeoutTimer;

        public MainWindow()
        {
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
