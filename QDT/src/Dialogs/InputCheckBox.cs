using System.Windows.Forms;
using Vescon.EplAddin.Qdt.Cfg;

namespace Vescon.EplAddin.Qdt.Dialogs
{
    public sealed partial class InputCheckBox : Form, IWindow
    {
        public InputCheckBox()
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

        public bool State
        {
            get => rbTrue.Checked;
            set => rbTrue.Checked = value;
        }

        public bool LoadStateFromConfig { get; set; } = true;

        public string GetFormId()
        {
            return "InputCheckBox";
        }

        public void LoadAdditionalConfigData(Data data)
        {
            if (LoadStateFromConfig)
            {
                rbTrue.Checked = data.Get("State", true);
                rbFalse.Checked = !rbTrue.Checked;
            }
        }

        public void SaveAdditionalConfigData(Data data)
        {
            data.Insert("State", rbTrue.Checked);
        }

        public void SetInitialState(bool stateTrue, bool stateFalse)
        {
            rbTrue.Checked = stateTrue;
            rbFalse.Checked = stateFalse;
        }
    }
}
