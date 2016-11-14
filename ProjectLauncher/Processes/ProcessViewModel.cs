using System.Diagnostics;
using UE4Launcher.Debugging;

namespace UE4Launcher.Processes
{
    class ProcessViewModel
    {
        private readonly Process _process;

        public string Id => _process.Id.ToString();
        public string Title
            => string.IsNullOrEmpty(_process.MainWindowTitle) ? _process.ProcessName : _process.MainWindowTitle;
        public string Name => _process.ProcessName;
        public string StartTime => _process.StartTime.ToLongTimeString();

        public ProcessViewModel(Process process)
        {
            _process = process;
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
