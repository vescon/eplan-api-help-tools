namespace Vescon.Eplan.Utilities.Addin
{
    public struct ToolBarButtonInfo
    {
        public ToolBarButtonInfo(string actionPath, string iconPath, string toolTip)
            : this()
        {
            ActionName = actionPath;
            IconPath = iconPath;
            TooTip = toolTip;
        }

        public string ActionName { get; }
        public string IconPath { get; }
        public string TooTip { get; }
    }
}