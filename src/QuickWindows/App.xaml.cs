using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuickWindows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var processService = new ProcessService();
            var shortcutService = new ShortcutService();
            var singletonService = new NamedPipesSingletonService(shortcutService);
            var appService = new AppService(
                processService,
                shortcutService
            );

            if (!singletonService.TryActivate())
            {
                Shutdown();
                return;
            }

            // TODO make this load from disk.
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(ModifierKeys.Control | ModifierKeys.Shift, Key.C),
                AppAction.ActivateApp
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(Key.Escape), AppAction.DeactivateApp
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(Key.Enter), AppAction.FocusProcess
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(Key.Up), AppAction.PreviousProcess
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(ModifierKeys.Control, Key.I), AppAction.PreviousProcess
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(Key.Down), AppAction.NextProcess
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(ModifierKeys.Control, Key.K), AppAction.NextProcess
            );
            appService.ShortcutService.TryAddShortcut(
                new KeyStroke(ModifierKeys.Control | ModifierKeys.Shift, Key.O), AppAction.ManageShortcuts
            );

            var window = new MainWindow(appService);
            window.Show();
        }
    }
}
