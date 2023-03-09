using System.Linq;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel.Graphics;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class RemoveUnusedPlaceholdersVariables : ActionBase, IEplAction
    {
        public override string MenuCaption => "Remove unused placeholders' variables...";
        public override string Description => "Removes unused placeholders' variables in the project.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var selectionSet = new SelectionSet();

                var prjs = selectionSet.SelectedProjects
                    .ToList();

                if (prjs.Count == 0)
                {
                    ShowWarning("Please select one or more projects.");
                    return true;
                }

                var msg = prjs.Count == 1 
                    ? $"Unused placeholders' variables in the project '{prjs[0].ProjectName}' will be removed.\n\nAre you sure to continue ?"
                    : $"Unused placeholders' variables in the {prjs.Count} selected projects.\n\nAre you sure to continue ?";
                
                if (!Ask(msg))
                    return true; // cancelled

                var changedCount = 0;

                var ok = false;
                foreach (var prj in prjs)
                {
                    prj.LockAllObjects();

                    ok = new ActionWithSimpleProgress(prj, "Removing unused placeholders' variables...")
                        .IterateOverPages(page =>
                        {
                            var placeHolders = page.AllPlacements.OfType<PlaceHolder>().ToList();

                            foreach (var p in placeHolders)
                            {
                                var countBefore = p.NumberOfVariables;
                                p.DeleteUnusedVariables();
                                var countAfter = p.NumberOfVariables;

                                if (countAfter < countBefore)
                                    ++changedCount;
                            }
                        });

                    if (!ok)
                        break;
                }

                msg = ok 
                    ? $"Done.\nChanged placeholders count: {changedCount}."
                    : $"Action cancelled.\nChanged placeholders count: {changedCount}.";
                ShowInfo(msg);

                return true;
            });
        }
    }
}