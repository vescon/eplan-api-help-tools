using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using PlaceholderConfigLoader.EplAddin;

namespace PlaceholderConfigLoader
{
    public class Addin : AddinBase<Addin>, IEplAddIn
    {
        public Addin()
        {
            IsSingleAction = true;
        }

        public static string Title => "Placeholder's Configuration Loader";

        protected override string GetTitle()
        {
            return Title;
        }

        protected override MenuItem GetMenuItem()
        {
            return new MenuItem(
                LoadAction.MenuCaption,
                LoadAction.Name,
                LoadAction.Description);
        }

        protected override void OnProjectOpen(Project project)
        {
        }

        protected override void OnEplanStart()
        {
        }

        protected override void OnRegistered()
        {
        }

        protected override void OnUnRegistered()
        {
        }

        protected override void OnInitialized()
        {
        }
    }
}