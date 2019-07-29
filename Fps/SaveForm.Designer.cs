namespace Fps
{
    partial class SaveForm
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
            this.okbutton = new System.Windows.Forms.Button();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.etablecheckBox = new System.Windows.Forms.CheckBox();
            this.pymolcheckBox = new System.Windows.Forms.CheckBox();
            this.pdbcheckBox = new System.Windows.Forms.CheckBox();
            this.overlaycheckBox = new System.Windows.Forms.CheckBox();
            this.RcheckBox = new System.Windows.Forms.CheckBox();
            this.filenametextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lpcheckBox = new System.Windows.Forms.CheckBox();
            this.overlaystatescheckBox = new System.Windows.Forms.CheckBox();
            this.selected_radioButton = new System.Windows.Forms.RadioButton();
            this.all_radioButton = new System.Windows.Forms.RadioButton();
            this.srcheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // okbutton
            // 
            this.okbutton.Location = new System.Drawing.Point(167, 121);
            this.okbutton.Name = "okbutton";
            this.okbutton.Size = new System.Drawing.Size(75, 23);
            this.okbutton.TabIndex = 2;
            this.okbutton.Text = "Continue >>";
            this.okbutton.UseVisualStyleBackColor = true;
            this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
            // 
            // cancelbutton
            // 
            this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbutton.Location = new System.Drawing.Point(249, 121);
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.cancelbutton.TabIndex = 3;
            this.cancelbutton.Text = "Cancel";
            this.cancelbutton.UseVisualStyleBackColor = true;
            this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
            // 
            // etablecheckBox
            // 
            this.etablecheckBox.AutoSize = true;
            this.etablecheckBox.Checked = true;
            this.etablecheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.etablecheckBox.Location = new System.Drawing.Point(167, 10);
            this.etablecheckBox.Name = "etablecheckBox";
            this.etablecheckBox.Size = new System.Drawing.Size(73, 17);
            this.etablecheckBox.TabIndex = 4;
            this.etablecheckBox.Text = "Chi2 table";
            this.etablecheckBox.UseVisualStyleBackColor = true;
            // 
            // pymolcheckBox
            // 
            this.pymolcheckBox.AutoSize = true;
            this.pymolcheckBox.Checked = true;
            this.pymolcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pymolcheckBox.Location = new System.Drawing.Point(12, 33);
            this.pymolcheckBox.Name = "pymolcheckBox";
            this.pymolcheckBox.Size = new System.Drawing.Size(87, 17);
            this.pymolcheckBox.TabIndex = 5;
            this.pymolcheckBox.Text = "Pymol scripts";
            this.pymolcheckBox.UseVisualStyleBackColor = true;
            this.pymolcheckBox.CheckedChanged += new System.EventHandler(this.pymolcheckBox_CheckedChanged);
            // 
            // pdbcheckBox
            // 
            this.pdbcheckBox.AutoSize = true;
            this.pdbcheckBox.Location = new System.Drawing.Point(30, 56);
            this.pdbcheckBox.Name = "pdbcheckBox";
            this.pdbcheckBox.Size = new System.Drawing.Size(106, 17);
            this.pdbcheckBox.TabIndex = 6;
            this.pdbcheckBox.Text = "Pymol saves pdb";
            this.pdbcheckBox.UseVisualStyleBackColor = true;
            // 
            // overlaycheckBox
            // 
            this.overlaycheckBox.AutoSize = true;
            this.overlaycheckBox.Location = new System.Drawing.Point(12, 102);
            this.overlaycheckBox.Name = "overlaycheckBox";
            this.overlaycheckBox.Size = new System.Drawing.Size(62, 17);
            this.overlaycheckBox.TabIndex = 7;
            this.overlaycheckBox.Text = "Overlay";
            this.overlaycheckBox.UseVisualStyleBackColor = true;
            // 
            // RcheckBox
            // 
            this.RcheckBox.AutoSize = true;
            this.RcheckBox.Location = new System.Drawing.Point(167, 33);
            this.RcheckBox.Name = "RcheckBox";
            this.RcheckBox.Size = new System.Drawing.Size(103, 17);
            this.RcheckBox.TabIndex = 8;
            this.RcheckBox.Text = "Model distances";
            this.RcheckBox.UseVisualStyleBackColor = true;
            // 
            // filenametextBox
            // 
            this.filenametextBox.Location = new System.Drawing.Point(167, 95);
            this.filenametextBox.Name = "filenametextBox";
            this.filenametextBox.Size = new System.Drawing.Size(157, 20);
            this.filenametextBox.TabIndex = 9;
            this.filenametextBox.Text = "structure";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "File name prefix";
            // 
            // lpcheckBox
            // 
            this.lpcheckBox.AutoSize = true;
            this.lpcheckBox.Location = new System.Drawing.Point(30, 79);
            this.lpcheckBox.Name = "lpcheckBox";
            this.lpcheckBox.Size = new System.Drawing.Size(128, 17);
            this.lpcheckBox.TabIndex = 12;
            this.lpcheckBox.Text = "Add labeling positions";
            this.lpcheckBox.UseVisualStyleBackColor = true;
            // 
            // overlaystatescheckBox
            // 
            this.overlaystatescheckBox.AutoSize = true;
            this.overlaystatescheckBox.Location = new System.Drawing.Point(12, 125);
            this.overlaystatescheckBox.Name = "overlaystatescheckBox";
            this.overlaystatescheckBox.Size = new System.Drawing.Size(99, 17);
            this.overlaystatescheckBox.TabIndex = 13;
            this.overlaystatescheckBox.Text = "Overlay (states)";
            this.overlaystatescheckBox.UseVisualStyleBackColor = true;
            // 
            // selected_radioButton
            // 
            this.selected_radioButton.AutoSize = true;
            this.selected_radioButton.Checked = true;
            this.selected_radioButton.Location = new System.Drawing.Point(12, 9);
            this.selected_radioButton.Name = "selected_radioButton";
            this.selected_radioButton.Size = new System.Drawing.Size(67, 17);
            this.selected_radioButton.TabIndex = 14;
            this.selected_radioButton.TabStop = true;
            this.selected_radioButton.Text = "Selected";
            this.selected_radioButton.UseVisualStyleBackColor = true;
            this.selected_radioButton.CheckedChanged += new System.EventHandler(this.selected_radioButton_CheckedChanged);
            // 
            // all_radioButton
            // 
            this.all_radioButton.AutoSize = true;
            this.all_radioButton.Location = new System.Drawing.Point(108, 9);
            this.all_radioButton.Name = "all_radioButton";
            this.all_radioButton.Size = new System.Drawing.Size(36, 17);
            this.all_radioButton.TabIndex = 15;
            this.all_radioButton.Text = "All";
            this.all_radioButton.UseVisualStyleBackColor = true;
            // 
            // srcheckBox
            // 
            this.srcheckBox.AutoSize = true;
            this.srcheckBox.Enabled = false;
            this.srcheckBox.Location = new System.Drawing.Point(167, 56);
            this.srcheckBox.Name = "srcheckBox";
            this.srcheckBox.Size = new System.Drawing.Size(130, 17);
            this.srcheckBox.TabIndex = 16;
            this.srcheckBox.Text = "Simulation results (bin)";
            this.srcheckBox.UseVisualStyleBackColor = true;
            // 
            // SaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelbutton;
            this.ClientSize = new System.Drawing.Size(336, 153);
            this.Controls.Add(this.srcheckBox);
            this.Controls.Add(this.all_radioButton);
            this.Controls.Add(this.selected_radioButton);
            this.Controls.Add(this.overlaystatescheckBox);
            this.Controls.Add(this.lpcheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filenametextBox);
            this.Controls.Add(this.RcheckBox);
            this.Controls.Add(this.overlaycheckBox);
            this.Controls.Add(this.pdbcheckBox);
            this.Controls.Add(this.pymolcheckBox);
            this.Controls.Add(this.etablecheckBox);
            this.Controls.Add(this.cancelbutton);
            this.Controls.Add(this.okbutton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SaveForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Save:";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okbutton;
        private System.Windows.Forms.Button cancelbutton;
        private System.Windows.Forms.CheckBox etablecheckBox;
        private System.Windows.Forms.CheckBox pymolcheckBox;
        private System.Windows.Forms.CheckBox pdbcheckBox;
        private System.Windows.Forms.CheckBox overlaycheckBox;
        private System.Windows.Forms.CheckBox RcheckBox;
        private System.Windows.Forms.TextBox filenametextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox lpcheckBox;
        private System.Windows.Forms.CheckBox overlaystatescheckBox;
        private System.Windows.Forms.RadioButton selected_radioButton;
        private System.Windows.Forms.RadioButton all_radioButton;
        private System.Windows.Forms.CheckBox srcheckBox;
    }
}