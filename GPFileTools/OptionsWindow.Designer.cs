namespace GPFileTools
{
    partial class OptionsWindow
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
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.categorieslist = new System.Windows.Forms.ListBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.optionsGridView = new System.Windows.Forms.DataGridView();
            this.Option = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Values = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.optionsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(378, 332);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 2;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(297, 332);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(216, 332);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // categorieslist
            // 
            this.categorieslist.FormattingEnabled = true;
            this.categorieslist.IntegralHeight = false;
            this.categorieslist.Location = new System.Drawing.Point(12, 13);
            this.categorieslist.Name = "categorieslist";
            this.categorieslist.Size = new System.Drawing.Size(127, 303);
            this.categorieslist.TabIndex = 5;
            this.categorieslist.SelectedIndexChanged += new System.EventHandler(this.categorieslist_SelectedIndexChanged);
            this.categorieslist.KeyDown += new System.Windows.Forms.KeyEventHandler(this.categorieslist_KeyDown);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLoad.Location = new System.Drawing.Point(12, 332);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 7;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(93, 332);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(90, 23);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save/Export";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // optionsGridView
            // 
            this.optionsGridView.AllowUserToAddRows = false;
            this.optionsGridView.AllowUserToResizeRows = false;
            this.optionsGridView.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.optionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.optionsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Option,
            this.Values});
            this.optionsGridView.Location = new System.Drawing.Point(145, 13);
            this.optionsGridView.Name = "optionsGridView";
            this.optionsGridView.RowHeadersWidth = 10;
            this.optionsGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.optionsGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.optionsGridView.ShowEditingIcon = false;
            this.optionsGridView.Size = new System.Drawing.Size(308, 303);
            this.optionsGridView.TabIndex = 9;
            this.optionsGridView.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.optionsGridView_UserDeletedRow);
            // 
            // Option
            // 
            this.Option.Frozen = true;
            this.Option.HeaderText = "Option";
            this.Option.Name = "Option";
            this.Option.ReadOnly = true;
            // 
            // Values
            // 
            this.Values.HeaderText = "Value";
            this.Values.Name = "Values";
            this.Values.Width = 202;
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 366);
            this.Controls.Add(this.optionsGridView);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.categorieslist);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.DoubleBuffered = true;
            this.Name = "OptionsWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Options";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.OptionsWindow_Layout);
            this.Load += new System.EventHandler(this.OptionsWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.optionsGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ListBox categorieslist;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.DataGridView optionsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Option;
        private System.Windows.Forms.DataGridViewTextBoxColumn Values;
    }
}