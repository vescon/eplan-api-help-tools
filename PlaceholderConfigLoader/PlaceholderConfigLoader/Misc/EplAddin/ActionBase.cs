using System;
using System.ServiceModel;
using System.Windows;

using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;

namespace PlaceholderConfigLoader.EplAddin
{
    abstract class ActionBase<TAction> : IEplActionEnable
    {
        public static string Name => typeof(TAction).FullName.Replace('.', '_');

        public bool OnRegister(
            ref string name, 
            ref int ordinal)
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
                ex.Detail.HandleException();
            }
            catch (Exception ex)
            {
                ex.HandleException();
            }

            return true;
        }

        protected void MsgError(string msg)
        {
            MessageBox.Show(msg, Addin.Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected void MsgWarning(string msg)
        {
            MessageBox.Show(msg, Addin.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected void MsgInfo(string msg)
        {
            MessageBox.Show(msg, Addin.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected bool MsgConfirm(string msg)
        {
            return MessageBox.Show(msg, Addin.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        protected bool GetCurrentProject(out Project project)
        {
            project = EplanHelper.GetCurrentProject();
            if (project != null)
                return true;

            MsgError("Please, select a single project.");
            return false;
        }
    }
}
