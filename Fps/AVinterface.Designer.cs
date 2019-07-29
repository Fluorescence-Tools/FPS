namespace Fps
{
    partial class AVinterface
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
            this.moleculescomboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dyescomboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.atomIDnumericBoxInt32 = new NumericControls.NumericBoxInt32();
            this.atomnametextBox = new System.Windows.Forms.TextBox();
            this.LnumericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.R1label = new System.Windows.Forms.Label();
            this.WnumericBoxDouble = new NumericControls.NumericBoxDouble();
            this.R1numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.calculatebutton = new System.Windows.Forms.Button();
            this.addbutton = new System.Windows.Forms.Button();
            this.savebutton = new System.Windows.Forms.Button();
            this.closebutton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.DradioButton = new System.Windows.Forms.RadioButton();
            this.AradioButton = new System.Windows.Forms.RadioButton();
            this.rmptextBox = new System.Windows.Forms.TextBox();
            this.lpnametextBox = new System.Windows.Forms.TextBox();
            this.newmoleculebutton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.simcombobox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.R2numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.R3numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.dyestructbutton = new System.Windows.Forms.Button();
            this.dyestructbox = new System.Windows.Forms.TextBox();
            this.dyestructlabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // moleculescomboBox
            // 
            this.moleculescomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.moleculescomboBox.FormattingEnabled = true;
            this.moleculescomboBox.Location = new System.Drawing.Point(12, 25);
            this.moleculescomboBox.Name = "moleculescomboBox";
            this.moleculescomboBox.Size = new System.Drawing.Size(175, 21);
            this.moleculescomboBox.TabIndex = 0;
            this.moleculescomboBox.SelectedIndexChanged += new System.EventHandler(this.moleculescomboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Molecule";
            // 
            // dyescomboBox
            // 
            this.dyescomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dyescomboBox.FormattingEnabled = true;
            this.dyescomboBox.Items.AddRange(new object[] {
            "Custom..."});
            this.dyescomboBox.Location = new System.Drawing.Point(198, 25);
            this.dyescomboBox.Name = "dyescomboBox";
            this.dyescomboBox.Size = new System.Drawing.Size(192, 21);
            this.dyescomboBox.TabIndex = 2;
            this.dyescomboBox.SelectedIndexChanged += new System.EventHandler(this.dyescomboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Linker and dye";
            // 
            // atomIDnumericBoxInt32
            // 
            this.atomIDnumericBoxInt32.Format = "G";
            this.atomIDnumericBoxInt32.Increment = 1;
            this.atomIDnumericBoxInt32.IsIndicator = false;
            this.atomIDnumericBoxInt32.Location = new System.Drawing.Point(12, 117);
            this.atomIDnumericBoxInt32.MaxValue = 2147483647;
            this.atomIDnumericBoxInt32.MinValue = 0;
            this.atomIDnumericBoxInt32.Name = "atomIDnumericBoxInt32";
            this.atomIDnumericBoxInt32.Size = new System.Drawing.Size(74, 20);
            this.atomIDnumericBoxInt32.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.atomIDnumericBoxInt32.SpinBoxVisible = true;
            this.atomIDnumericBoxInt32.StrictIncrement = false;
            this.atomIDnumericBoxInt32.TabIndex = 4;
            this.atomIDnumericBoxInt32.Value = 1;
            this.atomIDnumericBoxInt32.ValueChanged += new System.EventHandler(this.atomIDnumericBoxInt32_ValueChanged);
            // 
            // atomnametextBox
            // 
            this.atomnametextBox.BackColor = System.Drawing.SystemColors.Window;
            this.atomnametextBox.Location = new System.Drawing.Point(117, 117);
            this.atomnametextBox.Name = "atomnametextBox";
            this.atomnametextBox.ReadOnly = true;
            this.atomnametextBox.Size = new System.Drawing.Size(60, 20);
            this.atomnametextBox.TabIndex = 5;
            this.atomnametextBox.Text = "Invalid ID";
            // 
            // LnumericBoxDouble
            // 
            this.LnumericBoxDouble.Format = "G";
            this.LnumericBoxDouble.Increment = 1;
            this.LnumericBoxDouble.IsIndicator = false;
            this.LnumericBoxDouble.Location = new System.Drawing.Point(198, 73);
            this.LnumericBoxDouble.MaxValue = 1.7976931348623157E+308;
            this.LnumericBoxDouble.MinValue = 0;
            this.LnumericBoxDouble.Name = "LnumericBoxDouble";
            this.LnumericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.LnumericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.LnumericBoxDouble.SpinBoxVisible = true;
            this.LnumericBoxDouble.StrictIncrement = false;
            this.LnumericBoxDouble.TabIndex = 8;
            this.LnumericBoxDouble.Value = 23.5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(195, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Length";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(261, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Width";
            // 
            // R1label
            // 
            this.R1label.AutoSize = true;
            this.R1label.Location = new System.Drawing.Point(327, 57);
            this.R1label.Name = "R1label";
            this.R1label.Size = new System.Drawing.Size(62, 13);
            this.R1label.TabIndex = 13;
            this.R1label.Text = "Dye Radius";
            // 
            // WnumericBoxDouble
            // 
            this.WnumericBoxDouble.Format = "G";
            this.WnumericBoxDouble.Increment = 1;
            this.WnumericBoxDouble.IsIndicator = false;
            this.WnumericBoxDouble.Location = new System.Drawing.Point(264, 73);
            this.WnumericBoxDouble.MaxValue = 1.7976931348623157E+308;
            this.WnumericBoxDouble.MinValue = 0;
            this.WnumericBoxDouble.Name = "WnumericBoxDouble";
            this.WnumericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.WnumericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.WnumericBoxDouble.SpinBoxVisible = true;
            this.WnumericBoxDouble.StrictIncrement = false;
            this.WnumericBoxDouble.TabIndex = 14;
            this.WnumericBoxDouble.Value = 4.5;
            // 
            // R1numericBoxDouble
            // 
            this.R1numericBoxDouble.Format = "G";
            this.R1numericBoxDouble.Increment = 1;
            this.R1numericBoxDouble.IsIndicator = false;
            this.R1numericBoxDouble.Location = new System.Drawing.Point(330, 73);
            this.R1numericBoxDouble.MaxValue = 1.7976931348623157E+308;
            this.R1numericBoxDouble.MinValue = 0;
            this.R1numericBoxDouble.Name = "R1numericBoxDouble";
            this.R1numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.R1numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.R1numericBoxDouble.SpinBoxVisible = true;
            this.R1numericBoxDouble.StrictIncrement = false;
            this.R1numericBoxDouble.TabIndex = 15;
            this.R1numericBoxDouble.Value = 3.5;
            // 
            // calculatebutton
            // 
            this.calculatebutton.Location = new System.Drawing.Point(12, 156);
            this.calculatebutton.Name = "calculatebutton";
            this.calculatebutton.Size = new System.Drawing.Size(74, 23);
            this.calculatebutton.TabIndex = 16;
            this.calculatebutton.Text = "Calculate";
            this.calculatebutton.UseVisualStyleBackColor = true;
            this.calculatebutton.Click += new System.EventHandler(this.calculatebutton_Click);
            // 
            // addbutton
            // 
            this.addbutton.Location = new System.Drawing.Point(168, 197);
            this.addbutton.Name = "addbutton";
            this.addbutton.Size = new System.Drawing.Size(70, 23);
            this.addbutton.TabIndex = 17;
            this.addbutton.Text = "Add";
            this.addbutton.UseVisualStyleBackColor = true;
            this.addbutton.Click += new System.EventHandler(this.addbutton_Click);
            // 
            // savebutton
            // 
            this.savebutton.Location = new System.Drawing.Point(244, 197);
            this.savebutton.Name = "savebutton";
            this.savebutton.Size = new System.Drawing.Size(70, 23);
            this.savebutton.TabIndex = 18;
            this.savebutton.Text = "Save";
            this.savebutton.UseVisualStyleBackColor = true;
            this.savebutton.Click += new System.EventHandler(this.saveXYZbutton_Click);
            // 
            // closebutton
            // 
            this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closebutton.Location = new System.Drawing.Point(320, 197);
            this.closebutton.Name = "closebutton";
            this.closebutton.Size = new System.Drawing.Size(70, 23);
            this.closebutton.TabIndex = 19;
            this.closebutton.Text = "Close";
            this.closebutton.UseVisualStyleBackColor = true;
            this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(92, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "=>";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(92, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "=>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Atom ID";
            // 
            // DradioButton
            // 
            this.DradioButton.AutoSize = true;
            this.DradioButton.Checked = true;
            this.DradioButton.Location = new System.Drawing.Point(95, 200);
            this.DradioButton.Name = "DradioButton";
            this.DradioButton.Size = new System.Drawing.Size(33, 17);
            this.DradioButton.TabIndex = 23;
            this.DradioButton.TabStop = true;
            this.DradioButton.Text = "D";
            this.DradioButton.UseVisualStyleBackColor = true;
            // 
            // AradioButton
            // 
            this.AradioButton.AutoSize = true;
            this.AradioButton.Location = new System.Drawing.Point(131, 200);
            this.AradioButton.Name = "AradioButton";
            this.AradioButton.Size = new System.Drawing.Size(32, 17);
            this.AradioButton.TabIndex = 24;
            this.AradioButton.Text = "A";
            this.AradioButton.UseVisualStyleBackColor = true;
            // 
            // rmptextBox
            // 
            this.rmptextBox.BackColor = System.Drawing.SystemColors.Window;
            this.rmptextBox.Location = new System.Drawing.Point(117, 158);
            this.rmptextBox.Name = "rmptextBox";
            this.rmptextBox.ReadOnly = true;
            this.rmptextBox.Size = new System.Drawing.Size(273, 20);
            this.rmptextBox.TabIndex = 25;
            this.rmptextBox.Text = "Ready";
            // 
            // lpnametextBox
            // 
            this.lpnametextBox.BackColor = System.Drawing.SystemColors.Window;
            this.lpnametextBox.Location = new System.Drawing.Point(12, 199);
            this.lpnametextBox.Name = "lpnametextBox";
            this.lpnametextBox.Size = new System.Drawing.Size(74, 20);
            this.lpnametextBox.TabIndex = 26;
            this.lpnametextBox.Text = "(Name)";
            // 
            // newmoleculebutton
            // 
            this.newmoleculebutton.Image = global::Fps.Properties.Resources.openHS;
            this.newmoleculebutton.Location = new System.Drawing.Point(157, 24);
            this.newmoleculebutton.Name = "newmoleculebutton";
            this.newmoleculebutton.Size = new System.Drawing.Size(30, 23);
            this.newmoleculebutton.TabIndex = 27;
            this.newmoleculebutton.UseVisualStyleBackColor = true;
            this.newmoleculebutton.Click += new System.EventHandler(this.newmoleculebutton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // simcombobox
            // 
            this.simcombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.simcombobox.FormattingEnabled = true;
            this.simcombobox.Items.AddRange(new object[] {
            "Simple AV",
            "Three radii AV"});
            this.simcombobox.Location = new System.Drawing.Point(12, 71);
            this.simcombobox.Name = "simcombobox";
            this.simcombobox.Size = new System.Drawing.Size(175, 21);
            this.simcombobox.TabIndex = 28;
            this.simcombobox.SelectedIndexChanged += new System.EventHandler(this.simcombobox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Simulation type";
            // 
            // R2numericBoxDouble
            // 
            this.R2numericBoxDouble.Format = "G";
            this.R2numericBoxDouble.Increment = 1;
            this.R2numericBoxDouble.IsIndicator = false;
            this.R2numericBoxDouble.Location = new System.Drawing.Point(330, 99);
            this.R2numericBoxDouble.MaxValue = 1.7976931348623157E+308;
            this.R2numericBoxDouble.MinValue = 0;
            this.R2numericBoxDouble.Name = "R2numericBoxDouble";
            this.R2numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.R2numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.R2numericBoxDouble.SpinBoxVisible = true;
            this.R2numericBoxDouble.StrictIncrement = false;
            this.R2numericBoxDouble.TabIndex = 30;
            this.R2numericBoxDouble.Value = 3.5;
            this.R2numericBoxDouble.Visible = false;
            // 
            // R3numericBoxDouble
            // 
            this.R3numericBoxDouble.Format = "G";
            this.R3numericBoxDouble.Increment = 1;
            this.R3numericBoxDouble.IsIndicator = false;
            this.R3numericBoxDouble.Location = new System.Drawing.Point(329, 125);
            this.R3numericBoxDouble.MaxValue = 1.7976931348623157E+308;
            this.R3numericBoxDouble.MinValue = 0;
            this.R3numericBoxDouble.Name = "R3numericBoxDouble";
            this.R3numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.R3numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.R3numericBoxDouble.SpinBoxVisible = true;
            this.R3numericBoxDouble.StrictIncrement = false;
            this.R3numericBoxDouble.TabIndex = 31;
            this.R3numericBoxDouble.Value = 3.5;
            this.R3numericBoxDouble.Visible = false;
            // 
            // dyestructbutton
            // 
            this.dyestructbutton.Image = global::Fps.Properties.Resources.openHS;
            this.dyestructbutton.Location = new System.Drawing.Point(359, 115);
            this.dyestructbutton.Name = "dyestructbutton";
            this.dyestructbutton.Size = new System.Drawing.Size(30, 23);
            this.dyestructbutton.TabIndex = 32;
            this.dyestructbutton.UseVisualStyleBackColor = true;
            this.dyestructbutton.Visible = false;
            // 
            // dyestructbox
            // 
            this.dyestructbox.BackColor = System.Drawing.SystemColors.Window;
            this.dyestructbox.Location = new System.Drawing.Point(198, 117);
            this.dyestructbox.Name = "dyestructbox";
            this.dyestructbox.ReadOnly = true;
            this.dyestructbox.Size = new System.Drawing.Size(155, 20);
            this.dyestructbox.TabIndex = 33;
            this.dyestructbox.Visible = false;
            // 
            // dyestructlabel
            // 
            this.dyestructlabel.AutoSize = true;
            this.dyestructlabel.Location = new System.Drawing.Point(195, 101);
            this.dyestructlabel.Name = "dyestructlabel";
            this.dyestructlabel.Size = new System.Drawing.Size(70, 13);
            this.dyestructlabel.TabIndex = 34;
            this.dyestructlabel.Text = "Dye structure";
            this.dyestructlabel.Visible = false;
            // 
            // AVinterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closebutton;
            this.ClientSize = new System.Drawing.Size(402, 231);
            this.Controls.Add(this.dyestructlabel);
            this.Controls.Add(this.dyestructbox);
            this.Controls.Add(this.dyestructbutton);
            this.Controls.Add(this.R3numericBoxDouble);
            this.Controls.Add(this.R2numericBoxDouble);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.simcombobox);
            this.Controls.Add(this.newmoleculebutton);
            this.Controls.Add(this.lpnametextBox);
            this.Controls.Add(this.rmptextBox);
            this.Controls.Add(this.AradioButton);
            this.Controls.Add(this.DradioButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.closebutton);
            this.Controls.Add(this.savebutton);
            this.Controls.Add(this.addbutton);
            this.Controls.Add(this.calculatebutton);
            this.Controls.Add(this.R1numericBoxDouble);
            this.Controls.Add(this.WnumericBoxDouble);
            this.Controls.Add(this.R1label);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LnumericBoxDouble);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.atomnametextBox);
            this.Controls.Add(this.atomIDnumericBoxInt32);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dyescomboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.moleculescomboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AVinterface";
            this.Text = "AV simulation";
            this.Load += new System.EventHandler(this.AVinterface_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox moleculescomboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox dyescomboBox;
        private System.Windows.Forms.Label label2;
        private NumericControls.NumericBoxInt32 atomIDnumericBoxInt32;
        private System.Windows.Forms.TextBox atomnametextBox;
        private NumericControls.NumericBoxDouble LnumericBoxDouble;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label R1label;
        private NumericControls.NumericBoxDouble WnumericBoxDouble;
        private NumericControls.NumericBoxDouble R1numericBoxDouble;
        private System.Windows.Forms.Button calculatebutton;
        private System.Windows.Forms.Button addbutton;
        private System.Windows.Forms.Button savebutton;
        private System.Windows.Forms.Button closebutton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RadioButton DradioButton;
        private System.Windows.Forms.RadioButton AradioButton;
        private System.Windows.Forms.TextBox rmptextBox;
        private System.Windows.Forms.TextBox lpnametextBox;
        private System.Windows.Forms.Button newmoleculebutton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox simcombobox;
        private System.Windows.Forms.Label label9;
        private NumericControls.NumericBoxDouble R2numericBoxDouble;
        private NumericControls.NumericBoxDouble R3numericBoxDouble;
        private System.Windows.Forms.Button dyestructbutton;
        private System.Windows.Forms.TextBox dyestructbox;
        private System.Windows.Forms.Label dyestructlabel;

    }
}