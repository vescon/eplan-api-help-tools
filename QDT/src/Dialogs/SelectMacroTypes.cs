using System;
using System.Windows.Forms;
using Vescon.EplAddin.Qdt.Cfg;

namespace Vescon.EplAddin.Qdt.Dialogs
{
    public partial class SelectMacroTypes : Form, IWindow
    {
        public SelectMacroTypes()
        {
            InitializeComponent();
            Text = Addin.Title;

            Settings.Add(this);

            btnOK.Enabled = false;
        }

        public string Caption
        {
            get => lblSelect.Text;
            set => lblSelect.Text = value;
        }

        public bool PageMacros => cbPageMacros.Checked;
        public bool WindowMacros => cbWindowMacros.Checked;
        public bool SymbolMacros => cbSymbolMacros.Checked;

        public bool CompleteProject => rbCompleteProject.Checked;

        public string GetFormId()
        {
            return nameof(SelectMacroTypes);
        }

        public void LoadAdditionalConfigData(Data data)
        {
            cbSymbolMacros.Checked = data.Get("StateSymbolMacros", false);
            cbWindowMacros.Checked = data.Get("StateWindowMacros", false);
            cbPageMacros.Checked = data.Get("StatePageMacros", false);
            
            if (lblGenerateFrom.Enabled)
            {
                rbCompleteProject.Checked = data.Get("CompleteProject", false);
            }
        }

        public void SaveAdditionalConfigData(Data data)
        {
            data.Insert("StateSymbolMacros", cbSymbolMacros.Checked);
            data.Insert("StateWindowMacros", cbWindowMacros.Checked);
            data.Insert("StatePageMacros", cbPageMacros.Checked);

            if (lblGenerateFrom.Enabled)
            {
                data.Insert("CompleteProject", rbCompleteProject.Checked);
            }
        }

        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = cbPageMacros.Checked 
                || cbSymbolMacros.Checked 
                || cbWindowMacros.Checked;
        }

        public void EnableScopeSelection(bool enable)
        {
            if (enable)
            {
                lblGenerateFrom.Enabled = true;
                rbCompleteProject.Enabled = true;
                rbSelectedPages.Enabled = true;

                rbSelectedPages.Checked = true;
            }
            else
            {
                lblGenerateFrom.Enabled = false;
                rbCompleteProject.Enabled = false;
                rbSelectedPages.Enabled = false;
                
                rbCompleteProject.Checked = true;
            }
        }
    }
}
