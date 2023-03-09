using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;

namespace Vescon.EplAddin.Qdt
{
    public abstract class ActionBase : IEplActionEnable
    {
        public virtual string Name => GetType().FullName?.Replace('.', '_');
        public virtual string MenuCaption => throw new Exception("Action's menu caption not provided.");
        public virtual string Description => throw new Exception("Toolbar/Menu action's description not provided.");

        public static void HandleException(Exception ex)
        {
            var msg = ex.Message;
#if DEBUG
            msg += Environment.NewLine + ex.StackTrace;
#endif
            MessageBox.Show(
                msg,
                Addin.Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            var exceptions = new List<string>();
            while (ex != null)
            {
                exceptions.Add(ex.Message);
                ex = ex.InnerException;
            }

            BaseException innerExeption = null;

            for (var i = exceptions.Count - 1; i >= 0; --i)
            {
                innerExeption = new BaseException(exceptions[i], MessageLevel.Error, innerExeption);
            }

            new BaseException(
                $"Exception occured in the '{Addin.Title}' addin.",
                MessageLevel.Error,
                innerExeption)
                .FixMessage();
        }

        public bool OnRegister(
            // ReSharper disable once RedundantAssignment
            ref string name, 
            // ReSharper disable once RedundantAssignment
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

        protected static bool SaveOutput(StringBuilder output, bool openInExcel)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save list to a file",
                Filter = "CSV files|*.csv|Text files|*.txt|All files|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, output.ToString(), Encoding.Unicode);
                
                if (openInExcel)
                {
                    try
                    {
                        Process.Start("excel.exe", $"\"{saveFileDialog.FileName}\"");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Opening in Excel failed." + Environment.NewLine + ex.Message,
                            Addin.Title,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }

                return true;
            }

            return false;
        }

        protected static bool Execute(Func<bool> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                Cfg.Settings.Save();
            }

            return true;
        }
        
        protected static void ShowInfo(string msg)
        {
            MessageBox.Show(msg, Addin.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        protected static void ShowWarning(string msg)
        {
            MessageBox.Show(msg, Addin.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        protected static bool Ask(string msg)
        {
            return MessageBox.Show(msg, Addin.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
