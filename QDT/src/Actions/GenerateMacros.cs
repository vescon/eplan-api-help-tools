using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using Vescon.EplAddin.Qdt.Dialogs;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class GenerateMacros : ActionBase, IEplAction
    {
        public override string MenuCaption => "Generate macros...";
        public override string Description => "Generate selected types of macros.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var selectionSet = new SelectionSet();
                var prjs = selectionSet.SelectedProjects;
                if (prjs.Length == 0)
                {
                    ShowWarning("Please select a project or pages to generate macros from.");
                    return true;
                }

                var selectedPages = selectionSet.GetSelectedPages();
                bool allPagesSelected = selectedPages.Length == 0
                   || (prjs.Length == 1 && selectedPages.Length == prjs[0].Pages.Length);

                var dlg = new SelectMacroTypes
                {
                    Text = prjs.Length == 1 
                        ? $@"Generate macros from project: '{prjs[0].ProjectName}'."
                        : $@"Generate macros from {prjs.Length} selected projects."
                };

                dlg.EnableScopeSelection(!allPagesSelected);

                if (dlg.ShowDialog() != DialogResult.OK)
                    return true;

                var pageNames = allPagesSelected || dlg.CompleteProject
                    ? null 
                    : new HashSet<string>(selectedPages.Select(x => x.IdentifyingName));

                prjs.ForEach(x => DoGenerate(
                    x, 
                    dlg.PageMacros, 
                    dlg.WindowMacros, 
                    dlg.SymbolMacros,
                    pageNames));

                return true;
            });
        }

        private void DoGenerate(
            Project prj, 
            bool pageMacros, 
            bool windowMacros, 
            bool symbolMacros,
            ISet<string> pageNames)
        {
            using (new LockingStep())
            using (var tmpDir = new TemporaryDirectory())
            using (var prjManager = new ProjectManager())
            {
                var projectElkFileName = tmpDir.Path + "\\__macros.elk";

                Project prjCopy = null;

                new ActionWithSimpleProgress(
                        prj,
                        "Generating macros...",
                        true)
                    .OnError(ex =>
                    {
                        prjCopy?.Close();
                    })
                    .RunSteps(
                        p =>
                        {
                            p.SetActionText("Copying project...");

                            prjManager.CopyProject(
                                prj.ProjectLinkFilePath,
                                projectElkFileName,
                                ProjectManager.CopyMode.Snapshot);

                            prjCopy = prjManager.OpenProject(
                                projectElkFileName,
                                ProjectManager.OpenMode.Exclusive);
                        },
                        p =>
                        {
                            p.SetActionText("Preparing...");

                            if (!pageMacros)
                            {
                                prjCopy.Pages
                                    .ForEach(DisableMacro);
                            }
                            else if (pageNames != null)
                            {
                                prjCopy.Pages
                                    .Where(x => !pageNames.Contains(x.IdentifyingName))
                                    .ForEach(DisableMacro);
                            }

                            if (!symbolMacros
                                || !windowMacros
                                || pageNames != null)
                            {
                                var macroBoxes = new DMObjectsFinder(prjCopy).GetPlacements(null)
                                    .OfType<MacroBox>()
                                    .ToList();

                                if (!symbolMacros)
                                {
                                    macroBoxes
                                        .Where(IsSymbolMacro)
                                        .ForEach(DisableMacro);
                                }

                                if (!windowMacros)
                                {
                                    macroBoxes
                                        .Where(IsWindowMacro)
                                        .ForEach(DisableMacro);
                                }

                                if (pageNames != null)
                                {
                                    macroBoxes
                                        .Where(x => !pageNames.Contains(x.Page.IdentifyingName))
                                        .ForEach(DisableMacro);
                                }
                            }
                        },
                        p =>
                        {
                            p.SetActionText("Saving...");

                            new Masterdata()
                                .GenerateMacrosFromMacroProject(prjCopy);
                        },
                        p =>
                        {
                            p.SetActionText("Closing project...");

                            prjCopy.Close();
                        });
            }
        }

        private static void DisableMacro(Page page)
        {
            page.Properties.PAGE_MACRO = "";
        }

        private static void DisableMacro(MacroBox macroBox)
        {
            macroBox.Properties.MACROBOX_MACRO = "";
        }

        private static bool IsSymbolMacro(MacroBox macroBox)
        {
            var macro = macroBox.Properties.MACROBOX_MACRO;
            return !macro.IsEmpty 
                && macro.ToString().EndsWith(".ems");
        }

        private static bool IsWindowMacro(MacroBox macroBox)
        {
            return !IsSymbolMacro(macroBox);
        }
    }
}