using Eplan.EplApi.ApplicationFramework;

namespace PlaceholderConfigLoader.EplAddin
{
    abstract class ActionEnabledIfSingleMainFunctionSelected<TAction> : ActionEnabledIfProjectOpen<TAction>
    {
        public override bool Enabled(string actionName, ActionCallingContext context)
        {
            return EplanHelper.GetSelectedMainFunction() != null;
        }
    }
}