using System.ComponentModel;

namespace QuickWindows
{
    public enum AppAction
    {
        None,

        [Description("Activate")]
        ActivateApp,

        [Description("Next")]
        NextProcess,

        [Description("Previous")]
        PreviousProcess,

        [Description("Focus")]
        FocusProcess,

        [Description("Deactivate")]
        DeactivateApp,
        ManageShortcuts
    }
}
