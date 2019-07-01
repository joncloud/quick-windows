using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuickWindows
{
    class NamedPipesSingletonService : ISingletonService
    {
        readonly IShortcutService _shortcutService;
        public NamedPipesSingletonService(IShortcutService shortcutService)
        {
            _shortcutService = shortcutService;
        }

        void Activate()
        {
            // TODO this is janky, it really should be more specific than relying on something user configurable.
            var keyStroke = _shortcutService.GetShortcuts()
                .Where(tuple => tuple.Item2 == AppAction.ActivateApp)
                .Select(tuple => tuple.Item1)
                .FirstOrDefault();

            _shortcutService.TryRequestShortcut(keyStroke);
        }

        async void ListenAsync(CancellationToken cancellationToken)
        {
            do
            {
                // TODO Kinda janky to use a connection as the way to activate,
                // but it is the bare-minimum message.
                await _server.WaitForConnectionAsync(cancellationToken);
                _server.Disconnect();
                await App.Current.Dispatcher.BeginInvoke(new Action(Activate));
            } while (!cancellationToken.IsCancellationRequested);
        }

        NamedPipeServerStream _server;
        CancellationTokenSource _cts;
        public bool TryActivate()
        {
            try
            {
                _server = new NamedPipeServerStream("QuickWindows", PipeDirection.InOut, 1);
                _cts = new CancellationTokenSource();
                App.Current.Exit += Current_Exit;
                Task.Run(() => ListenAsync(_cts.Token), _cts.Token);
                return true;
            }
            catch (IOException)
            {
                using (var client = new NamedPipeClientStream(".", "QuickWindows", PipeDirection.Out))
                {
                    client.Connect(200);
                    Thread.Sleep(200);
                }

                return false;
            }
        }

        private void Current_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
