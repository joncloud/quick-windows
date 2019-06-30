using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Generic;

namespace QuickWindows
{
    public class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        readonly List<WindowProcess> _allProcesses;
        public BulkObservableCollection<WindowProcess> FilteredProcesses { get; }

        WindowProcess _selectedProcess;
        public WindowProcess SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                if (_selectedProcess != value)
                {
                    _selectedProcess = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedProcess)));
                }
            }
        }
        string _searchTerms = "";

        public string SearchTerms
        {
            get => _searchTerms;
            set
            {
                // TODO probably could look for > and then have specific process actions,
                // e.g., run Visual Studio and open XXX solution.
                if (_searchTerms != value)
                {
                    var moreSpecific = _searchTerms.Length < value.Length;

                    _searchTerms = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(SearchTerms)));

                    using var _ = FilteredProcesses.EnableBulkOperations();

                    if (string.IsNullOrWhiteSpace(_searchTerms))
                    {
                        FilteredProcesses.Clear();
                        foreach (var process in _allProcesses)
                        {
                            FilteredProcesses.Add(process);
                        }
                        if (FilteredProcesses.Any())
                        {
                            SelectedProcess = FilteredProcesses[0];
                        }
                    }
                    else if (moreSpecific)
                    {
                        var i = FilteredProcesses.Count;
                        var removed = false;
                        while (--i >= 0)
                        {
                            var process = FilteredProcesses[i];
                            if (!ProcessMatchesSearchTerms(process))
                            {
                                FilteredProcesses.RemoveAt(i);
                                removed = true;
                            }
                        }

                        if (removed)
                        {
                            if (FilteredProcesses.Any())
                            {
                                SelectedProcess = FilteredProcesses[0];
                            }
                            else
                            {
                                SelectedProcess = null;
                            }
                        }
                    }
                    else
                    {
                        FilteredProcesses.Clear();
                        foreach (var process in _allProcesses)
                        {
                            if (ProcessMatchesSearchTerms(process))
                            {
                                FilteredProcesses.Add(process);
                            }
                        }
                    }
                }
            }
        }

        bool ProcessMatchesSearchTerms(WindowProcess process) =>
            process.ProcessName.Contains(_searchTerms, StringComparison.OrdinalIgnoreCase) ||
            process.MainWindowTitle.Contains(_searchTerms, StringComparison.OrdinalIgnoreCase);

        public void SelectPrevious()
        {
            var index = FilteredProcesses.IndexOf(SelectedProcess);
            var target = --index;
            if (target < 0) target = FilteredProcesses.Count - 1;
            SelectedProcess = FilteredProcesses[target];
        }

        public void SelectNext()
        {
            var index = FilteredProcesses.IndexOf(SelectedProcess);
            var target = ++index;
            if (target == FilteredProcesses.Count) target = 0;
            SelectedProcess = FilteredProcesses[target];
        }

        public MainViewModel()
        {
            // TODO design time is blowing up
            var currentProcess = Process.GetCurrentProcess();
            _allProcesses = new List<WindowProcess>(
                WindowProcess.FromProcesses()
                    .Where(process => process.ProcessId != currentProcess.Id)
                    .OrderBy(process => process.ProcessName)
                    .ThenBy(process => process.MainWindowTitle)
            );
            FilteredProcesses = new BulkObservableCollection<WindowProcess>(_allProcesses);
            if (FilteredProcesses.Any())
            {
                SelectedProcess = FilteredProcesses[0];
            }
            FocusSelectedProcessCommand = new ActionCommand(FocusSelectedProcess);
            RefreshCommand = new ActionCommand(Refresh);
            ReadyToSearch = true;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs ea)
        {
            PropertyChanged?.Invoke(this, ea);
        }

        public ActionCommand FocusSelectedProcessCommand { get; }
        public ActionCommand RefreshCommand { get; }

        public void FocusSelectedProcess()
        {
            if (_selectedProcess == null) return;
            if (_selectedProcess.TryFocusMainWindow())
            {
                ReadyToSearch = false;
            }
            SearchTerms = "";
        }

        public void Refresh()
        {
            using var _y = FilteredProcesses.EnableBulkOperations();
            _allProcesses.Clear();
            FilteredProcesses.Clear();
            var currentProcess = Process.GetCurrentProcess();
            var processes = WindowProcess.FromProcesses()
                .Where(process => process.ProcessId != currentProcess.Id)
                .OrderBy(process => process.ProcessName)
                .ThenBy(process => process.MainWindowTitle);
            foreach (var process in processes)
            {
                _allProcesses.Add(process);
                FilteredProcesses.Add(process);
            }
            if (FilteredProcesses.Any())
            {
                SelectedProcess = FilteredProcesses[0];
            }
        }

        KeyboardHook _hook;
        public void RegisterGlobalHotKeys()
        {
            _hook = new KeyboardHook();
            _hook.KeyPressed += KeyboardHookKeyPressed;
            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.C);
        }

        bool _readyToSearch;
        public bool ReadyToSearch
        {
            get => _readyToSearch;
            set
            {
                if (_readyToSearch != value)
                {
                    _readyToSearch = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ReadyToSearch)));

                    // TODO probably can use data trigger
                    if (_readyToSearch)
                    {
                        App.Current.MainWindow.Visibility = Visibility.Visible;
                        App.Current.MainWindow.Activate();
                    }
                    else
                    {
                        SearchTerms = "";
                        App.Current.MainWindow.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        void KeyboardHookKeyPressed(object sender, KeyPressedEventArgs e)
        {
            Refresh();
            ReadyToSearch = true;
        }

        public void Dispose()
        {
            _hook?.Dispose();
        }
    }
}
