using System.Windows.Forms;
using Vescon.EplAddin.Qdt.Cfg;

namespace Vescon.EplAddin.Qdt.Dialogs
{
    public sealed partial class InputBox : Form, IWindow
    {
        public InputBox()
        {
            InitializeComponent();
            Text = Addin.Title;

            Settings.Add(this);
        }

        public string Caption
        {
            get => lblCaption.Text;
            set => lblCaption.Text = value;
        }

        public string Input
        {
            get => tbInput.Text;
            set => tbInput.Text = value;
        }

        public bool OpenInExcel
        {
            get => chkOpenInExcel.Checked;
            set => chkOpenInExcel.Checked = value;
        }

        public bool OpenInExcelVisible
        {
            get => chkOpenInExcel.Visible;
            set => chkOpenInExcel.Visible = value;
        }

        public string GetFormId()
        {
            return "InputBox";
        }

        public void LoadAdditionalConfigData(Data data)
        {
            chkOpenInExcel.Checked = data.Get("OpenInNotepad", false);
        }

        public void SaveAdditionalConfigData(Data data)
        {
            data.Insert("OpenInNotepad", chkOpenInExcel.Checked);
        }
    }
}
