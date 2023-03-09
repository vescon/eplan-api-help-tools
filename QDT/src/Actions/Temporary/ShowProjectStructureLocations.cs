using System;
using System.Linq;
using System.Text;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions.Temporary
{
    public class ShowProjectStructureLocations : ActionBase, IEplAction
    {
        public override string MenuCaption => nameof(ShowProjectStructureLocations);
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

                Enum.GetValues(typeof(Project.Hierarchy))
                    .Cast<Project.Hierarchy>()
                    .ForEach(h =>
                    {
                        var ll = prj.GetLocationObjects(h);
                        
                        if (ll == null 
                            || ll.Length == 0)
                            return;

                        var sb = new StringBuilder();
                        sb.AppendLine($"{h}:");
                        sb.AppendLine();

                        var i = 1;
                        ll.ForEach(l => sb.AppendLine($"{i++}. {l.Properties.LOCATION_FULLNAME}"));
                        ShowInfo(sb.ToString());
                    });
                
                return true;
            });
        }
    }
}