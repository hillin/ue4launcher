using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Timer = System.Timers.Timer;
using WinForms = System.Windows.Forms;

namespace UE4Launcher.Root
{
    public partial class MainWindow
    {

        private readonly MainWindowViewModel _viewModel;

        private readonly Timer _statusTimeoutTimer;
        private readonly WinForms.NotifyIcon _trayNotifier = new WinForms.NotifyIcon();

        private readonly ContextMenu _trayNotifierContextMenu;

        public MainWindow()
        {

            App.CurrentMainWindow = this;

            this.InitializeComponent();
            this.DataContext = _viewModel = new MainWindowViewModel();

            // status timeout timer
            _statusTimeoutTimer = new Timer { AutoReset = false };
            _statusTimeoutTimer.Elapsed += this.StatusTimeoutTimer_Elapsed;

            this.ResetStatusText();

            // tray notifier
            _trayNotifierContextMenu = (ContextMenu)this.FindResource("TrayNotifierContextMenu");

            _trayNotifier.MouseDown += this.TrayNotifier_MouseDown;
            _trayNotifier.DoubleClick += this.TrayNotifier_DoubleClick;
            var iconInfo = Application.GetResourceStream(new Uri("/Resources/Icons/app.ico", UriKind.Relative));
            if (iconInfo != null)
                using (var iconStream = iconInfo.Stream)
                    this._trayNotifier.Icon = new System.Drawing.Icon(iconStream);

            _trayNotifier.Visible = true;

            MouseHook.MouseAction += this.MouseHook_MouseAction;

            // minimize if start minimized
            if (((App)Application.Current).StartMinimized)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void MouseHook_MouseAction(object sender, EventArgs e)
        {
            if (!_trayNotifierContextMenu.IsOpen)
            {
                MouseHook.Stop();
                return;
            }

            var mousePosition = Mouse.GetPosition(_trayNotifierContextMenu);
            
            if (mousePosition.X <= 0
                || mousePosition.Y <= 0
                || mousePosition.X >= _trayNotifierContextMenu.RenderSize.Width
                || mousePosition.Y >= _trayNotifierContextMenu.RenderSize.Height)
            {
                this.CloseTrayNotifierContextMenu();
            }
        }

        private void CloseTrayNotifierContextMenu()
        {
            _trayNotifierContextMenu.IsOpen = false;
            MouseHook.Stop();
        }

        private void TrayNotifier_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.Show();
                this.WindowState = System.Windows.WindowState.Normal;
                this.Focus();
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        void TrayNotifier_MouseDown(object sender, WinForms.MouseEventArgs e)
        {
            if (_trayNotifierContextMenu.IsOpen)
            {
                this.CloseTrayNotifierContextMenu();
                return;
            }

            if (e.Button == WinForms.MouseButtons.Right)
            {
                this.OpenTrayNotifierContextMenu();

            }
        }

        private void OpenTrayNotifierContextMenu()
        {
            _trayNotifierContextMenu.IsOpen = true;
            MouseHook.Start();
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
            this.WindowState = System.Windows.WindowState.Minimized;
            e.Cancel = true;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.Hide();
            else
                this.Show();

            base.OnStateChanged(e);
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

        private void ExitMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!_viewModel.ConfirmSaveBeforeExit())
                return;

            Environment.Exit(0);
        }

        private void KillAllProcessesMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.Processes.KillAllProcesses();
        }
    }
}
