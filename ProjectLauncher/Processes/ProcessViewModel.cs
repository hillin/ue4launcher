using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UE4Launcher.Debugging;
using UE4Launcher.Root;

namespace UE4Launcher.Processes
{
	internal class ProcessViewModel : NotificationObject, ITrayContextMenuItem, IDisposable
    {

        private readonly Process _process;

        public string Id => _process.Id.ToString();
        public string Title
            => string.IsNullOrEmpty(_process.MainWindowTitle) ? _process.ProcessName : _process.MainWindowTitle;
        public string Name => _process.ProcessName;
        public string StartTime => _process.StartTime.ToLongTimeString();

        private TimeSpan _lastTotalProcessorTime;
        private DateTime _lastProfilingTime;

        private string _cpuUsageDisplay;

        public string CPUUsageDisplay
        {
            get => _cpuUsageDisplay;
	        private set
            {
                _cpuUsageDisplay = value;
                this.RaisePropertyChanged(nameof(this.CPUUsageDisplay));
            }
        }


        private string _memoryDisplay;
        public string MemoryDisplay
        {
            get => _memoryDisplay;
	        private set
            {
                _memoryDisplay = value;
                this.RaisePropertyChanged(nameof(this.MemoryDisplay));
            }
        }

        string ITrayContextMenuItem.Name => this.Title;

        bool ITrayContextMenuItem.IsEnabled => true;

        ImageSource ITrayContextMenuItem.Icon => Helpers.GetFileSystemIcon(_process.MainModule.FileName, true);

        private readonly ICommand _trayContextMenuCommand;
        private readonly DispatcherTimer _updateTimer;
        ICommand ITrayContextMenuItem.Command => _trayContextMenuCommand;

        string ITrayContextMenuItem.Description
            => $"Start Time: {this.StartTime}\nCPU Usage: {this.CPUUsageDisplay}\nMemory Usage: {this.MemoryDisplay}\nClick to activate, Ctrl-click to attach debugger, Alt-click to terminate";

        public ProcessViewModel(Process process)
        {
            _process = process;
            _updateTimer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _updateTimer.Tick += this.UpdateTimer_Tick;
            _updateTimer.Start();
            this.UpdateProfiling();

            _trayContextMenuCommand = new SimpleCommand(this.ExecuteTrayContextMenuCommand);

        }

        private void ExecuteTrayContextMenuCommand(object obj)
        {
            if (Helpers.IsCtrlDown)
                this.AttachDebugger(AutomaticDebuggerInfo.Instance);
            else if (Helpers.IsAltDown)
                this.Kill();
            else
                this.BringToFront();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateProfiling();

            this.RaisePropertyChanged(nameof(this.Title));
            this.RaisePropertyChanged(nameof(this.Name));
        }

        private void UpdateProfiling()
        {
            if (_process.HasExited)
                return;

            _process.Refresh();

            var now = DateTime.UtcNow;
            var totalProcessorTime = _process.TotalProcessorTime;
            var cpuUsage = (totalProcessorTime.TotalMilliseconds - _lastTotalProcessorTime.TotalMilliseconds)
                           / now.Subtract(_lastProfilingTime).TotalMilliseconds
                           / Environment.ProcessorCount;

            this.CPUUsageDisplay = cpuUsage.ToString("P1");

            var memory = (double)_process.WorkingSet64 / 1024 / 1024;
            this.MemoryDisplay = memory > 1024 ? $"{memory / 1024:F1} GiB" : $"{memory:F0} MiB";

            _lastProfilingTime = now;
            _lastTotalProcessorTime = totalProcessorTime;
        }

        public void Kill()
        {
            if (_process.HasExited)
                return;

            _process.Kill();
            this.Dispose();
        }

        public void AttachDebugger(IDebuggerInfo debugger)
        {
            if (_process.HasExited)
                return;

            debugger.AttachProcess(_process);
        }

        public void BringToFront()
        {
            if (_process.HasExited)
                return;

            Helpers.BringProcessToFront(_process);
        }

        public void Dispose()
        {
            _updateTimer.Stop();
        }
    }
}
