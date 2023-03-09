using Eplan.EplApi.ApplicationFramework;
using Vescon.Eplan.Utilities.Misc;

namespace Vescon.Eplan.Utilities.Action
{
    abstract class ActionEnabledIfSingleMainFunctionSelected : ActionEnabledIfProjectOpen
    {
        public override bool Enabled(string actionName, ActionCallingContext context)
        {
            return EplanHelper.GetSelectedMainFunction() != null;
        }
    }
}