using Eplan.EplApi.ApplicationFramework;
using Vescon.Eplan.Utilities.Misc;

namespace Vescon.Eplan.Utilities.Action
{
    abstract class ActionEnabledIfProjectOpen : ActionBase
    {
        public override bool Enabled(string actionName, ActionCallingContext context)
        {
            return EplanHelper.GetCurrentProject() != null;
        }
    }
}