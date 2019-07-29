using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Fps
{
    public partial class DistanceCalculator : Form
    {
        private Double R0 = 52;
        private Int32 Esamples = 0;
        private Vector3[] av1;
        private Vector3[] av2;
        private Double rmp, rda, sigmada, E, rdaE;
        private String avfile1, avfile2;

        public DistanceCalculator(Double r0, Int32 esamples)
        {
            R0 = r0;
            Esamples = esamples;
            InitializeComponent();
        }

        private void loadAVxyz(String avpath, Boolean loadav1)
        {
            String[] strdata;
            Char[] separator = new Char[] { ' ', '\t' };
            try
            {
                strdata = File.ReadAllLines(avpath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error reading file" + avpath, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (strdata == null || strdata.Length == 0)
            {
                MessageBox.Show("Invalid or empty AV file", "Error reading file" + avpath, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // size
            Int32 nlines, n = 0;
            if (!Int32.TryParse(strdata[0], out nlines))
            {
                MessageBox.Show("Invalid xyz file", "Error reading file" + avpath, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            // load
            String[] tmpstr;
            Double x, y, z;
            Vector3[] avtmp = new Vector3[nlines - 1];
            for (Int32 i = 0; i < nlines - 1; i++)
            {
                tmpstr = strdata[i + 2].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (Double.TryParse(tmpstr[1], out x) && Double.TryParse(tmpstr[2], out y) && Double.TryParse(tmpstr[3], out z))
                    avtmp[n++] = new Vector3(x, y, z);
            }
            if (n == 0)
            {
                MessageBox.Show("Invalid xyz file", "Error reading file" + avpath, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (loadav1)
            {
                av1 = new Vector3[n];
                Array.Copy(avtmp, av1, n);
                avfile1 = Path.GetFileNameWithoutExtension(avpath);
                AV1label.Text = "AV #1: " + avfile1 + " [" + n.ToString() + "]";
            }
            else
            {
                av2 = new Vector3[n];
                Array.Copy(avtmp, av2, n);
                avfile2 = Path.GetFileNameWithoutExtension(avpath);
                AV2label.Text = "AV #2: " + avfile2 + " [" + n.ToString() + "]";
            }
            Calculate();
        }

        private void Calculate()
        {
            if (av1 == null || av1.Length == 0 || av2 == null || av2.Length == 0) return;

            // Rmp
            Vector3 r1 = new Vector3(0.0, 0.0, 0.0);
            Vector3 r2 = new Vector3(0.0, 0.0, 0.0);
            for (Int32 i = 0; i < av1.Length; i++) r1 = r1 + av1[i];
            for (Int32 j = 0; j < av2.Length; j++) r2 = r2 + av2[j];
            r1 = r1 * (1.0/(Double)av1.Length);
            r2 = r2 * (1.0/(Double)av2.Length);
            rmp = Vector3.Abs(r1 - r2);
            rmplabel.Text = "Rmp = " + rmp.ToString("F1") + " A";

            // <RDA>, sigmaDA
            Random rnd = new Random();
            Double Rmean = 0.0, var = 0.0, tmp;
            for (Int32 j = 0; j < Esamples; j++)
                Rmean += Vector3.Abs(av1[rnd.Next(av1.Length - 1)] - av2[rnd.Next(av2.Length - 1)]);
            rda = Rmean / ((Double)Esamples);
            rdalabel.Text = "<RDA> = " + rda.ToString("F1") + " A";
            for (Int32 j = 0; j < Esamples; j++)
            {
                tmp = rda - Vector3.Abs(av1[rnd.Next(av1.Length - 1)] - av2[rnd.Next(av2.Length - 1)]);
                var += tmp * tmp;
            }
            sigmada = Math.Sqrt(var / Esamples);
            sigmadalabel.Text = "sigmaDA = " + sigmada.ToString("F1") + " A";

            // E, <RDA>E
            Double Emean = 0.0, R06 = R0 * R0 * R0 * R0 * R0 * R0;
            for (Int32 j = 0; j < Esamples; j++)
            {
                tmp = Vector3.SquareNormDiff(av1[rnd.Next(av1.Length - 1)], av2[rnd.Next(av2.Length - 1)]);
                Emean += 1.0 / (1.0 + tmp * tmp * tmp / R06);
            }
            E = Emean / ((Double)Esamples);
            elabel.Text = "E = " + E.ToString("F3");
            rdaE = R0 * Math.Pow((1.0 / E - 1.0), 1.0 / 6.0);
            rdaelabel.Text = "<RDA>E = " + rdaE.ToString("F1") + " A (R0 = " + R0.ToString() + " A)";
        }

        private void loadAVbutton_Click(object sender, EventArgs e)
        {
            Boolean loadav1 = (sender == (object)loadAV1button);
            openFileDialog1.Filter = "XYZ coordinate file (*.xyz)|*.xyz";
            openFileDialog1.Title = "Load accessible volume #" + (loadav1 ? "1" : "2");
            openFileDialog1.FileName = "";
            openFileDialog1.Multiselect = false;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                if (File.Exists(openFileDialog1.FileName))
                    loadAVxyz(openFileDialog1.FileName, loadav1);
        }

        private void Clipboard_Click(object sender, EventArgs e)
        {
            if (av1 == null || av1.Length == 0 || av2 == null || av2.Length == 0) return;
            Clipboard.Clear();
            String s = "AV #1\t" + avfile1 + "\nAV #2\t" + avfile2 + "\nRmp\t" + rmp.ToString("F1") + 
                "\n<RDA>\t" + rda.ToString("F1") + "\nsigmaDA\t" + sigmada.ToString("F1") +
                "\nE\t" + E.ToString("F3") + "\n<RDA>E\t" + rdaE.ToString("F1");
            Clipboard.SetText(s);
        }
    }
}