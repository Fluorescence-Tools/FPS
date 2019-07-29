using System;
using System.IO;
namespace Fps
{
    public class SpringEngine : SimulationBase
    {
        private double kclash, drmaxclash;

        const double minviscosity = 0.0001;
        const double maxviscosity = 0.2;
        const double maxdE = 100.0;         // max increase in energy
        private double dr4dt = 0.1;         // target dr
        private double viscosity, viscosity_per_dt;
        private double dt, rdt;
        private double Etot_last;
        private Boolean optimize_selected_now;

        // local copies
        private int Nmolecules;
        private double minmass = 1.0e12;

        private LabelingPositionList labelingpos_cm; // local coordinates (lp - molecule CM)
        private Vector3[] lpossim;  // storage for simulation
        private int[] LtoM;  // lp to molecule index

        private int[] DtoL1, DtoL2; // distance to lp index
        private int[] DtoM1, DtoM2; // distance to molecule index
        private double[] kplus, kminus;     // spring constants
        private double[] drmaxplus, drmaxminus;     // dr corresponding to max force

        protected Vector3[] translation;  // disregards original CMs
        protected Matrix3[] rotation;
        protected Vector3[] translationm1;  // at t - dt
        protected Matrix3[] rotationm1;
        private Vector3[] translationm2;  // at t - 2dt -- to be able to increase dt
        private Matrix3[] rotationm2;
        private Vector3[] force;
        private Vector3[] torque;
        protected double Eclash;
        protected double Ebond;

        public override object Clone()
        {
            SpringEngine se_new = new SpringEngine(this._molecules, this._labelingpositions, this._distances);
            se_new.SimulationParameters = this.SimulationParameters;
            return se_new;
        }

        public SpringEngine(MoleculeList ms, LabelingPositionList ls, DistanceList ds) : base(ms, ls, ds)
        {
            this.Nmolecules = ms.Count;
            translation = new Vector3[Nmolecules];
            rotation = new Matrix3[Nmolecules];
            translationm1 = new Vector3[Nmolecules];
            rotationm1 = new Matrix3[Nmolecules];
            translationm2 = new Vector3[Nmolecules];
            rotationm2 = new Matrix3[Nmolecules];
            for (int i = 0; i < Nmolecules; i++)
            {
                rotation[i] = Matrix3.E;
                rotationm1[i] = Matrix3.E;
            }
            force = new Vector3[Nmolecules];
            torque = new Vector3[Nmolecules];
        }

        private void PrepareSimulation()
        {
            // molecules
            Nmolecules = _molecules.Count;
            int maxatomsincluster = 0;
            for (int i = 0; i < Nmolecules; i++)
            {
                minmass = Math.Min(minmass, _molecules[i].Mass);
                maxatomsincluster = Math.Max(maxatomsincluster, _molecules[i].MaxAtomsInCluster);
            }

            // labeling positions
            int im;
            labelingpos_cm = new LabelingPositionList();
            labelingpos_cm.AddRange(this._labelingpositions);
            lpossim = new Vector3[labelingpos_cm.Count];
            LtoM = new int[labelingpos_cm.Count];
            // convert to local coordinates
            for (int i = 0; i < labelingpos_cm.Count; i++)
            {
                im = _molecules.FindIndex(labelingpos_cm[i].Molecule);
                LtoM[i] = im;
                labelingpos_cm[i] -= _molecules[im].CM;
                lpossim[i] = labelingpos_cm[i];
            }

            // distances
            Distance dist_i;
            DtoL1 = new int[_distances.Count];
            DtoM1 = new int[_distances.Count];
            DtoL2 = new int[_distances.Count];
            DtoM2 = new int[_distances.Count];
            kplus = new double[_distances.Count];
            kminus = new double[_distances.Count];
            drmaxplus = new double[_distances.Count];
            drmaxminus = new double[_distances.Count];
            double minerror = double.MaxValue;
            for (int i = 0; i < _distances.Count; i++)
            {
                DtoL1[i] = labelingpos_cm.FindIndex(_distances[i].Position1);
                DtoM1[i] = _molecules.FindIndex(labelingpos_cm.Find(_distances[i].Position1).Molecule);
                DtoL2[i] = labelingpos_cm.FindIndex(_distances[i].Position2);
                DtoM2[i] = _molecules.FindIndex(labelingpos_cm.Find(_distances[i].Position2).Molecule);

                // check if bond
                dist_i = _distances[i];
                dist_i.IsBond = (labelingpos_cm[DtoL1[i]].Dye == DyeType.Unknown && labelingpos_cm[DtoL1[i]].AVData.AtomID > 0 &&
                    labelingpos_cm[DtoL1[i]].AVData.AVType == AVSimlationType.None && labelingpos_cm[DtoL2[i]].Dye == DyeType.Unknown &&
                    labelingpos_cm[DtoL2[i]].AVData.AtomID > 0 && labelingpos_cm[DtoL2[i]].AVData.AVType == AVSimlationType.None);
                _distances[i] = dist_i;

                // if bond, set small vdW radii to "exclude" from clashing
                if (dist_i.IsBond)
                {
                    foreach (var lp in new[] { labelingpos_cm[DtoL1[i]], labelingpos_cm[DtoL2[i]] })
                    {
                        int m = _molecules.FindIndex(lp.Molecule);
                        int natom = Array.BinarySearch<int>(_molecules[m].OriginalAtomID, lp.AVData.AtomID);
                        _molecules[m].vdWR[natom] = AtomData.vdWRNoClash;
                        int natom_cluster = Array.FindIndex<int>(_molecules[m].ClusteredAtomOriginalIndex, n => n == natom);
                        if (natom_cluster >= 0)
                        {
                            _molecules[m].ClusteredAtomvdwR[natom_cluster] = AtomData.vdWRNoClash;
                            System.Runtime.InteropServices.Marshal.Copy(new[] { (float)AtomData.vdWRNoClash }, 0,
                                FpsNativeWrapper.Aligned16(_molecules[m].XYZvdwRVectorArray) + (4 * natom_cluster + 3) * sizeof(float), 1);
                        }
                    }
                }

                // "spring constants"
                kplus[i] = 2.0 / _distances[i].ErrPlus / _distances[i].ErrPlus;
                kminus[i] = 2.0 / _distances[i].ErrMinus / _distances[i].ErrMinus;
                //Console.Out.WriteLine("distance#" + i + " k+= " + kplus[i] + " k-= " + kminus[i] + " err+ " + _distances[i].ErrPlus + " err- " + _distances[i].ErrMinus);

                drmaxplus[i] = SimulationParameters.MaxForce / kplus[i];
                drmaxminus[i] = -SimulationParameters.MaxForce / kminus[i];
                minerror = Math.Min(minerror, Math.Min(_distances[i].ErrPlus, _distances[i].ErrMinus));
            }

            // simulation parameters
            kclash = 2.0 / SimulationParameters.ClashTolerance / SimulationParameters.ClashTolerance;
            drmaxclash = -SimulationParameters.MaxForce / kclash;
            dr4dt = Math.Min(SimulationParameters.ClashTolerance * 0.5, minerror);
        }

        /// <summary>
        /// Randomly redistributes subunits until no clashes are detected
        /// </summary>
        public override double SetState()
        {
            Vector3 tshake;
            Vector3 ushake;
            double angleshake, uz, uxy, uphi;
            Matrix3 rotshake;
            int lastshaked;

            PrepareSimulation();
            _originalstate = new SimulationResult() { InternalNumber = -1 };

            for (int i = 0; i < _molecules.Count; i++)
            {
                translation[i] = new Vector3(0.0, 0.0, 0.0);
                rotation[i] = Matrix3.E;
            }
            do
            {
                for (int i = 1; i < _molecules.Count; i++)
                {
                    lastshaked = 0;
                    if (!_molecules[i].Selected)
                    {
                        tshake = (new Vector3(_rnd.NextDouble(), _rnd.NextDouble(), _rnd.NextDouble()) + (-0.5)) * 10.0;
                        translation[i] += tshake;
                        lastshaked = i;
                        uz = -1.0 + _rnd.NextDouble() * 2.0;
                        uxy = Math.Sqrt(1.0 - uz * uz);
                        uphi = _rnd.NextDouble() * 2.0 * Math.PI;
                        ushake = new Vector3(uxy * Math.Cos(uphi), uxy * Math.Sin(uphi), uz);
                        angleshake = 2.0 * Math.PI * _rnd.NextDouble();
                        rotshake = Matrix3.Rotation(ushake, angleshake);
                        rotation[i] = rotshake * rotation[i];
                    }
                    else
                    {
                        translation[i] = translation[lastshaked] + _molecules[i].CM - _molecules[lastshaked].CM;
                        rotation[i] = rotation[lastshaked];
                    }
                }
            }
            while (CheckForClashes(false));
            return ForceAndTorque();
        }

        public override double SetState(SimulationResult sr)
        {
            PrepareSimulation();
            _originalstate = sr;
            for (int i = 0; i < _molecules.Count; i++)
            {
                translation[i] = sr.Translation[i] - _molecules[0].CM + _molecules[i].CM;
                rotation[i] = sr.Rotation[i];
            }
            double E = ForceAndTorque();
            CheckForClashes(true);
            return E + Eclash;
        }

        public override SimulationResult Simulate()
        {
            double E, K, Etot, theta, abst, normF, normT;
            Molecule m;
            Vector3 tnorm;

            // select dt and do the first time step
            E = ForceAndTorque();
            CheckForClashes(true);
            E = E + Eclash;
            dt = dr4dt / Math.Sqrt(2.0 * Math.Max(E, Nmolecules) / minmass) * SimulationParameters.TimeStepFactor;
            rdt = 1.0 / dt;
            Etot_last = E;

            double Mtot = 0.0, ktot = 0.0;
            for (int i = 0; i < Nmolecules; i++) Mtot += _molecules[i].Mass;
            for (int i = 0; i < kplus.Length; i++) ktot += Math.Max(kplus[i], kminus[i]);
            viscosity_per_dt = SimulationParameters.ViscosityFactor * 2.0 * dt * Math.Sqrt(ktot / Mtot);
            viscosity = viscosity_per_dt * dt;
            viscosity = Math.Max(minviscosity, viscosity);
            viscosity = Math.Min(maxviscosity, viscosity);

            for (int i = 0; i < Nmolecules; i++)
            {
                m = _molecules[i];
                translationm1[i] = translation[i];
                rotationm1[i] = rotation[i];

                translation[i] += force[i] * (0.5 / m.Mass) * dt * dt;

                abst = Vector3.Abs(torque[i]);
                tnorm = torque[i] * (1.0 / abst);
                theta = 0.5 * dt * dt * abst / m.SimpleI;
                rotation[i] *= Matrix3.Rotation(tnorm, theta);
            }

            // main loop
            int niter = 0, maxniter;
            optimize_selected_now = (SimulationParameters.OptimizeSelected == OptimizeSelected.Selected
                || SimulationParameters.OptimizeSelected == OptimizeSelected.SelectedThenAll);
            if (SimulationParameters.OptimizeSelected == OptimizeSelected.SelectedThenAll)
                maxniter = SimulationParameters.MaxIterations / 2;
            else maxniter = SimulationParameters.MaxIterations;
            do
            {
                E = Iterate(out normF, out normT, out K);
                Etot = E + K;
                niter++;
                if ((niter << 28) == 0 || Eclash > 10.0 || Etot > Etot_last + maxdE) Cleanup(ref Etot);
                Etot_last = Etot;
            }
            while (((normF > SimulationParameters.FTolerance) || (normT > SimulationParameters.TTolerance)
                || (K > SimulationParameters.KTolerance * Nmolecules)) && (niter < maxniter));

            // repeat if selected then all
            if (SimulationParameters.OptimizeSelected == OptimizeSelected.SelectedThenAll)
            {
                dt = dr4dt / Math.Sqrt(2.0 * Math.Max(E, Nmolecules) / minmass) * SimulationParameters.TimeStepFactor;
                rdt = 1.0 / dt;
                viscosity = SimulationParameters.ViscosityFactor * 2.0 * dt * Math.Sqrt(ktot / Mtot);
                viscosity = Math.Max(minviscosity, viscosity);
                viscosity = Math.Min(maxviscosity, viscosity);

                niter = 0;
                optimize_selected_now = false;
                do
                {
                    E = Iterate(out normF, out normT, out K);
                    Etot = E + K;
                    niter++;
                    if ((niter << 28) == 0 || Eclash > 10.0 || Etot > Etot_last + maxdE) Cleanup(ref Etot);
                    Etot_last = Etot;
                }
                while (((normF > SimulationParameters.FTolerance) || (normT > SimulationParameters.TTolerance)
                    || (K > SimulationParameters.KTolerance * Nmolecules)) && (niter < maxniter));
            }

            SimulationResult sr = new SimulationResult();
            sr.E = E;
            sr.Eclash = Eclash;
            sr.Ebond = Ebond;
            sr.Converged = (niter < SimulationParameters.MaxIterations);
            sr.Translation = new Vector3[Nmolecules];
            sr.Rotation = new Matrix3[Nmolecules];
            sr.ParentStructure = _originalstate.InternalNumber;
            sr.SimulationMethod = _originalstate.SimulationMethod | SimulationMethods.Docking;
            for (int i = 0; i < Nmolecules; i++)
            {
                sr.Translation[i] = Matrix3.Transpose(rotation[0]) * (translation[i] - translation[0])
                    - _molecules[i].CM + _molecules[0].CM;
                sr.Rotation[i] = Matrix3.RepairRotation(Matrix3.Transpose(rotation[0]) * rotation[i]);
            }
            sr.Molecules = _molecules;
            sr.CalculateBestFitTranslation(false);
            return sr;
        }

        /// <summary>
        /// Do one iteration
        /// </summary>
        /// <param name="normF">Sum of F*F</param>
        /// <param name="normT">Sum of T*T</param>
        /// <param name="K">Kinetic energy</param>
        /// <returns>Potential energy (before the iteration)</returns>
        private double Iterate(out double normF, out double normT, out double K)
        {
            double E, theta, thetaw, abst, absw;
            Molecule m;
            Vector3 force_i, torque_i, tmpt, v, w;
            Matrix3 tmpr;

            E = ForceAndTorque();
            CheckForClashes(true);
            E += Eclash;
            normF = 0; normT = 0.0; K = 0.0;

            for (int i = 0; i < Nmolecules; i++)
            {
                m = _molecules[i];
                tmpt = translationm1[i];
                tmpr = rotation[i] * Matrix3.Transpose(rotationm1[i]);
                translationm1[i] = translation[i];
                rotationm1[i] = rotation[i];
                force_i = force[i];
                normF += force_i * force_i;
                torque_i = torque[i];
                normT += torque_i * torque_i;

                translation[i] = translation[i] * (2.0 - viscosity) - tmpt * (1.0 - viscosity)
                    + force_i * (dt * dt / m.Mass);
                v = (translation[i] - tmpt) * 0.5 * rdt;
                K += 0.5 * m.Mass * (v * v);

                abst = torque_i.Normalize();
                theta = dt * dt * abst / m.SimpleI;
                thetaw = Matrix3.AngleAndAxis(tmpr, out w);
                rotation[i] = Matrix3.Rotation(torque_i, theta) * Matrix3.Rotation(w, -thetaw * viscosity)
                    * tmpr * rotation[i];
                absw = thetaw * rdt;
                K += 0.5 * m.SimpleI * absw * absw;
            }
            return E;
        }

        private double Iterate()
        {
            double K, normF, normT;
            return Iterate(out normF, out normT, out K);
        }

        /// <summary>
        /// Re-normalize rotation matrices and adjust dt if needed
        /// </summary>
        /// <param name="E">current potential energy</param>
        private void Cleanup(ref double Etot)
        {
            double E, normF, normT, K = 0.0;

            // re-normalize rotation matrices
            for (int i = 0; i < Nmolecules; i++)
            {
                rotation[i] = Matrix3.RepairRotation(rotation[i]);
                rotationm1[i] = Matrix3.RepairRotation(rotationm1[i]);
            }

            // increase dt if possible
            double dtmax = dr4dt / Math.Sqrt(2.0 * Math.Max(Etot, Nmolecules) / minmass) * SimulationParameters.TimeStepFactor;
            if (dtmax > 2.0 * dt)
            {
                // increase dt by factor of 2
                for (int i = 0; i < Nmolecules; i++)
                {
                    translationm2[i] = translationm1[i];
                    rotationm2[i] = rotationm1[i];
                }
                E = Iterate(out normF, out normT, out K);
                Etot = E + K;
                for (int i = 0; i < Nmolecules; i++)
                {
                    translationm1[i] = translationm2[i];
                    rotationm1[i] = rotationm2[i];
                }
                dt *= 2.0;
                rdt = 1.0 / dt;
                viscosity = viscosity_per_dt * dt;
                viscosity = Math.Max(minviscosity, viscosity);
                viscosity = Math.Min(maxviscosity, viscosity);
            }
        }

        /// <summary>
        /// Forces and torques excluding clashes
        /// </summary>
        protected double ForceAndTorque()
        {
            double E = 0.0, dE;
            Ebond = 0.0;
            int im1, im2;
            for (int i = 0; i < Nmolecules; i++)
            {
                force[i] = new Vector3();
                torque[i] = new Vector3();
            }
            // rotate and shift all lps
            for (int i = 0; i < labelingpos_cm.Count; i++)
            {
                im1 = LtoM[i];
                lpossim[i] = rotation[im1] * labelingpos_cm[i] + translation[im1];
            }

            Vector3 r, f;
            double absr, absF, dr;
            for (int i = 0; i < _distances.Count; i++)
            {
                if (optimize_selected_now && !_distances[i].IsSelected) continue;
                r = lpossim[DtoL1[i]] - lpossim[DtoL2[i]];
                absr = r.Normalize();
                dr = absr - _distances[i].R;
                if (dr > 0)
                {
                    absF = Math.Min(dr * kplus[i], SimulationParameters.MaxForce);
                    dE = (dr < drmaxplus[i]) ? 0.5 * absF * dr : absF * (dr - 0.5 * drmaxplus[i]);
                    //Console.Out.WriteLine("distance#" + i + " dE= " + dE + " dr= " + dr + " k+= " + kplus[i] + " drmax+= " + drmaxplus[i] );
                }
                else
                {
                    absF = Math.Max(dr * kminus[i], -SimulationParameters.MaxForce);
                    dE = (dr > drmaxminus[i]) ? 0.5 * absF * dr : absF * (dr - 0.5 * drmaxminus[i]);
                    //Console.Out.WriteLine("distance#" + i + " dE= " + dE + " dr= " + dr + " k-= " + kminus[i]  + " drmax-= " + drmaxminus[i]);
                }
                E += dE;
                if (_distances[i].IsBond) Ebond += dE;

                im1 = DtoM1[i];
                im2 = DtoM2[i];
                f = r * absF;
                force[im1] -= f;
                force[im2] += f;
                torque[im1] += Vector3.Cross(lpossim[DtoL1[i]] - translation[im1], -f);
                torque[im2] += Vector3.Cross(lpossim[DtoL2[i]] - translation[im2], f);
            }
            return E;
        }

        #region Clashes

        // all molecules
        protected Boolean CheckForClashes(Boolean all)
        {
            // not "all" means only initially mobile molecules
            Boolean clash = false;
            Eclash = 0.0;
            for (int im1 = 0; im1 < _molecules.Count; im1++)
                for (int im2 = im1 + 1; im2 < _molecules.Count; im2++)
                    clash |= CheckForClashes(im1, im2) && (!_molecules[im2].Selected | all);
            return clash;
        }

        // two molecules
        private Boolean CheckForClashes(int im1, int im2)
        {
            Molecule m1 = _molecules[im1];
            Molecule m2 = _molecules[im2];
            Vector3 dcm = translation[im1] - translation[im2], rc1, rc2;

            // check clusters
            Vector3[] rc2cache = new Vector3[m2.AtomClusters.Length];
            double sumr, dsq, dx2, dy2, dz2;
            Vector3 sumrv;
            AtomCluster c1, c2;
            Matrix3 rot1to2 = Matrix3.Transpose(rotation[im2]) * rotation[im1];
            Matrix3 rot2to1 = Matrix3.Transpose(rot1to2);
            Vector3 dcm1t = Matrix3.Transpose(rotation[im1]) * dcm; 
            Vector3 dcm2t = Matrix3.Transpose(rotation[im2]) * dcm;
            int[] c2tocheck = new int[m2.AtomClusters.Length];
            int j2, n2tocheck;
            int[] mustbechecked1 = new int[m1.AtomClusters.Length];
            int[] mustbechecked2 = new int[m2.AtomClusters.Length];

            // check if we can skip some of m2
            n2tocheck = 0;
            for (int j = 0; j < m2.AtomClusters.Length; j++)
            {
                c2 = m2.AtomClusters[j];
                rc2 = rot2to1 * c2 - dcm1t;
                dx2 = rc2.X * rc2.X;
                dy2 = rc2.Y * rc2.Y;
                dz2 = rc2.Z * rc2.Z;
                sumrv = m1.MaxRadius + c2.ClusterRadius;
                if ((dy2 + dz2 <= sumrv.X * sumrv.X) && (dx2 + dz2 <= sumrv.Y * sumrv.Y) && (dx2 + dy2 <= sumrv.Z * sumrv.Z))
                {
                    rc2cache[n2tocheck] = c2;
                    c2tocheck[n2tocheck++] = j;
                }
            }

            for (int i = 0; i < m1.AtomClusters.Length; i++)
            {
                c1 = m1.AtomClusters[i];
                rc1 = rot1to2 * c1 + dcm2t;
                // if completely out of reach, skip the check
                dx2 = rc1.X * rc1.X;
                dy2 = rc1.Y * rc1.Y;
                dz2 = rc1.Z * rc1.Z;
                sumrv = m2.MaxRadius + c1.ClusterRadius;
                if ((dy2 + dz2 > sumrv.X * sumrv.X) || (dx2 + dz2 > sumrv.Y * sumrv.Y) || (dx2 + dy2 > sumrv.Z * sumrv.Z))
                    continue;
                // otherwise check selected clusters from m2
                for (int j = 0; j < n2tocheck; j++)
                {
                    j2 = c2tocheck[j];
                    rc2 = rc2cache[j];
                    c2 = m2.AtomClusters[j2];
                    dsq = Vector3.SquareNormDiff(rc1, rc2);
                    sumr = c1.ClusterRadius + c2.ClusterRadius;
                    if (dsq < sumr * sumr) // clusters overlap, check all atoms
                    {
                        mustbechecked1[i] = 1;
                        mustbechecked2[j2] = 1;
                        // clash |= CheckForClashes(c1, c2, im1, im2, dcm); // => replaced with unmanaged
                    }
                }
            }
            
            // this runs in m1's coordinate system!
            Vector3 fclash = new Vector3(), tclash1 = new Vector3(), tclash2 = new Vector3();
            double dEClash = FpsNativeWrapper.CheckForClashes(FpsNativeWrapper.Aligned16(m1.XYZvdwRVectorArray),
                FpsNativeWrapper.Aligned16(m2.XYZvdwRVectorArray), m1.AtomClusters, m2.AtomClusters,
                m1.AtomClusters.Length, m2.AtomClusters.Length, mustbechecked1, mustbechecked2, 
                rot2to1, -dcm1t, kclash, ref fclash, ref tclash1, ref tclash2);
            force[im1] += rotation[im1] * fclash;
            force[im2] -= rotation[im1] * fclash;
            torque[im1] += rotation[im1] * tclash1;
            torque[im2] += rotation[im1] * tclash2;
            Eclash += dEClash;

            return dEClash > 1.0e-8;
        }

        // two clusters, all atoms
        //private Boolean CheckForClashes(AtomCluster c1, AtomCluster c2, int im1, int im2, Vector3 dcm)
        //{
        //    Molecule m1 = _molecules[im1];
        //    Molecule m2 = _molecules[im2];
        //    Vector3 f, r1, r2, d;
        //    Vector3 rc2 = rotation[im2] * c2 - dcm;
        //    double sumr, rsq, dx, dy, dz, absF, vdwri, r, dr;

        //    Boolean clash = false;
        //    for (int aj = 0; aj < c2.NAtoms; aj++)
        //        r2cache[aj] = rotation[im2] * m2.ClusteredAtomXYZ[c2.StartIndexInClusterArrays + aj];
        //    for (int ai = 0; ai < c1.NAtoms; ai++)
        //    {
        //        r1 = rotation[im1] * m1.ClusteredAtomXYZ[c1.StartIndexInClusterArrays + ai];
        //        vdwri = m1.ClusteredAtomvdwR[c1.StartIndexInClusterArrays + ai];
        //        // if completely out of reach, skip the check
        //        dx = r1.X - rc2.X;
        //        dy = r1.Y - rc2.Y;
        //        dz = r1.Z - rc2.Z;
        //        rsq = dx * dx + dy * dy + dz * dz;
        //        sumr = c2.ClusterRadius + vdwri;
        //        if (rsq > sumr * sumr) continue;
        //        d = r1 + dcm;
        //        for (int aj = 0; aj < c2.NAtoms; aj++)
        //        {
        //            r2 = r2cache[aj];
        //            dx = d.X - r2.X;
        //            dy = d.Y - r2.Y;
        //            dz = d.Z - r2.Z;
        //            rsq = dx * dx + dy * dy + dz * dz;
        //            sumr = vdwri + m2.ClusteredAtomvdwR[c2.StartIndexInClusterArrays + aj];

        //            if (rsq < sumr * sumr)
        //            {
        //                clash = true;
        //                r = Math.Sqrt(rsq);
        //                dr = r - sumr;
        //                absF = Math.Max(kclash * dr, -SimulationParameters.MaxForce);
        //                Eclash += (dr > drmaxclash) ? 0.5 * absF * dr : absF * (dr - 0.5 * drmaxclash);
        //                absF /= r;
        //                f = new Vector3(dx * absF, dy * absF, dz * absF);
        //                force[im1] -= f;
        //                force[im2] += f;
        //                torque[im1] += Vector3.Cross(r1, -f);
        //                torque[im2] += Vector3.Cross(r2, f);
        //            }
        //        }
        //    }
        //    return clash;
        //}

        #endregion

        protected double RandomNorm()
        {
            double u, v, q, x1, x2;

            const double ei = 0.27597, eo = 0.27846;
            const double sqrt2e = 1.71552776992141, a = 0.449871, b = 0.386595;

            for (; ;)
            {
                // Generate P = (u,v) uniform in rectangle enclosing
                // acceptance region:
                //   0 < u < 1
                // - sqrt(2/e) < v < sqrt(2/e)
                // The constant below is 2*sqrt(2/e).

                u = _rnd.NextDouble();
                v = sqrt2e * (_rnd.NextDouble() - 0.5);

                // Evaluate the quadratic form
                x1 = u - a;
                x2 = Math.Abs(v) + b;
                q = x1 * x1 + (0.19600 * x2 - 0.25472 * x1) * x2;

                // Accept P if inside inner ellipse
                if (q < ei) break;

                // Reject P if outside outer ellipse
                if (q > eo) continue;

                // Between ellipses: perform exact test
                if (v * v <= -4.0 * Math.Log(u) * u * u) break;
            }

            return v / u;
        }
    }
}
