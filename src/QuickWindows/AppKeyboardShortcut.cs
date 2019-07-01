using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace QuickWindows
{
    public class AppKeyboardShortcut
    {
        public KeyStroke KeyStroke { get; set; }
        public AppAction AppAction { get; set; }
    }

    public class AppKeyboardShortcutViewModel
    {
        public AppKeyboardShortcut Shortcut { get; }
        public AppKeyboardShortcutViewModel(AppKeyboardShortcut appKeyboardShortcut) =>
            Shortcut = appKeyboardShortcut ?? throw new ArgumentNullException(nameof(appKeyboardShortcut));

        bool HasModifier(ModifierKeys modifier) =>
            Shortcut.KeyStroke.ModifierKeys.HasFlag(modifier);
        public bool HasControl => HasModifier(ModifierKeys.Control);
        public bool HasWindows => HasModifier(ModifierKeys.Windows);
        public bool HasShift => HasModifier(ModifierKeys.Shift);
        public bool HasAlt => HasModifier(ModifierKeys.Alt);
        public Key Key => Shortcut.KeyStroke.Key;
        public string KeyDescription => Key.ToString();
        public AppAction Action => Shortcut.AppAction;
        public string ActionDescription => typeof(AppAction)
            .GetMember(Action.ToString())[0]
            .GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? Action.ToString();
    }

    public class AppActionViewModel
    {
        public AppAction AppAction { get; }
        public string Description { get; }
        public AppActionViewModel(AppAction appAction)
        {
            AppAction = appAction;
            
            Description = typeof(AppAction)
                .GetMember(appAction.ToString())[0]
                .GetCustomAttribute<DescriptionAttribute>()
                ?.Description ?? appAction.ToString();
        }
    }

    public class AppKeyboardShortcutBuilder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool _hasControl;
        public bool HasControl
        {
            get => _hasControl;
            set
            {
                if (_hasControl == value) return;

                _hasControl = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasControl)));
                ResetVisualization();
            }
        }

        bool _hasWindows;
        public bool HasWindows
        {
            get => _hasWindows;
            set
            {
                if (_hasWindows == value) return;

                _hasWindows = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasWindows)));
                ResetVisualization();
            }
        }

        bool _hasShift;
        public bool HasShift
        {
            get => _hasShift;
            set
            {
                if (_hasShift == value) return;

                _hasShift = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasShift)));
                ResetVisualization();
            }
        }

        bool _hasAlt;
        public bool HasAlt
        {
            get => _hasAlt;
            set
            {
                if (_hasAlt == value) return;

                _hasAlt = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasAlt)));
                ResetVisualization();
            }
        }

        Key _key;
        public Key Key
        {
            get => _key;
            set
            {
                if (_key == value) return;

                _key = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Key)));
                ResetVisualization();
            }
        }

        AppAction _action;
        public AppAction Action
        {
            get => _action;
            set
            {
                if (_action == value) return;

                _action = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Action)));
            }
        }

        public void SetKeyStroke(KeyStroke keyStroke)
        {
            HasAlt = keyStroke.ModifierKeys.HasFlag(ModifierKeys.Alt);
            HasControl = keyStroke.ModifierKeys.HasFlag(ModifierKeys.Control);
            HasShift = keyStroke.ModifierKeys.HasFlag(ModifierKeys.Shift);
            HasWindows = keyStroke.ModifierKeys.HasFlag(ModifierKeys.Windows);
            Key = keyStroke.Key;
        }

        void ResetVisualization()
        {
            var sb = new StringBuilder();
            if (HasWindows) sb.Append(" WIN +");
            if (HasControl) sb.Append(" CTRL +");
            if (HasAlt) sb.Append(" ALT +");
            if (HasShift) sb.Append(" SHIFT +");
            if (Key != Key.None) sb.Append(" ").Append(Key);

            Visualization = sb.ToString();
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Visualization)));
        }
        public string Visualization { get; private set; } = "";

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void Reset()
        {
            HasAlt = HasControl = HasShift = HasWindows = false;
            Key = Key.None;
            Action = AppAction.ActivateApp;
        }

        ModifierKeys GetModifiers()
        {
            var modifiers = ModifierKeys.None;

            if (HasAlt) modifiers |= ModifierKeys.Alt;
            if (HasControl) modifiers |= ModifierKeys.Control;
            if (HasShift) modifiers |= ModifierKeys.Shift;
            if (HasWindows) modifiers |= ModifierKeys.Windows;

            return modifiers;
        }

        public AppKeyboardShortcut Build() =>
            new AppKeyboardShortcut
            {
                AppAction = Action,
                KeyStroke = new KeyStroke(GetModifiers(), Key)
            };
    }

    public class AppShortcutsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppKeyboardShortcutBuilder Builder { get; }
        public ObservableCollection<AppKeyboardShortcutViewModel> Shortcuts { get; }
        public List<AppActionViewModel> AvailableAppActions { get; }

        AppActionViewModel _selectedAppAction;
        public AppActionViewModel SelectedAppAction
        {
            get => _selectedAppAction;
            set
            {
                if (_selectedAppAction == value) return;

                _selectedAppAction = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedAppAction)));
                Builder.Action = _selectedAppAction.AppAction;
            }
        }

        public ICommand AddCommand { get; }
        readonly IShortcutService _shortcutService;
        public AppShortcutsViewModel(IShortcutService shortcutService)
        {
            _shortcutService = shortcutService;
            Builder = new AppKeyboardShortcutBuilder();
            Shortcuts = new ObservableCollection<AppKeyboardShortcutViewModel>(
                _shortcutService.GetShortcuts()
                    .Select(tuple =>
                    {
                        var shortcut = new AppKeyboardShortcut
                        {
                            AppAction = tuple.Item2,
                            KeyStroke = tuple.Item1
                        };
                        return new AppKeyboardShortcutViewModel(
                            shortcut
                        );
                    })
            );
            AvailableAppActions = new List<AppActionViewModel>
            {
                new AppActionViewModel(AppAction.ActivateApp),
                new AppActionViewModel(AppAction.DeactivateApp),
                new AppActionViewModel(AppAction.FocusProcess),
                new AppActionViewModel(AppAction.NextProcess),
                new AppActionViewModel(AppAction.PreviousProcess)
            };
            SelectedAppAction = AvailableAppActions[0];
            AddCommand = new ActionCommand(Add);
        }

        public void Add()
        {
            var shortcut = Builder.Build();
            if (!_shortcutService.TryAddShortcut(shortcut.KeyStroke, shortcut.AppAction))
            {
                return;
            }

            Shortcuts.Add(new AppKeyboardShortcutViewModel(shortcut));
            SelectedAppAction = AvailableAppActions[0];
            Builder.Reset();
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
