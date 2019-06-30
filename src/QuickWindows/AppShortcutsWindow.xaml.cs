using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuickWindows
{
    /// <summary>
    /// Interaction logic for AppShortcutsWindow.xaml
    /// </summary>
    public partial class AppShortcutsWindow : Window
    {
        AppShortcutsViewModel ViewModel => DataContext as AppShortcutsViewModel;

        public AppShortcutsWindow()
        {
            InitializeComponent();
        }

        Key GetKeyIgnoringModifiers(ModifierKeys modifierKeys, Key key)
        {
            if (modifierKeys.HasFlag(ModifierKeys.Alt))
            {
                if (key == Key.LeftAlt) return Key.None;
                if (key == Key.RightAlt) return Key.None;
            }
            if (modifierKeys.HasFlag(ModifierKeys.Control))
            {
                if (key == Key.LeftCtrl) return Key.None;
                if (key == Key.RightCtrl) return Key.None;
            }
            if (modifierKeys.HasFlag(ModifierKeys.Shift))
            {
                if (key == Key.LeftShift) return Key.None;
                if (key == Key.RightShift) return Key.None;
            }
            if (modifierKeys.HasFlag(ModifierKeys.Windows))
            {
                if (key == Key.LWin) return Key.None;
                if (key == Key.RWin) return Key.None;
            }
            return key;
        }

        void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var keyStroke = new KeyStroke(
                Keyboard.Modifiers,
                GetKeyIgnoringModifiers(Keyboard.Modifiers, e.Key)
            );

            ViewModel.Builder.SetKeyStroke(keyStroke);
        }
    }
}
