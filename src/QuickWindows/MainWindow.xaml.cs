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

        private void SearchTerms_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    e.Handled = true;
                    ViewModel.ReadyToSearch = false;
                    break;
                case Key.Enter:
                    e.Handled = true;
                    ViewModel.FocusSelectedProcess();
                    break;
                case Key.Tab:
                    e.Handled = true;
                    break;
                case Key.Up:
                    e.Handled = true;
                    ViewModel.SelectPrevious();
                    break;
                case Key.Down:
                    e.Handled = true;
                    ViewModel.SelectNext();
                    break;
            }
        }
    }
}
