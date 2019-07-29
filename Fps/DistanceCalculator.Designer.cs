namespace Fps
{
    partial class DistanceCalculator
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
            this.AV1label = new System.Windows.Forms.Label();
            this.AV2label = new System.Windows.Forms.Label();
            this.loadAV1button = new System.Windows.Forms.Button();
            this.loadAV2button = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdaelabel = new System.Windows.Forms.Label();
            this.sigmadalabel = new System.Windows.Forms.Label();
            this.rdalabel = new System.Windows.Forms.Label();
            this.rmplabel = new System.Windows.Forms.Label();
            this.clipboardbutton = new System.Windows.Forms.Button();
            this.closebutton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.elabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AV1label
            // 
            this.AV1label.AutoSize = true;
            this.AV1label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AV1label.Location = new System.Drawing.Point(45, 14);
            this.AV1label.Name = "AV1label";
            this.AV1label.Size = new System.Drawing.Size(87, 16);
            this.AV1label.TabIndex = 0;
            this.AV1label.Text = "AV #1: Empty";
            // 
            // AV2label
            // 
            this.AV2label.AutoSize = true;
            this.AV2label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AV2label.Location = new System.Drawing.Point(45, 36);
            this.AV2label.Name = "AV2label";
            this.AV2label.Size = new System.Drawing.Size(87, 16);
            this.AV2label.TabIndex = 1;
            this.AV2label.Text = "AV #2: Empty";
            // 
            // loadAV1button
            // 
            this.loadAV1button.Image = global::Fps.Properties.Resources.openHS;
            this.loadAV1button.Location = new System.Drawing.Point(13, 10);
            this.loadAV1button.Name = "loadAV1button";
            this.loadAV1button.Size = new System.Drawing.Size(30, 23);
            this.loadAV1button.TabIndex = 2;
            this.loadAV1button.UseVisualStyleBackColor = true;
            this.loadAV1button.Click += new System.EventHandler(this.loadAVbutton_Click);
            // 
            // loadAV2button
            // 
            this.loadAV2button.Image = global::Fps.Properties.Resources.openHS;
            this.loadAV2button.Location = new System.Drawing.Point(13, 32);
            this.loadAV2button.Name = "loadAV2button";
            this.loadAV2button.Size = new System.Drawing.Size(30, 23);
            this.loadAV2button.TabIndex = 3;
            this.loadAV2button.UseVisualStyleBackColor = true;
            this.loadAV2button.Click += new System.EventHandler(this.loadAVbutton_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.elabel);
            this.panel1.Controls.Add(this.rdaelabel);
            this.panel1.Controls.Add(this.sigmadalabel);
            this.panel1.Controls.Add(this.rdalabel);
            this.panel1.Controls.Add(this.rmplabel);
            this.panel1.Location = new System.Drawing.Point(13, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(319, 109);
            this.panel1.TabIndex = 4;
            // 
            // rdaelabel
            // 
            this.rdaelabel.AutoSize = true;
            this.rdaelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdaelabel.Location = new System.Drawing.Point(3, 85);
            this.rdaelabel.Name = "rdaelabel";
            this.rdaelabel.Size = new System.Drawing.Size(60, 16);
            this.rdaelabel.TabIndex = 3;
            this.rdaelabel.Text = "<RDA>E";
            // 
            // sigmadalabel
            // 
            this.sigmadalabel.AutoSize = true;
            this.sigmadalabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sigmadalabel.Location = new System.Drawing.Point(3, 45);
            this.sigmadalabel.Name = "sigmadalabel";
            this.sigmadalabel.Size = new System.Drawing.Size(64, 16);
            this.sigmadalabel.TabIndex = 2;
            this.sigmadalabel.Text = "sigmaDA";
            // 
            // rdalabel
            // 
            this.rdalabel.AutoSize = true;
            this.rdalabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdalabel.Location = new System.Drawing.Point(3, 25);
            this.rdalabel.Name = "rdalabel";
            this.rdalabel.Size = new System.Drawing.Size(51, 16);
            this.rdalabel.TabIndex = 1;
            this.rdalabel.Text = "<RDA>";
            // 
            // rmplabel
            // 
            this.rmplabel.AutoSize = true;
            this.rmplabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rmplabel.Location = new System.Drawing.Point(3, 5);
            this.rmplabel.Name = "rmplabel";
            this.rmplabel.Size = new System.Drawing.Size(37, 16);
            this.rmplabel.TabIndex = 0;
            this.rmplabel.Text = "Rmp";
            // 
            // clipboardbutton
            // 
            this.clipboardbutton.Location = new System.Drawing.Point(11, 181);
            this.clipboardbutton.Name = "clipboardbutton";
            this.clipboardbutton.Size = new System.Drawing.Size(75, 23);
            this.clipboardbutton.TabIndex = 5;
            this.clipboardbutton.Text = "Clipboard";
            this.clipboardbutton.UseVisualStyleBackColor = true;
            this.clipboardbutton.Click += new System.EventHandler(this.Clipboard_Click);
            // 
            // closebutton
            // 
            this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closebutton.Location = new System.Drawing.Point(259, 181);
            this.closebutton.Name = "closebutton";
            this.closebutton.Size = new System.Drawing.Size(75, 23);
            this.closebutton.TabIndex = 6;
            this.closebutton.Text = "Close";
            this.closebutton.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // elabel
            // 
            this.elabel.AutoSize = true;
            this.elabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.elabel.Location = new System.Drawing.Point(3, 65);
            this.elabel.Name = "elabel";
            this.elabel.Size = new System.Drawing.Size(17, 16);
            this.elabel.TabIndex = 4;
            this.elabel.Text = "E";
            // 
            // DistanceCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closebutton;
            this.ClientSize = new System.Drawing.Size(344, 212);
            this.Controls.Add(this.closebutton);
            this.Controls.Add(this.clipboardbutton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.loadAV2button);
            this.Controls.Add(this.loadAV1button);
            this.Controls.Add(this.AV2label);
            this.Controls.Add(this.AV1label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DistanceCalculator";
            this.ShowIcon = false;
            this.Text = "DistanceCalculator";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AV1label;
        private System.Windows.Forms.Label AV2label;
        private System.Windows.Forms.Button loadAV1button;
        private System.Windows.Forms.Button loadAV2button;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label rdaelabel;
        private System.Windows.Forms.Label sigmadalabel;
        private System.Windows.Forms.Label rdalabel;
        private System.Windows.Forms.Label rmplabel;
        private System.Windows.Forms.Button clipboardbutton;
        private System.Windows.Forms.Button closebutton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label elabel;
    }
}