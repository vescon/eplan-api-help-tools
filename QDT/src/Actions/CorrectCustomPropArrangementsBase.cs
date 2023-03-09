using System.Linq;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public abstract class CorrectCustomPropArrangementsBase : ActionBase
    {
        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var prj = new SelectionSet().GetCurrentProject(false);
                if (prj == null)
                {
                    ShowWarning("There's no any opened project.");
                    return true;
                }

                if (!Ask($"Are you sure to change custom property arrangements in the project '{prj.ProjectName}' ?"))
                    return true; // cancelled

                var changedCount = 0;

                var ok = new ActionWithSimpleProgress(prj, "Correcting custom property arrangements...")
                    .IterateOverPages(page =>
                    {
                        var symbolRefs = page.AllPlacements.OfType<SymbolReference>().ToList();
                        var symbolRefsWithUserDefinedSchemas = symbolRefs
                            .Where(x =>
                            {
                                var schemas = x.PropertyPlacementsSchemas;
                                return schemas.Selected.ID == 128;
                            });

                        foreach (var symbolRef in symbolRefsWithUserDefinedSchemas)
                        {
                            if (TryCorrectSymbol(symbolRef))
                            {
                                ++changedCount;
                                if (symbolRef is FunctionBase)
                                    new BaseException(
                                        string.Format(
                                            "Prop. arrangement '{0}' set on '{1}'.",
                                            symbolRef.PropertyPlacementsSchemas.Selected.Name,
                                            ((FunctionBase)symbolRef).Name),
                                        MessageLevel.Message).FixMessage();
                                else
                                    new BaseException(
                                        string.Format(
                                            "Prop. arrangement '{0}' set.",
                                            symbolRef.PropertyPlacementsSchemas.Selected.Name),
                                        MessageLevel.Message).FixMessage();
                            }
                        }
                    });

                var msg = ok 
                    ? "Correction process performed successfully."
                    : "Correction process cancelled.";
                ShowInfo(msg + $"\nChanged objects count: {changedCount}");

                return true;
            });
        }

        protected abstract bool TryCorrectSymbol(SymbolReference symbolRef);
    }
}