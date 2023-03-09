using System;
using System.Linq;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using Vescon.EplAddin.Qdt.Dialogs;

namespace Vescon.EplAddin.Qdt.Actions.Temporary
{
    public class SetProperty11023 : ActionBase, IEplAction
    {
        public override string MenuCaption => "Set property 11023 of selected pages";
        public override string Description => "Sets the property 11023 of selected pages.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var pages = new SelectionSet().GetSelectedPages()
                    .Where(x => x != null)
                    .ToList();
                
                if (!pages.Any())
                {
                    ShowWarning("Please select one or more pages.");
                    return true;
                }

                var dlg = new InputBox
                {
                    OpenInExcelVisible = false,
                    Caption = "Enter new value for the property 11023:"
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                    return true;

                using (new LockingStep())
                    pages.ForEach(x =>
                    {
                        try
                        {
                            x.LockObject();
                            x.Properties[11023] = dlg.Input;
                        }
                        catch (Exception ex)
                        {
                            ShowWarning($"Setting property 11023 of the page '{x.IdentifyingName}' failed.\n{ex.Message}");
                        }        
                    });

                return true;
            });
        }
    }
}