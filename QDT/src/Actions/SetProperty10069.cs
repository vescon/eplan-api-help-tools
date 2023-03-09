using System;
using System.Linq;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using Vescon.EplAddin.Qdt.Dialogs;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class SetProperty10069 : ActionBase, IEplAction
    {
        public override string MenuCaption => "Set property 10069 of selected projects";
        public override string Description => "Sets the property 10069 of selected projects, which can not be set from P8's UI.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var projects = new SelectionSet().SelectedProjects
                    .Where(x => x != null)
                    .ToList();
                
                if (!projects.Any())
                {
                    ShowWarning("Please select one or more projects.");
                    return true;
                }

                var dlg = new InputBox
                {
                    OpenInExcelVisible = false,
                    Caption = "Enter new value for the property 10069:"
                };
                
                if (dlg.ShowDialog() != DialogResult.OK)
                    return true;

                var msg = "Are you sure to change the property 10069 on the following project(s):";
                for (var i = 0; i < projects.Count && i < 10; ++i)
                    msg += "\n\t- " + projects[i];
                if (projects.Count > 10)
                    msg += "\n\t...";
                msg += " ?";
                if (!Ask(msg))
                    return true; // cancelled

                projects.ForEach(x =>
                {
                    try
                    {
                        using (new LockingStep())
                        {
                            x.LockObject();
                            x.Properties[10069] = dlg.Input;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowWarning($"Setting property 10069 of the project '{x}' failed.\n{ex.Message}");
                    }        
                });

                return true;
            });
        }
    }
}