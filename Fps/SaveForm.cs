using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Fps
{
    public partial class SaveForm : Form
    {
        private FPSMode _fpsmode;
        public FPSMode FPSMode
        {
            get { return _fpsmode; }
            set 
            { 
                _fpsmode = value;
                pymolcheckBox.Visible = (_fpsmode == FPSMode.Dock);
                pdbcheckBox.Visible = (_fpsmode == FPSMode.Dock);
                lpcheckBox.Visible = (_fpsmode == FPSMode.Dock);
                overlaycheckBox.Visible = (_fpsmode == FPSMode.Dock);
                overlaystatescheckBox.Visible = (_fpsmode == FPSMode.Dock);
                srcheckBox.Visible = (_fpsmode == FPSMode.Dock);
                filenametextBox.Text = (_fpsmode == FPSMode.Dock) ? "structure" : "screening_";
            }
        }

        private SimulationResult[] _dataToSave;
        private SimulationResult[] _dataToSave_all;
        public SimulationResult[] DataToSaveAll
        {
            get { return _dataToSave_all; }
            set 
            {
                _dataToSave_all = value;
                Int32[] inum = new Int32[_dataToSave_all.Length];
                for (Int32 i = 0; i < _dataToSave_all.Length; i++)
                    inum[i] = _dataToSave_all[i].InternalNumber;
                Array.Sort(inum, _dataToSave_all);
            }
        }
        private SimulationResult[] _dataToSave_selected;
        public SimulationResult[] DataToSaveSelected
        {
            get { return _dataToSave_selected; }
            set
            {
                _dataToSave_selected = value;
                Int32[] inum = new Int32[_dataToSave_selected.Length];
                for (Int32 i = 0; i < _dataToSave_selected.Length; i++)
                    inum[i] = _dataToSave_selected[i].InternalNumber;
                Array.Sort(inum, _dataToSave_selected);
            }
        }

        private FilteringResult[] _fdataToSave;
        private FilteringResult[] _fdataToSave_all;
        public FilteringResult[] FilteringDataToSaveAll
        {
            get { return _fdataToSave_all; }
            set
            {
                _fdataToSave_all = value;
                Int32[] inum = new Int32[_fdataToSave_all.Length];
                for (Int32 i = 0; i < _fdataToSave_all.Length; i++)
                    inum[i] = _fdataToSave_all[i].InternalNumber;
                Array.Sort(inum, _fdataToSave_all);
            }
        }
        private FilteringResult[] _fdataToSave_selected;
        public FilteringResult[] FilteringDataToSaveSelected
        {
            get { return _fdataToSave_selected; }
            set
            {
                _fdataToSave_selected = value;
                Int32[] inum = new Int32[_fdataToSave_selected.Length];
                for (Int32 i = 0; i < _fdataToSave_selected.Length; i++)
                    inum[i] = _fdataToSave_selected[i].InternalNumber;
                Array.Sort(inum, _fdataToSave_selected);
            }
        }

        private SimulationResult _rmsdref;
        public SimulationResult RMSDReference
        {
            get { return _rmsdref; }
            set { _rmsdref = value; }
        }

        private MoleculeList molecules;
        public MoleculeList OriginalMolecules
        {
            get { return molecules; }
            set { molecules = value; }
        }

        private LabelingPositionList labelingpos;
        public LabelingPositionList OriginalLabelingPositions
        {
            get { return labelingpos; }
            set { labelingpos = value; }
        }

        private DistanceList dist;
        public DistanceList OriginalDistances
        {
            get { return dist; }
            set { dist = value; }
        }

        private Boolean _bestfit;
        public Boolean BestFit
        {
            get { return _bestfit; }
            set { _bestfit = value; }
        }

        private Boolean _vsref;
        public Boolean RMSDvsReference
        {
            get { return _vsref; }
            set { _vsref = value; }
        }

        private Double[] _globalcf;
        public Double[] ConversionFunction
        {
            get { return _globalcf; }
            set { _globalcf = value; }
        }
	
        public SaveForm()
        {
            InitializeComponent();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pymolcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!pymolcheckBox.Checked)
            {
                pdbcheckBox.Checked = false;
                lpcheckBox.Checked = false;
            }
            pdbcheckBox.Enabled = pymolcheckBox.Checked;
            lpcheckBox.Enabled = pymolcheckBox.Checked;
            
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            if (_fpsmode == FPSMode.Dock)
                if (selected_radioButton.Checked) _dataToSave = _dataToSave_selected;
                else _dataToSave = _dataToSave_all;
            else
                if (selected_radioButton.Checked) _fdataToSave = _fdataToSave_selected;
                else _fdataToSave = _fdataToSave_all;

            String savepath;
            folderBrowserDialog1.ShowNewFolderButton = true;
            if (folderBrowserDialog1.SelectedPath == String.Empty)
                folderBrowserDialog1.SelectedPath = (_fpsmode == FPSMode.Dock) ? 
                    Path.GetDirectoryName(molecules[0].FullFileName) : Path.GetDirectoryName(_fdataToSave[0].FullFileName);
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                savepath = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar;

                if (_fpsmode == FPSMode.Dock)
                {
                    if (pymolcheckBox.Checked)
                        for (Int32 i = 0; i < _dataToSave.Length; i++)
                            SaveSimulationResult(_dataToSave[i], savepath + filenametextBox.Text
                                + _dataToSave[i].InternalNumber.ToString(), pdbcheckBox.Checked);
                    if (overlaycheckBox.Checked) SaveOverlay(savepath + "Overlay");
                    if (overlaystatescheckBox.Checked) SaveOverlayStates(savepath + "OverlayStates");
                    if (RcheckBox.Checked) SaveDistances(savepath + "Rtable");
                    if (srcheckBox.Checked) SaveSimulationResuntsBin(savepath + "SimulationResults.bin");
                    if (etablecheckBox.Checked) SaveEnergyTable(savepath + "chi2table.txt");
                }
                else if (_fpsmode == FPSMode.Filter)
                {
                    if (RcheckBox.Checked) SaveFilterDistances(savepath + filenametextBox.Text + "Rtable");
                    if (etablecheckBox.Checked) SaveFilterTable(savepath + filenametextBox.Text + "chi2table.txt");         
                }
                this.Close();
            }
        }

        private void SaveSimulationResult(SimulationResult sr, String fname, Boolean addpdbsave)
        {
            String converged = sr.Converged ? " (converged)" : " (not converged)";
            String fullfname = fname + ".pml";
            Vector3 u; Double theta;
            using (StreamWriter sw = new StreamWriter(fullfname, false))
            {
                sw.WriteLine("# Energy = " + sr.E.ToString("F8") + converged);
                for (Int32 i = 0; i < molecules.Count; i++)
                    sw.WriteLine("load " + molecules[i].FullFileName);
                for (Int32 i = 1; i < molecules.Count; i++)
                {
                    theta = Matrix3.AngleAndAxis(sr.Rotation[i], out u) * 180.0 / Math.PI;
                    sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                        + ", " + molecules[i].Name + ", origin=[" + molecules[i].CM.ToString() + "]");
                    sw.WriteLine("translate [" + sr.Translation[i].ToString() + "], " + molecules[i].Name);
                }
                if (_bestfit)
                {
                    sw.WriteLine("select all");
                    sw.WriteLine("translate [" + sr.BestFitTranslation.ToString() + "], sele");
                    theta = Matrix3.AngleAndAxis(sr.BestFitRotation, out u) * 180.0 / Math.PI;
                    sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                        + ", sele, origin=[0, 0, 0]");
                }
                if (addpdbsave)
                {
                    sw.WriteLine("select all");
                    sw.WriteLine("save " + fname + ".pdb, sele");
                }
                String lpnames = "";
                if (lpcheckBox.Checked)
                {
                    Int32 im;
                    Vector3 r;
                    LabelingPosition lpref;
                    foreach (LabelingPosition l in labelingpos)
                    {
                        im = molecules.FindIndex(l.Molecule);
                        if ((sr.SimulationMethod & SimulationMethods.Refinement) != 0)
                            lpref = sr.RefinedLabelingPositions.Find(l.Name);
                        else lpref = l;
                        r = sr.Rotation[im] * (lpref - molecules[im].CM) + molecules[im].CM + sr.Translation[im];
                        sw.WriteLine("pseudoatom " + l.Name + ", pos=[" + r.ToString() + "]");
                        sw.WriteLine("label " + l.Name + ", \"" + l.Name + "\"");
                        sw.WriteLine("show spheres, " + l.Name);
                        if (l.Dye == DyeType.Donor) sw.WriteLine("color green, " + l.Name);
                        if (l.Dye == DyeType.Acceptor) sw.WriteLine("color red, " + l.Name);
                        lpnames += " + " + l.Name;
                    }
                }
                if (_bestfit && lpnames != String.Empty)
                {
                    sw.WriteLine("deselect");
                    sw.WriteLine("select " + lpnames.Substring(3));
                    sw.WriteLine("translate [" + sr.BestFitTranslation.ToString() + "], sele");
                    theta = Matrix3.AngleAndAxis(sr.BestFitRotation, out u) * 180.0 / Math.PI;
                    sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                        + ", sele, origin=[0, 0, 0]");
                }
                sw.WriteLine("deselect");
                sw.Close();
            }
        }
        private void SaveOverlay(String ofilename)
        {
            String fullfname = ofilename + ".pml";
            Vector3 u; Double theta;
            using (StreamWriter sw = new StreamWriter(fullfname, false))
            {
                sw.Write("# Overlay of structures ");
                for (Int32 j = 0; j < _dataToSave.Length - 1; j++)
                    sw.Write(_dataToSave[j].InternalNumber.ToString() + ", ");
                sw.WriteLine(_dataToSave[_dataToSave.Length - 1].InternalNumber.ToString());

                for (Int32 i = 0; i < molecules.Count; i++)
                    sw.WriteLine("load " + molecules[i].FullFileName);

                SimulationResult sr;
                for (Int32 j = 0; j < _dataToSave.Length; j++)
                {
                    sr = _dataToSave[j];
                    for (Int32 i = 0; i < molecules.Count; i++)
                    {
                        theta = Matrix3.AngleAndAxis(sr.Rotation[i], out u) * 180.0 / Math.PI;
                        sw.WriteLine("copy _tmp" + i.ToString() + ", " + molecules[i].Name);
                        sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                            + ", _tmp" + i.ToString() + ", origin=[" + molecules[i].CM.ToString() + "]");
                        sw.WriteLine("translate [" + sr.Translation[i].ToString() + "], _tmp" + i.ToString());
                    }
                    sw.WriteLine("select object _tmp*");
                    sw.WriteLine("create _tmpjoin, sele");
                    if (_bestfit)
                    {
                        sw.WriteLine("translate [" + sr.BestFitTranslation.ToString() + "], _tmpjoin");
                        theta = Matrix3.AngleAndAxis(sr.BestFitRotation, out u) * 180.0 / Math.PI;
                        sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                            + ", _tmpjoin, origin=[0, 0, 0]");
                    }
                    sw.WriteLine("copy " + filenametextBox.Text + _dataToSave[j].InternalNumber.ToString() + ", _tmpjoin");
                    sw.WriteLine("delete _tmp*");
                }
                for (Int32 i = 0; i < molecules.Count; i++)
                    sw.WriteLine("delete " + molecules[i].Name);

                sw.Close();
            }
        }

        private void SaveOverlayStates(String ofilename)
        {
            String fullfname = ofilename + ".pml";
            Vector3 u; Double theta;
            using (StreamWriter sw = new StreamWriter(fullfname, false))
            {
                sw.Write("# Overlay of structures ");
                for (Int32 j = 0; j < _dataToSave.Length - 1; j++)
                    sw.Write(_dataToSave[j].InternalNumber.ToString() + ", ");
                sw.WriteLine(_dataToSave[_dataToSave.Length - 1].InternalNumber.ToString());

                for (Int32 i = 0; i < molecules.Count; i++)
                    sw.WriteLine("load " + molecules[i].FullFileName);

                SimulationResult sr;
                for (Int32 j = 0; j < _dataToSave.Length; j++)
                {
                    sr = _dataToSave[j];
                    for (Int32 i = 0; i < molecules.Count; i++)
                    {
                        theta = Matrix3.AngleAndAxis(sr.Rotation[i], out u) * 180.0 / Math.PI;
                        sw.WriteLine("copy _tmp" + i.ToString() + ", " + molecules[i].Name);
                        sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                            + ", _tmp" + i.ToString() + ", origin=[" + molecules[i].CM.ToString() + "]");
                        sw.WriteLine("translate [" + sr.Translation[i].ToString() + "], _tmp" + i.ToString());
                    }
                    sw.WriteLine("select object _tmp*");
                    if (_bestfit)
                    {
                        sw.WriteLine("translate [" + sr.BestFitTranslation.ToString() + "], sele");
                        theta = Matrix3.AngleAndAxis(sr.BestFitRotation, out u) * 180.0 / Math.PI;
                        sw.WriteLine("rotate [" + u.ToString() + "], " + theta.ToString("F3")
                            + ", sele, origin=[0, 0, 0]");
                    }
                    sw.WriteLine("save _tmp.pdb, sele");
                    sw.WriteLine("load _tmp.pdb, " + filenametextBox.Text);
                    sw.WriteLine("delete _tmp*");
                }
                for (Int32 i = 0; i < molecules.Count; i++)
                    sw.WriteLine("delete " + molecules[i].Name);

                sw.Close();
            }
        }

        private void SaveEnergyTable(String efilename)
        {
            String fname = this.filenametextBox.Text;
            SimulationResult tmp;
            using (StreamWriter sw = new StreamWriter(efilename, false))
            {
                _rmsdref = _vsref ? _rmsdref : _dataToSave[0];
                String rmsrefdtext = "RMSD vs " + _rmsdref.InternalNumber;
                sw.WriteLine("File\tchi2\tchi2_bond\tchi2_clash\tConvergence\tMethod\t" + "RMSD vs previous" + 
                    '\t' + rmsrefdtext + '\t' + rmsrefdtext + " (selected)");
                tmp = _dataToSave[0];
                sw.WriteLine(String.Format("{0}{1}\t{2:F8}\t{3:F4}\t{4:F4}\t{5}\t{6}\t---\t{7:F6}\t{8:F6}", fname, tmp.InternalNumber,
                    tmp.E, tmp.Ebond, tmp.Eclash, tmp.Converged, tmp.SimulationMethod, tmp.RMSD(_rmsdref, _bestfit), tmp.RMSD(_rmsdref, _bestfit, true)));
                for (Int32 i = 1; i < _dataToSave.Length; i++)
                {
                    tmp = _dataToSave[i];
                    sw.WriteLine(String.Format("{0}{1}\t{2:F8}\t{3:F4}\t{4:F4}\t{5}\t{6}\t{7:F6}\t{8:F6}\t{9:F6}", fname, tmp.InternalNumber,
                    tmp.E, tmp.Ebond, tmp.Eclash, tmp.Converged, tmp.SimulationMethod,
                    tmp.RMSD(_dataToSave[i - 1], _bestfit), tmp.RMSD(_rmsdref, _bestfit), tmp.RMSD(_rmsdref, _bestfit, true)));
                }
                sw.Close();
            }
        }
        private void SaveFilterTable(String efilename)
        {
            FilteringResult fr;
            using (StreamWriter sw = new StreamWriter(efilename, false))
            {
                sw.WriteLine("File\tChi2r\tNaNs\tRefRMSD\t>1sigma\t>2sigma\t>3sigma");
                for (Int32 j = 0; j < _fdataToSave.Length; j++)
                {
                    fr = _fdataToSave[j];
                    sw.WriteLine(fr.ShortFileName + '\t' + fr.E.ToString("F3") + '\t' + fr.InvalidR
                        + '\t' + fr.RefRMSD.ToString("F3") + '\t' + fr.Sigma1 + '\t' + fr.Sigma2 + '\t' + fr.Sigma3);
                }
                sw.Close();
            }
        }

        private void SaveDistances(String rfilename)
        {
            LabelingPosition l1, l2;
            LabelingPosition[] l1index = new LabelingPosition[dist.Count];
            LabelingPosition[] l2index = new LabelingPosition[dist.Count];
            Double R;
            using (StreamWriter sw = new StreamWriter(rfilename + "_Rmp.txt", false))
            {
                sw.Write("Structure\tNumber");
                for (Int32 i = 0; i < dist.Count; i++)
                {
                    l1 = labelingpos.Find(dist[i].Position1);
                    l2 = labelingpos.Find(dist[i].Position2);
                    l1index[i] = l1;
                    l2index[i] = l2;
                    sw.Write('\t' + l1.Name + '_' + l2.Name);
                }
                sw.WriteLine();
                for (Int32 j = 0; j < _dataToSave.Length; j++)
                {
                    sw.Write(filenametextBox.Text + _dataToSave[j].InternalNumber.ToString() + '\t' + _dataToSave[j].InternalNumber.ToString());
                    for (Int32 i = 0; i < dist.Count; i++)
                        sw.Write('\t' + _dataToSave[j].ModelDistance(l1index[i], l2index[i]).ToString("F3"));
                    sw.WriteLine();
                }
                sw.Close();
            }

            // saving converted distances if requested
            if (dist.DataType != DistanceDataType.Rmp)
            {
                using (StreamWriter sw = new StreamWriter(rfilename + '_' + dist.DataType.ToString() + ".txt", false))
                {
                    sw.Write("Structure\tNumber");
                    for (Int32 i = 0; i < dist.Count; i++)
                    {
                        l1 = labelingpos.Find(dist[i].Position1);
                        l2 = labelingpos.Find(dist[i].Position2);
                        sw.Write('\t' + l1.Name + '_' + l2.Name);
                    }
                    sw.WriteLine();
                    for (Int32 j = 0; j < _dataToSave.Length; j++)
                    {
                        sw.Write(filenametextBox.Text + _dataToSave[j].InternalNumber.ToString() + '\t' + _dataToSave[j].InternalNumber.ToString());
                        for (Int32 i = 0; i < dist.Count; i++)
                        {
                            R = (l1index[i].Dye != DyeType.Unknown && l2index[i].Dye != DyeType.Unknown) ?
                                Distance.Convert(_dataToSave[j].ModelDistance(l1index[i], l2index[i]), _globalcf) : _dataToSave[j].ModelDistance(l1index[i], l2index[i]);
                            sw.Write('\t' + R.ToString("F3"));
                        }
                        sw.WriteLine();
                    }
                    sw.Close();
                }
            }
        }
        private void SaveFilterDistances(String rfilename)
        {
            LabelingPosition l1, l2;
            using (StreamWriter sw = new StreamWriter(rfilename + "_Rmp.txt", false))
            {
                sw.Write("File\tNumber");
                for (Int32 i = 0; i < dist.Count; i++)
                {
                    l1 = labelingpos.Find(dist[i].Position1);
                    l2 = labelingpos.Find(dist[i].Position2);
                    sw.Write('\t' + l1.Name + '_' + l2.Name);
                }
                sw.WriteLine();
                for (Int32 j = 0; j < _fdataToSave.Length; j++)
                {
                    sw.Write(_fdataToSave[j].ShortFileName + '\t' + j.ToString());
                    for (Int32 i = 0; i < dist.Count; i++)
                        sw.Write('\t' + _fdataToSave[j].RmpModel[i].R.ToString("F3"));
                    sw.WriteLine();
                }
                sw.Close();
            }

            // saving converted distances if requested
            if (dist.DataType != DistanceDataType.Rmp)
            {
                using (StreamWriter sw = new StreamWriter(rfilename + '_' + dist.DataType.ToString() + ".txt", false))
                {
                    sw.Write("File\tNumber");
                    for (Int32 i = 0; i < dist.Count; i++)
                    {
                        l1 = labelingpos.Find(dist[i].Position1);
                        l2 = labelingpos.Find(dist[i].Position2);
                        sw.Write('\t' + l1.Name + '_' + l2.Name);
                    }
                    sw.WriteLine();
                    for (Int32 j = 0; j < _fdataToSave.Length; j++)
                    {
                        sw.Write(_fdataToSave[j].ShortFileName + '\t' + j.ToString());
                        for (Int32 i = 0; i < dist.Count; i++)
                            sw.Write('\t' + _fdataToSave[j].RModel[i].R.ToString("F3"));
                        sw.WriteLine();
                    }
                    sw.Close();
                }
            }
        }

        private void SaveSimulationResuntsBin(String srfilename)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(srfilename, FileMode.Create))
            {
                bf.Serialize(fs, _dataToSave);
                fs.Close();
            }
        }

        private void selected_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.selected_radioButton.Checked)
            {
                srcheckBox.Checked = false;
                srcheckBox.Enabled = false;
            }
            else
            {
                srcheckBox.Enabled = true;
            }
        }
    }
}