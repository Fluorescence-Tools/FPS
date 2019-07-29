using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NumericControlsTest
{
    public partial class Form1 : Form
    {
        private Int32[] m;
        private Int32[] Fdata = new Int32[] { 0, 3, 2, 5, 7 };


        public Int32[] F1Data
        {
            get { return Fdata; }
            set { Fdata = value; }
        }

        public Form1()
        {
            InitializeComponent();
            //this.numericArrayDouble1.TemplateElement.Increment = 2;
            //this.numericArrayDouble1.TemplateElement.Value = 3;
            //this.numericArrayDouble1.ApplyTemplate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m!=null) numericBoxDouble1.Value = m[m.Length - 1];
            this.Close();
        }

        private void numericDial1_ValueChanged(object sender, EventArgs e)
        {
            numericBoxDouble1.Value = numericDial1.Value;
        }

        private void numericBoxDouble1_ValueChanged(object sender, EventArgs e)
        {
            numericDial1.Value = numericBoxDouble1.Value; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m = new Int32[50];
            numericArrayDouble1.Visible = !numericArrayDouble1.Visible;
            foreach (Control c in this.Controls)
                if (c.GetType() == typeof(TextBox)) c.Visible = !c.Visible;
        }

        private void numericArrayDouble1_ValueChanged(object sender, EventArgs e)
        {
            this.numericBoxDouble1.Value = numericArrayDouble1[0];
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            button2.Select();
        }

    }
}