using Eplan.EplApi.ApplicationFramework;

namespace PlaceholderConfigLoader.EplAddin
{
    abstract class ActionEnabledIfProjectOpen<TAction> : ActionBase<TAction>
    {
        public override bool Enabled(string actionName, ActionCallingContext context)
        {
            return EplanHelper.GetCurrentProject() != null;
        }
    }
}