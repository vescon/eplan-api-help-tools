using System;
using System.Linq;
using System.Text;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;

namespace Vescon.EplAddin.Qdt.Actions.Temporary
{
    public class ShowPlacementsLocations : ActionBase, IEplAction
    {
        public override string MenuCaption => "Show position";
        public override string Description => "";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                /*
                                var prj = new SelectionSet().GetCurrentProject(false);
                                if (prj == null)
                                {
                                    ShowWarning("There's no any opened project.");
                                }

                                var arr = prj.Properties[10019].ToString().Split('\t');
                                var sb = new StringBuilder();
                                var i = 1;
                                arr.Skip(1).ForEach(x => sb.AppendLine($"{i++}. {x}"));
                                ShowInfo(sb.ToString());
                */

                var placements = new SelectionSet().Selection
                    .OfType<Placement>()
                    .ToList();

                var sb = new StringBuilder();
                var i = 1;
                placements.ForEach(x =>
                {
                    x.Location = new PointD(Math.Round(x.Location.X, 0), Math.Round(x.Location.Y, 0));
                    sb.AppendLine($"{i++}. X = {x.Location.X}\tY = {x.Location.Y}");
                });

                ShowInfo(sb.ToString());
                
                return true;
            });
        }
    }
}