using System;
using System.Drawing;
using System.Windows.Forms;

namespace GPFileTools
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.Invalidate(true);
        }
        public ProgressForm(String[] filenames)
        {
            InitializeComponent();
            progressBar1.Maximum = filenames.Length;
            pffilenames = filenames;
            this.Invalidate(true);
        }

        private String[] pffilenames;

        public String[] Filenames
        {
            get { return pffilenames; }
            set 
            { 
                pffilenames = value;
                progressBar1.Maximum = pffilenames.Length;
            }
        }
        private Int32 pfCount;

        public Int32 Count
        {
            get { return pfCount; }
            set 
            {
                pfCount = value;
                progressBar1.Value = pfCount;
                progressBar1.Invalidate();
            }
        }

	

    }
}