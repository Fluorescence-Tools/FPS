using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Fps
{   
    public enum FPSMode { None, Dock, Filter };

    public partial class SpringTheoryMain : Form
    {
        #region Fields and properties

        private FPSMode _fpsmode = FPSMode.None;

        MoleculeList molecules = new MoleculeList(20);
        String[] molecules_to_filter;
        LabelingPositionList labelingpos;
        DistanceList dist, convdist;
        private SimulationResult[] srs;
        private SimulationResult sr_rmsdreference;
        private FilteringResult[] frs;
        private FilteringResult fr_rmsdreference;
        private bool rmsdvsref;

        private SpringEngine se;
        private FilterEngine fe;
        private DateTime time0;
        private int nthreads_user = -1;
        private int nrepeats;
        private List<Vector3[]> avcache;
        private double[] globalcf, globalcfinv;  // conversion functions
        private Object current_structure_lock = new Object();
        private Object structures_done_lock = new Object();
        private SaveForm saveform;
        private SimParametersForm spform;
        private ConversionForm convform;

        private Color refinedcolor = Color.LightGreen;
        private Color errorcolor = Color.LightSalmon;

        private SimulationJobManager sjm;
        private FilterJobManager fjm;
        private int NThreads;
        private bool _ischecked = false;
        public bool IsChecked
        {
            get { return _ischecked; }
            set 
            {
                _ischecked = value;
                this.startstopButton.Enabled = _ischecked;
                this.timer1.Enabled = !_ischecked;
                if (_ischecked) statusLabel1.Text = "Ready";
            }
        }
        private bool _ishighlighted = false;

        private ProjectData projectdata = new ProjectData();
        private GPFileTools.OptionsManager om;
        #endregion

        public SpringTheoryMain()
        {
            InitializeComponent();
        }
        public SpringTheoryMain(int _nthreads_user)
        {
            this.nthreads_user = _nthreads_user;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.statusLabel1.Width = this.statusStrip1.Width - this.stripProgressBar1.Width - 12;

            om = new GPFileTools.OptionsManager();
            om.AddCategory("Data");
            om.AddProperty(projectdata, "Data", "ProjectFPSMode");
            om.AddProperty(projectdata, "Data", "MoleculesPaths");
            om.AddProperty(projectdata, "Data", "LabelingPositionsPath");
            om.AddProperty(projectdata, "Data", "DistancesPath");
            om.AddCategory("Search parameters");
            om.AddProperty(projectdata, "Search parameters", "DockParameters");
            om.AddCategory("Refine parameters");
            om.AddProperty(projectdata, "Refine parameters", "RefineParameters");
            om.AddCategory("Error parameters");
            om.AddProperty(projectdata, "Error parameters", "ErrorEstimationParameters");
            om.AddCategory("Sample parameters");
            om.AddProperty(projectdata, "Sample parameters", "SampleParameters");
            om.AddCategory("Screening parameters");
            om.AddProperty(projectdata, "Screening parameters", "ScreeningParameters");
            om.AddCategory("Conversion function");
            om.AddProperty(projectdata, "Conversion function", "ConversionParameters");
            om.AddCategory("AV parameters");
            om.AddProperty(projectdata, "AV parameters", "AVGlobalParameters");
            om.AddCategory("Selected distances");
            om.AddProperty(projectdata, "Selected distances", "SelectedDistances");

            this.LabelsdataGridView.Columns[2].ValueType = typeof(double);
            this.LabelsdataGridView.Columns[3].ValueType = typeof(double);
            this.LabelsdataGridView.Columns[4].ValueType = typeof(double);
            this.LabelsdataGridView.Columns[2].DefaultCellStyle.Format = "F3";
            this.LabelsdataGridView.Columns[3].DefaultCellStyle.Format = "F3";
            this.LabelsdataGridView.Columns[4].DefaultCellStyle.Format = "F3";

            this.RdataGridView.Columns[3].ValueType = typeof(double);
            this.RdataGridView.Columns[4].ValueType = typeof(double);
            this.RdataGridView.Columns[5].ValueType = typeof(double);
            this.RdataGridView.Columns[6].ValueType = typeof(double);
            this.RdataGridView.Columns[7].ValueType = typeof(double);
            this.RdataGridView.Columns[3].DefaultCellStyle.Format = "F1";
            this.RdataGridView.Columns[4].DefaultCellStyle.Format = "F1";
            this.RdataGridView.Columns[5].DefaultCellStyle.Format = "F1";
            this.RdataGridView.Columns[6].DefaultCellStyle.Format = "F1";
            this.RdataGridView.Columns[7].DefaultCellStyle.Format = "F1";

            this.structuredataGridView.Columns[0].ValueType = typeof(SimulationResult);
            this.structuredataGridView.Columns[1].ValueType = typeof(double);
            this.structuredataGridView.Columns[2].ValueType = typeof(double);
            this.structuredataGridView.Columns[0].DefaultCellStyle.Format = "D";
            this.structuredataGridView.Columns[1].DefaultCellStyle.Format = "F6";
            this.structuredataGridView.Columns[2].DefaultCellStyle.Format = "F6";

            spform = new SimParametersForm(projectdata);
            convform = new ConversionForm();
            srs = new SimulationResult[0];

            if (nthreads_user > 0) NThreads = nthreads_user;
            else
            {
                switch (Environment.ProcessorCount)
                {
                    case 1: NThreads = 1; break;
                    case 2:
                    case 3: NThreads = 2; break;
                    default: NThreads = Environment.ProcessorCount - 1; break;
                }
            }

            sjm = new SimulationJobManager();
            sjm.NThreads = this.NThreads;
            fjm = new FilterJobManager();
            fjm.NThreads = this.NThreads;

            timer1.Enabled = true;
#if DEBUG
            this.debugToolStripMenuItem.Visible = true;
#else
            this.debugToolStripMenuItem.Visible = false;
#endif

        }

        #region Load-Save

        // change dock <-> filter <-> unknown
        private void ChangeFPSMode(FPSMode newmode)
        {
            actioncomboBox.Items.Clear();
            switch (newmode)
            {
                case FPSMode.None:
                    moleculesToolStripMenuItem.Enabled = true;
                    moleculesForScreeningToolStripMenuItem.Enabled = true;
                    moleculesForScreeningfolderToolStripMenuItem.Enabled = true;
                    MoleculeslistBox.Enabled = true;
                    mAddbutton.Enabled = true;
                    tabControl1.TabPages[1].Text = "View results / Filter";
                    break;
                case FPSMode.Dock:
                    moleculesToolStripMenuItem.Enabled = true;
                    moleculesForScreeningToolStripMenuItem.Enabled = false;
                    moleculesForScreeningfolderToolStripMenuItem.Enabled = false;
                    MoleculeslistBox.Enabled = true;
                    mAddbutton.Enabled = true;
                    actioncomboBox.Items.AddRange(ProjectData.DockModes);
                    actioncomboBox.SelectedIndex = 0;
                    tabControl1.TabPages[1].Text = "View results";
                    repetitionslabel.Visible = true;
                    numberofrepetitions.Visible = true;
                    rmsdbutton.Visible = true;
                    bestfitcheckBox.Visible = true;
                    break;
                case FPSMode.Filter:
                    moleculesToolStripMenuItem.Enabled = false;
                    moleculesForScreeningToolStripMenuItem.Enabled = true;
                    moleculesForScreeningfolderToolStripMenuItem.Enabled = true;
                    MoleculeslistBox.Enabled = false;
                    mAddbutton.Enabled = false;             
                    actioncomboBox.Items.AddRange(ProjectData.FilterModes);
                    actioncomboBox.SelectedIndex = 0;
                    tabControl1.TabPages[1].Text = "Filter";
                    repetitionslabel.Visible = false;
                    numberofrepetitions.Visible = false;
                    rmsdbutton.Visible = true;
                    bestfitcheckBox.Visible = true;
                    break;
                default:
                    break;
            }
            _fpsmode = newmode;
            IsChecked = false;
        }

        // load molecules for docking
        private void mAddbutton_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "pdb";
            openFileDialog1.Filter = "PDB files (*.pdb)|*.pdb|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Multiselect = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                foreach (String filename in openFileDialog1.FileNames)
                    AddMolecule(filename);

            openFileDialog1.Multiselect = false;
            IsChecked = false;
            if (molecules.Count != 0) ChangeFPSMode(FPSMode.Dock);
        }
        // remove molecules
        private void mDeletebutton_Click(object sender, EventArgs e)
        {
            int n = this.MoleculeslistBox.SelectedIndex;
            if (n >= 0 && n < molecules.Count)
            {
                molecules[n].Dispose();
                molecules.RemoveAt(n);
                this.MoleculeslistBox.Items.RemoveAt(n);
                IsChecked = false;
            }
            if (molecules.Count == 0) ChangeFPSMode(FPSMode.None);
        }

        // load structures for filtering
        private void LoadMoleculesForFiltering(String[] molecules_to_filter)
        {
            frs = new FilteringResult[molecules_to_filter.Length];
            for (int i = 0; i < molecules_to_filter.Length; i++)
            {
                frs[i].FullFileName = molecules_to_filter[i];
                frs[i].ShortFileName = Path.GetFileNameWithoutExtension(molecules_to_filter[i]);
                frs[i].InternalNumber = i + 1;
                frs[i].E = MiscData.ENotCalculated; // not processed yet
            }
            ChangeFPSMode(FPSMode.Filter);
            DisplayFilteredStructures();
        }
        // selected pdb files
        private void moleculesForScreeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "pdb";
            openFileDialog1.Filter = "PDB files (*.pdb)|*.pdb|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Multiselect = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK && openFileDialog1.FileNames.Length > 0)
            {
                molecules_to_filter = new String[openFileDialog1.FileNames.Length];
                Array.Copy(openFileDialog1.FileNames, molecules_to_filter, openFileDialog1.FileNames.Length);
                LoadMoleculesForFiltering(molecules_to_filter);
            }
            openFileDialog1.Multiselect = false;
            IsChecked = false;
        }
        // load all pdb files from a folder, for screening
        private void moleculesForScreeningfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Load all pdb files:";
            folderBrowserDialog1.ShowNewFolderButton = false;
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                String[] pdbfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.pdb");
                molecules_to_filter = new String[pdbfiles.Length];
                Array.Copy(pdbfiles, molecules_to_filter, pdbfiles.Length);
                LoadMoleculesForFiltering(molecules_to_filter);
            }
            IsChecked = false;
        }

        /// <summary>
        /// Load a structure from a pdb file
        /// </summary>
        /// <param name="filename">File name</param>
        private void AddMolecule(String filename)
        {
            Molecule m1 = new Molecule(filename);
            if (m1.Error.Length > 0)
            {
                MessageBox.Show("Unable to load " + filename + ": " + m1.Error,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // check if already exists
            bool repeated = false;
            foreach (Molecule m in molecules) repeated = repeated || (m.Name == m1.Name);
            if (repeated)
            {
                MessageBox.Show("Molecule " + m1.Name + " is already loaded!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            molecules.Add(m1);
            this.MoleculeslistBox.Items.Add(m1.Name + "  (" + m1.FullFileName + ")", false);
            this.statusLabel1.Text = "Molecule " + m1.Name + " loaded.";
        }
        // change random-initial-position property of a molecule
        private void MoleculeslistBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            molecules[e.Index].Selected = (e.NewValue == CheckState.Checked);
        }

        // load labeling positions
        private void loadlabelstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files(*.txt;*.dat)|*.txt;*.dat|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                labelingpos = new LabelingPositionList(openFileDialog1.FileName);
                if (labelingpos.Error.Length > 0)
                {
                    MessageBox.Show("Unable to load " + openFileDialog1.FileName + ": " + labelingpos.Error,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DisplayNewLabelsdataGridView();
                this.statusLabel1.Text = "Labeling positions loaded from " + openFileDialog1.FileName + ".";
            }
            // make sure that conversion functions are updated
            globalcf = null;
            globalcfinv = null;
            IsChecked = false;
        }
        // load distances
        private void distancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files(*.txt;*.dat)|*.txt;*.dat|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                dist = new DistanceList(openFileDialog1.FileName);
                if (dist.Error.Length > 0)
                {
                    MessageBox.Show("Unable to load " + openFileDialog1.FileName + ": " + dist.Error,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DisplayNewRdataGridView();
                this.statusLabel1.Text = "Distances loaded from " + openFileDialog1.FileName + ".";
            }
            // make sure that conversion functions are updated
            globalcf = null;
            globalcfinv = null;
            if (labelingpos != null && avcache != null && avcache.Count == 0)
            {
                LabelingPosition l;
                for (int i = 0; i < labelingpos.Count; i++)
                {
                    l = labelingpos[i];
                    l.AVData.AVReady = !(l.AVData.AVType != AVSimlationType.None);
                    labelingpos[i] = l;
                }
            }
            IsChecked = false;
        }

        private void labelingPositionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // saving labeling positions file
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files(*.txt;*.dat)|*.txt;*.dat|All files (*.*)|*.*";
            saveFileDialog1.FileName = "";
            saveFileDialog1.CheckFileExists = false;

            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    String DorA;
                    foreach (LabelingPosition l in labelingpos)
                    {
                        if (l.Dye == DyeType.Donor) DorA = "D";
                        else if (l.Dye == DyeType.Acceptor) DorA = "A";
                        else DorA = "U";
                        sw.Write(l.Name + '\t' + l.Molecule + '\t' + DorA + '\t');
                        if (l.AVData.AVType == AVSimlationType.None)
                            sw.WriteLine(AVSimlationTypeShort.XYZ.ToString() + '\t' + ((Vector3)l).ToString('\t'));
                        else if (l.AVData.AVType == AVSimlationType.SingleDyeR)
                            sw.WriteLine(AVSimlationTypeShort.AV1.ToString() + '\t' + l.AVData.L.ToString() + '\t'
                                + l.AVData.W.ToString() + '\t' + l.AVData.R.ToString() + '\t' + l.AVData.AtomID.ToString());
                        else if (l.AVData.AVType == AVSimlationType.ThreeDyeR)
                            sw.WriteLine(AVSimlationTypeShort.AV3.ToString() + '\t' + l.AVData.L.ToString() + '\t'
                                + l.AVData.W.ToString() + '\t' + l.AVData.R1.ToString() + '\t'
                                + l.AVData.R2.ToString() + '\t' + l.AVData.R3.ToString() + '\t' + l.AVData.AtomID.ToString());
                    }
                    sw.Close();
                }
            }
        }
        private void labelingPositionsxyzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // saving labeling positions data as xyz
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files(*.txt;*.dat)|*.txt;*.dat|All files (*.*)|*.*";
            saveFileDialog1.FileName = "";
            saveFileDialog1.CheckFileExists = false;

            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false))
                {
                    String DorA;
                    foreach (LabelingPosition l in labelingpos)
                    {
                        if (l.Dye == DyeType.Donor) DorA = "D";
                        else if (l.Dye == DyeType.Acceptor) DorA = "A";
                        else DorA = "U";
                        sw.Write(l.Name + '\t' + l.Molecule + '\t' + DorA + '\t');
                        sw.WriteLine(((Vector3)l).ToString('\t'));
                    }
                    sw.Close();
                }
            }
        }

        private void simulationResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "bin";
            openFileDialog1.Filter = "Binary files(*.bin)|*.bin";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open))
                {
                    srs = (SimulationResult[])bf.Deserialize(fs);
                    fs.Close();
                }
                for (int i = 0; i < srs.Length; i++)
                    srs[i].Molecules = molecules;
                if (srs.Length > 0) saveButton.Enabled = true;
                DisplayNewStructures();
            }
        }

        ////////// project files /////////////
        // load project
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "bin";
            openFileDialog1.Filter = "Settings files(*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                try
                {
                    if (!om.Load(openFileDialog1.FileName)) return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load " + openFileDialog1.FileName + ": " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // clear old data
                if (molecules != null)
                {
                    molecules.Clear();
                    MoleculeslistBox.Items.Clear();
                }
                if (dist != null) dist.Clear();
                if (labelingpos != null) labelingpos.Clear();
                if (srs != null) srs = new SimulationResult[0];
                globalcf = null;
                globalcfinv = null;
                this.structuredataGridView.Rows.Clear();

                // load and display new data
                ChangeFPSMode(projectdata.ProjectFPSMode);
                if (_fpsmode == FPSMode.Dock)
                    for (int i = 0; i < projectdata.MoleculesPaths.Length; i++)
                        AddMolecule(projectdata.MoleculesPaths[i]);
                else if (_fpsmode == FPSMode.Filter)
                    LoadMoleculesForFiltering(projectdata.MoleculesPaths);
                else return;

                labelingpos = new LabelingPositionList(projectdata.LabelingPositionsPath);
                dist = new DistanceList(projectdata.DistancesPath);
                if (projectdata.SelectedDistances != null && projectdata.SelectedDistances.Length == dist.Count)
                    for (int i = 0; i < dist.Count; i++) dist.SetSelected(i, projectdata.SelectedDistances[i]);
                DisplayNewLabelsdataGridView();
                DisplayNewRdataGridView();
                this.statusLabel1.Text = "Project loaded from " + openFileDialog1.FileName + ".";
                
                IsChecked = false;
            }
        }

        // save project
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = "bin";
            saveFileDialog1.Filter = "Settings files(*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog1.FileName = "";
            saveFileDialog1.CheckFileExists = false;

            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                projectdata.ProjectFPSMode = _fpsmode;
                if (_fpsmode == FPSMode.Dock)
                {
                    projectdata.MoleculesPaths = new String[molecules.Count];
                    for (int i = 0; i < molecules.Count; i++)
                        projectdata.MoleculesPaths[i] = molecules[i].FullFileName;
                }
                else if (_fpsmode == FPSMode.Filter)
                {
                    projectdata.MoleculesPaths = new String[frs.Length];
                    for (int i = 0; i < frs.Length; i++)
                        projectdata.MoleculesPaths[i] = frs[i].FullFileName;
                }
                projectdata.LabelingPositionsPath = labelingpos.FullPath;
                projectdata.DistancesPath = dist.FullPath;
                projectdata.SelectedDistances = new bool[dist.Count];
                for (int i = 0; i < dist.Count; i++) projectdata.SelectedDistances[i] = dist[i].IsSelected;

                om.Save(saveFileDialog1.FileName);
                om.Export(saveFileDialog1.FileName + ".txt");
            }
        }

        // saving results
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveform == null) saveform = new SaveForm();
            saveform.FPSMode = this._fpsmode;
            SimulationResult[] srs_selected;
            FilteringResult[] frs_selected;
            int n = 0;
            DataGridViewRow r;
            if (_fpsmode == FPSMode.Dock)
            {
                srs_selected = new SimulationResult[structuredataGridView.SelectedRows.Count];

                for (int i = 0; i < structuredataGridView.SelectedRows.Count; i++)
                {
                    r = structuredataGridView.SelectedRows[i];
                    srs_selected[n++] = srs[((SimulationResult)r.Cells[0].Value).InternalNumber - 1];
                }
                saveform.DataToSaveSelected = srs_selected;
                saveform.DataToSaveAll = srs;
                saveform.OriginalMolecules = molecules;
                saveform.BestFit = this.bestfitcheckBox.Checked;
                saveform.RMSDReference = sr_rmsdreference;
                saveform.RMSDvsReference = rmsdvsref & (sr_rmsdreference.InternalNumber > 0);
                saveform.ConversionFunction = globalcf;
            }
            else if (_fpsmode == FPSMode.Filter)
            {
                frs_selected = new FilteringResult[structuredataGridView.SelectedRows.Count];
                for (int i = 0; i < structuredataGridView.SelectedRows.Count; i++)
                {
                    r = structuredataGridView.SelectedRows[i];
                    frs_selected[n++] = frs[((FilteringResult)r.Cells[0].Value).InternalNumber - 1];
                }
                saveform.FilteringDataToSaveSelected = frs_selected;
                saveform.FilteringDataToSaveAll = frs;
            }
            saveform.OriginalLabelingPositions = labelingpos;
            saveform.OriginalDistances = dist;

            saveform.ShowDialog();
        }
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument1.DefaultPageSettings.Landscape = true;
            if (printDialog1.ShowDialog() == DialogResult.OK) printDocument1.Print();
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap stb = new Bitmap(this.Width, this.Height);
            this.DrawToBitmap(stb, new Rectangle(0, 0, this.Width, this.Height));
            e.Graphics.DrawImage(stb, new Point(0, 0));
        }

        #endregion

        #region Fill DataGridView structures

        private void DisplayNewLabelsdataGridView()
        {
            this.LabelsdataGridView.SuspendLayout();
            this.LabelsdataGridView.Rows.Clear();

            DataGridViewRow[] r = new DataGridViewRow[labelingpos.Count];
            LabelingPosition l;
            for (int i = 0; i < labelingpos.Count; i++)
            {
                l = labelingpos[i];
                r[i] = new DataGridViewRow();
                r[i].CreateCells(this.LabelsdataGridView);
                r[i].SetValues(l.Name, l.Molecule, l.X, l.Y, l.Z);
                if (!l.AVData.AVReady)
                {
                    r[i].Cells[2].Style.ForeColor = Color.LightGray;
                    r[i].Cells[3].Style.ForeColor = Color.LightGray;
                    r[i].Cells[4].Style.ForeColor = Color.LightGray;
                }
            }
            this.LabelsdataGridView.Rows.AddRange(r);
            this.LabelsdataGridView.ResumeLayout();
        }

        private void DisplayNewRdataGridView()
        {
            this.RdataGridView.SuspendLayout();
            this.RdataGridView.Rows.Clear();

            DataGridViewRow[] r = new DataGridViewRow[dist.Count];
            for (int i = 0; i < dist.Count; i++)
            {
                r[i] = new DataGridViewRow();
                r[i].CreateCells(this.RdataGridView);
                r[i].SetValues(dist[i].IsSelected, dist[i].Position1, dist[i].Position2, 
                    dist[i].R, dist[i].ErrPlus, dist[i].ErrMinus);
            }
            this.RdataGridView.Rows.AddRange(r);
            this.RdataGridView.ResumeLayout();
            // determine data type
            if (dist.DataType == DistanceDataType.Rmp)
            {
                radioButtonRmp.Enabled = true; radioButtonRmp.Checked = true;
            }
            else if (dist.DataType == DistanceDataType.RDAMean)
            {
                radioButtonRDAMean.Enabled = true; radioButtonRDAMean.Checked = true;
            }
            else if (dist.DataType == DistanceDataType.RDAMeanE)
            {
                radioButtonRDAMeanE.Enabled = true; radioButtonRDAMeanE.Checked = true;
            }
        }

        private void DisplayNewStructures(SimulationResult sreference, bool vsreference)
        {
            double rmsd;
            int i, ishift, n;
            bool bestfit = bestfitcheckBox.Checked;
            SimulationResult srtmp, srtmplast;
            if (srs == null || srs.Length == 0 || !_ischecked) return;
            double dof = (double)Math.Max(dist.Count - 6 * (molecules.Count - 1), 1);
            this.structuredataGridView.SuspendLayout();
            this.structuredataGridView.Columns[2].HeaderText = vsreference ?
                "RMSD vs " + sreference.InternalNumber.ToString() : "RMSD vs prev.";
            if (srs.Length < this.structuredataGridView.RowCount) this.structuredataGridView.Rows.Clear();
            DataGridViewRow[] r = new DataGridViewRow[srs.Length - this.structuredataGridView.RowCount];

            // recalculate rmsd of old structures
            srtmp = (this.structuredataGridView.RowCount > 0) ?
                (SimulationResult)this.structuredataGridView.Rows[0].Cells[0].Value : srs[0];
            n = srtmp.InternalNumber - 1;
            rmsd = vsreference ? srs[n].RMSD(sreference, bestfit) : 0.0;
            if (this.structuredataGridView.RowCount > 0)
                this.structuredataGridView.Rows[0].Cells[2].Value = rmsd;
            else
            {
                r[0] = new DataGridViewRow();
                r[0].CreateCells(this.structuredataGridView);
                r[0].SetValues(srs[0], srs[0].E / dof, rmsd);
            }
            for (i = 1; i < this.structuredataGridView.RowCount; i++)
            {
                srtmplast = srtmp;
                srtmp = (SimulationResult)this.structuredataGridView.Rows[i].Cells[0].Value;
                n = srtmp.InternalNumber - 1;
                rmsd = vsreference ? srs[n].RMSD(sreference, bestfit) : srs[n].RMSD(srtmplast, bestfit);
                this.structuredataGridView.Rows[i].Cells[2].Value = rmsd;
            }

            // show new structures
            for (; i < srs.Length; i++)
            {
                ishift = i - this.structuredataGridView.RowCount;
                r[ishift] = new DataGridViewRow();
                r[ishift].CreateCells(this.structuredataGridView);
                rmsd = vsreference ? srs[i].RMSD(sreference, bestfit) : srs[i].RMSD(srs[i - 1], bestfit);
                r[ishift].SetValues(srs[i], srs[i].E / dof, rmsd);
                if ((srs[i].SimulationMethod & SimulationMethods.ErrorEstimation) != 0)
                {
                    r[ishift].Cells[0].Style.BackColor = errorcolor;
                    r[ishift].Cells[1].Style.BackColor = errorcolor;
                    r[ishift].Cells[2].Style.BackColor = errorcolor;
                }
                else if ((srs[i].SimulationMethod & SimulationMethods.Refinement) != 0
                    || (srs[i].SimulationMethod & SimulationMethods.MetropolisSampling) != 0)
                {
                    r[ishift].Cells[0].Style.BackColor = refinedcolor;
                    r[ishift].Cells[1].Style.BackColor = refinedcolor;
                    r[ishift].Cells[2].Style.BackColor = refinedcolor;
                }
            }
            this.structuredataGridView.Rows.AddRange(r);
            this.structuredataGridView.ResumeLayout();
            if (this.structuredataGridView.SelectedRows.Count == 1)
                CalculateModelDistances((SimulationResult)structuredataGridView.SelectedRows[0].Cells[0].Value);

            rmsdvsref = vsreference;
        }

        private void DisplayNewStructures()
        {
            DisplayNewStructures(new SimulationResult(), false);
        }
        private void DisplayNewStructures(SimulationResult sreference)
        {
            DisplayNewStructures(sreference, true);
        }

        private void DisplayFilteredStructures()
        {
            if (frs == null || frs.Length == 0) return;

            this.structuredataGridView.SuspendLayout();
            this.structuredataGridView.Columns[2].HeaderText = "ref. atoms RMSD";
            this.structuredataGridView.Rows.Clear();
            DataGridViewRow[] r = new DataGridViewRow[frs.Length];
            int distcount = (dist == null) ? 1 : Math.Max(dist.Count, 1);
            for (int i = 0; i < frs.Length; i++)
            {
                r[i] = new DataGridViewRow();
                r[i].CreateCells(this.structuredataGridView);
                r[i].SetValues(frs[i], frs[i].E, frs[i].RefRMSD);
                if (frs[i].InvalidR > 0)
                {
                    r[i].Cells[0].Style.BackColor = errorcolor;
                    r[i].Cells[1].Style.BackColor = errorcolor;
                    r[i].Cells[2].Style.BackColor = errorcolor;
                }
                if (frs[i].E < 0.0)
                {
                    r[i].Cells[1].Style.ForeColor = Color.White; // invisible
                }
            }
            this.structuredataGridView.Rows.AddRange(r);
            this.structuredataGridView.ResumeLayout();
        }

        private void DisplayFilteredStructures(FilteringResult freference)
        {
            if (frs == null || frs.Length == 0) return;
            Molecule m = new Molecule(freference.FullFileName);
            bool bestfit = this.bestfitcheckBox.Checked;

            this.structuredataGridView.SuspendLayout();
            this.structuredataGridView.Columns[2].HeaderText = "RMSD vs " + freference.InternalNumber.ToString();
            this.structuredataGridView.Rows.Clear();
            DataGridViewRow[] r = new DataGridViewRow[frs.Length];
            int distcount = (dist == null) ? 1 : Math.Max(dist.Count, 1);
            double[] rmsd = new double[frs.Length];
            Parallel.For(0, frs.Length, new ParallelOptions() { MaxDegreeOfParallelism = this.NThreads }, i =>
                rmsd[i] = frs[i].RMSD(m, bestfit));
            for (int i = 0; i < frs.Length; i++)
            {
                r[i] = new DataGridViewRow();
                r[i].CreateCells(this.structuredataGridView);
                r[i].SetValues(frs[i], frs[i].E, rmsd[i]);
                if (frs[i].InvalidR > 0)
                {
                    r[i].Cells[0].Style.BackColor = errorcolor;
                    r[i].Cells[1].Style.BackColor = errorcolor;
                    r[i].Cells[2].Style.BackColor = errorcolor;
                }
                if (frs[i].E < 0.0)
                {
                    r[i].Cells[1].Style.ForeColor = Color.White; // invisible
                }
            }
            this.structuredataGridView.Rows.AddRange(r);
            this.structuredataGridView.ResumeLayout();
        }
        private void structuredataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            SimulationResult sr1, sr2;
            FilteringResult fr1, fr2;
            if (e.Column.Index == 0 && _fpsmode == FPSMode.Dock)
            {
                sr1 = (SimulationResult)e.CellValue1;
                sr2 = (SimulationResult)e.CellValue2;
                e.SortResult = sr1.InternalNumber.CompareTo(sr2.InternalNumber);
                e.Handled = true;
            }
            else if (e.Column.Index == 0 && _fpsmode == FPSMode.Filter)
            {
                fr1 = (FilteringResult)e.CellValue1;
                fr2 = (FilteringResult)e.CellValue2;
                e.SortResult = fr1.InternalNumber.CompareTo(fr2.InternalNumber);
                e.Handled = true;
            }
        }

        #endregion

        private void aVButton_Click(object sender, EventArgs e)
        {
            AVinterface avform = new AVinterface(molecules);
            avform.AllowLoadPDBs = false;
            avform.AVGridParameters = projectdata.AVGlobalParameters;
            avform.ShowDialog();
            if ((avform.CalculatedPositions != null) && avform.CalculatedPositions.Count > 0)
            {
                if (labelingpos == null) labelingpos = new LabelingPositionList();
                labelingpos.AddRange(avform.CalculatedPositions);
                DisplayNewLabelsdataGridView();
            }
            IsChecked = false;
        }

        private void startstopButton_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy && this.startstopButton.Text == "Start")
            {
                Check();
                backgroundWorker1.RunWorkerAsync(actioncomboBox.SelectedItem.ToString());
                this.startstopButton.Text = "Stop";
            }
            else
            {
                backgroundWorker1.CancelAsync();
                this.startstopButton.Text = "Start";
            }
        }

        #region BackgroundWorkers

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument.ToString() == "Prepare")
            {
                for (int i = 0; i<molecules.Count; i++)
                {
                    if (!molecules[i].Prepared) molecules[i].Prepare();
                    ((BackgroundWorker)sender).ReportProgress((int)(100.0 * (double)(i + 1) / (double)molecules.Count));
                }
                e.Result = "";
            }
            else if (e.Argument.ToString() == "PrepareAV")
            {
                #region AV simulations
                AVEngine av;
                LabelingPosition l;
                int m, natom;
                Vector3[] t;
                avcache = new List<Vector3[]>(labelingpos.Count);
                for (int i = 0; i < labelingpos.Count; i++)
                {
                    if (!labelingpos[i].AVData.AVReady && labelingpos[i].AVData.AVType != AVSimlationType.None)
                    {
                        l = labelingpos[i];
                        m = molecules.FindIndex(l.Molecule);
                        av = new AVEngine(molecules[m], projectdata.AVGlobalParameters);
                        natom = Array.BinarySearch<int>(molecules[m].OriginalAtomID, l.AVData.AtomID);
                        if (l.AVData.AVType == AVSimlationType.SingleDyeR)
                            av.Calculate1R(l.AVData.L, l.AVData.W, l.AVData.R, natom);
                        else if (l.AVData.AVType == AVSimlationType.ThreeDyeR)
                            av.Calculate3R(l.AVData.L, l.AVData.W, l.AVData.R1, l.AVData.R2, l.AVData.R3, natom);
                        l.X = av.Rmp.X; l.Y = av.Rmp.Y; l.Z = av.Rmp.Z;
                        l.AVData.AVReady = true;
                        labelingpos[i] = l;
                        t = new Vector3[av.R.Length];
                        Array.Copy(av.R, t, av.R.Length);
                        for (int j = 0; j < t.Length; j++) t[j] = t[j] - l;
                        avcache.Add(t);
                    }
                    else if (labelingpos[i].AVData.AVType == AVSimlationType.None)
                    {
                        if (!labelingpos[i].AVData.AVReady) // "ATOM" type LP
                        {
                            l = labelingpos[i];
                            m = molecules.FindIndex(l.Molecule);
                            natom = Array.BinarySearch<int>(molecules[m].OriginalAtomID, l.AVData.AtomID);
                            l.X = molecules[m].XLocal[natom] + molecules[m].CM.X;
                            l.Y = molecules[m].YLocal[natom] + molecules[m].CM.Y;
                            l.Z = molecules[m].ZLocal[natom] + molecules[m].CM.Z;
                            l.AVData.AVReady = true;
                            labelingpos[i] = l;
                        }
                        avcache.Add(new Vector3[0]);
                    }
                    ((BackgroundWorker)sender).ReportProgress((int)(100.0 * (double)(i + 1) / (double)labelingpos.Count));
                }
                e.Result = "AV simulations ready";
                #endregion
            }
            else if (e.Argument.ToString() == "Conversion")
            {
                #region Conversion function
                convdist = new DistanceList();
                convdist.Capacity = dist.Count;

                const int NperR = 10;
                const int NperRange = 50;
                convform.NSamples = projectdata.AVGlobalParameters.ESamples;

                Random rnd = new Random();
                LabelingPosition l1, l2;
                Vector3[] r1, r2;
                double absr;

                // count distances with AV LPs
                int ndist = 0, n = 0;
                for (int i = 0; i < dist.Count; i++)
                {
                    // identify lps
                    l1 = labelingpos.Find(dist[i].Position1);
                    l2 = labelingpos.Find(dist[i].Position2);
                    if (l1.AVData.AVType != AVSimlationType.None && l2.AVData.AVType != AVSimlationType.None) ndist++;
                }
                if (ndist == 0 || dist.DataType == DistanceDataType.Rmp)
                {
                    globalcf = new double[] { 0.0, 1.0 };
                    globalcfinv = new double[] { 0.0, 1.0 };
                    convdist.AddRange(dist);
                    e.Result = "Conversion function ready";
                    return;
                }
                double[] Rmp = new double[NperR * ndist + NperRange];
                double[] Raveraged = new double[NperR * ndist + NperRange];
                n = 0;
                // calculate Rmps and <R> for experimental distances ~ +/- 6 sigma
                for (int i = 0; i < dist.Count; i++)
                {
                    l1 = labelingpos.Find(dist[i].Position1);
                    l2 = labelingpos.Find(dist[i].Position2);
                    if (l1.AVData.AVType != AVSimlationType.None && l2.AVData.AVType != AVSimlationType.None)
                    {
                        r1 = avcache[labelingpos.FindIndex(l1.Name)];
                        r2 = avcache[labelingpos.FindIndex(l2.Name)];

                        for (int nr = 0; nr < NperR; nr++)
                        {
                            absr = Math.Abs(dist[i].R + 6.0 * (rnd.NextDouble() - 0.5)
                                * (dist[i].ErrMinus + dist[i].ErrPlus));
                            Rmp[n] = absr;
                            Raveraged[n++] = convform.AveragedDistance(r1, r2, absr,
                                dist.DataType == DistanceDataType.RDAMeanE, true, projectdata.ConversionParameters.R0, rnd);
                        }
                    }
                    ((BackgroundWorker)sender).ReportProgress((int)(100.0 * (double)(i + 1) / ((double)dist.Count + NperRange / NperR)));
                }

                // scan the whole distance range with random LPs
                absr = 0.0;
                for (int nr = 0; nr < NperRange; nr++)
                {
                    do l1 = labelingpos[rnd.Next(labelingpos.Count - 1)];
                    while (l1.AVData.AVType == AVSimlationType.None);
                    do l2 = labelingpos[rnd.Next(labelingpos.Count - 1)];
                    while (l2.AVData.AVType == AVSimlationType.None);
                    r1 = avcache[labelingpos.FindIndex(l1.Name)];
                    r2 = avcache[labelingpos.FindIndex(l2.Name)];

                    Rmp[n] = absr;
                    Raveraged[n++] = convform.AveragedDistance(r1, r2, absr,
                        dist.DataType == DistanceDataType.RDAMeanE, true, projectdata.ConversionParameters.R0, rnd);
                    absr += 2.0 * projectdata.ConversionParameters.R0 / (double)NperRange;
                }
                ((BackgroundWorker)sender).ReportProgress(100);

                // conversion function
                globalcf = new double[projectdata.ConversionParameters.PolynomOrder + 1];
                globalcfinv = new double[projectdata.ConversionParameters.PolynomOrder + 1];
                convform.X = Rmp;
                convform.Y = Raveraged;
                convform.PolynomOrder = projectdata.ConversionParameters.PolynomOrder;
                convform.Convert(false);
                Array.Copy(convform.C, globalcf, convform.C.Length);
                convform.Convert(true);
                Array.Copy(convform.C, globalcfinv, convform.C.Length);
                avcache.Clear();

                // convert distances
                for (int i = 0; i < dist.Count; i++)
                {
                    l1 = labelingpos.Find(dist[i].Position1);
                    l2 = labelingpos.Find(dist[i].Position2);
                    if (l1.Dye != DyeType.Unknown && l2.Dye != DyeType.Unknown) convdist.Add(dist[i].Convert(globalcfinv));
                    else convdist.Add(dist[i]);
                }

                e.Result = "Conversion function ready";
                #endregion
            }

            else if (Array.Exists(ProjectData.DockModes, s => s == e.Argument.ToString())
                || Array.Exists(ProjectData.FilterModes, s => s == e.Argument.ToString()))
            {
                time0 = DateTime.Now;
                JobManagerBase jm;
                if (_fpsmode == FPSMode.Dock)
                {
                    se.SimulationParameters = projectdata.GetFpsParameters(e.Argument.ToString());
                    jm = sjm;
                }
                else
                {
                    FilterEngineParameters fepar = new FilterEngineParameters();
                    fepar.OptimizeSelected = (projectdata.ScreeningParameters.OptimizeSelected == OptimizeSelected.Selected);
                    fepar.R0 = projectdata.ConversionParameters.R0;
                    fe.FilterParameters = fepar;
                    fe.AVGridParameters = projectdata.AVGlobalParameters;
                    jm = fjm;
                }

                if (_fpsmode == FPSMode.Dock && e.Argument.ToString() != "Dock")
                {
                    Console.WriteLine("Docking 1.");
                    sjm.InitialStates = new SimulationResult[structuredataGridView.SelectedRows.Count];
                    for (int i = 0; i < structuredataGridView.SelectedRows.Count; i++)
                        sjm.InitialStates[i] = (SimulationResult)structuredataGridView.SelectedRows[i].Cells[0].Value;
                    sjm.InitialStates = sjm.InitialStates.OrderBy(sr => sr.InternalNumber).ToArray();
                    Console.WriteLine("Docking 2.");
                }

                // action-specific parameters
                switch (e.Argument.ToString())
                {
                    case "Dock":
                        sjm.InitialStates = null;
                        sjm.SimulationPrototype = se;
                        sjm.FinalStatesPerInitialState = (int)this.numberofrepetitions.Value;
                        break;
                    case "Refine":
                        Refinement se_r = new Refinement(se);
                        se_r.ProjectDataCopy = projectdata;
                        sjm.SimulationPrototype = se_r;
                        sjm.FinalStatesPerInitialState = 1;
                        break;
                    case "Error estimation":
                        ErrorEstimation se_e = new ErrorEstimation(se);
                        sjm.SimulationPrototype = se_e;
                        sjm.FinalStatesPerInitialState = (int)this.numberofrepetitions.Value;
                        break;
                    case "Sample":
                        MetropolisSampler se_m = new MetropolisSampler(se);
                        sjm.SimulationPrototype = se_m;
                        sjm.FinalStatesPerInitialState = (int)this.numberofrepetitions.Value;
                        break;
                    case "Screening":
                        fjm.StatesToFilter = frs;
                        fjm.FilterPrototype = fe;
                        break;
                    default:
                        break;
                }

                nrepeats = _fpsmode == FPSMode.Filter ? frs.Length : 
                    sjm.FinalStatesPerInitialState * (sjm.InitialStates == null ? 1 : sjm.InitialStates.Length);
                int laststructuresdone = -1;
                Console.WriteLine("N repreats: " + nrepeats);


                jm.StartJobAsync();
                Console.WriteLine("StructuresDone: " + jm.StructuresDone);
                while (!jm.SimulationCompleted)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        Console.WriteLine("Background job cancelled");
                        jm.Cancel();
                        break;
                    }
                    if (jm.StructuresDone > nrepeats)
                    {
                        ((BackgroundWorker)sender).ReportProgress((jm.StructuresDone * 100) / nrepeats,
                            string.Format("Working: {0}; structure {1} of {2}...", e.Argument.ToString(), jm.StructuresDone + 1, nrepeats));
                        laststructuresdone = jm.StructuresDone;
                        Console.WriteLine("StructuresDone: " + jm.StructuresDone);
                        Console.WriteLine("Last structure done");
                    }
                    System.Threading.Thread.Sleep(50);
                }
                Console.WriteLine("Simulation completed!");

                if (_fpsmode == FPSMode.Dock)
                {
                    // Sort by E if search or refine; for error estimation or sampling, sort by original number
                    SimulationResult[] srs_completed;
                    if (e.Argument.ToString() == "Dock" || e.Argument.ToString() == "Refine")
                        srs_completed = sjm.FinalStates.Where(sr => (sr.ParentStructure != 0)).OrderBy(sr => sr.E).ToArray();
                    else
                        srs_completed = sjm.FinalStates.Where(sr => (sr.ParentStructure != 0)).OrderBy(sr => sr.ParentStructure).ToArray();

                    for (int i = 0; i < srs_completed.Length; i++) srs_completed[i].InternalNumber = i + 1 + srs.Length;

                    nrepeats = srs_completed.Length;
                    Array.Resize(ref srs, srs.Length + nrepeats);
                    Array.Copy(srs_completed, 0, srs, srs.Length - nrepeats, nrepeats);
                }

                TimeSpan ts = DateTime.Now - time0;
                ((BackgroundWorker)sender).ReportProgress(100);
                if (backgroundWorker1.CancellationPending) e.Result = "Cancelled (" + ts.TotalSeconds.ToString("F3") + " s)";
                else e.Result = "Ready (" + ts.TotalSeconds.ToString("F3") + " s)";
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.stripProgressBar1.Value = e.ProgressPercentage;
            if (e.UserState != null) this.statusLabel1.Text = e.UserState.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.ToString() == "AV simulations ready") DisplayNewLabelsdataGridView();
            else if (e.Result.ToString() == "Conversion function ready") radioButtonRmp.Enabled = true;
            else if (e.Result.ToString() != String.Empty) this.statusLabel1.Text = e.Result.ToString();
            if (_fpsmode == FPSMode.Dock) DisplayNewStructures();
            else if (_fpsmode == FPSMode.Filter) DisplayFilteredStructures();
            this.startstopButton.Text = "Start";
        }

        #endregion

        private bool Check()
        {
            bool error;

            if (_ischecked)
            {
                this.startstopButton.Enabled = true;
                this.saveButton.Enabled = true;
                return true;
            }

            // no data
            if (_fpsmode == FPSMode.None)
            {
                this.statusLabel1.Text = "Waiting for input data";
                return false;
            }

            // molecules
            if (_fpsmode == FPSMode.Dock && (molecules == null || molecules.Count == 0))
            { 
                this.statusLabel1.Text = "Waiting for input data"; 
                return false;
            }
            if (_fpsmode == FPSMode.Filter && (frs == null || frs.Length == 0))
            {
                this.statusLabel1.Text = "Waiting for input data";
                return false;
            }
            // prepare if necessary
            if (_fpsmode == FPSMode.Dock && molecules != null && !molecules.TrueForAll(m => m.Prepared)
                && !backgroundWorker1.IsBusy)
            {
                this.statusLabel1.Text = "Checking molecules..."; 
                backgroundWorker1.RunWorkerAsync("Prepare");
            }
 
            if (backgroundWorker1.IsBusy) return false; // not yet ready
            if (_fpsmode == FPSMode.Dock && molecules.Count < 2) // nothing to simulate
            {
                this.statusLabel1.Text = "Two or more molecules required";
                return false;
            }

            // labeling positions
            if (labelingpos == null || labelingpos.Count == 0)
            {
                this.statusLabel1.Text = "Labeling position data required";
                return false;
            }
            // unique names
            error = false;
            foreach (LabelingPosition l in labelingpos)
                if (labelingpos.FindIndex(l.Name) != labelingpos.FindLastIndex(l.Name)) error = true;
            if (error)
            {
                this.statusLabel1.Text = "Labeling positions must have unique names";
                return false;
            }
            // existing molecule names
            if (_fpsmode == FPSMode.Dock)
            {
                foreach (LabelingPosition l in labelingpos)
                    if (molecules.FindIndex(l.Molecule) < 0)
                    {
                        error = true;
                        // highlight
                        foreach (DataGridViewRow r in LabelsdataGridView.Rows)
                            if (r.Cells[0].Value.ToString() == l.Name) r.Cells[1].Style.ForeColor = Color.Red;
                        _ishighlighted = true;
                    }
                if (error)
                {
                    this.statusLabel1.Text = "Labeling positions must belong to existing molecules";
                    return false;
                }
            }

            // all AV type LPs have Rmp calculated
            if (_fpsmode == FPSMode.Dock && !labelingpos.TrueForAll(delegate(LabelingPosition l) { return l.AVData.AVReady; })
                && !backgroundWorker1.IsBusy)
            {
                this.statusLabel1.Text = "Running AV simulations...";
                backgroundWorker1.RunWorkerAsync("PrepareAV");
            }
            if (backgroundWorker1.IsBusy) return false; // not yet ready

            // distances
            if (dist == null || dist.Count == 0)
            {
                this.statusLabel1.Text = "Distance data required";
                return false;
            }
            // existing lp names
            error = false;
            foreach (Distance d in dist)
                if (labelingpos.FindIndex(d.Position1) < 0 || labelingpos.FindIndex(d.Position2) < 0)
                {
                    error = true;
                    // highlight
                    foreach (DataGridViewRow r in RdataGridView.Rows)
                    {
                        if (labelingpos.FindIndex(r.Cells[1].Value.ToString()) < 0) r.Cells[1].Style.ForeColor = Color.Red;
                        if (labelingpos.FindIndex(r.Cells[2].Value.ToString()) < 0) r.Cells[2].Style.ForeColor = Color.Red;
                    }
                    _ishighlighted = true;
                }
            if (error)
            {
                this.statusLabel1.Text = "Distances must be between existing labeling positions";
                return false;
            }

            // generate conversion function
            if (_fpsmode == FPSMode.Dock)
                if (globalcf == null && !backgroundWorker1.IsBusy)
                {
                    this.statusLabel1.Text = "Generating conversion function...";
                    backgroundWorker1.RunWorkerAsync("Conversion");
                }
            if (backgroundWorker1.IsBusy) return false; // not yet ready

            // if all checks passed:
            if (_fpsmode == FPSMode.Dock) se = new SpringEngine(molecules, labelingpos, convdist);
            else if (_fpsmode == FPSMode.Filter)
            {
                fe = new FilterEngine(labelingpos, dist);
                fe.AVGridParameters = projectdata.AVGlobalParameters;
                fe.ReadRefAtoms(new Molecule(frs[0].FullFileName), false);
            }
            this.statusLabel1.Text = "Ready";

            // un-highlight
            if (_ishighlighted)
            {
                foreach (DataGridViewRow r in RdataGridView.Rows)
                {
                    r.Cells[1].Style.ForeColor = Color.Black;
                    r.Cells[2].Style.ForeColor = Color.Black;
                }
                foreach (DataGridViewRow r in LabelsdataGridView.Rows)
                    r.Cells[1].Style.ForeColor = Color.Black;
                _ishighlighted = false;
            }

            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // try to prepare loaded molecules in bg while the user is doing other things
            this.IsChecked = Check();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool view = (this.tabControl1.SelectedIndex == 1);
            this.RdataGridView.Columns["ErrorPlus"].Visible = !view;
            this.RdataGridView.Columns["ErrorMinus"].Visible = !view;
            this.RdataGridView.Columns["Rmodel"].Visible = view;
            this.RdataGridView.Columns["Deviation"].Visible = view;
            structuredataGridView_SelectionChanged(sender, e);
        }

        private void structuredataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) return;
            DataGridViewRow r;
            if (_fpsmode == FPSMode.Dock && structuredataGridView.SelectedRows.Count == 1)
            {
                r = structuredataGridView.SelectedRows[0];
                double[] Rmodel = CalculateModelDistances((SimulationResult)r.Cells[0].Value);
            }
            else if (_fpsmode == FPSMode.Filter && structuredataGridView.SelectedRows.Count == 1)
            {
                r = structuredataGridView.SelectedRows[0];
                double[] Rmodel = CalculateModelDistances((FilteringResult)r.Cells[0].Value);
            }
        }

        private double[] CalculateModelDistances(SimulationResult sr)
        {
            double[] Rmodel = new double[dist.Count];
            double d, r, dmrmsd = 0.0;
            DataGridViewRow row;
            int id;
            double derr;
            for (int i = 0; i < RdataGridView.Rows.Count; i++)
            {
                row = RdataGridView.Rows[i];
                id = dist.FindIndex(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
                // identify labeling positions
                LabelingPosition l1 = labelingpos.Find(dist[id].Position1);
                LabelingPosition l2 = labelingpos.Find(dist[id].Position2);

                Rmodel[i] = sr.ModelDistance(l1, l2);
                if (!radioButtonRmp.Checked && l1.Dye != DyeType.Unknown && l2.Dye != DyeType.Unknown)
                {
                    Rmodel[i] = Distance.Convert(Rmodel[i], globalcf);
                    r = dist[id].R;
                }
                else r = convdist[id].R;
                d = Rmodel[i] - r;
                if (!radioButtonRmp.Checked && l1.Dye != DyeType.Unknown && l2.Dye != DyeType.Unknown)
                    derr = d > 0.0 ? dist[id].ErrPlus : dist[id].ErrMinus;
                else derr = d > 0.0 ? convdist[id].ErrPlus : convdist[id].ErrMinus;
                dmrmsd += d * d;
                RdataGridView.Rows[i].Cells[3].Value = r;
                RdataGridView.Rows[i].Cells["Rmodel"].Value = Rmodel[i];
                RdataGridView.Rows[i].Cells["Deviation"].Value = wdev_checkBox.Checked ? d / derr : d;
                // data-model RMSD
                if (tabControl1.SelectedIndex == 1)
                    this.statusLabel1.Text = "RMSD (data-model) = " + String.Format("{0:F3}", Math.Sqrt(dmrmsd / dist.Count));
            }
            return Rmodel;
        }

        private double[] CalculateModelDistances(FilteringResult fr)
        {
            if (fr.RModel == null || fr.RModel.Count == 0) return null;
            double[] Rmodel = new double[dist.Count];
            double d, r, dmrmsd = 0.0;
            DataGridViewRow row;
            int id;
            double derr;
            for (int i = 0; i < RdataGridView.Rows.Count; i++)
            {
                row = RdataGridView.Rows[i];
                id = dist.FindIndex(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
                LabelingPosition l1 = labelingpos.Find(dist[id].Position1);
                LabelingPosition l2 = labelingpos.Find(dist[id].Position2);
                Rmodel[i] = fr.RModel.Find(l1.Name, l2.Name).R;
                r = dist[id].R;
                d = Rmodel[i] - r;
                derr = d > 0.0 ? dist[id].ErrPlus : dist[id].ErrMinus;
                dmrmsd += d * d;
                RdataGridView.Rows[i].Cells[3].Value = r;
                RdataGridView.Rows[i].Cells["Rmodel"].Value = Rmodel[i];
                RdataGridView.Rows[i].Cells["Deviation"].Value = wdev_checkBox.Checked ? d / derr : d; ;
                // data-model RMSD
                if (tabControl1.SelectedIndex == 1)
                    this.statusLabel1.Text = "RMSD (data-model) = " + String.Format("{0:F3}", Math.Sqrt(dmrmsd / dist.Count));
            }
            return Rmodel;
        }

        private void radioButtonRmp_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked || RdataGridView.Rows.Count == 0) return;
            // display normal or converted distances
            if (radioButtonRmp.Checked && dist != null && dist.DataType != DistanceDataType.Rmp)
            {
                for (int i = 0; i < dist.Count; i++)
                {
                    RdataGridView.Rows[i].Cells[3].Value = convdist[i].R;
                    RdataGridView.Rows[i].Cells["ErrorPlus"].Value = convdist[i].ErrPlus;
                    RdataGridView.Rows[i].Cells["ErrorMinus"].Value = convdist[i].ErrMinus;
                }
                RdataGridView.Columns[3].HeaderText = "R*";
            }
            else
            {
                for (int i = 0; i < dist.Count; i++)
                {
                    RdataGridView.Rows[i].Cells[3].Value = dist[i].R;
                    RdataGridView.Rows[i].Cells["ErrorPlus"].Value = dist[i].ErrPlus;
                    RdataGridView.Rows[i].Cells["ErrorMinus"].Value = dist[i].ErrMinus;
                }
                RdataGridView.Columns[3].HeaderText = "R";
            }
            structuredataGridView_SelectionChanged(sender, e);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spform.SetFpsParameters(projectdata);
            spform.ConversionParameters = projectdata.ConversionParameters;
            spform.AVGridParameters = projectdata.AVGlobalParameters;
            DialogResult dr = spform.ShowDialog();
            if (dr == DialogResult.OK)
            {
                projectdata.SetFpsParameters(spform.GetFpsParameters());
                if (spform.ConversionParameters.PolynomOrder != projectdata.ConversionParameters.PolynomOrder)
                {
                    globalcf = new double[spform.ConversionParameters.PolynomOrder + 1];
                    globalcfinv = new double[spform.ConversionParameters.PolynomOrder + 1];
                    convform.Convert(false);
                    Array.Copy(convform.C, globalcf, convform.C.Length);
                    convform.Convert(true);
                    Array.Copy(convform.C, globalcfinv, convform.C.Length);
                }
                projectdata.ConversionParameters = spform.ConversionParameters;
                projectdata.AVGlobalParameters = spform.AVGridParameters;
            }
        }

        private void rmsdbutton_Click(object sender, EventArgs e)
        {
            DataGridViewRow r;
            if (structuredataGridView.SelectedRows.Count == 1 && _fpsmode == FPSMode.Dock)
            {
                r = structuredataGridView.SelectedRows[0];
                sr_rmsdreference = (SimulationResult)r.Cells[0].Value;
                DisplayNewStructures(sr_rmsdreference);
            }
            else if (_fpsmode == FPSMode.Dock)
                DisplayNewStructures();
            else if (structuredataGridView.SelectedRows.Count == 1 && _fpsmode == FPSMode.Filter)
            {
                r = structuredataGridView.SelectedRows[0];
                fr_rmsdreference = (FilteringResult)r.Cells[0].Value;
                DisplayFilteredStructures(fr_rmsdreference);
            }
            else if (_fpsmode == FPSMode.Filter)
                DisplayFilteredStructures();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (molecules != null)
            {
                foreach (Molecule m in molecules) m.Dispose();
                molecules.Clear();
            }
            if (labelingpos != null) labelingpos.Clear();
            if (dist != null) dist.Clear();
            if (MoleculeslistBox.Items != null) MoleculeslistBox.Items.Clear();
            LabelsdataGridView.Rows.Clear();
            RdataGridView.Rows.Clear();
            structuredataGridView.Rows.Clear();
            if (srs != null) srs = new SimulationResult[0];
            globalcf = null;
            globalcfinv = null;
            radioButtonRDAMeanE.Enabled = false;
            radioButtonRDAMean.Enabled = false;
            radioButtonRmp.Checked = true;
            radioButtonRmp.Checked = false;
            radioButtonRmp.Enabled = false;
            this.IsChecked = false;
            ChangeFPSMode(FPSMode.None);
        }

        private void clearresultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (srs != null) srs = new SimulationResult[0];
            structuredataGridView.Rows.Clear();
        }

        private void RdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // on-off
            DataGridViewRow r;
            int id;
            if (e.ColumnIndex == 0 && e.RowIndex != -1 && dist != null && e.RowIndex < dist.Count)
            {
                r = RdataGridView.Rows[e.RowIndex];
                id = dist.FindIndex(r.Cells[1].Value.ToString(), r.Cells[2].Value.ToString());
                dist.SetSelected(id, !(bool)r.Cells[0].Value);
                if (convdist != null)
                {
                    convdist.SetSelected(id, !(bool)r.Cells[0].Value);
                    if (se != null) se.Distances = convdist;
                }
                else if (se != null) se.Distances = dist;
            }
        }

        private void structuredataGridView_Sorted(object sender, EventArgs e)
        {
            DisplayNewStructures();
        }

        private void conversionFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            convform.XName = "Rmp";
            if (dist != null && dist.DataType == DistanceDataType.RDAMean) convform.YName = "<RDA>";
            else if (dist != null && dist.DataType == DistanceDataType.RDAMeanE) convform.YName = "<RDA>E";
            else convform.YName = "Rmp";
            convform.PolynomOrder = projectdata.ConversionParameters.PolynomOrder;
            convform.ShowDialog();
            if (convform.DialogResult == DialogResult.OK)
            {
                ConversionParameters cp = projectdata.ConversionParameters;
                cp.PolynomOrder = convform.PolynomOrder;
                projectdata.ConversionParameters = cp;
                globalcf = new double[convform.PolynomOrder + 1];
                globalcfinv = new double[convform.PolynomOrder + 1];
                convform.Convert(false);
                Array.Copy(convform.C, globalcf, convform.C.Length);
                convform.Convert(true);
                Array.Copy(convform.C, globalcfinv, convform.C.Length);
            }
        }

        private void calculateAverageDistancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DistanceCalculator df = new DistanceCalculator(projectdata.ConversionParameters.R0, projectdata.AVGlobalParameters.ESamples);
            df.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (molecules != null)
            {
                foreach (Molecule m in molecules) m.Dispose();
                molecules.Clear();
            }
            Application.Exit();
        }

        #region Debugging

        private void saveClustersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveClustersToolStripMenuItem.Checked = !saveClustersToolStripMenuItem.Checked;
            DebugData.SaveAtomClusters = saveClustersToolStripMenuItem.Checked;
        }

        private void saveTrajectoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveTrajectoriesToolStripMenuItem.Checked = !saveTrajectoriesToolStripMenuItem.Checked;
            DebugData.SaveTrajectories = saveTrajectoriesToolStripMenuItem.Checked;
        }

        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Check();
            statusLabel1.Text = "Shaking...";
            se.SimulationParameters = projectdata.DockParameters;
            double E = se.SetState();
            statusLabel1.Text = "Ready (E = " + E.ToString() + ")";
        }

        private void optimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Check();
            this.statusLabel1.Text = "Simulating...";
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync("Simulate");
        }

        #endregion

        #region Help and misc

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + Path.DirectorySeparatorChar + "fps_help_v1.0.pdf");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        #endregion
    }
}