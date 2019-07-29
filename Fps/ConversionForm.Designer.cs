namespace Fps
{
    partial class ConversionForm
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
            this.xarray = new NumericControls.NumericArrayDouble();
            this.yarray = new NumericControls.NumericArrayDouble();
            this.orderbox = new NumericControls.NumericBoxInt32();
            this.label1 = new System.Windows.Forms.Label();
            this.coeftsarray = new NumericControls.NumericArrayDouble();
            this.closebutton = new System.Windows.Forms.Button();
            this.copybutton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.errorBoxDouble = new NumericControls.NumericBoxDouble();
            this.inversecheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // xarray
            // 
            this.xarray.ElementsFlow = System.Windows.Forms.Orientation.Vertical;
            this.xarray.ElementsVisible = 10;
            this.xarray.EmptyElementsVisible = true;
            this.xarray.IndexSize = new System.Drawing.Size(40, 20);
            this.xarray.Location = new System.Drawing.Point(10, 33);
            this.xarray.Name = "xarray";
            this.xarray.Size = new System.Drawing.Size(101, 209);
            this.xarray.TabIndex = 0;
            this.xarray.Value = new double[0];
            // 
            // yarray
            // 
            this.yarray.ElementsFlow = System.Windows.Forms.Orientation.Vertical;
            this.yarray.ElementsVisible = 10;
            this.yarray.EmptyElementsVisible = true;
            this.yarray.IndexSize = new System.Drawing.Size(40, 20);
            this.yarray.Location = new System.Drawing.Point(119, 33);
            this.yarray.Name = "yarray";
            this.yarray.Size = new System.Drawing.Size(101, 209);
            this.yarray.TabIndex = 1;
            this.yarray.Value = new double[0];
            // 
            // orderbox
            // 
            this.orderbox.Format = "G";
            this.orderbox.Increment = 1;
            this.orderbox.IsIndicator = false;
            this.orderbox.Location = new System.Drawing.Point(236, 33);
            this.orderbox.MaxValue = 6;
            this.orderbox.MinValue = 1;
            this.orderbox.Name = "orderbox";
            this.orderbox.Size = new System.Drawing.Size(66, 20);
            this.orderbox.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.orderbox.SpinBoxVisible = true;
            this.orderbox.StrictIncrement = true;
            this.orderbox.TabIndex = 2;
            this.orderbox.Value = 3;
            this.orderbox.ValueChanged += new System.EventHandler(this.orderbox_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(233, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Polynom order";
            // 
            // coeftsarray
            // 
            this.coeftsarray.ElementsFlow = System.Windows.Forms.Orientation.Vertical;
            this.coeftsarray.ElementsVisible = 7;
            this.coeftsarray.EmptyElementsVisible = false;
            this.coeftsarray.IndexSize = new System.Drawing.Size(0, 20);
            this.coeftsarray.Location = new System.Drawing.Point(322, 33);
            this.coeftsarray.Name = "coeftsarray";
            this.coeftsarray.Size = new System.Drawing.Size(75, 146);
            this.coeftsarray.TabIndex = 4;
            this.coeftsarray.Value = new double[0];
            // 
            // closebutton
            // 
            this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closebutton.Location = new System.Drawing.Point(322, 219);
            this.closebutton.Name = "closebutton";
            this.closebutton.Size = new System.Drawing.Size(75, 23);
            this.closebutton.TabIndex = 5;
            this.closebutton.Text = "Close";
            this.closebutton.UseVisualStyleBackColor = true;
            this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
            // 
            // copybutton
            // 
            this.copybutton.Location = new System.Drawing.Point(322, 185);
            this.copybutton.Name = "copybutton";
            this.copybutton.Size = new System.Drawing.Size(75, 23);
            this.copybutton.TabIndex = 6;
            this.copybutton.Text = "Clipboard";
            this.copybutton.UseVisualStyleBackColor = true;
            this.copybutton.Click += new System.EventHandler(this.copybutton_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(236, 219);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(10, 13);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(29, 13);
            this.labelX.TabIndex = 10;
            this.labelX.Text = "Rmp";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(119, 13);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(49, 13);
            this.labelY.TabIndex = 11;
            this.labelY.Text = "<RDA>E";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(319, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Coefficients";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Error";
            // 
            // errorBoxDouble
            // 
            this.errorBoxDouble.Format = "F6";
            this.errorBoxDouble.Increment = 0;
            this.errorBoxDouble.IsIndicator = true;
            this.errorBoxDouble.Location = new System.Drawing.Point(236, 159);
            this.errorBoxDouble.MaxValue = 1.7976931348623157E+308;
            this.errorBoxDouble.MinValue = -1.7976931348623157E+308;
            this.errorBoxDouble.Name = "errorBoxDouble";
            this.errorBoxDouble.Size = new System.Drawing.Size(66, 20);
            this.errorBoxDouble.SpinBoxAlignment = System.Windows.Forms.LeftRightAlignment.Right;
            this.errorBoxDouble.SpinBoxVisible = false;
            this.errorBoxDouble.StrictIncrement = false;
            this.errorBoxDouble.TabIndex = 15;
            this.errorBoxDouble.Value = 0;
            // 
            // inversecheckBox
            // 
            this.inversecheckBox.AutoSize = true;
            this.inversecheckBox.Location = new System.Drawing.Point(236, 71);
            this.inversecheckBox.Name = "inversecheckBox";
            this.inversecheckBox.Size = new System.Drawing.Size(61, 17);
            this.inversecheckBox.TabIndex = 16;
            this.inversecheckBox.Text = "Inverse";
            this.inversecheckBox.UseVisualStyleBackColor = true;
            this.inversecheckBox.CheckedChanged += new System.EventHandler(this.orderbox_ValueChanged);
            // 
            // ConversionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closebutton;
            this.ClientSize = new System.Drawing.Size(412, 252);
            this.Controls.Add(this.inversecheckBox);
            this.Controls.Add(this.errorBoxDouble);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelY);
            this.Controls.Add(this.labelX);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.copybutton);
            this.Controls.Add(this.closebutton);
            this.Controls.Add(this.coeftsarray);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.orderbox);
            this.Controls.Add(this.yarray);
            this.Controls.Add(this.xarray);
            this.Name = "ConversionForm";
            this.Text = "Rmp - <RDA>E conversion";
            this.Load += new System.EventHandler(this.ConversionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NumericControls.NumericArrayDouble xarray;
        private NumericControls.NumericArrayDouble yarray;
        private NumericControls.NumericBoxInt32 orderbox;
        private System.Windows.Forms.Label label1;
        private NumericControls.NumericArrayDouble coeftsarray;
        private System.Windows.Forms.Button closebutton;
        private System.Windows.Forms.Button copybutton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private NumericControls.NumericBoxDouble errorBoxDouble;
        private System.Windows.Forms.CheckBox inversecheckBox;
    }
}