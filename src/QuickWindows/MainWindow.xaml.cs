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
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ViewModel.RegisterGlobalHotKeys();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.Dispose();
            base.OnClosed(e);
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

            if (ViewModel.TryRequestShortcut(keyStroke))
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
