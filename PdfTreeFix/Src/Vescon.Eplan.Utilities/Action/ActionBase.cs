using System;
using System.ServiceModel;
using System.Windows;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Vescon.Eplan.Utilities.Addin;
using Vescon.Eplan.Utilities.Misc;

namespace Vescon.Eplan.Utilities.Action
{
    public abstract class ActionBase : IEplActionEnable
    {
        public virtual string Name => GetType().FullName?.Replace('.', '_');

        public virtual string Description
        {
            get { throw new Exception("Toolbar/Menu action's description not provided."); }
        }

        public virtual string MenuCaption
        {
            get { throw new Exception("Action's menu caption not provided."); }
        }

        public virtual string IconPath => null;

        public bool OnRegister(ref string name, ref int ordinal)
        {
            name = Name;
            ordinal = 20;
            return true;
        }

        public abstract bool Execute(ActionCallingContext ctx);

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }

        public virtual bool Enabled(string actionName, ActionCallingContext ctx)
        {
            return true;
        }

        protected bool Execute(Func<bool> action)
        {
            try
            {
                return action();
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                ex.Detail.HandleException(Title);
            }
            catch (Exception ex)
            {
                ex.HandleException(Title);
            }

            return true;
        }

        protected void Error(string msg)
        {
            MessageBox.Show(msg, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected void Info(string msg)
        {
            MessageBox.Show(msg, Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected void Warning(string msg)
        {
            MessageBox.Show(msg, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected bool Confirm(string msg)
        {
            return MessageBox.Show(msg, Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        protected bool GetCurrentProject(out Project project)
        {
            project = EplanHelper.GetCurrentProject();
            if (project != null)
                return true;

            Error("Please, select a single project.");
            return false;
        }
        
        protected virtual string Title => Name;
    }
}
