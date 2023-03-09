using System.Collections.Generic;
using System.Linq;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class RemoveAllCustomPropArrangements : ActionBase, IEplAction
    {
        public override string MenuCaption => "Remove all custom property arrangements";
        public override string Description => "Removes all user defined property arrangements from the project.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                RemoveAllCustomSchemas();
                return true;
            });
        }

        public void RemoveAllCustomSchemas()
        {
            var prj = new SelectionSet().GetCurrentProject(false);
            if (prj == null)
            {
                ShowWarning("There's no any opened project.");
                return;
            }

            var page = prj.Pages.FirstOrDefault(x => x.PageType == DocumentTypeManager.DocumentType.Circuit
                                                     || x.PageType == DocumentTypeManager.DocumentType.CircuitSingleLine);
            if (page == null)
            {
                ShowWarning("The project doesn't contain a schematic page.");
                return;
            }

            SymbolReference symbolRef = null;
            try
            {
                var customArrangements = new List<string>();

                if (new ActionWithSimpleProgress(prj)
                    .IterateOverProjectSymbols(symbol =>
                    {
                        foreach (var x in symbol.Variants)
                        {
                            if (symbolRef == null)
                            {
                                symbolRef = new SymbolReference();
                                symbolRef.Create(page, x);
                            }

                            symbolRef.SymbolVariant = x;
                            symbolRef.PropertyPlacementsSchemas.All
                                .Where(a => a.IsCustom)
                                .Select(a => a.Name)
                                .ForEach(customArrangements.AddIfNotExists);
                        }
                    }) == false)
                    return; // cancelled

                if (customArrangements.Count == 0)
                {
                    ShowWarning("Custom property arrangements have not been found in the project.");
                    return;
                }

                var msg = $"{customArrangements.Count} custom property arrangement(s) have been found in the project:";
                for (var i = 0; i < customArrangements.Count && i < 10; ++i)
                    msg += "\n\t- " + customArrangements[i];
                if (customArrangements.Count > 10)
                    msg += "\n\t...";
                msg += "\n\nDo you want to remove them ?";
                if (!Ask(msg))
                    return; // cancelled

                new ActionWithSimpleProgress(prj)
                    .IterateOverProjectSymbols(symbol =>
                    {
                        foreach (var x in symbol.Variants)
                        {
                            symbolRef.SymbolVariant = x;
                            var customSchemas = symbolRef.PropertyPlacementsSchemas.All.Where(a => a.IsCustom);
                            foreach (var a in customSchemas)
                            {
                                symbolRef.PropertyPlacementsSchemas.Remove(a);
                            }
                        }
                    });
            }
            finally
            {
                if (symbolRef != null)
                    symbolRef.Remove();
            }
        }
    }
}