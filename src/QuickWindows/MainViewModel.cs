using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace QuickWindows
{
    public class ActionCommand : ICommand
    {
        readonly Action _action;
        public ActionCommand(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();
    }

    public class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        public ObservableCollection<WindowProcess> AllProcesses { get; }
        public ObservableCollection<WindowProcess> FilteredProcesses { get; }

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
                    _searchTerms = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(SearchTerms)));

                    if (string.IsNullOrWhiteSpace(_searchTerms))
                    {
                        FilteredProcesses.Clear();
                        foreach (var process in AllProcesses)
                        {
                            FilteredProcesses.Add(process);
                        }
                        if (FilteredProcesses.Any())
                        {
                            SelectedProcess = FilteredProcesses[0];
                        }
                    }
                    else
                    {
                        var i = FilteredProcesses.Count;
                        var removed = false;
                        while (--i >= 0)
                        {
                            var process = FilteredProcesses[i];
                            if (!process.ProcessName.Contains(_searchTerms, StringComparison.OrdinalIgnoreCase) &&
                                !process.MainWindowTitle.Contains(_searchTerms, StringComparison.OrdinalIgnoreCase))
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
                }
            }
        }

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
            AllProcesses = new ObservableCollection<WindowProcess>(
                WindowProcess.FromProcesses()
                    .Where(process => process.ProcessId != currentProcess.Id)
            );
            FilteredProcesses = new ObservableCollection<WindowProcess>(AllProcesses);
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
            AllProcesses.Clear();
            FilteredProcesses.Clear();
            var currentProcess = Process.GetCurrentProcess();
            var processes = WindowProcess.FromProcesses()
                .Where(process => process.ProcessId != currentProcess.Id);
            foreach (var process in processes)
            {
                AllProcesses.Add(process);
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
            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Windows, Keys.L);
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
                        App.Current.MainWindow.Activate();
                        App.Current.MainWindow.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        App.Current.MainWindow.WindowState = WindowState.Minimized;
                        SearchTerms = "";
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
