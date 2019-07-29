using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GPFileTools;

namespace GPFileToolsTest
{
    public struct VT1
    {
        public Int32 v1;
        public Double v2;
    }
    public partial class Form1 : Form
    {
        OptionsManager om = new OptionsManager();
        SpreadsheetAscii sr = new SpreadsheetAscii();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Double[] data1 = new Double[1];
            Double[] data2 = new Double[1];
            Double[,] dataz = new Double[1,1];

            String fheader = "";
            DateTime d1 = DateTime.Now;

            String[] burfiles = new String[] { @"D:\Programs\C# gp\GPFileTools\test data\m000.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m001.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m002.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m003.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m004.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m005.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m006.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m007.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m008.bur",
                 @"D:\Programs\C# gp\GPFileTools\test data\m009.bur" };

           // sr.ReadXY<Double>(burfiles, ref data1, ref data2, ref fheader, "Number of Photons (green)", "Number of Photons (red)");
            //sr.WriteXY(@"D:\Programs\C# gp\GPFileTaals\test data\", data1, data2, "Number of Photons (green)\tNumber of Photons (red)");
            // sr.ReadXY<Double>(@"D:\Programs\C# gp\GPFileTools\test data\m000.bur", ref data1, ref data2, "First photon", "Number of Photons (yellow)");
           // sr.WriteY(@"D:\Programs\C# gp\GPFileTools\test data\m000m.txt", data1, "N1\tN2");
            sr.Delimeters = new Char[] { '\t', ' '};
            sr.ReadZ<Double>(@"D:\Programs\sim2dlight\tw.txt", ref dataz);
            TimeSpan dd = DateTime.Now - d1;
            Int32 test = 15;
            //om.AddVariable((object) test, "Parameters", "Delta T");
            //om.Export(@"D:\Programs\C# gp\GPFileTools\test data\opts_z.txt");
            //om.Load(@"D:\Programs\C# gp\GPFileTools\test data\opts_z.bin");
        /*    String[] tLines = new String[data1.Length];
            for (int i = 0; i < data1.Length; i++)
            {
                tLines[i] = String.Format("{0:G}\t{1:G}", data1[i], data2[i]);
            }
            textBox1.Lines = tLines; */
            //textBox1.Text = dd.TotalMilliseconds.ToString();
            Int32 s = 0;
            for (Int32 i = 0; i < data1.Length; i++) if (data1[i] != 0) s++;

            //textBox1.Text += "ms; " + String.Format("{0:G} records read", data1.Length);
            om.ShowOptionsWindow();
            om.OverwritePrompt = false;
            om.Save(@"D:\Programs\C# gp\GPFileTools\test data\test.bin");
            om.Export(@"D:\Programs\C# gp\GPFileTools\test data\test.txt");
            //GC.Collect();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sr.OverwritePrompt = true;
            //sr.WriteZ<Double>(@"D:\Programs\C# gp\GPFileTools\test data\", dataz);

            om.AddCategory("General");
            om.AddProperty(this, "General", "Text");
            om.AddProperty(this, "General", "BackColor");
            om.AddProperty(this, "General", "Width");
            om.AddProperty(this, "General", "AutoSize");
            om.AddProperty(this, "General", "Height");
            om.AddProperty(this, "General", "AutoSizeMode");

            om.AddCategory("Parameters");
            sr.Delimeters = new Char[] { 'a', 'b', 'c', 'e' };
            om.AddObject(sr, "SpreadsheetWriter");
            om.AddObject(this.button2, "Button2");
            om.AddControl(this.numericUpDown1, "Parameters");
            om.AddControl(this.textBox1, "Parameters");
            om.AddProperty(this.numericUpDown1, "Parameters", "Value");
            om.AddCompositeControl(this);
            om.RemoveProperty("Form1", "label1");

            VT1 s;
            s.v1 = 1;
            s.v2 = 2.0;
            om.AddCategory("Structure");
            om.AddObject(s, "Structure");


        }
    }
}