using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using UE4Launcher.Debugging;
using UE4Launcher.Root;

namespace UE4Launcher.Processes
{
	internal class ProcessPageViewModel : PageViewModelBase, IDebuggerSupportedViewModel
	{

        private readonly Dictionary<int, ProcessViewModel> _processIdToViewModelMap;

        public ObservableCollection<ProcessViewModel> Processes { get; }

        private ProcessViewModel _selectedProcess;
        public ProcessViewModel SelectedProcess
        {
            get => _selectedProcess;
	        set
            {
                _selectedProcess = value;
                this.RaisePropertyChanged(nameof(this.SelectedProcess));
                this.RaisePropertyChanged(nameof(this.HasProcessSelected));
            }
        }

        public bool HasAnyProcess => this.Processes.Count > 0;
        public bool HasProcessSelected => this.SelectedProcess != null;


		public ObservableCollection<IDebuggerInfo> Debuggers { get; } = new ObservableCollection<IDebuggerInfo>();
		ICollection<IDebuggerInfo> IDebuggerSupportedViewModel.Debuggers => this.Debuggers;

		private IDebuggerInfo _selectedDebugger;

		public IDebuggerInfo SelectedDebugger
		{
			get => _selectedDebugger;
			set
			{
				_selectedDebugger = value;
				this.RaisePropertyChanged(nameof(this.SelectedDebugger));
			}
		}

		public ProcessPageViewModel(MainWindowViewModel owner)
            : base(owner)
        {
            _processIdToViewModelMap = new Dictionary<int, ProcessViewModel>();
            this.Processes = new ObservableCollection<ProcessViewModel>();

	        this.RefreshDebuggers();

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
                    pair.Value.Dispose();
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

        public void BringSelectedProcessToFront()
        {
            this.SelectedProcess?.BringToFront();
        }
    }
}
