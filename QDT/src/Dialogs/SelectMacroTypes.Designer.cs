namespace Vescon.EplAddin.Qdt.Dialogs
{
    partial class SelectMacroTypes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblSelect = new System.Windows.Forms.Label();
            this.cbWindowMacros = new System.Windows.Forms.CheckBox();
            this.cbPageMacros = new System.Windows.Forms.CheckBox();
            this.cbSymbolMacros = new System.Windows.Forms.CheckBox();
            this.lblGenerateFrom = new System.Windows.Forms.Label();
            this.rbSelectedPages = new System.Windows.Forms.RadioButton();
            this.rbCompleteProject = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(182, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(101, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblSelect
            // 
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(12, 9);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(185, 13);
            this.lblSelect.TabIndex = 2;
            this.lblSelect.Text = "Select types of macros for generation:";
            // 
            // cbWindowMacros
            // 
            this.cbWindowMacros.AutoSize = true;
            this.cbWindowMacros.Location = new System.Drawing.Point(23, 60);
            this.cbWindowMacros.Name = "cbWindowMacros";
            this.cbWindowMacros.Size = new System.Drawing.Size(102, 17);
            this.cbWindowMacros.TabIndex = 3;
            this.cbWindowMacros.Text = "Window macros";
            this.cbWindowMacros.UseVisualStyleBackColor = true;
            this.cbWindowMacros.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbPageMacros
            // 
            this.cbPageMacros.AutoSize = true;
            this.cbPageMacros.Location = new System.Drawing.Point(23, 83);
            this.cbPageMacros.Name = "cbPageMacros";
            this.cbPageMacros.Size = new System.Drawing.Size(88, 17);
            this.cbPageMacros.TabIndex = 3;
            this.cbPageMacros.Text = "Page macros";
            this.cbPageMacros.UseVisualStyleBackColor = true;
            this.cbPageMacros.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbSymbolMacros
            // 
            this.cbSymbolMacros.AutoSize = true;
            this.cbSymbolMacros.Location = new System.Drawing.Point(23, 37);
            this.cbSymbolMacros.Name = "cbSymbolMacros";
            this.cbSymbolMacros.Size = new System.Drawing.Size(97, 17);
            this.cbSymbolMacros.TabIndex = 3;
            this.cbSymbolMacros.Text = "Symbol macros";
            this.cbSymbolMacros.UseVisualStyleBackColor = true;
            this.cbSymbolMacros.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // lblGenerateFrom
            // 
            this.lblGenerateFrom.AutoSize = true;
            this.lblGenerateFrom.Location = new System.Drawing.Point(12, 119);
            this.lblGenerateFrom.Name = "lblGenerateFrom";
            this.lblGenerateFrom.Size = new System.Drawing.Size(77, 13);
            this.lblGenerateFrom.TabIndex = 2;
            this.lblGenerateFrom.Text = "Generate from:";
            // 
            // rbSelectedPages
            // 
            this.rbSelectedPages.AutoSize = true;
            this.rbSelectedPages.Location = new System.Drawing.Point(23, 139);
            this.rbSelectedPages.Name = "rbSelectedPages";
            this.rbSelectedPages.Size = new System.Drawing.Size(99, 17);
            this.rbSelectedPages.TabIndex = 4;
            this.rbSelectedPages.TabStop = true;
            this.rbSelectedPages.Text = "Selected pages";
            this.rbSelectedPages.UseVisualStyleBackColor = true;
            // 
            // rbCompleteProject
            // 
            this.rbCompleteProject.AutoSize = true;
            this.rbCompleteProject.Location = new System.Drawing.Point(23, 162);
            this.rbCompleteProject.Name = "rbCompleteProject";
            this.rbCompleteProject.Size = new System.Drawing.Size(104, 17);
            this.rbCompleteProject.TabIndex = 4;
            this.rbCompleteProject.TabStop = true;
            this.rbCompleteProject.Text = "Complete project";
            this.rbCompleteProject.UseVisualStyleBackColor = true;
            // 
            // SelectMacroTypes
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(269, 261);
            this.Controls.Add(this.rbCompleteProject);
            this.Controls.Add(this.rbSelectedPages);
            this.Controls.Add(this.cbPageMacros);
            this.Controls.Add(this.cbSymbolMacros);
            this.Controls.Add(this.cbWindowMacros);
            this.Controls.Add(this.lblGenerateFrom);
            this.Controls.Add(this.lblSelect);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(285, 300);
            this.Name = "SelectMacroTypes";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.CheckBox cbWindowMacros;
        private System.Windows.Forms.CheckBox cbPageMacros;
        private System.Windows.Forms.CheckBox cbSymbolMacros;
        private System.Windows.Forms.Label lblGenerateFrom;
        private System.Windows.Forms.RadioButton rbSelectedPages;
        private System.Windows.Forms.RadioButton rbCompleteProject;
    }
}