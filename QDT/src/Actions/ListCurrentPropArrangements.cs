using System;
using System.Linq;
using System.Text;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.MasterData;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class ListCurrentPropArrangements : ActionBase, IEplAction
    {
        public override string MenuCaption => "List current property arrangements";
        public override string Description => "Lists current property arrangements of project's symbol references to a file.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var output = PrepareOutput(out var projectName);

                if (output != null 
                    && SaveOutput(output, false))
                {
                    var msg = string.Format(
                        "Export finished successfully." + Environment.NewLine + 
                        "Project: '{0}'",
                        projectName);
                    ShowInfo(msg);
                }

                return true;
            });
        }

        public StringBuilder PrepareOutput()
        {
            return PrepareOutput(out _);
        }

        public StringBuilder PrepareOutput(out string projectName)
        {
            projectName = "";

            var prj = new SelectionSet().GetCurrentProject(false);
            if (prj == null)
            {
                ShowWarning("There's no any opened project.");
                return null;
            }

            projectName = prj.ProjectName;

            var output = new StringBuilder();
            
            output.AppendLine(JoinedWithTabs(
                "Project:",
                prj.ProjectName));
            
            output.AppendLine(JoinedWithTabs(
                "Symbol",
                "Location",
                "Property placements schema",
                "Function",
                "Placement type"));

            var ok = new ActionWithSimpleProgress(prj, "Analyzing placements...")
                .IterateOverPages(page =>
                {
                    page.AllPlacements
                        .OfType<SymbolReference>()
                        .Where(x => IsAllowedSymbol(x.SymbolVariant))
                        .Select(x => new
                        {
                            SymbolName = x.SymbolVariant.SymbolName,

                            FunctionLocation = x.Properties[19007].ToString(),
                            
                            FunctionName = (x as FunctionBase)?.Name ?? "",
                            
                            FunctionPlacementType = AsString((x as Function)?.ManualPlacementType),
                            
                            PropPlacementsSchemaName = GetPropPlacementsSchemaName(x)
                        })
                        .OrderBy(x => x.FunctionLocation)
                        .ForEach(x =>
                        {
                            output.AppendLine(JoinedWithTabs(
                                x.SymbolName, 
                                ApostrophePrefixed(x.FunctionLocation), 
                                x.PropPlacementsSchemaName, 
                                ApostrophePrefixed(x.FunctionName),
                                x.FunctionPlacementType));
                        });
                });

            return ok 
                ? output 
                : null; // cancelled
        }

        private string ApostrophePrefixed(string text)
        {
            return text.IsEmpty() ? "" : "'" + text;
        }

        private string AsString(DocumentTypeManager.DocumentType? documentType)
        {
            return documentType != null 
                ? $"({(int)documentType.Value}) {documentType.Value}" 
                : "";
        }

        private string JoinedWithTabs(params string[] items)
        {
            if (items.Length == 0)
                return "";

            var sb = new StringBuilder();
            
            sb.Append(items[0] ?? "");
            
            items
                .Skip(1)
                .ForEach(x => sb.Append("\t" + x));
            
            return sb.ToString();
        }

        private string GetPropPlacementsSchemaName(SymbolReference symbolRef)
        {
            var schemaName = symbolRef.PropertyPlacementsSchemas.Selected.Name;
            
            if (schemaName == null)
            {
                schemaName = symbolRef.PropertyPlacementsSchemas.Selected.ID == 128
                     ? "User-defined (id: 128)"
                     : "<null>";
            }

            return schemaName;
        }

        private bool IsAllowedSymbol(SymbolVariant symbolVariant)
        {
            if (symbolVariant.SymbolLibraryName != "SPECIAL")
                return true;

            var symbolName = symbolVariant.SymbolName;
            return symbolName != "CO"
                && symbolName != "ACBP"
                && symbolName != "TLRO"
                && symbolName != "TLRU"
                && symbolName != "TOUR"
                && symbolName != "TOUL"
                && symbolName != "TS"
                && symbolName != "CR"
                && symbolName != "CRL"
                && symbolName != "BR";
        }
    }
}
