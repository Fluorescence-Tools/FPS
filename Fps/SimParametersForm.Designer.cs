namespace Fps
{
    partial class SimParametersForm
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
            this.config_comboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ttol_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ftol_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label6 = new System.Windows.Forms.Label();
            this.ktol_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label5 = new System.Windows.Forms.Label();
            this.etol_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label1 = new System.Windows.Forms.Label();
            this.viscosity_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.niter_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.clashtol_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.clashtol_comboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.rkt_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dt_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.maxF_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.applybutton = new System.Windows.Forms.Button();
            this.selR_comboBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.polorder_numericBoxInt32 = new NumericControls.NumericBoxInt32();
            this.label11 = new System.Windows.Forms.Label();
            this.R0_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.esamples_numericBoxInt32 = new NumericControls.NumericBoxInt32();
            this.label18 = new System.Windows.Forms.Label();
            this.av_linksearchorder_numericBoxInt32 = new NumericControls.NumericBoxInt32();
            this.label17 = new System.Windows.Forms.Label();
            this.avgrid_min_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label16 = new System.Windows.Forms.Label();
            this.av_linkersphere_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.label15 = new System.Windows.Forms.Label();
            this.avgrid_rel_numericBoxDouble = new NumericControls.NumericBoxDouble();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // config_comboBox
            // 
            this.config_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.config_comboBox.FormattingEnabled = true;
            this.config_comboBox.Location = new System.Drawing.Point(12, 25);
            this.config_comboBox.Name = "config_comboBox";
            this.config_comboBox.Size = new System.Drawing.Size(157, 21);
            this.config_comboBox.TabIndex = 0;
            this.config_comboBox.SelectedIndexChanged += new System.EventHandler(this.config_comboBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ttol_numericBoxDouble);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.ftol_numericBoxDouble);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.ktol_numericBoxDouble);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.etol_numericBoxDouble);
            this.groupBox1.Location = new System.Drawing.Point(199, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(118, 195);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Convergence";
            // 
            // ttol_numericBoxDouble
            // 
            this.ttol_numericBoxDouble.Format = "G";
            this.ttol_numericBoxDouble.Increment = 0.01D;
            this.ttol_numericBoxDouble.IsIndicator = false;
            this.ttol_numericBoxDouble.Location = new System.Drawing.Point(52, 97);
            this.ttol_numericBoxDouble.MaxValue = 100D;
            this.ttol_numericBoxDouble.MinValue = 0D;
            this.ttol_numericBoxDouble.Name = "ttol_numericBoxDouble";
            this.ttol_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.ttol_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.ttol_numericBoxDouble.SpinBoxVisible = true;
            this.ttol_numericBoxDouble.StrictIncrement = false;
            this.ttol_numericBoxDouble.TabIndex = 11;
            this.ttol_numericBoxDouble.Value = 0.02D;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "|T|";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "|F|";
            // 
            // ftol_numericBoxDouble
            // 
            this.ftol_numericBoxDouble.Format = "G";
            this.ftol_numericBoxDouble.Increment = 0.01D;
            this.ftol_numericBoxDouble.IsIndicator = false;
            this.ftol_numericBoxDouble.Location = new System.Drawing.Point(52, 71);
            this.ftol_numericBoxDouble.MaxValue = 100D;
            this.ftol_numericBoxDouble.MinValue = 0D;
            this.ftol_numericBoxDouble.Name = "ftol_numericBoxDouble";
            this.ftol_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.ftol_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.ftol_numericBoxDouble.SpinBoxVisible = true;
            this.ftol_numericBoxDouble.StrictIncrement = false;
            this.ftol_numericBoxDouble.TabIndex = 8;
            this.ftol_numericBoxDouble.Value = 0.001D;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "K";
            // 
            // ktol_numericBoxDouble
            // 
            this.ktol_numericBoxDouble.Format = "G";
            this.ktol_numericBoxDouble.Increment = 0.01D;
            this.ktol_numericBoxDouble.IsIndicator = false;
            this.ktol_numericBoxDouble.Location = new System.Drawing.Point(52, 45);
            this.ktol_numericBoxDouble.MaxValue = 100D;
            this.ktol_numericBoxDouble.MinValue = 0D;
            this.ktol_numericBoxDouble.Name = "ktol_numericBoxDouble";
            this.ktol_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.ktol_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.ktol_numericBoxDouble.SpinBoxVisible = true;
            this.ktol_numericBoxDouble.StrictIncrement = false;
            this.ktol_numericBoxDouble.TabIndex = 6;
            this.ktol_numericBoxDouble.Value = 0.001D;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "E";
            // 
            // etol_numericBoxDouble
            // 
            this.etol_numericBoxDouble.Format = "G";
            this.etol_numericBoxDouble.Increment = 0.01D;
            this.etol_numericBoxDouble.IsIndicator = false;
            this.etol_numericBoxDouble.Location = new System.Drawing.Point(52, 19);
            this.etol_numericBoxDouble.MaxValue = 1000D;
            this.etol_numericBoxDouble.MinValue = 0D;
            this.etol_numericBoxDouble.Name = "etol_numericBoxDouble";
            this.etol_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.etol_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.etol_numericBoxDouble.SpinBoxVisible = true;
            this.etol_numericBoxDouble.StrictIncrement = false;
            this.etol_numericBoxDouble.TabIndex = 4;
            this.etol_numericBoxDouble.Value = 100D;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Configuration";
            // 
            // viscosity_numericBoxDouble
            // 
            this.viscosity_numericBoxDouble.Format = "G";
            this.viscosity_numericBoxDouble.Increment = 0.1D;
            this.viscosity_numericBoxDouble.IsIndicator = false;
            this.viscosity_numericBoxDouble.Location = new System.Drawing.Point(115, 19);
            this.viscosity_numericBoxDouble.MaxValue = 500D;
            this.viscosity_numericBoxDouble.MinValue = 0.01D;
            this.viscosity_numericBoxDouble.Name = "viscosity_numericBoxDouble";
            this.viscosity_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.viscosity_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.viscosity_numericBoxDouble.SpinBoxVisible = true;
            this.viscosity_numericBoxDouble.StrictIncrement = false;
            this.viscosity_numericBoxDouble.TabIndex = 3;
            this.viscosity_numericBoxDouble.Value = 1D;
            // 
            // niter_numericBoxDouble
            // 
            this.niter_numericBoxDouble.Format = "G";
            this.niter_numericBoxDouble.Increment = 50D;
            this.niter_numericBoxDouble.IsIndicator = false;
            this.niter_numericBoxDouble.Location = new System.Drawing.Point(115, 69);
            this.niter_numericBoxDouble.MaxValue = 1000D;
            this.niter_numericBoxDouble.MinValue = 0.001D;
            this.niter_numericBoxDouble.Name = "niter_numericBoxDouble";
            this.niter_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.niter_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.niter_numericBoxDouble.SpinBoxVisible = true;
            this.niter_numericBoxDouble.StrictIncrement = false;
            this.niter_numericBoxDouble.TabIndex = 4;
            this.niter_numericBoxDouble.Value = 100D;
            // 
            // clashtol_numericBoxDouble
            // 
            this.clashtol_numericBoxDouble.Format = "G";
            this.clashtol_numericBoxDouble.Increment = 0.01D;
            this.clashtol_numericBoxDouble.IsIndicator = false;
            this.clashtol_numericBoxDouble.Location = new System.Drawing.Point(115, 119);
            this.clashtol_numericBoxDouble.MaxValue = 10D;
            this.clashtol_numericBoxDouble.MinValue = 0.01D;
            this.clashtol_numericBoxDouble.Name = "clashtol_numericBoxDouble";
            this.clashtol_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.clashtol_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.clashtol_numericBoxDouble.SpinBoxVisible = true;
            this.clashtol_numericBoxDouble.StrictIncrement = false;
            this.clashtol_numericBoxDouble.TabIndex = 5;
            this.clashtol_numericBoxDouble.Value = 0.15D;
            // 
            // clashtol_comboBox
            // 
            this.clashtol_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clashtol_comboBox.Enabled = false;
            this.clashtol_comboBox.FormattingEnabled = true;
            this.clashtol_comboBox.Items.AddRange(new object[] {
            "^2",
            "^6",
            "^12"});
            this.clashtol_comboBox.Location = new System.Drawing.Point(115, 169);
            this.clashtol_comboBox.Name = "clashtol_comboBox";
            this.clashtol_comboBox.Size = new System.Drawing.Size(60, 21);
            this.clashtol_comboBox.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.rkt_numericBoxDouble);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.dt_numericBoxDouble);
            this.groupBox2.Controls.Add(this.maxF_numericBoxDouble);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.clashtol_comboBox);
            this.groupBox2.Controls.Add(this.viscosity_numericBoxDouble);
            this.groupBox2.Controls.Add(this.niter_numericBoxDouble);
            this.groupBox2.Controls.Add(this.clashtol_numericBoxDouble);
            this.groupBox2.Location = new System.Drawing.Point(12, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(181, 195);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Simulation";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(40, 147);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 13);
            this.label20.TabIndex = 17;
            this.label20.Text = "reciprocal kT";
            // 
            // rkt_numericBoxDouble
            // 
            this.rkt_numericBoxDouble.Enabled = false;
            this.rkt_numericBoxDouble.Format = "G";
            this.rkt_numericBoxDouble.Increment = 0.01D;
            this.rkt_numericBoxDouble.IsIndicator = false;
            this.rkt_numericBoxDouble.Location = new System.Drawing.Point(115, 144);
            this.rkt_numericBoxDouble.MaxValue = 1000D;
            this.rkt_numericBoxDouble.MinValue = 0.0001D;
            this.rkt_numericBoxDouble.Name = "rkt_numericBoxDouble";
            this.rkt_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.rkt_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.rkt_numericBoxDouble.SpinBoxVisible = true;
            this.rkt_numericBoxDouble.StrictIncrement = false;
            this.rkt_numericBoxDouble.TabIndex = 16;
            this.rkt_numericBoxDouble.Value = 10D;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(55, 97);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "Max force";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(39, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "dt adjustment";
            // 
            // dt_numericBoxDouble
            // 
            this.dt_numericBoxDouble.Format = "G";
            this.dt_numericBoxDouble.Increment = 0.1D;
            this.dt_numericBoxDouble.IsIndicator = false;
            this.dt_numericBoxDouble.Location = new System.Drawing.Point(115, 44);
            this.dt_numericBoxDouble.MaxValue = 100D;
            this.dt_numericBoxDouble.MinValue = 0.01D;
            this.dt_numericBoxDouble.Name = "dt_numericBoxDouble";
            this.dt_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.dt_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.dt_numericBoxDouble.SpinBoxVisible = true;
            this.dt_numericBoxDouble.StrictIncrement = false;
            this.dt_numericBoxDouble.TabIndex = 11;
            this.dt_numericBoxDouble.Value = 1D;
            // 
            // maxF_numericBoxDouble
            // 
            this.maxF_numericBoxDouble.Format = "G";
            this.maxF_numericBoxDouble.Increment = 1D;
            this.maxF_numericBoxDouble.IsIndicator = false;
            this.maxF_numericBoxDouble.Location = new System.Drawing.Point(115, 94);
            this.maxF_numericBoxDouble.MaxValue = 1000000000000D;
            this.maxF_numericBoxDouble.MinValue = 4D;
            this.maxF_numericBoxDouble.Name = "maxF_numericBoxDouble";
            this.maxF_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.maxF_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.maxF_numericBoxDouble.SpinBoxVisible = true;
            this.maxF_numericBoxDouble.StrictIncrement = false;
            this.maxF_numericBoxDouble.TabIndex = 14;
            this.maxF_numericBoxDouble.Value = 400D;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(33, 172);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Clash potential";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Clash tolerance (A)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Max iterations (k)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Viscosity adjustment";
            // 
            // cancelbutton
            // 
            this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbutton.Location = new System.Drawing.Point(242, 420);
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.cancelbutton.TabIndex = 8;
            this.cancelbutton.Text = "Cancel";
            this.cancelbutton.UseVisualStyleBackColor = true;
            this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
            // 
            // applybutton
            // 
            this.applybutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applybutton.Location = new System.Drawing.Point(161, 420);
            this.applybutton.Name = "applybutton";
            this.applybutton.Size = new System.Drawing.Size(75, 23);
            this.applybutton.TabIndex = 9;
            this.applybutton.Text = "OK";
            this.applybutton.UseVisualStyleBackColor = true;
            this.applybutton.Click += new System.EventHandler(this.applybutton_Click);
            // 
            // selR_comboBox
            // 
            this.selR_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selR_comboBox.FormattingEnabled = true;
            this.selR_comboBox.Items.AddRange(new object[] {
            "Selected",
            "All",
            "Selected, then all"});
            this.selR_comboBox.Location = new System.Drawing.Point(175, 25);
            this.selR_comboBox.Name = "selR_comboBox";
            this.selR_comboBox.Size = new System.Drawing.Size(142, 21);
            this.selR_comboBox.TabIndex = 10;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(176, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Optimize distances:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.polorder_numericBoxInt32);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.R0_numericBoxDouble);
            this.groupBox3.Location = new System.Drawing.Point(12, 259);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(305, 48);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Conversion function";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(159, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Polynom order";
            // 
            // polorder_numericBoxInt32
            // 
            this.polorder_numericBoxInt32.Format = "G";
            this.polorder_numericBoxInt32.Increment = 1;
            this.polorder_numericBoxInt32.IsIndicator = false;
            this.polorder_numericBoxInt32.Location = new System.Drawing.Point(240, 19);
            this.polorder_numericBoxInt32.MaxValue = 6;
            this.polorder_numericBoxInt32.MinValue = 1;
            this.polorder_numericBoxInt32.Name = "polorder_numericBoxInt32";
            this.polorder_numericBoxInt32.Size = new System.Drawing.Size(59, 20);
            this.polorder_numericBoxInt32.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.polorder_numericBoxInt32.SpinBoxVisible = true;
            this.polorder_numericBoxInt32.StrictIncrement = true;
            this.polorder_numericBoxInt32.TabIndex = 15;
            this.polorder_numericBoxInt32.Value = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Foerster radius";
            // 
            // R0_numericBoxDouble
            // 
            this.R0_numericBoxDouble.Format = "G";
            this.R0_numericBoxDouble.Increment = 0.5D;
            this.R0_numericBoxDouble.IsIndicator = false;
            this.R0_numericBoxDouble.Location = new System.Drawing.Point(89, 19);
            this.R0_numericBoxDouble.MaxValue = 1000D;
            this.R0_numericBoxDouble.MinValue = 0.01D;
            this.R0_numericBoxDouble.Name = "R0_numericBoxDouble";
            this.R0_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.R0_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.R0_numericBoxDouble.SpinBoxVisible = true;
            this.R0_numericBoxDouble.StrictIncrement = false;
            this.R0_numericBoxDouble.TabIndex = 13;
            this.R0_numericBoxDouble.Value = 52D;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.esamples_numericBoxInt32);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.av_linksearchorder_numericBoxInt32);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.avgrid_min_numericBoxDouble);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.av_linkersphere_numericBoxDouble);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.avgrid_rel_numericBoxDouble);
            this.groupBox4.Location = new System.Drawing.Point(12, 313);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(305, 101);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "AV simulations";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(13, 74);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(70, 13);
            this.label19.TabIndex = 14;
            this.label19.Text = "E samples (k)";
            // 
            // esamples_numericBoxInt32
            // 
            this.esamples_numericBoxInt32.Format = "G";
            this.esamples_numericBoxInt32.Increment = 10;
            this.esamples_numericBoxInt32.IsIndicator = false;
            this.esamples_numericBoxInt32.Location = new System.Drawing.Point(89, 71);
            this.esamples_numericBoxInt32.MaxValue = 1000;
            this.esamples_numericBoxInt32.MinValue = 10;
            this.esamples_numericBoxInt32.Name = "esamples_numericBoxInt32";
            this.esamples_numericBoxInt32.Size = new System.Drawing.Size(60, 20);
            this.esamples_numericBoxInt32.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.esamples_numericBoxInt32.SpinBoxVisible = true;
            this.esamples_numericBoxInt32.StrictIncrement = false;
            this.esamples_numericBoxInt32.TabIndex = 15;
            this.esamples_numericBoxInt32.Value = 200;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(160, 48);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(73, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "Search nodes";
            // 
            // av_linksearchorder_numericBoxInt32
            // 
            this.av_linksearchorder_numericBoxInt32.Format = "G";
            this.av_linksearchorder_numericBoxInt32.Increment = 1;
            this.av_linksearchorder_numericBoxInt32.IsIndicator = false;
            this.av_linksearchorder_numericBoxInt32.Location = new System.Drawing.Point(239, 45);
            this.av_linksearchorder_numericBoxInt32.MaxValue = 10;
            this.av_linksearchorder_numericBoxInt32.MinValue = 1;
            this.av_linksearchorder_numericBoxInt32.Name = "av_linksearchorder_numericBoxInt32";
            this.av_linksearchorder_numericBoxInt32.Size = new System.Drawing.Size(60, 20);
            this.av_linksearchorder_numericBoxInt32.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.av_linksearchorder_numericBoxInt32.SpinBoxVisible = true;
            this.av_linksearchorder_numericBoxInt32.StrictIncrement = false;
            this.av_linksearchorder_numericBoxInt32.TabIndex = 6;
            this.av_linksearchorder_numericBoxInt32.Value = 3;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(170, 22);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Min. grid (A)";
            // 
            // avgrid_min_numericBoxDouble
            // 
            this.avgrid_min_numericBoxDouble.Format = "G";
            this.avgrid_min_numericBoxDouble.Increment = 0.1D;
            this.avgrid_min_numericBoxDouble.IsIndicator = false;
            this.avgrid_min_numericBoxDouble.Location = new System.Drawing.Point(239, 19);
            this.avgrid_min_numericBoxDouble.MaxValue = 10D;
            this.avgrid_min_numericBoxDouble.MinValue = 0.1D;
            this.avgrid_min_numericBoxDouble.Name = "avgrid_min_numericBoxDouble";
            this.avgrid_min_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.avgrid_min_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.avgrid_min_numericBoxDouble.SpinBoxVisible = true;
            this.avgrid_min_numericBoxDouble.StrictIncrement = false;
            this.avgrid_min_numericBoxDouble.TabIndex = 4;
            this.avgrid_min_numericBoxDouble.Value = 0.4D;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 13);
            this.label16.TabIndex = 3;
            this.label16.Text = "Allowed sphere";
            // 
            // av_linkersphere_numericBoxDouble
            // 
            this.av_linkersphere_numericBoxDouble.Format = "G";
            this.av_linkersphere_numericBoxDouble.Increment = 0.1D;
            this.av_linkersphere_numericBoxDouble.IsIndicator = false;
            this.av_linkersphere_numericBoxDouble.Location = new System.Drawing.Point(89, 45);
            this.av_linkersphere_numericBoxDouble.MaxValue = 10D;
            this.av_linkersphere_numericBoxDouble.MinValue = 0D;
            this.av_linkersphere_numericBoxDouble.Name = "av_linkersphere_numericBoxDouble";
            this.av_linkersphere_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.av_linkersphere_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.av_linkersphere_numericBoxDouble.SpinBoxVisible = true;
            this.av_linkersphere_numericBoxDouble.StrictIncrement = false;
            this.av_linkersphere_numericBoxDouble.TabIndex = 2;
            this.av_linkersphere_numericBoxDouble.Value = 0.5D;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(22, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "AV grid (rel)";
            // 
            // avgrid_rel_numericBoxDouble
            // 
            this.avgrid_rel_numericBoxDouble.Format = "G";
            this.avgrid_rel_numericBoxDouble.Increment = 0.1D;
            this.avgrid_rel_numericBoxDouble.IsIndicator = false;
            this.avgrid_rel_numericBoxDouble.Location = new System.Drawing.Point(89, 19);
            this.avgrid_rel_numericBoxDouble.MaxValue = 1D;
            this.avgrid_rel_numericBoxDouble.MinValue = 0.1D;
            this.avgrid_rel_numericBoxDouble.Name = "avgrid_rel_numericBoxDouble";
            this.avgrid_rel_numericBoxDouble.Size = new System.Drawing.Size(60, 20);
            this.avgrid_rel_numericBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.avgrid_rel_numericBoxDouble.SpinBoxVisible = true;
            this.avgrid_rel_numericBoxDouble.StrictIncrement = false;
            this.avgrid_rel_numericBoxDouble.TabIndex = 0;
            this.avgrid_rel_numericBoxDouble.Value = 0.2D;
            // 
            // SimParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelbutton;
            this.ClientSize = new System.Drawing.Size(326, 451);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.selR_comboBox);
            this.Controls.Add(this.applybutton);
            this.Controls.Add(this.cancelbutton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.config_comboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SimParametersForm";
            this.ShowIcon = false;
            this.Text = "FPS parameters:";
            this.Load += new System.EventHandler(this.SimParameters_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox config_comboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private NumericControls.NumericBoxDouble viscosity_numericBoxDouble;
        private NumericControls.NumericBoxDouble niter_numericBoxDouble;
        private NumericControls.NumericBoxDouble clashtol_numericBoxDouble;
        private System.Windows.Forms.ComboBox clashtol_comboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private NumericControls.NumericBoxDouble ftol_numericBoxDouble;
        private System.Windows.Forms.Label label6;
        private NumericControls.NumericBoxDouble ktol_numericBoxDouble;
        private System.Windows.Forms.Label label5;
        private NumericControls.NumericBoxDouble etol_numericBoxDouble;
        private NumericControls.NumericBoxDouble ttol_numericBoxDouble;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button cancelbutton;
        private System.Windows.Forms.Button applybutton;
        private System.Windows.Forms.ComboBox selR_comboBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private NumericControls.NumericBoxDouble R0_numericBoxDouble;
        private System.Windows.Forms.Label label12;
        private NumericControls.NumericBoxInt32 polorder_numericBoxInt32;
        private System.Windows.Forms.Label label13;
        private NumericControls.NumericBoxDouble dt_numericBoxDouble;
        private System.Windows.Forms.Label label14;
        private NumericControls.NumericBoxDouble maxF_numericBoxDouble;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label15;
        private NumericControls.NumericBoxDouble avgrid_rel_numericBoxDouble;
        private System.Windows.Forms.Label label16;
        private NumericControls.NumericBoxDouble av_linkersphere_numericBoxDouble;
        private NumericControls.NumericBoxInt32 av_linksearchorder_numericBoxInt32;
        private System.Windows.Forms.Label label17;
        private NumericControls.NumericBoxDouble avgrid_min_numericBoxDouble;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private NumericControls.NumericBoxInt32 esamples_numericBoxInt32;
        private System.Windows.Forms.Label label20;
        private NumericControls.NumericBoxDouble rkt_numericBoxDouble;
    }
}