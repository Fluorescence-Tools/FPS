using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace Fps
{
    public partial class AVinterface : Form
    {
        private Boolean _standalone = false;
        public AVinterface(MoleculeList ms)
        {
            _molecules = ms;
            _standalone = false;
            InitializeComponent();
        }
        public AVinterface()    // standalone
        {
            InitializeComponent();
            _standalone = true;
            this.AllowLoadPDBs = true;
            addbutton.Visible = false;
        }

        private MoleculeList _molecules;
        private Molecule _lastcalculated;

        private LabelingPositionList _labelpos;
        public LabelingPositionList CalculatedPositions
        {
            get { return _labelpos; }
        }

        private AVGlobalParameters _avpar;
        public AVGlobalParameters AVGridParameters
        {
            get { return _avpar; }
            set { _avpar = value; }
        }
	
        private Int32 _natom;
        private AVEngine ave;
        private String _lastparams;

        private Boolean _allowLoadPDBs = false;
        public Boolean AllowLoadPDBs
        {
            get { return _allowLoadPDBs; }
            set
            {
                _allowLoadPDBs = value;
                newmoleculebutton.Visible = _allowLoadPDBs;
                moleculescomboBox.Width = _allowLoadPDBs ? 139 : 175;
            }
        }
	
        private void AVinterface_Load(object sender, EventArgs e)
        {
            rmptextBox.Text = "Ready";
            moleculescomboBox.Items.Clear();
            if (_standalone) _molecules = new MoleculeList(20);
            for (Int32 i = 0; i < _molecules.Count; i++) moleculescomboBox.Items.Add(_molecules[i].Name);
            if (moleculescomboBox.Items.Count > 0) moleculescomboBox.SelectedIndex = 0;
            calculatebutton.Enabled = (_molecules.Count > 0);

            // load default dye parameters
            dyescomboBox.Items.AddRange(LinkerData.LinkerList);
            dyescomboBox.SelectedIndex = 0;

            simcombobox.SelectedIndex = 0;

            _labelpos = new LabelingPositionList();
            savebutton.Enabled = false;
            addbutton.Enabled = false;
        }

        private void closebutton_Click(object sender, EventArgs e)
        {
            GC.Collect();
            this.Close();
        }

        private void moleculescomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 m = moleculescomboBox.SelectedIndex;
            atomIDnumericBoxInt32.Value = Math.Min(_molecules[m].MaxOriginalAtomID, atomIDnumericBoxInt32.Value);
            atomIDnumericBoxInt32.MaxValue = _molecules[m].MaxOriginalAtomID;
            atomIDnumericBoxInt32_ValueChanged(sender, e);
        }

        private void atomIDnumericBoxInt32_ValueChanged(object sender, EventArgs e)
        {
            Int32 m = moleculescomboBox.SelectedIndex;
            if (m < 0) return;
            Int32 n = Array.BinarySearch<Int32>(_molecules[m].OriginalAtomID, atomIDnumericBoxInt32.Value);
            if ((n < _molecules[m].NAtoms) && (n >= 0))
                atomnametextBox.Text = _molecules[m].Atoms[n] + " (" + _molecules[m].AtomsStandard[n] + ")";
            else atomnametextBox.Text = "Invalid ID";
            _natom = n;
        }

        private void dyescomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 d = dyescomboBox.SelectedIndex;
            if (d > 0)
            {
                LnumericBoxDouble.Value = LinkerData.LList[d - 1];
                WnumericBoxDouble.Value = LinkerData.WList[d - 1];
                if (simcombobox.SelectedIndex == 0) R1numericBoxDouble.Value = LinkerData.RList[d - 1];
                else if (simcombobox.SelectedIndex == 1) R1numericBoxDouble.Value = LinkerData.R1List[d - 1];
                R2numericBoxDouble.Value = LinkerData.R2List[d - 1];
                R3numericBoxDouble.Value = LinkerData.R3List[d - 1];
                if (LinkerData.DorA[d - 1] == "D") DradioButton.Checked = true;
                else if (LinkerData.DorA[d - 1] == "A") AradioButton.Checked = true;
            }
        }

        private void calculatebutton_Click(object sender, EventArgs e)
        {
            savebutton.Enabled = false;
            addbutton.Enabled = false;
            ave = _standalone ? new AVEngine(_molecules[moleculescomboBox.SelectedIndex]) : 
                new AVEngine(_molecules[moleculescomboBox.SelectedIndex], _avpar);
            _lastcalculated = _molecules[moleculescomboBox.SelectedIndex];
            if (simcombobox.SelectedIndex == 0) // simple AV
            {
                _lastparams = "molecule " + _lastcalculated.Name + " (" + _lastcalculated.FullFileName + "); atom "
                    + atomIDnumericBoxInt32.Value.ToString() + " (" + atomnametextBox.Text + "); linker length = "
                    + LnumericBoxDouble.Value.ToString() + " width = " + WnumericBoxDouble.Value.ToString()
                    + " dye radius = " + R1numericBoxDouble.Value.ToString();
                ave.Calculate1R(LnumericBoxDouble.Value, WnumericBoxDouble.Value, R1numericBoxDouble.Value, _natom);
            }
            else if (simcombobox.SelectedIndex == 1) // 3 radii
            {
                _lastparams = "molecule " + _lastcalculated.Name + " (" + _lastcalculated.FullFileName + "); atom "
                    + atomIDnumericBoxInt32.Value.ToString() + " (" + atomnametextBox.Text + "); linker length = "
                    + LnumericBoxDouble.Value.ToString() + " width = " + WnumericBoxDouble.Value.ToString()
                    + " dye radii = " + R1numericBoxDouble.Value.ToString() + " / " + R2numericBoxDouble.Value.ToString()
                    + " / " + R3numericBoxDouble.Value.ToString();
                ave.Calculate3R(LnumericBoxDouble.Value, WnumericBoxDouble.Value,
                    R1numericBoxDouble.Value, R2numericBoxDouble.Value, R3numericBoxDouble.Value, _natom);
            }
            rmptextBox.Text = "Rmp = (" + ave.Rmp.ToString() + ")";
            addbutton.Enabled = !_standalone;
            savebutton.Enabled = true;
        }

        private void addbutton_Click(object sender, EventArgs e)
        {
            if (_lastcalculated == null) return;
            LabelingPosition l = new LabelingPosition();
            l.Molecule = _lastcalculated.Name;
            l.Name = lpnametextBox.Text;
            l.X = ave.Rmp.X; l.Y = ave.Rmp.Y; l.Z = ave.Rmp.Z;
            l.Dye = this.DradioButton.Checked ? DyeType.Donor : DyeType.Acceptor;
            l.AVData.AVType = (AVSimlationType)(simcombobox.SelectedIndex + 1);
            l.AVData.AVReady = true; l.AVData.AtomID = atomIDnumericBoxInt32.Value;
            l.AVData.L = LnumericBoxDouble.Value; l.AVData.W = WnumericBoxDouble.Value;
            l.AVData.R = R1numericBoxDouble.Value; l.AVData.R1 = R1numericBoxDouble.Value;
            l.AVData.R2 = R2numericBoxDouble.Value; l.AVData.R3 = R3numericBoxDouble.Value;            
            _labelpos.Add(l);
        }

        private void saveXYZbutton_Click(object sender, EventArgs e)
        {
            if (ave == null) return;
            if ((ave.R == null) || (ave.R.Length == 0)) return;

            saveFileDialog1.Filter = "XYZ coordinate file (*.xyz)|*.xyz|Text file (*.txt)|*.txt|Binary density file (*.avsim)|*.avsim";
            String DorA = (DradioButton.Checked ? "D" : "A");
            saveFileDialog1.FileName = lpnametextBox.Text + "(" + DorA + ")";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                if (Path.GetExtension(saveFileDialog1.FileName).Equals(".xyz", StringComparison.OrdinalIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                    {
                        sw.WriteLine((ave.R.Length + 1).ToString());
                        sw.WriteLine(DorA + " dye; " + _lastparams);
                        for (Int32 i = 0; i < ave.R.Length; i++)
                            sw.WriteLine(DorA + ' ' + ave.R[i].ToString(' '));
                        sw.WriteLine(DorA + "mp " + ave.Rmp.ToString(' '));
                        sw.Close();
                    }
                }
                else if (Path.GetExtension(saveFileDialog1.FileName).Equals(".avsim", StringComparison.OrdinalIgnoreCase))
                {
                    Byte[] density = ave.Density;
                    using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        GZipStream gzfs = new GZipStream(fs, CompressionMode.Compress);
                        bw.Write(this.LnumericBoxDouble.Value);
                        bw.Write(this.WnumericBoxDouble.Value);
                        bw.Write(this.R1numericBoxDouble.Value);
                        bw.Write(ave.XGrid.Min); bw.Write(ave.XGrid.NNodes); bw.Write(ave.XGrid.Step);
                        bw.Write(ave.YGrid.Min); bw.Write(ave.YGrid.NNodes); bw.Write(ave.YGrid.Step);
                        bw.Write(ave.ZGrid.Min); bw.Write(ave.ZGrid.NNodes); bw.Write(ave.ZGrid.Step);
                        gzfs.Write(density, 0, density.Length);

                        bw.Close();
                        fs.Close();
                    }
                }
                else // saving as tab-delimited text
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                    {
                        for (Int32 i = 0; i < ave.R.Length; i++)
                            sw.WriteLine(ave.R[i].ToString('\t'));
                        sw.Close();
                    }
                }

            }
        }

        private void newmoleculebutton_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "pdb";
            openFileDialog1.Filter = "PDB files (*.pdb)|*.pdb|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                foreach (String filename in openFileDialog1.FileNames)
                {

                    Molecule m1 = new Molecule(filename);
                    if (m1.Error.Length > 0)
                    {
                        MessageBox.Show("Unable to load " + filename + ": " + m1.Error,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    _molecules.Add(m1);
                    this.moleculescomboBox.Items.Add(m1.Name);
                }
            }
            if (moleculescomboBox.Items.Count > 0)
            {
                moleculescomboBox.SelectedIndex = moleculescomboBox.Items.Count - 1;
                calculatebutton.Enabled = true;
            }
        }

        private void simcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 d = dyescomboBox.SelectedIndex;
            switch (simcombobox.SelectedIndex)
            {
                case 0:
                    R1numericBoxDouble.Visible = true;
                    if (d>0) R1numericBoxDouble.Value = LinkerData.RList[d - 1];
                    R2numericBoxDouble.Visible = false;
                    R3numericBoxDouble.Visible = false;
                    R1label.Visible = true;
                    R1label.Text = "Dye radius";
                    dyestructbox.Visible = false;
                    dyestructbutton.Visible = false;
                    dyestructlabel.Visible = false;
                    break;
                case 1:
                    R1numericBoxDouble.Visible = true;
                    if (d > 0) R1numericBoxDouble.Value = LinkerData.R1List[d - 1];
                    R2numericBoxDouble.Visible = true;
                    R3numericBoxDouble.Visible = true;
                    R1label.Visible = true;
                    R1label.Text = "Dye radii";
                    dyestructbox.Visible = false;
                    dyestructbutton.Visible = false;
                    dyestructlabel.Visible = false;
                    break;
                case 2:
                    R1numericBoxDouble.Visible = false;
                    R2numericBoxDouble.Visible = false;
                    R3numericBoxDouble.Visible = false;
                    R1label.Visible = false;
                    dyestructbox.Visible = true;
                    dyestructbutton.Visible = true;
                    dyestructlabel.Visible = true;
                    break;
                default:
                    break;
            }
        }


    }

}