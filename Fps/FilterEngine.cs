using System;
using System.Collections.Generic;

namespace Fps
{
    [Serializable]
    public struct FilterEngineParameters
    {
        public Double R0;
        public Boolean OptimizeSelected;
    }

    // reference atoms define the coordinate system for which LP's xyz is provided
    public struct ReferenceAtom
    {
        public Int32 OriginalN;       // in the pdb file
        public Int32 ConvertedN;      // array element
        public Double X;              // coordinates
        public Double Y;
        public Double Z;

        public static implicit operator Vector3(ReferenceAtom r)
        {
            return new Vector3(r.X, r.Y, r.Z);
        }
    }

    public class FilterEngine : ICloneable 
    {
        private Random rnd;

        private FilterEngineParameters _filterParameters;
        public FilterEngineParameters FilterParameters
        {
            get { return _filterParameters; }
            set { _filterParameters = value; }
        }

        private LabelingPositionList labelingpos; // original
        public LabelingPositionList LabelingPositions
        {
            get { return labelingpos; }
            set
            {
                labelingpos = new LabelingPositionList();
                labelingpos.AddRange(value);
                labelingpos.FullPath = value.FullPath;
            }
        }

        private DistanceList dist;
        public DistanceList Distances
        {
            get { return dist; }
            set
            {
                dist = new DistanceList();
                dist.AddRange(value);
                dist.DataType = value.DataType;
            }
        }
        private List<ReferenceAtom[]> referenceAtoms; // "reference frame" to align XYZ of an LP 
        public List<ReferenceAtom[]> ReferenceAtoms
        {
            get { return referenceAtoms; }
            set
            {
                referenceAtoms = new List<ReferenceAtom[]>(value.Count);
                referenceAtoms.AddRange(value);
            }
        }

        private AVGlobalParameters avparam;
        public AVGlobalParameters AVGridParameters
        {
            get { return avparam; }
            set { avparam = value; }
        }

        public Object Clone()
        {
            FilterEngine fe_new = new FilterEngine(this.labelingpos, this.dist);
            fe_new.FilterParameters = this.FilterParameters;
            fe_new.ReferenceAtoms = this.referenceAtoms;
            fe_new.AVGridParameters = this.avparam;
            return fe_new;
        }

        public FilterEngine(LabelingPositionList ls, DistanceList ds)
        {
            this.LabelingPositions = ls;
            this.Distances = ds;
#if DEBUG
            rnd = new Random(53672429);
#else
            rnd = new Random();
#endif           
            // just in case default AV parameters
            this.avparam.GridSize = 0.2;
            this.avparam.MinGridSize = 0.4;
            this.avparam.LinkerInitialSphere = 0.5;
            this.avparam.LinkSearchNodes = 3;
            this.avparam.ESamples = 200000;
        }

        /// <summary>
        /// Read reference atoms from an LP file
        /// </summary>
        /// <param name="m">Example molecule</param>
        /// <param name="classicpdb">True for pdb line format, false if tab-delimited</param>
        public void ReadRefAtoms(Molecule m, Boolean classicpdb)
        {
            // re-read the lp file to get reference frames
            referenceAtoms = new List<ReferenceAtom[]>(labelingpos.Count);
            Int32 j;
            String[] strdata;
            strdata = System.IO.File.ReadAllLines(labelingpos.FullPath);

            String[] tmpstr;
            Double x, y, z;
            Char[] separator = new Char[] {' ', '\t'};
            ReferenceAtom[] reftmp;
            for (Int32 i = 0; i < strdata.Length; i++)
            {
                // find LP line
                if (!LabelingPositionList.ValidLPLine(strdata[i])) continue;
                // now LP is found, count following pdb lines if any
                j = 0;
                if (classicpdb)
                    while (i + j + 1 < strdata.Length && strdata[i + j + 1].Length > 54 && strdata[i + j + 1].Substring(0, 4) == "ATOM") j++;
                else
                    while (i + j + 1 < strdata.Length && strdata[i + j + 1].Length > 10 && strdata[i + j + 1].Substring(0, 4) == "ATOM") j++;
                reftmp = new ReferenceAtom[j];
                // read reference atoms
                j = 0;
                if (classicpdb)
                    while (i + j + 1 < strdata.Length && strdata[i + j + 1].Length > 54 && strdata[i + j + 1].Substring(0, 4) == "ATOM")
                    {
                        tmpstr = strdata[i + j + 1].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        reftmp[j].OriginalN = Int32.Parse(tmpstr[1]);
                        reftmp[j].ConvertedN = Array.BinarySearch<Int32>(m.OriginalAtomID, reftmp[j].OriginalN);
                        if (Double.TryParse(strdata[i + j + 1].Substring(30, 8), out x) &&
                            Double.TryParse(strdata[i + j + 1].Substring(38, 8), out y) &&
                            Double.TryParse(strdata[i + j + 1].Substring(46, 8), out z))
                        {
                            reftmp[j].X = x;
                            reftmp[j].Y = y;
                            reftmp[j].Z = z;
                        }
                        j++;
                    }
                else
                    while (i + j + 1 < strdata.Length && strdata[i + j + 1].Length > 10 && strdata[i + j + 1].Substring(0, 4) == "ATOM")
                    {
                        tmpstr = strdata[i + j + 1].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        reftmp[j].OriginalN = Int32.Parse(tmpstr[1]);
                        reftmp[j].ConvertedN = Array.BinarySearch<Int32>(m.OriginalAtomID, reftmp[j].OriginalN);
                        if (Double.TryParse(tmpstr[6], out x) &&
                            Double.TryParse(tmpstr[7], out y) &&
                            Double.TryParse(tmpstr[8], out z))
                        {
                            reftmp[j].X = x;
                            reftmp[j].Y = y;
                            reftmp[j].Z = z;
                        }
                        j++;
                    }
                referenceAtoms.Add(reftmp);
                i += j;
            }
        }

        // process a structure
        public double CalculateChi2(ref FilteringResult fr)
        {
            Molecule m;
            if (fr.MoleculeWeakReference == null || !fr.MoleculeWeakReference.TryGetTarget(out m))
                m = new Molecule(fr.FullFileName);
            fr.MoleculeWeakReference = new WeakReference<Molecule>(m);
            if (m.Error.Length > 0)
            {
                System.Windows.Forms.MessageBox.Show("Error reading file " + fr.FullFileName + ": " + m.Error, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return MiscData.ENotCalculated;
            }
            AVEngine av = new AVEngine(m, avparam);
            Int32 natom, ilp = 0, ilp1, ilp2, iref, nref_total = 0;
            List<Vector3[]> avcache = new List<Vector3[]>(labelingpos.Count);
            Vector3[] t;
            Vector3[] rmp = new Vector3[labelingpos.Count];
            Vector3 cr, cm; Matrix3 U;
            ReferenceAtom rr;
            Double R06 = _filterParameters.R0 * _filterParameters.R0 * _filterParameters.R0 *
                _filterParameters.R0 * _filterParameters.R0 * _filterParameters.R0,
                dr, refrmsd_t = 0.0;

            fr.E = 0.0;
            fr.InvalidR = 0;
            fr.RefRMSD = 0.0;
            fr.Sigma1 = 0;
            fr.Sigma2 = 0;
            fr.Sigma3 = 0;

            foreach (LabelingPosition l in this.labelingpos)
            {
                // calculate AVs and mean positions
                if (l.AVData.AVType == AVSimlationType.SingleDyeR)
                {
                    natom = Array.BinarySearch<Int32>(m.OriginalAtomID, l.AVData.AtomID);
                    av.Calculate1R(l.AVData.L, l.AVData.W, l.AVData.R, natom);
                    t = new Vector3[av.R.Length];
                    Array.Copy(av.R, t, av.R.Length);
                    avcache.Add(t);
                    rmp[ilp++] = av.Rmp;
                }
                else if (l.AVData.AVType == AVSimlationType.ThreeDyeR)
                {
                    natom = Array.BinarySearch<Int32>(m.OriginalAtomID, l.AVData.AtomID);
                    av.Calculate3R(l.AVData.L, l.AVData.W, l.AVData.R1, l.AVData.R2, l.AVData.R3, natom);
                    t = new Vector3[av.R.Length];
                    Array.Copy(av.R, t, av.R.Length);
                    avcache.Add(t);
                    rmp[ilp++] = av.Rmp;
                }
                else if (l.AVData.AVType == AVSimlationType.None && referenceAtoms[ilp].Length > 0)
                {
                    // align reference atoms with the structure
                    // translation
                    cr = new Vector3();
                    cm = new Vector3();
                    for (iref = 0; iref < referenceAtoms[ilp].Length; iref++)
                    {
                        rr = referenceAtoms[ilp][iref];
                        natom = rr.ConvertedN;
                        cr += rr;
                        cm += new Vector3(m.XLocal[natom] + m.CM.X, m.YLocal[natom] + m.CM.Y, m.ZLocal[natom] + m.CM.Z);
                    }
                    cr *= -1.0 / ((Double)referenceAtoms[ilp].Length);
                    cm *= -1.0 / ((Double)referenceAtoms[ilp].Length);

                    // rotation: see also SimulationResult.CalculateBestFitRotation
                    Mapack.Matrix Rxt = new Mapack.Matrix(referenceAtoms[ilp].Length, 3);
                    Mapack.Matrix Ry = new Mapack.Matrix(3, referenceAtoms[ilp].Length);

                    for (iref = 0; iref < referenceAtoms[ilp].Length; iref++)
                    {
                        rr = referenceAtoms[ilp][iref];
                        natom = rr.ConvertedN;
                        Rxt[iref, 0] = rr.X + cr.X; Rxt[iref, 1] = rr.Y + cr.Y; Rxt[iref, 2] = rr.Z + cr.Z;
                        Ry[0, iref] = m.XLocal[natom] + m.CM.X + cm.X;
                        Ry[1, iref] = m.YLocal[natom] + m.CM.Y + cm.Y;
                        Ry[2, iref] = m.ZLocal[natom] + m.CM.Z + cm.Z;
                    }

                    // Kabsch solution
                    Mapack.Matrix R = Ry * Rxt;
                    Mapack.SingularValueDecomposition svdR = new Mapack.SingularValueDecomposition(R);
                    Mapack.Matrix V = svdR.VMatrix;
                    Mapack.Matrix rS = new Mapack.Matrix(3, 3);
                    rS[0, 0] = 1.0 / svdR.Diagonal[0];
                    rS[1, 1] = 1.0 / svdR.Diagonal[1];
                    rS[2, 2] = (R.Determinant > 0.0) ? 1.0 / svdR.Diagonal[2] : -1.0 / svdR.Diagonal[2];
                    Mapack.Matrix Um = R * V * rS * V.Transpose();
                    U = new Matrix3(Um[0, 0], Um[0, 1], Um[0, 2],
                        Um[1, 0], Um[1, 1], Um[1, 2], Um[2, 0], Um[2, 1], Um[2, 2]);
                    U = Matrix3.RepairRotation(U);

                    // reference rmsd
                    for (iref = 0; iref < referenceAtoms[ilp].Length; iref++)
                    {
                        rr = referenceAtoms[ilp][iref];
                        natom = rr.ConvertedN;
                        refrmsd_t += Vector3.SquareNormDiff(U * (rr + cr),
                            new Vector3(m.XLocal[natom], m.YLocal[natom], m.ZLocal[natom]) + m.CM + cm);
                        nref_total++;
                    }

                    rmp[ilp++] = U * (l + cr) - cm;
                    avcache.Add(new Vector3[0]);
                }
                else
                {
                    rmp[ilp++] = l;
                    avcache.Add(new Vector3[0]);
                }
            }

            // calculate mp and FRET distances
            Distance d, dmp;
            Int32 activeR = 0;
            fr.RModel = new DistanceList(dist.Count);
            fr.RmpModel = new DistanceList(dist.Count);
            fr.RmpModel.DataType = dist.DataType;
            for (Int32 i = 0; i < dist.Count; i++)
            {
                dmp = new Distance();
                d = new Distance();
                dmp.Position1 = dist[i].Position1;
                dmp.Position2 = dist[i].Position2;
                d.Position1 = dist[i].Position1;
                d.Position2 = dist[i].Position2;
                ilp1 = labelingpos.FindIndex(d.Position1);
                ilp2 = labelingpos.FindIndex(d.Position2);
                dmp.R = Vector3.Abs(rmp[ilp1] - rmp[ilp2]);
                if (dist.DataType == DistanceDataType.Rmp || 
                    labelingpos[ilp1].AVData.AVType == AVSimlationType.None || labelingpos[ilp2].AVData.AVType == AVSimlationType.None)
                    d.R = dmp.R; // i.e. no clouds -> Rmp
                else if (dist.DataType == DistanceDataType.RDAMeanE)
                {
                    if (avcache[ilp1].Length == 0 || avcache[ilp2].Length == 0) d.R = Double.NaN;
                    else d.R = FpsNativeWrapper.RdaMeanEFromAv(avcache[ilp1], avcache[ilp1].Length, avcache[ilp2], avcache[ilp2].Length,
                        avparam.ESamples, rnd.Next(), this._filterParameters.R0);               
                }
                else if (dist.DataType == DistanceDataType.RDAMean)
                {
                    if (avcache[ilp1].Length == 0 || avcache[ilp2].Length == 0) d.R = Double.NaN;
                    else d.R = FpsNativeWrapper.RdaMeanFromAv(avcache[ilp1], avcache[ilp1].Length, avcache[ilp2], avcache[ilp2].Length,
                        avparam.ESamples, rnd.Next()); 
                }

                fr.RModel.Add(d);
                fr.RmpModel.Add(dmp);

                dr = d.R - dist[i].R;
                if (Double.IsNaN(d.R)) fr.InvalidR++;
                else if (!this._filterParameters.OptimizeSelected || dist[i].IsSelected)
                {
                    fr.E += dr > 0.0 ? dr * dr / dist[i].ErrPlus / dist[i].ErrPlus :
                        dr * dr / dist[i].ErrMinus / dist[i].ErrMinus;
                    activeR++;
                    if (dr > dist[i].ErrPlus) fr.Sigma1++;
                    if (dr > 2.0 * dist[i].ErrPlus) fr.Sigma2++;
                    if (dr > 3.0 * dist[i].ErrPlus) fr.Sigma3++;
                    if (dr < -dist[i].ErrMinus) fr.Sigma1++;
                    if (dr < -2.0 * dist[i].ErrMinus) fr.Sigma2++;
                    if (dr < -3.0 * dist[i].ErrMinus) fr.Sigma3++;
                }
            }

            fr.E = fr.E / (Double)activeR;
            fr.RefRMSD = nref_total == 0 ? 0.0 : Math.Sqrt(refrmsd_t / (Double)nref_total);
            return fr.E;

        }


    }
}
