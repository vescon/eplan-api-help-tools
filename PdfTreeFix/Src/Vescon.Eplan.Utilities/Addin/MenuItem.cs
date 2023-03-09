namespace Vescon.Eplan.Utilities.Addin
{
    public struct MenuItem
    {
        public MenuItem(string menuCaption, string actionName, string actionDescription)
            : this()
        {
            MenuCaption = menuCaption;
            ActionName = actionName;
            ActionDescription = actionDescription;
        }

        public string MenuCaption { get; }
        public string ActionName { get; }
        public string ActionDescription { get; }
        public bool SeparatorBefore { get; set; }
    }
}