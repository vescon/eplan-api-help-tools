using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class ListPlacementsOnSpecificLayersWholeProject : ListPlacementsOnSpecificLayersBase, IEplAction
    {
        public override string MenuCaption => "List placements on given layers (all project's pages)";
        public override string Description => "Lists placements existing on the given layers to a file.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var project = new SelectionSet().GetCurrentProject(false);
                if (project == null)
                {
                    ShowWarning("There's no any opened project.");
                    return true;
                }

                var pages = project.Pages;

                if (pages == null
                    || pages.Length == 0)
                {
                    ShowWarning("The project has no pages.");
                    return true;
                }

                AnalyzePages(pages, project);

                return true;
            });
        }
    }
}