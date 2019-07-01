using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WinFormsKeys = System.Windows.Forms.Keys;

namespace QuickWindows
{
    public struct KeyStroke : IEquatable<KeyStroke>
    {
        public ModifierKeys ModifierKeys { get; }
        public Key Key { get; }

        public KeyStroke(Key key)
        {
            ModifierKeys = ModifierKeys.None;
            Key = key;
        }

        public KeyStroke(ModifierKeys modifierKeys, Key key)
        {
            ModifierKeys = modifierKeys;
            Key = key;
        }

        public bool Equals(KeyStroke other) =>
            ModifierKeys == other.ModifierKeys &&
            Key == other.Key;

        public override bool Equals(object obj) =>
            obj is KeyStroke other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(
                ModifierKeys,
                Key
            );

        static readonly Dictionary<WinFormsKeys, Key> _map;
        static readonly Dictionary<Key, WinFormsKeys> _reverseMap;

        static KeyStroke()
        {
            _map = new Dictionary<WinFormsKeys, Key>
            {
                [WinFormsKeys.None] = Key.None,
                [WinFormsKeys.Cancel] = Key.Cancel,
                [WinFormsKeys.Back] = Key.Back,
                [WinFormsKeys.Tab] = Key.Tab,
                [WinFormsKeys.LineFeed] = Key.LineFeed,
                [WinFormsKeys.Clear] = Key.Clear,
                [WinFormsKeys.Return] = Key.Return,
                [WinFormsKeys.Pause] = Key.Pause,
                [WinFormsKeys.Capital] = Key.Capital,
                [WinFormsKeys.KanaMode] = Key.KanaMode,
                [WinFormsKeys.JunjaMode] = Key.JunjaMode,
                [WinFormsKeys.FinalMode] = Key.FinalMode,
                [WinFormsKeys.HanjaMode] = Key.HanjaMode,
                [WinFormsKeys.Escape] = Key.Escape,
                [WinFormsKeys.Space] = Key.Space,
                [WinFormsKeys.PageUp] = Key.PageUp,
                [WinFormsKeys.Next] = Key.Next,
                [WinFormsKeys.End] = Key.End,
                [WinFormsKeys.Home] = Key.Home,
                [WinFormsKeys.Left] = Key.Left,
                [WinFormsKeys.Up] = Key.Up,
                [WinFormsKeys.Right] = Key.Right,
                [WinFormsKeys.Down] = Key.Down,
                [WinFormsKeys.Select] = Key.Select,
                [WinFormsKeys.Print] = Key.Print,
                [WinFormsKeys.Execute] = Key.Execute,
                [WinFormsKeys.PrintScreen] = Key.Snapshot,
                [WinFormsKeys.Insert] = Key.Insert,
                [WinFormsKeys.Delete] = Key.Delete,
                [WinFormsKeys.Help] = Key.Help,
                [WinFormsKeys.D0] = Key.D0,
                [WinFormsKeys.D1] = Key.D1,
                [WinFormsKeys.D2] = Key.D2,
                [WinFormsKeys.D3] = Key.D3,
                [WinFormsKeys.D4] = Key.D4,
                [WinFormsKeys.D5] = Key.D5,
                [WinFormsKeys.D6] = Key.D6,
                [WinFormsKeys.D7] = Key.D7,
                [WinFormsKeys.D8] = Key.D8,
                [WinFormsKeys.D9] = Key.D9,
                [WinFormsKeys.A] = Key.A,
                [WinFormsKeys.B] = Key.B,
                [WinFormsKeys.C] = Key.C,
                [WinFormsKeys.D] = Key.D,
                [WinFormsKeys.E] = Key.E,
                [WinFormsKeys.F] = Key.F,
                [WinFormsKeys.G] = Key.G,
                [WinFormsKeys.H] = Key.H,
                [WinFormsKeys.I] = Key.I,
                [WinFormsKeys.J] = Key.J,
                [WinFormsKeys.K] = Key.K,
                [WinFormsKeys.L] = Key.L,
                [WinFormsKeys.M] = Key.M,
                [WinFormsKeys.N] = Key.N,
                [WinFormsKeys.O] = Key.O,
                [WinFormsKeys.P] = Key.P,
                [WinFormsKeys.Q] = Key.Q,
                [WinFormsKeys.R] = Key.R,
                [WinFormsKeys.S] = Key.S,
                [WinFormsKeys.T] = Key.T,
                [WinFormsKeys.U] = Key.U,
                [WinFormsKeys.V] = Key.V,
                [WinFormsKeys.W] = Key.W,
                [WinFormsKeys.X] = Key.X,
                [WinFormsKeys.Y] = Key.Y,
                [WinFormsKeys.Z] = Key.Z,
                [WinFormsKeys.LWin] = Key.LWin,
                [WinFormsKeys.RWin] = Key.RWin,
                [WinFormsKeys.Apps] = Key.Apps,
                [WinFormsKeys.Sleep] = Key.Sleep,
                [WinFormsKeys.NumPad0] = Key.NumPad0,
                [WinFormsKeys.NumPad1] = Key.NumPad1,
                [WinFormsKeys.NumPad2] = Key.NumPad2,
                [WinFormsKeys.NumPad3] = Key.NumPad3,
                [WinFormsKeys.NumPad4] = Key.NumPad4,
                [WinFormsKeys.NumPad5] = Key.NumPad5,
                [WinFormsKeys.NumPad6] = Key.NumPad6,
                [WinFormsKeys.NumPad7] = Key.NumPad7,
                [WinFormsKeys.NumPad8] = Key.NumPad8,
                [WinFormsKeys.NumPad9] = Key.NumPad9,
                [WinFormsKeys.Multiply] = Key.Multiply,
                [WinFormsKeys.Add] = Key.Add,
                [WinFormsKeys.Separator] = Key.Separator,
                [WinFormsKeys.Subtract] = Key.Subtract,
                [WinFormsKeys.Decimal] = Key.Decimal,
                [WinFormsKeys.Divide] = Key.Divide,
                [WinFormsKeys.F1] = Key.F1,
                [WinFormsKeys.F2] = Key.F2,
                [WinFormsKeys.F3] = Key.F3,
                [WinFormsKeys.F4] = Key.F4,
                [WinFormsKeys.F5] = Key.F5,
                [WinFormsKeys.F6] = Key.F6,
                [WinFormsKeys.F7] = Key.F7,
                [WinFormsKeys.F8] = Key.F8,
                [WinFormsKeys.F9] = Key.F9,
                [WinFormsKeys.F10] = Key.F10,
                [WinFormsKeys.F11] = Key.F11,
                [WinFormsKeys.F12] = Key.F12,
                [WinFormsKeys.F13] = Key.F13,
                [WinFormsKeys.F14] = Key.F14,
                [WinFormsKeys.F15] = Key.F15,
                [WinFormsKeys.F16] = Key.F16,
                [WinFormsKeys.F17] = Key.F17,
                [WinFormsKeys.F18] = Key.F18,
                [WinFormsKeys.F19] = Key.F19,
                [WinFormsKeys.F20] = Key.F20,
                [WinFormsKeys.F21] = Key.F21,
                [WinFormsKeys.F22] = Key.F22,
                [WinFormsKeys.F23] = Key.F23,
                [WinFormsKeys.F24] = Key.F24,
                [WinFormsKeys.NumLock] = Key.NumLock,
                [WinFormsKeys.Scroll] = Key.Scroll,
                [WinFormsKeys.BrowserBack] = Key.BrowserBack,
                [WinFormsKeys.BrowserForward] = Key.BrowserForward,
                [WinFormsKeys.BrowserRefresh] = Key.BrowserRefresh,
                [WinFormsKeys.BrowserStop] = Key.BrowserStop,
                [WinFormsKeys.BrowserSearch] = Key.BrowserSearch,
                [WinFormsKeys.BrowserFavorites] = Key.BrowserFavorites,
                [WinFormsKeys.BrowserHome] = Key.BrowserHome,
                [WinFormsKeys.VolumeMute] = Key.VolumeMute,
                [WinFormsKeys.VolumeDown] = Key.VolumeDown,
                [WinFormsKeys.VolumeUp] = Key.VolumeUp,
                [WinFormsKeys.MediaNextTrack] = Key.MediaNextTrack,
                [WinFormsKeys.MediaPreviousTrack] = Key.MediaPreviousTrack,
                [WinFormsKeys.MediaStop] = Key.MediaStop,
                [WinFormsKeys.MediaPlayPause] = Key.MediaPlayPause,
                [WinFormsKeys.LaunchMail] = Key.LaunchMail,
                [WinFormsKeys.SelectMedia] = Key.SelectMedia,
                [WinFormsKeys.LaunchApplication1] = Key.LaunchApplication1,
                [WinFormsKeys.LaunchApplication2] = Key.LaunchApplication2,
                [WinFormsKeys.Oem1] = Key.Oem1,
                [WinFormsKeys.OemMinus] = Key.OemMinus,
                [WinFormsKeys.OemPeriod] = Key.OemPeriod,
                [WinFormsKeys.OemQuestion] = Key.OemQuestion,
                [WinFormsKeys.Oemtilde] = Key.Oem3,
                [WinFormsKeys.OemOpenBrackets] = Key.OemOpenBrackets,
                [WinFormsKeys.Oem5] = Key.Oem5,
                [WinFormsKeys.Oem6] = Key.Oem6,
                [WinFormsKeys.Oem7] = Key.OemQuotes,
                [WinFormsKeys.Oem8] = Key.Oem8,
                [WinFormsKeys.OemBackslash] = Key.OemBackslash,
                [WinFormsKeys.Attn] = Key.DbeNoRoman,
                [WinFormsKeys.EraseEof] = Key.EraseEof,
                [WinFormsKeys.Play] = Key.Play,
                [WinFormsKeys.Zoom] = Key.DbeNoCodeInput,
                [WinFormsKeys.NoName] = Key.NoName,
                [WinFormsKeys.Pa1] = Key.Pa1,
                [WinFormsKeys.OemClear] = Key.OemClear
            };

            _reverseMap = new Dictionary<Key, WinFormsKeys>(
                _map.Select(pair => new KeyValuePair<Key, WinFormsKeys>(pair.Value, pair.Key))
            );
        }

        public WinFormsKeys GetWinFormsKey() =>
            _reverseMap[Key];

        public static bool TryConvert(KeyPressedEventArgs args, out KeyStroke value)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            if (_map.TryGetValue(args.Key, out var key))
            {
                value = new KeyStroke(args.Modifier, key);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
