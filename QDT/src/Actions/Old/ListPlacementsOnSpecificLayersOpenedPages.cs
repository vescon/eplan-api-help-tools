using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions.Old
{
    public class ListPlacementsOnSpecificLayersOpenedPages : ListPlacementsOnSpecificLayersBase, IEplAction
    {
        public override string MenuCaption => "List placements to a file";
        public override string Description => "Lists placements existing on the given layers to a file (for pages opened in GED).";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var pages = new SelectionSet().OpenedPages;
                if (pages == null
                    || pages.Length == 0)
                {
                    ShowWarning("There are no any opened pages.");
                    return true;
                }

                AnalyzePages(pages, null);

                return true;
            });
        }
    }
}