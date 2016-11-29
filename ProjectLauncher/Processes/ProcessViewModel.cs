using System;
using System.Diagnostics;
using System.Windows.Threading;
using UE4Launcher.Debugging;

namespace UE4Launcher.Processes
{
    class ProcessViewModel : NotificationObject
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
            get { return _cpuUsageDisplay; }
            private set
            {
                _cpuUsageDisplay = value;
                this.RaisePropertyChanged(nameof(this.CPUUsageDisplay));
            }
        }


        private string _memoryDisplay;
        public string MemoryDisplay
        {
            get { return _memoryDisplay; }
            private set
            {
                _memoryDisplay = value;
                this.RaisePropertyChanged(nameof(this.MemoryDisplay));
            }
        }

        public ProcessViewModel(Process process)
        {
            _process = process;
            var timer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += this.ProfilingUpdateTimer_Tick;
            timer.Start();
            this.UpdateProfiling();
        }

        private void ProfilingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateProfiling();
        }

        private void UpdateProfiling()
        {
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
            _process.Kill();
        }

        public void AttachDebugger(DebuggerInfo debugger)
        {
            debugger.AttachProcess(_process);
        }
    }
}
