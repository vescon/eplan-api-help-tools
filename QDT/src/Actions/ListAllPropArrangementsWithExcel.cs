using System.Linq;
using System.Text;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class ListAllPropArrangementsWithExcel : ActionBase, IEplAction
    {
        public override string MenuCaption => "List all property arrangements (and open in Excel)";
        public override string Description => "Lists all property placement schemas of all project's symbol references to a file and opens the file in Excel.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var output = PrepareOutput();
                if (output != null)
                    SaveOutput(output, true);

                return true;
            });
        }

        private StringBuilder PrepareOutput()
        {
            var selectionSet = new SelectionSet();

            var prjs = selectionSet.SelectedProjects
                .Select(x => new
                {
                    Project = x,
                    Page = x.Pages.FirstOrDefault(y => y.PageType == DocumentTypeManager.DocumentType.Circuit
                                                       || y.PageType == DocumentTypeManager.DocumentType.CircuitSingleLine)
                })
                .Where(x => x.Page != null)
                .ToList();

            if (prjs.Count == 0)
            {
                ShowWarning("Please select one or more projects containing a circuit page.");
                return null;
            }

            var output = new StringBuilder();

            foreach (var x in prjs)
            {
                x.Project.LockAllObjects();

                if (!WriteOutput(output, x.Project, x.Page))
                    return null; // cancelled

                output.AppendLine("");
            }

            return output;
        }

        private bool WriteOutput(StringBuilder output, Project prj, Page page)
        {
            output.AppendLine($"Project:\t{prj.ProjectName}");
            output.AppendLine("Symbol\tVariant\tProperty arrangement schema\tUser defined");

            SymbolReference symbolRef = null;
            try
            {
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
                            foreach (var a in symbolRef.PropertyPlacementsSchemas.All)
                            {
                                output.AppendLine(
                                    string.Format(
                                        "{0}\t{1}\t{2}\t{3}",
                                        x.Parent.Name,
                                        x.VariantNr,
                                        a.Name,
                                        a.IsCustom));
                            }
                        }
                    }) == false)
                    return false; // cancelled
            }
            finally
            {
                if (symbolRef != null)
                    symbolRef.Remove();
            }

            return true;
        }
    }
}