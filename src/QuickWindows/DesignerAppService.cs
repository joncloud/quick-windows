namespace QuickWindows
{
    public class DesignerAppService : IAppService
    {
        public IProcessService ProcessService { get; }
        public IShortcutService ShortcutService { get; }

        public DesignerAppService()
        {
            ProcessService = new DesignerProcessService();
            ShortcutService = new DesignerShortcutService();
        }
    }
}
