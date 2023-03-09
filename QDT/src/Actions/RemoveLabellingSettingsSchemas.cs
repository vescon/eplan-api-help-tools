using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using CheckedListBox = Vescon.EplAddin.Qdt.Dialogs.CheckedListBox;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class RemoveLabellingSettingsSchemas : ActionBase, IEplAction
    {
        public override string MenuCaption => "Remove 'Labeling' settings schemas...";
        public override string Description => "Removes several 'Labeling' settings schemas at once.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var schemasNames = new Dictionary<string, string>();

                var ss = new SchemeSetting();
                ss.Init("USER.Labelling.Config");
                var schemasCount = ss.GetCount();

                for (var i = 0; i < schemasCount; ++i)
                {
                    ss.SetScheme(i);

                    if (ss.ReadOnly)
                        continue;

                    schemasNames.Add(ss.GetName(), ss.MLangName.GetStringToDisplay(ISOCode.Language.L_de_DE));
                }

                var dlg = new CheckedListBox
                {
                    Caption = "Select the 'Labeling' schemas to delete:",

                    Items = schemasNames.Values.Cast<object>().ToArray()
                };

                dlg.BeforeClosing = reason =>
                {
                    if (dlg.DialogResult == DialogResult.OK 
                        && dlg.CheckedIndices.Length == schemasCount)
                    {
                        ShowWarning("You cannot remove all schemas. Please deselect at least one.");
                        return true;
                    }

                    return false;
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                    return true;

                var checkedIndices = dlg.CheckedIndices;

                var msg = new StringBuilder();
                msg.AppendLine("The following schema(s) will be deleted:");
                checkedIndices
                    .ForEach(i => msg.AppendLine($"\t- {schemasNames.ElementAt(i).Value}"));
                msg.AppendLine(string.Empty);
                msg.AppendLine("Are you sure to continue ?");

                if (MessageBox.Show(
                    msg.ToString(),
                    Addin.Title,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning) != DialogResult.OK)
                    return true;

                checkedIndices
                    .ForEach(i =>
                    {
                        var schemaName = schemasNames.ElementAt(i).Key;
                        ss.RemoveScheme(schemaName);
                    });

                ShowInfo("Done.");

                return true;
            });
        }
    }
}