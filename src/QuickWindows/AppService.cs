namespace QuickWindows
{
    public class AppService : IAppService
    {
        public AppService(IProcessService processService, IShortcutService shortcutService)
        {
            ProcessService = processService;
            ShortcutService = shortcutService;
        }

        public IProcessService ProcessService { get; }
        public IShortcutService ShortcutService { get; }
    }
}
