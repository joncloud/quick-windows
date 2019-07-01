namespace QuickWindows
{
    public interface IAppService
    {
        IProcessService ProcessService { get; }
        IShortcutService ShortcutService { get; }
    }
}
