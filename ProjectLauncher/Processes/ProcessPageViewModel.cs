using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using UE4Launcher.Debugging;

namespace UE4Launcher.Processes
{
    class ProcessPageViewModel : NotificationObject
    {
        private readonly MainWindowViewModel _owner;

        private readonly Dictionary<int, ProcessViewModel> _processIdToViewModelMap;

        public ObservableCollection<ProcessViewModel> Processes { get; }

        private ProcessViewModel _selectedProcess;
        public ProcessViewModel SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                this.RaisePropertyChanged(nameof(this.SelectedProcess));
                this.RaisePropertyChanged(nameof(this.HasProcessSelected));
            }
        }

        public bool HasAnyProcess => this.Processes.Count > 0;
        public bool HasProcessSelected => this.SelectedProcess != null;


        public DebuggerInfo[] Debuggers => DebuggerInfo.Debuggers;

        public DebuggerInfo SelectedDebugger
        {
            get
            {
                var index = Preferences.Default.DebuggerIndex;
                if (index < 0 || index >= this.Debuggers.Length || !this.Debuggers[index].IsAvailable)
                    index = 0;

                return this.Debuggers[index];
            }
            set
            {
                Preferences.Default.DebuggerIndex = Array.IndexOf(this.Debuggers, value);
                Preferences.Default.Save();
                this.RaisePropertyChanged(nameof(this.SelectedDebugger));
            }
        }

        public ProcessPageViewModel(MainWindowViewModel owner)
        {
            _owner = owner;
            _processIdToViewModelMap = new Dictionary<int, ProcessViewModel>();
            this.Processes = new ObservableCollection<ProcessViewModel>();

            this.RefreshProcesses();

            var processRefreshTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            processRefreshTimer.Tick += this.ProcessRefreshTimer_Tick;
            processRefreshTimer.Start();
        }

        private void ProcessRefreshTimer_Tick(object sender, EventArgs e)
        {
            this.RefreshProcesses();
        }

        public void RefreshProcesses()
        {
            var processes =
                ExecutableFileInfo.ExecutableFiles.SelectMany(e => Process.GetProcessesByName(e.ProcessName))
                                  .ToDictionary(p => p.Id);

            var removedProcesses = new List<int>();
            foreach (var pair in _processIdToViewModelMap)
            {
                if (!processes.ContainsKey(pair.Key))
                {
                    removedProcesses.Add(pair.Key);
                    this.Processes.Remove(pair.Value);
                }
            }

            foreach (var process in removedProcesses)
                _processIdToViewModelMap.Remove(process);

            foreach (var pair in processes)
            {
                if (!_processIdToViewModelMap.ContainsKey(pair.Key))
                {
                    var viewModel = new ProcessViewModel(pair.Value);
                    _processIdToViewModelMap.Add(pair.Key, viewModel);
                    this.Processes.Add(viewModel);
                }
            }

            this.RaisePropertyChanged(nameof(this.HasAnyProcess));
        }

        public void KillAllProcesses()
        {
            foreach (var process in this.Processes)
                process.Kill();

            App.ReportStatus("All processes killed.");

            this.RefreshProcesses();

        }

        public void AttachDebuggerToSelectedProcess()
        {
            this.SelectedProcess?.AttachDebugger(this.SelectedDebugger);
        }

        public void KillSelectedProcess()
        {
            if (this.SelectedProcess != null)
            {
                this.SelectedProcess?.Kill();
                App.ReportStatus("Processes killed.");
            }
            this.RefreshProcesses();
        }
    }
}
