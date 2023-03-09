using System;
using System.Linq;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using Vescon.EplAddin.Qdt.Dialogs;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class SetProperty11076 : ActionBase, IEplAction
    {
        public override string MenuCaption => "Set property 11076 of selected pages";
        public override string Description => "Sets the property 11076 of selected pages.";

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

                var dlg = new InputCheckBox
                {
                    Caption = "Set/Clear the property 11076 of selected page(s):",
                    LoadStateFromConfig = false
                };

                var values = pages
                    .Select(x => !x.Properties[11076].IsEmpty && x.Properties[11076].ToBool())
                    .ToList();
                
                dlg.SetInitialState(
                    values.All(x => x), 
                    values.All(x => !x));

                if (dlg.ShowDialog() != DialogResult.OK)
                    return true;

                using (new LockingStep())
                    pages.ForEach(x =>
                    {
                        try
                        {
                            x.LockObject();
                            x.Properties[11076] = dlg.State;
                        }
                        catch (Exception ex)
                        {
                            ShowWarning($"Setting property 11076 of the page '{x.IdentifyingName}' failed.\n{ex.Message}");
                        }        
                    });

                return true;
            });
        }
    }
}