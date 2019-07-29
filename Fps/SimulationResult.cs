using System;

namespace Fps
{
    [Flags]
    public enum SimulationMethods
    {
        Unknown = 0,
        Docking = 0x0001,
        Refinement = 0x0002,
        ErrorEstimation = 0x0004,
        MetropolisSampling = 0x0008
    }

    // docking results
    [Serializable]
    public struct SimulationResult
    {
        public Double E;                    // "energy" -> not reduced chi2
        public Double Eclash;               // clash energy
        public Double Ebond;                // "bond" (ATOM-ATOM) energy
        public Vector3[] Translation;
        public Matrix3[] Rotation;
        public Vector3 BestFitTranslation;  // best fit with respect to another structure,
        public Matrix3 BestFitRotation;     // applied to this (whole) structure
        public Boolean Converged;           // fit converged?
        public SimulationMethods SimulationMethod;   // how the structure was obtained
        public Int32 InternalNumber;        // to avoid a mess when dataview is sorted
        public Int32 ParentStructure;       // if a result of refinement or error estimation

        [NonSerialized()]
        public MoleculeList Molecules;
        public LabelingPositionList RefinedLabelingPositions;

        /// <summary>
        /// RMSD compared to another structure
        /// </summary>
        /// <param name="sr_other">another structure</param>
        /// <returns>rmsd value</returns>
        public Double RMSD(SimulationResult sr_other, Boolean bestfit, Boolean selected_only)
        {
            Double sd = 0.0;
            Matrix3 U;
            Vector3 r, t;
            Int32 NAtomstotal = 0;
            Molecule m;
            if (bestfit) this.CalculateBestFitRotation(sr_other, false);

            for (Int32 i = 0; i < this.Molecules.Count; i++)
            {
                m = this.Molecules[i];
                if (selected_only && !m.Selected) continue;
                if (bestfit)
                {
                    U = this.BestFitRotation * this.Rotation[i] - sr_other.Rotation[i];
                    t = this.BestFitRotation * (this.Translation[i] + m.CM + this.BestFitTranslation)
                        - sr_other.Translation[i] - m.CM - sr_other.BestFitTranslation;
                }
                else
                {
                    U = this.Rotation[i] - sr_other.Rotation[i];
                    t = this.Translation[i] - sr_other.Translation[i];
                }
                for (Int32 j = 0; j < m.NAtoms; j++)
                {
                    r = new Vector3(m.XLocal[j], m.YLocal[j], m.ZLocal[j]);
                    sd += Vector3.SquareNormDiff(U * r, t);
                }
                NAtomstotal += m.NAtoms;
            }
            return Math.Sqrt(sd / (Double)NAtomstotal);
        }
        public Double RMSD(SimulationResult sr2, Boolean bestfit)
        {
            return this.RMSD(sr2, bestfit, false);
        }

        /// <summary>
        /// Best translation: centroid or CM -> origin
        /// </summary>
        /// <param name="weighted">apply weights = mass</param>
        public void CalculateBestFitTranslation(Boolean weighted)
        {
            // translation: match centroinds or CMs
            Vector3 r, c = new Vector3();
            Int32 NAtomstotal = 0;
            Molecule m;
            Double sumw = 0.0;
            for (Int32 i = 0; i < this.Molecules.Count; i++)
            {
                m = this.Molecules[i];
                if (weighted)
                {
                    c += (this.Translation[i] + m.CM) * m.Mass;
                    sumw += m.Mass;
                }
                else   // unweighted
                    for (Int32 j = 0; j < m.NAtoms; j++)
                    {
                        r = new Vector3(m.XLocal[j], m.YLocal[j], m.ZLocal[j]);
                        c += this.Rotation[i] * r + this.Translation[i] + m.CM;
                    }
                NAtomstotal += m.NAtoms;
            }
            if (!weighted) sumw = (Double)NAtomstotal;
            this.BestFitTranslation = c * (-1.0 / sumw);
        }

        /// <summary>
        /// Best rotation to overlay with a reference structure
        /// See Kabsch papers for details: Kabsch W. Acta Cryst. A32 (1976) 922, A34 (1978) 827
        /// It is assumed that translations are already calculated in the same (weighted/unweighted) mode!!!
        /// </summary>
        /// <param name="refsr">reference structure</param>
        /// <param name="weighted">apply weights = mass</param>
        public void CalculateBestFitRotation(SimulationResult refsr, Boolean weighted)
        {
            if (Double.IsNaN(this.E) || Double.IsNaN(refsr.E))
            {
                this.BestFitRotation = Matrix3.E;
                return;
            }
            Vector3 r, r1, r2;
            Int32 NAtomstotal = 0, n;
            Molecule m;
            Double w;
            for (Int32 i = 0; i < this.Molecules.Count; i++) NAtomstotal += this.Molecules[i].NAtoms;

            // rotation
            Mapack.Matrix Rxt = new Mapack.Matrix(NAtomstotal, 3);
            Mapack.Matrix Ry = new Mapack.Matrix(3, NAtomstotal);
            n = 0;
            for (Int32 i = 0; i < this.Molecules.Count; i++)
            {
                m = this.Molecules[i];
                for (Int32 j = 0; j < m.NAtoms; j++)
                {
                    r = new Vector3(m.XLocal[j], m.YLocal[j], m.ZLocal[j]);
                    r1 = this.Rotation[i] * r + this.Translation[i] + m.CM + this.BestFitTranslation;
                    r2 = refsr.Rotation[i] * r + refsr.Translation[i] + m.CM + refsr.BestFitTranslation;
                    w = weighted ? m.AtomMass[j] : 1.0;
                    Rxt[n, 0] = r1.X; Rxt[n, 1] = r1.Y; Rxt[n, 2] = r1.Z;
                    Ry[0, n] = r2.X * w; Ry[1, n] = r2.Y * w; Ry[2, n] = r2.Z * w;
                    n++;
                }
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
            Matrix3 U = new Matrix3(Um[0, 0], Um[0, 1], Um[0, 2],
                Um[1, 0], Um[1, 1], Um[1, 2], Um[2, 0], Um[2, 1], Um[2, 2]);
            this.BestFitRotation = Matrix3.RepairRotation(U);
        }

        /// <summary>
        /// Model distance between labeling positions l1 and l2
        /// </summary>
        /// <param name="l1">position 1</param>
        /// <param name="l2">position 2</param>
        /// <returns>Model distance</returns>
        public Double ModelDistance(LabelingPosition l1, LabelingPosition l2)
        {
            Int32 im1 = this.Molecules.FindIndex(l1.Molecule);
            Int32 im2 = this.Molecules.FindIndex(l2.Molecule);
            // take refined LPs if available
            LabelingPosition lp1ref, lp2ref;
            if ((this.SimulationMethod & SimulationMethods.Refinement) != 0 && this.RefinedLabelingPositions != null)
            {
                lp1ref = this.RefinedLabelingPositions.Find(l1.Name);
                lp2ref = this.RefinedLabelingPositions.Find(l2.Name);
            }
            else
            {
                lp1ref = l1;
                lp2ref = l2;
            }
            Vector3 d = this.Rotation[im1] * (lp1ref - this.Molecules[im1].CM) + this.Molecules[im1].CM + this.Translation[im1]
                - this.Rotation[im2] * (lp2ref - this.Molecules[im2].CM) - this.Molecules[im2].CM - this.Translation[im2];
            return Vector3.Abs(d);
        }

        // for formatting
        public override string ToString()
        {
            if (this.SimulationMethod == SimulationMethods.Docking) return this.InternalNumber.ToString();
            else return String.Format("{0} ({1})", this.InternalNumber, this.ParentStructure);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////

    // filtering results
    [Serializable]
    public struct FilteringResult
    {
        public String FullFileName;         // full path to molecule file
        public String ShortFileName;        // file name excl. path
        public Double E;                    // "energy"
        public Matrix3 BestFitRotation;     // applied to this (whole) structure
        public Int32 InternalNumber;        // to avoid a mess when dataview is sorted
        public Double RefRMSD;              // rmsd of the labeling position's reference frame
        public Int32 Sigma1;                // violations by >1 sigma
        public Int32 Sigma2;                // violations by >2 sigma
        public Int32 Sigma3;                // violations by >3 sigma

        public DistanceList RModel;            // model distances of the same type as input distances; 
                                            // = Rmp if at least one LP is of XYZ type
        public DistanceList RmpModel;          // mean position model distances

        public Int32 InvalidR;              // number of NaN distances (due to empty AVs)

        [NonSerialized()]
        public WeakReference<Molecule> MoleculeWeakReference;  // try to avoid reloading from the disk if possible

        public Double RMSD(Molecule mref, Boolean bestfit)
        {
            Double sd = 0.0;
            Vector3 r, r0;
            Molecule m;
            if (this.MoleculeWeakReference == null || !this.MoleculeWeakReference.TryGetTarget(out m))
                m = new Molecule(this.FullFileName);
            this.MoleculeWeakReference = new WeakReference<Molecule>(m);

            // same atom numbering is assumed
            Int32 NAtomstotal = Math.Min(mref.NAtoms, m.NAtoms);
            if (bestfit) this.CalculateBestFitRotation(mref, m);
            else this.BestFitRotation = Matrix3.E;

            for (Int32 j = 0; j < NAtomstotal; j++)
            {
                r0 = new Vector3(mref.XLocal[j], mref.YLocal[j], mref.ZLocal[j]);
                r = new Vector3(m.XLocal[j], m.YLocal[j], m.ZLocal[j]);
                sd += Vector3.SquareNormDiff(this.BestFitRotation * r, r0);
            }
            return Math.Sqrt(sd / (Double)NAtomstotal);;
        }

        // see SimulationResult.CalculateBestFitRotation
        public void CalculateBestFitRotation(Molecule mref, Molecule m)
        {
            int NAtomstotal = Math.Min(mref.NAtoms, m.NAtoms);

            // rotation
            Mapack.Matrix Rxt = new Mapack.Matrix(NAtomstotal, 3);
            Mapack.Matrix Ry = new Mapack.Matrix(3, NAtomstotal);
            for (int j = 0; j < NAtomstotal; j++)
            {
                Rxt[j, 0] = m.XLocal[j]; Rxt[j, 1] = m.YLocal[j]; Rxt[j, 2] = m.ZLocal[j];
                Ry[0, j] = mref.XLocal[j]; Ry[1, j] = mref.YLocal[j]; Ry[2, j] = mref.ZLocal[j];
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
            Matrix3 U = new Matrix3(Um[0, 0], Um[0, 1], Um[0, 2],
                Um[1, 0], Um[1, 1], Um[1, 2], Um[2, 0], Um[2, 1], Um[2, 2]);
            this.BestFitRotation = Matrix3.RepairRotation(U);
        }

        // for formatting
        public override string ToString()
        {
            return this.ShortFileName;
        }

    }

}