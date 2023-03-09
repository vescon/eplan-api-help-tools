using System.Linq;
using System.Text;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions.Temporary
{
    public class ShowProperty10019 : ActionBase, IEplAction
    {
        public override string MenuCaption => "Show property 10019 (structure options)";
        public override string Description => "";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var prj = new SelectionSet().GetCurrentProject(false);
                if (prj == null)
                {
                    ShowWarning("There's no any opened project.");
                    return true;
                }

                var arr = prj.Properties[10019].ToString().Split('\t');
                var sb = new StringBuilder();
                var i = 1;
                arr.Skip(1).ForEach(x => sb.AppendLine($"{i++}. {x}"));
                
                ShowInfo(sb.ToString());

                return true;
            });
        }
    }
}