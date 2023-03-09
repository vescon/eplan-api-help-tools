using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class PdfExportFromMultipleProject : ActionBase, IEplAction
    {
        public override string MenuCaption => "PDF export of selected projects...";
        public override string Description => "Executes PDF export from one or more selected projects.";

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

                var outputPath = GetOutputPath(prjs.Count);
                if (outputPath == null) 
                    return true;

                var actionsToRun = prjs
                    .Select(prj => new Action<Progress>(prgrss =>
                    {
                        prgrss.SetOverallActionText(prj.ProjectName);

                        var fileName = Path.Combine(outputPath, $"{prj.ProjectName}.pdf");

                        new Export().PdfProject(
                            prj, 
                            string.Empty, // schema
                            fileName,
                            Export.DegreeOfColor.FromScheme,
                            true,
                            "",
                            true);
                    }))
                    .ToArray();

                var ok = new ActionWithSimpleProgress(null, null, true)
                    .RunSteps(actionsToRun);

                var msg = ok 
                    ? $"{prjs.Count} project(s) exported to the folder:\n'{outputPath}'." 
                    : "Action cancelled.";
                ShowInfo(msg);

                return true;
            });
        }

        private static string GetOutputPath(int count)
        {
            var settings = Cfg.Settings.GetData(nameof(PdfExportFromMultipleProject));

            var dlg = new FolderBrowserDialog
            {
                Description = $@"{count} project(s) selected."
                    + Environment.NewLine
                    + Environment.NewLine
                    + @"Select an output folder for the PDF export:",
                
                SelectedPath = settings.Get("OutputPath", string.Empty)
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return null;

            settings.Insert("OutputPath", dlg.SelectedPath);
            
            Cfg.Settings.Save();

            return dlg.SelectedPath;
        }
    }
}