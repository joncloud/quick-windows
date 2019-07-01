using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace QuickWindows
{ 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            _appService = new DesignerAppService();
            DataContext = new MainViewModel(_appService);

            InitializeComponent();
        }

        readonly IAppService _appService;
        public MainWindow(IAppService appService)
        {
            _appService = appService;
            DataContext = new MainViewModel(appService);

            InitializeComponent();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            ViewModel.ReadyToSearch = false;
        }

        private void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyStroke = new KeyStroke(
                Keyboard.Modifiers,
                e.Key
            );

            if (_appService.ShortcutService.TryRequestShortcut(keyStroke))
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }

        void HandleMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ViewModel.FocusSelectedProcess();
        }
    }
}
