using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.Graphics;
using Vescon.EplAddin.Qdt.Dialogs;

namespace Vescon.EplAddin.Qdt.Actions
{
    public abstract class ListPlacementsOnSpecificLayersBase : ActionBase
    {
        protected void AnalyzePages(IEnumerable<Page> pages, Project project)
        {
            if (!AskForLayersNames(
                    out var layersToInspect, 
                    out var openInExcel))
                return;

            var output = new StringBuilder();

            if (project != null)
                output.AppendLine($"Project:\t{project.ProjectName}");

            BeginOutput(output, layersToInspect);

            var pagesWithBlocks = new List<string>();
            var counter = 0;

            new ActionWithSimpleProgress(project, "Searching placements...")
                .IterateOverPages(
                    pages.ToList(),
                    page =>
                    {
                        var c = SearchObjects(page, output, layersToInspect, out var hasBlocks);
                        counter += c;

                        if (hasBlocks)
                            pagesWithBlocks.Add(page.IdentifyingName);
                    });

            FinishOutput(output, counter, pagesWithBlocks);

            SaveOutput(output, openInExcel);
        }

        private int SearchObjects(Page page, StringBuilder output, ICollection<string> layersToInspect, out bool hasBlocks)
        {
            var countOfObjectsFound = 0;

            var allPlacements = page.AllPlacements;
            var graphicalPlacements = allPlacements.OfType<GraphicalPlacement>().ToList();
            var functions = allPlacements.OfType<Function>().ToList();

            graphicalPlacements
                .Where(x => x.Layer != null && layersToInspect.Contains(x.Layer.Name.ToLowerInvariant()))
                .ForEach(x =>
                {
                    ++countOfObjectsFound;
                    OutputLine(page, x, x.Layer.Name, output);
                });

            functions.ForEach(x =>
            {
                var graphics = new List<GraphicalPlacement>();
                GetRecursive(new[] {x.GetGraphics()}, graphics);

                graphics
                    .Where(y => y.Layer != null && layersToInspect.Contains(y.Layer.Name.ToLowerInvariant()))
                    .ForEach(y =>
                    {
                        ++countOfObjectsFound;
                        OutputLine(page, x, y.Layer.Name, output);
                    });
            });

            hasBlocks = allPlacements.Any(x => x is Block);
            
            return countOfObjectsFound;
        }

        private void BeginOutput(StringBuilder output, List<string> layersToInspect)
        {
            output.Append("Layers:");
            layersToInspect.ForEach(x => output.Append("\t" + x));
            output.AppendLine();
            output.AppendLine();
            output.AppendLine("Page\tLayer\tObject\tX\tY");
        }

        private void FinishOutput(StringBuilder output, int counter, List<string> pagesWithBlocks)
        {
            output.AppendLine();
            output.AppendLine($"{counter} object(s) found.");

            pagesWithBlocks
                .ForEach(x => output.AppendLine($"Page '{x}' contains BLOCKS. Checking placements in BLOCKS is not supported now !"));
        }

        private bool AskForLayersNames(out List<string> layersToInspect, out bool openInExcel)
        {
            layersToInspect = null;
            openInExcel = false;

            var settings = Cfg.Settings.GetData("ActionSearchPlacementsOnSpecificLayers");

            var dlg = new InputBox
            {
                Text = Addin.Title,
                Caption = "Enter coma separated layers' names:",
                Input = settings.Get("Input", string.Empty)
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            settings.Insert("Input", dlg.Input);

            layersToInspect = dlg.Input.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLowerInvariant().Trim())
                .ToList();

            if (layersToInspect.Any() == false)
            {
                ShowWarning("Layer names were not specified.");
                return false;
            }

            openInExcel = dlg.OpenInExcel;
            
            return true;
        }

        private void OutputLine(Page page, Placement x, string layerName, StringBuilder output)
        {
            var line = string.Format(
                "{0}\t{1}\t{2}\t={3}\t={4}",
                page.IdentifyingName,
                layerName,
                x.GetType().Name,
                x.Location.X,
                x.Location.Y);

            output.AppendLine(line);
        }

        private void GetRecursive(IEnumerable<Placement> placements, ICollection<GraphicalPlacement> graphics)
        {
            if (placements == null)
                return;

            foreach (var x in placements)
            {
                if (x is Group group)
                    GetRecursive(group.SubPlacements, graphics);
                else if (x is GraphicalPlacement graphicalPlacement)
                    graphics.Add(graphicalPlacement);
            }
        }
    }
}