using System;
using System.Linq;
using System.Windows.Forms;
using Vescon.EplAddin.Qdt.Cfg;

namespace Vescon.EplAddin.Qdt.Dialogs
{
    public partial class CheckedListBox : Form, IWindow
    {
        private bool _synchronizing;

        private int _allItemsCount;
        private int _allSpecialItemsCount;

        public CheckedListBox()
        {
            InitializeComponent();
            Text = Addin.Title;

            Settings.Add(this);

            btnOK.Enabled = false;
        }

        public string Caption
        {
            get => lblCaption.Text;
            set => lblCaption.Text = value;
        }

        public object[] Items
        {
            get => lbContent.Items.Cast<object>().ToArray();

            set
            {
                lbContent.Items.AddRange(value);

                _allItemsCount = lbContent.Items.Count;
                _allSpecialItemsCount = lbContent.Items.Cast<object>().Count(IsSpecial);
                cbAllSpecial.Enabled = _allSpecialItemsCount > 0;
            }
        }

        public int[] CheckedIndices => lbContent.CheckedIndices.Cast<int>().ToArray();

        public Func<CloseReason, bool> BeforeClosing { get; set; }

        public string GetFormId()
        {
            return "CheckedListBox";
        }

        public void LoadAdditionalConfigData(Data data)
        {
        }

        public void SaveAdditionalConfigData(Data data)
        {
        }

        private void lbContent_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker) (() =>
            {
                btnOK.Enabled = lbContent.CheckedItems.Count > 0;

                Synchronize(() =>
                {
                    var checkedItemsCount = lbContent.CheckedItems.Count;

                    if (checkedItemsCount == 0)
                        cbAll.CheckState = CheckState.Unchecked;
                    else if (checkedItemsCount == _allItemsCount)
                        cbAll.CheckState = CheckState.Checked;
                    else
                        cbAll.CheckState = CheckState.Indeterminate;

                    var checkedSpecialItemsCount = lbContent.CheckedItems.Cast<object>().Count(IsSpecial);

                    if (checkedSpecialItemsCount == 0)
                        cbAllSpecial.CheckState = CheckState.Unchecked;
                    else if (checkedSpecialItemsCount == _allSpecialItemsCount)
                        cbAllSpecial.CheckState = CheckState.Checked;
                    else
                        cbAllSpecial.CheckState = CheckState.Indeterminate;
                });
            }));
        }

        private void CheckedListBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BeforeClosing != null)
                e.Cancel = BeforeClosing(e.CloseReason);
        }

        private void cbAll_CheckedChanged(object sender, EventArgs e)
        {
            Synchronize(() =>
            {
                for (var i = 0; i < lbContent.Items.Count; ++i)
                {
                    lbContent.SetItemChecked(i, cbAll.Checked);
                }
            });
        }

        private void cbAllSpecial_CheckedChanged(object sender, EventArgs e)
        {
            Synchronize(() =>
            {
                for (var i = 0; i < lbContent.Items.Count; ++i)
                {
                    if (IsSpecial(lbContent.Items[i]))
                    {
                        lbContent.SetItemChecked(i, cbAllSpecial.Checked);
                    }                    
                }
            });
        }

        private bool IsSpecial(object item)
        {
            return item.ToString().ToLowerInvariant().StartsWith("mb ");
        }

        private void Synchronize(Action action)
        {
            if (_synchronizing)
                return;
            try
            {
                _synchronizing = true;
                action();
            }
            finally
            {
                _synchronizing = false;
            }
        }
    }
}
