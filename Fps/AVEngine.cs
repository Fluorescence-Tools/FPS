using System;

namespace Fps
{
    /// <summary>
    /// represents grid parameters for one coordinate
    /// </summary>
    public struct Grid
    {
        public Int32 NNodes;
        public Double Min;
        public Double Step;
    }

    public enum AVSimlationType
    {
        None, // XYZ defined externally
        SingleDyeR,
        ThreeDyeR,
        RealDyeStructure,
        DensityFile // external simulation result
    }
    public enum AVSimlationTypeShort    // the same; short text codes for reading/writing lps
    {
        XYZ, AV1, AV3, AVS, EDF
    }

    /// <summary>
    /// Global grid parameters
    /// </summary>
    [Serializable]
    public struct AVGlobalParameters
    {
        public Double GridSize;             // FRETnpsTools: 0.2
        public Double MinGridSize;          // min grid size
        public Double LinkerInitialSphere;  // FRETnpsTools: 0.5 (guess)
        public Int32 LinkSearchNodes;       // FRETnpsTools: 3 (guess)
        public Int32 ESamples;              // e.g. to calculate <RDA>E
    }

    /// <summary>
    /// Data needed to reproduce an AV simulation
    /// </summary>
    [Serializable]
    public struct AVSimlationData
    {
        public AVSimlationType AVType;
        public Boolean AVReady;
        public Double L;
        public Double W;
        public Double R;
        public Double R1;
        public Double R2;
        public Double R3;
        public Int32 AtomID;
    }

    // accessible volume simulations
    public class AVEngine
    {
        private Molecule mol;

        public AVEngine(Molecule m)
        {
            mol = m;
            AVGlobalParameters avdefault;
            avdefault.GridSize = 0.2;
            avdefault.LinkerInitialSphere = 0.5;
            avdefault.LinkSearchNodes = 3;
            avdefault.MinGridSize = 0.4;
            avdefault.ESamples = 200000;
            this._avGrid = avdefault;
        }

        public AVEngine(Molecule m, AVGlobalParameters avparam)
        {
            mol = m;
            this._avGrid = avparam;
        }

        #region AV in C#

        public static int AvCalculate1R(double L, double W, double R, int atom_i, double dg,
            double[] XLocal, double[] YLocal, double[] ZLocal,
            double[] vdWR, int NAtoms, double vdWRMax,
            double linkersphere, int linknodes, byte[] density)
        {
            // grid
            Double x0 = XLocal[atom_i], y0 = YLocal[atom_i], z0 = ZLocal[atom_i];
            Int32 npm = (Int32)Math.Floor(L / dg);
            Int32 ng = 2 * npm + 1, n;
            Int32 ng3 = ng * ng * ng;
            Double[] xg = new Double[ng];
            Double[] yg = new Double[ng];
            Double[] zg = new Double[ng];
            for (Int32 i = -npm; i <= npm; i++)
            {
                n = i + npm;
                xg[n] = i * dg; yg[n] = i * dg; zg[n] = i * dg;
            }

            // select atoms potentially within reach, excluding the attachment point
            Double rmaxsq = (L + R + vdWRMax) * (L + R + vdWRMax), rmax, r, rsq, dx, dy, dz;
            Int32[] atomindex = new Int32[NAtoms];
            Int32 natomsgrid;
            n = 0;
            for (Int32 i = 0; i < NAtoms; i++)
            {
                dx = XLocal[i] - x0; dy = YLocal[i] - y0; dz = ZLocal[i] - z0;
                rsq = dx * dx + dy * dy + dz * dz;
                if ((rsq < rmaxsq) && (i != atom_i)) atomindex[n++] = i;
            }
            natomsgrid = n;
            // local coordinates
            Double[] xa = new Double[natomsgrid];
            Double[] ya = new Double[natomsgrid];
            Double[] za = new Double[natomsgrid];
            Double[] vdWr = new Double[natomsgrid];
            for (Int32 i = 0; i < natomsgrid; i++)
            {
                n = atomindex[i]; vdWr[i] = vdWR[n];
                xa[i] = XLocal[n] - x0; ya[i] = YLocal[n] - y0; za[i] = ZLocal[n] - z0;
            }

            // search for allowed positions
            Byte[] clash = new Byte[ng3];
            for (Int32 i = 0; i < ng3; i++) clash[i] = 0;

            // search for positions causing clashes with atoms
            Int32 ix2, iy2, ir2, rmaxsqint;
            Double dx2, dy2, dr2, rmaxsq_dye, rmaxsq_linker;
            Int32 ixmin, ixmax, iymin, iymax, izmin, izmax, offset;
            for (Int32 i = 0; i < natomsgrid; i++)
            {
                rmaxsq_dye = (vdWr[i] + R) * (vdWr[i] + R);
                rmaxsq_linker = (vdWr[i] + 0.5 * W) * (vdWr[i] + 0.5 * W);
                rmax = vdWr[i] + Math.Max(R, 0.5 * W);
                rmaxsq = rmax * rmax;
                ixmin = Math.Max((Int32)Math.Ceiling((xa[i] - rmax) / dg), -npm);
                ixmax = Math.Min((Int32)Math.Floor((xa[i] + rmax) / dg), npm);
                iymin = Math.Max((Int32)Math.Ceiling((ya[i] - rmax) / dg), -npm);
                iymax = Math.Min((Int32)Math.Floor((ya[i] + rmax) / dg), npm);
                izmin = Math.Max((Int32)Math.Ceiling((za[i] - rmax) / dg), -npm);
                izmax = Math.Min((Int32)Math.Floor((za[i] + rmax) / dg), npm);

                for (Int32 ix = ixmin; ix <= ixmax; ix++)
                {
                    dx2 = (xg[ix + npm] - xa[i]) * (xg[ix + npm] - xa[i]);
                    dy = Math.Sqrt(Math.Max(rmaxsq - dx2, 0.0));
                    iymin = Math.Max((Int32)Math.Ceiling((ya[i] - dy) / dg), -npm);
                    iymax = Math.Min((Int32)Math.Floor((ya[i] + dy) / dg), npm);
                    offset = ng * (ng * (ix + npm) + iymin + npm) + npm;
                    for (Int32 iy = iymin; iy <= iymax; iy++)
                    {
                        dy2 = (yg[iy + npm] - ya[i]) * (yg[iy + npm] - ya[i]);
                        for (Int32 iz = izmin; iz <= izmax; iz++)
                        {
                            dr2 = dx2 + dy2 + (zg[iz + npm] - za[i]) * (zg[iz + npm] - za[i]);
                            clash[iz + offset] |= (Byte)(((dr2 <= rmaxsq_dye ? 1 : 0) << 1) | (dr2 <= rmaxsq_linker ? 1 : 0));
                        }
                        offset += ng;
                    }
                }
            }

            // route linker as a flexible pipe
            Double[] rlink = new Double[ng3];
            Double rlink0;
            Int32 ix0, iy0, iz0, linknodes_eff, dlz = 2 * linknodes + 1;
            Int32 nnew = 0;
            Int32[] newpos = new Int32[ng3];

            for (int i = 0; i < ng3; i++) rlink[i] = ((clash[i] & 0x01) == 1) ? -L : L + L;

            // (1) all positions within linkerinitialsphere*W from the attachment point are allowed
            rmaxsqint = (Int32)Math.Floor(linkersphere * linkersphere * W * W / dg / dg);
            ixmax = Math.Min((Int32)Math.Floor(linkersphere * W / dg), npm);
            n = 0;
            for (Int32 ix = -ixmax; ix <= ixmax; ix++)
            {
                ix2 = ix * ix;
                offset = ng * (ng * (ix + npm) - ixmax + npm) + npm;
                for (Int32 iy = -ixmax; iy <= ixmax; iy++)
                {
                    iy2 = iy * iy;
                    for (Int32 iz = -ixmax; iz <= ixmax; iz++)
                        if (ix2 + iy2 + iz * iz <= rmaxsqint)
                        {
                            rlink[iz + offset] = Math.Sqrt((Double)(ix2 + iy2 + iz * iz)) * dg;
                            newpos[nnew++] = ng * (ng * (npm + ix) + iy + npm) + npm + iz;
                        }
                    offset += ng;
                }
            }

            // (2) propagate from new positions
            Double[] sqrts_dg = new Double[(2 * linknodes * linknodes + 1) * (2 * linknodes + 1)];
            for (Int32 ix = 0; ix <= linknodes; ix++)
                for (Int32 iy = 0; iy <= linknodes; iy++)
                    for (Int32 iz = -linknodes; iz <= linknodes; iz++)
                        sqrts_dg[(ix * ix + iy * iy) * dlz + iz + linknodes] = Math.Sqrt((Double)(ix * ix + iy * iy + iz * iz)) * dg;
            while (nnew > 0)
            {
                for (n = 0; n < nnew; n++)
                {
                    rlink0 = rlink[newpos[n]];
                    linknodes_eff = Math.Min(linknodes, (Int32)Math.Floor((L - rlink0) / dg));
                    ix0 = newpos[n] / (ng * ng);
                    iy0 = newpos[n] / ng - ix0 * ng;
                    iz0 = newpos[n] - ix0 * ng * ng - iy0 * ng;
                    ixmin = Math.Max(-linknodes_eff, -ix0);
                    ixmax = Math.Min(linknodes_eff, 2 * npm - ix0);
                    iymin = Math.Max(-linknodes_eff, -iy0);
                    iymax = Math.Min(linknodes_eff, 2 * npm - iy0);
                    izmin = Math.Max(-linknodes_eff, -iz0);
                    izmax = Math.Min(linknodes_eff, 2 * npm - iz0);

                    for (Int32 ix = ixmin; ix <= ixmax; ix++)
                    {
                        offset = newpos[n] + ng * (ng * ix + iymin);
                        ix2 = ix * ix;
                        for (Int32 iy = iymin; iy <= iymax; iy++)
                        {
                            ir2 = (ix2 + iy * iy) * dlz + linknodes;
                            for (Int32 iz = izmin; iz <= izmax; iz++)
                            {
                                r = sqrts_dg[ir2 + iz] + rlink0;
                                if ((rlink[iz + offset] > r) && (r < L))
                                {
                                    rlink[iz + offset] = r;
                                    clash[iz + offset] |= 0x04;
                                }
                            }
                            offset += ng;
                        }
                    }
                }

                // update "new" positions
                nnew = 0;
                for (Int32 i = 0; i < ng3; i++)
                {
                    if ((clash[i] & 0x04) == 0x04) newpos[nnew++] = i;
                    clash[i] &= 0x03;
                }
            }

            // search for positions satisfying everything
            n = 0;
            for (Int32 i = 0; i < ng3; i++)
                if ((clash[i] == 0) && (rlink[i] <= L))
                {
                    density[i] = 1;
                    n++;
                }

            return n;
        }

        public static int AvCalculate3R(double L, double W, double R1, double R2, double R3, int atom_i, double dg,
            double[] XLocal, double[] YLocal, double[] ZLocal,
            double[] vdWR, int NAtoms, double vdWRMax,
            double linkersphere, int linknodes, byte[] density)
        {
            // grid
            Double Rmax = Math.Max(R1, R2); Rmax = Math.Max(Rmax, R3);
            Double x0 = XLocal[atom_i], y0 = YLocal[atom_i], z0 = ZLocal[atom_i];
            Int32 npm = (Int32)Math.Floor(L / dg);
            Int32 ng = 2 * npm + 1, n;
            Int32 ng3 = ng * ng * ng;
            Double[] xg = new Double[ng];
            Double[] yg = new Double[ng];
            Double[] zg = new Double[ng];
            for (Int32 i = -npm; i <= npm; i++)
            {
                n = i + npm;
                xg[n] = i * dg; yg[n] = i * dg; zg[n] = i * dg;
            }

            // select atoms potentially within reach, excluding the attachment point
            Double rmaxsq = (L + Rmax + vdWRMax) * (L + Rmax + vdWRMax), rmax, r, rsq, dx, dy, dz;
            Int32[] atomindex = new Int32[NAtoms];
            Int32 natomsgrid;
            n = 0;
            for (Int32 i = 0; i < NAtoms; i++)
            {
                dx = XLocal[i] - x0; dy = YLocal[i] - y0; dz = ZLocal[i] - z0;
                rsq = dx * dx + dy * dy + dz * dz;
                if ((rsq < rmaxsq) && (i != atom_i)) atomindex[n++] = i;
            }
            natomsgrid = n;
            // local coordinates
            Double[] xa = new Double[natomsgrid];
            Double[] ya = new Double[natomsgrid];
            Double[] za = new Double[natomsgrid];
            Double[] vdWr = new Double[natomsgrid];
            for (Int32 i = 0; i < natomsgrid; i++)
            {
                n = atomindex[i]; vdWr[i] = vdWR[n];
                xa[i] = XLocal[n] - x0; ya[i] = YLocal[n] - y0; za[i] = ZLocal[n] - z0;
            }

            // search for allowed positions
            Byte[] clash = new Byte[ng3];
            for (Int32 i = 0; i < ng3; i++) clash[i] = 0;

            // search for positions causing clashes with atoms
            Int32 ix2, iy2, ir2, rmaxsqint;
            Double dx2, dy2, dr2, rmaxsq_dye1, rmaxsq_dye2, rmaxsq_dye3, rmaxsq_linker;
            Int32 ixmin, ixmax, iymin, iymax, izmin, izmax, offset;
            for (Int32 i = 0; i < natomsgrid; i++)
            {
                rmaxsq_dye1 = (vdWr[i] + R1) * (vdWr[i] + R1);
                rmaxsq_dye2 = (vdWr[i] + R2) * (vdWr[i] + R2);
                rmaxsq_dye3 = (vdWr[i] + R3) * (vdWr[i] + R3);
                rmaxsq_linker = (vdWr[i] + 0.5 * W) * (vdWr[i] + 0.5 * W);
                rmax = vdWr[i] + Math.Max(Rmax, 0.5 * W);
                rmaxsq = rmax * rmax;
                ixmin = Math.Max((Int32)Math.Ceiling((xa[i] - rmax) / dg), -npm);
                ixmax = Math.Min((Int32)Math.Floor((xa[i] + rmax) / dg), npm);
                iymin = Math.Max((Int32)Math.Ceiling((ya[i] - rmax) / dg), -npm);
                iymax = Math.Min((Int32)Math.Floor((ya[i] + rmax) / dg), npm);
                izmin = Math.Max((Int32)Math.Ceiling((za[i] - rmax) / dg), -npm);
                izmax = Math.Min((Int32)Math.Floor((za[i] + rmax) / dg), npm);

                for (Int32 ix = ixmin; ix <= ixmax; ix++)
                {
                    dx2 = (xg[ix + npm] - xa[i]) * (xg[ix + npm] - xa[i]);
                    dy = Math.Sqrt(Math.Max(rmaxsq - dx2, 0.0));
                    iymin = Math.Max((Int32)Math.Ceiling((ya[i] - dy) / dg), -npm);
                    iymax = Math.Min((Int32)Math.Floor((ya[i] + dy) / dg), npm);
                    offset = ng * (ng * (ix + npm) + iymin + npm) + npm;
                    for (Int32 iy = iymin; iy <= iymax; iy++)
                    {
                        dy2 = (yg[iy + npm] - ya[i]) * (yg[iy + npm] - ya[i]);
                        for (Int32 iz = izmin; iz <= izmax; iz++)
                        {
                            dr2 = dx2 + dy2 + (zg[iz + npm] - za[i]) * (zg[iz + npm] - za[i]);
                            clash[iz + offset] |= (Byte)(((dr2 <= rmaxsq_dye3 ? 1 : 0) << 3) | ((dr2 <= rmaxsq_dye2 ? 1 : 0) << 2) |
                                ((dr2 <= rmaxsq_dye1 ? 1 : 0) << 1) | (dr2 <= rmaxsq_linker ? 1 : 0));
                        }
                        offset += ng;
                    }
                }
            }

            // route linker as a flexible pipe
            Double[] rlink = new Double[ng3];
            Double rlink0;
            Int32 ix0, iy0, iz0, linknodes_eff, dlz = 2 * linknodes + 1;
            Int32 nnew = 0;
            Int32[] newpos = new Int32[ng3];

            for (Int32 i = 0; i < ng3; i++) rlink[i] = (clash[i] & 0x01) == 1 ? -L : L + L;

            // (1) all positions within linkerinitialsphere*W from the attachment poInt32 are allowed
            rmaxsqint = (Int32)Math.Floor(linkersphere * linkersphere * W * W / dg / dg);
            ixmax = Math.Min((Int32)Math.Floor(linkersphere * W / dg), npm);
            n = 0;
            for (Int32 ix = -ixmax; ix <= ixmax; ix++)
            {
                ix2 = ix * ix;
                offset = ng * (ng * (ix + npm) - ixmax + npm) + npm;
                for (Int32 iy = -ixmax; iy <= ixmax; iy++)
                {
                    iy2 = iy * iy;
                    for (Int32 iz = -ixmax; iz <= ixmax; iz++)
                        if (ix2 + iy2 + iz * iz <= rmaxsqint)
                        {
                            rlink[iz + offset] = Math.Sqrt((Double)(ix2 + iy2 + iz * iz)) * dg;
                            newpos[nnew++] = ng * (ng * (npm + ix) + iy + npm) + npm + iz;
                        }
                    offset += ng;
                }
            }

            // (2) propagate from new positions
            Double[] sqrts_dg = new Double[(2 * linknodes * linknodes + 1) * (2 * linknodes + 1)];
            for (Int32 ix = 0; ix <= linknodes; ix++)
                for (Int32 iy = 0; iy <= linknodes; iy++)
                    for (Int32 iz = -linknodes; iz <= linknodes; iz++)
                        sqrts_dg[(ix * ix + iy * iy) * dlz + iz + linknodes] = Math.Sqrt((Double)(ix * ix + iy * iy + iz * iz)) * dg;
            while (nnew > 0)
            {
                for (n = 0; n < nnew; n++)
                {
                    rlink0 = rlink[newpos[n]];
                    linknodes_eff = Math.Min(linknodes, (Int32)Math.Floor((L - rlink0) / dg));
                    ix0 = newpos[n] / (ng * ng);
                    iy0 = newpos[n] / ng - ix0 * ng;
                    iz0 = newpos[n] - ix0 * ng * ng - iy0 * ng;
                    ixmin = Math.Max(-linknodes_eff, -ix0);
                    ixmax = Math.Min(linknodes_eff, 2 * npm - ix0);
                    iymin = Math.Max(-linknodes_eff, -iy0);
                    iymax = Math.Min(linknodes_eff, 2 * npm - iy0);
                    izmin = Math.Max(-linknodes_eff, -iz0);
                    izmax = Math.Min(linknodes_eff, 2 * npm - iz0);

                    for (Int32 ix = ixmin; ix <= ixmax; ix++)
                    {
                        offset = newpos[n] + ng * (ng * ix + iymin);
                        ix2 = ix * ix;
                        for (Int32 iy = iymin; iy <= iymax; iy++)
                        {
                            ir2 = (ix2 + iy * iy) * dlz + linknodes;
                            for (Int32 iz = izmin; iz <= izmax; iz++)
                            {
                                r = sqrts_dg[ir2 + iz] + rlink0;
                                if ((rlink[iz + offset] > r) && (r < L))
                                {
                                    rlink[iz + offset] = r;
                                    clash[iz + offset] |= 0x10;
                                }
                            }
                            offset += ng;
                        }
                    }
                }

                // update "new" positions
                nnew = 0;
                for (Int32 i = 0; i < ng3; i++)
                {
                    if ((clash[i] & 0x10) == 0x10) newpos[nnew++] = i;
                    clash[i] &= 0x0F;
                }
            }

            // search for positions satisfying everything
            n = 0; Int32 dn = 0;
            for (Int32 i = 0; i < ng3; i++)
            {
                if ((clash[i] & 0x01) == 1 || rlink[i] > L) continue;
                dn = ((~clash[i] & 0x08) >> 3) + ((~clash[i] & 0x04) >> 2) + ((~clash[i] & 0x02) >> 1);
                density[i] = (Byte)dn;
                n += dn;
            }
            return n;
        }

        private const int RMEAN_MINSAMPLE = 4;
        private const int RMEAN_MAXSAMPLE = 64;

        public static double RdaMeanFromAv(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
            int nsamples, int rndseed)
        {
            MersenneTwister rmt = new MersenneTwister(rndseed);
            Double dx, dy, dz, r = 0.0;
            Int32 i1, i2;
            Int32 jmax = Math.Min(RMEAN_MAXSAMPLE, RMEAN_MINSAMPLE + Math.Min(av1length, av2length) / 100), imax = nsamples / jmax;
            for (Int32 i = 0; i < imax; i++)
            {
                i1 = (Int32)(rmt.NextDouble(false) * (av1length - jmax + 1));
                i2 = (Int32)(rmt.NextDouble(false) * (av2length - jmax + 1));
                for (Int32 j = 0; j < jmax; j++)
                {
                    dx = av1[i1 + j].X - av2[i2 + j].X;
                    dy = av1[i1 + j].Y - av2[i2 + j].Y;
                    dz = av1[i1 + j].Z - av2[i2 + j].Z;
                    r += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                }
            }
            return r / ((Double)(imax * jmax));
        }

        public static double RdaMeanEFromAv(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
            int nsamples, int rndseed, double R0)
        {
            MersenneTwister rmt = new MersenneTwister(rndseed);
            Double dx, dy, dz, r2, e = 0.0, R0r6 = 1.0 / (R0 * R0 * R0 * R0 * R0 * R0);
            Int32 i1, i2;
            Int32 jmax = Math.Min(RMEAN_MAXSAMPLE, RMEAN_MINSAMPLE + Math.Min(av1length, av2length) / 100), imax = nsamples / jmax;
            for (Int32 i = 0; i < imax; i++)
            {
                i1 = (Int32)(rmt.NextDouble(false) * (av1length - jmax + 1));
                i2 = (Int32)(rmt.NextDouble(false) * (av2length - jmax + 1));
                for (Int32 j = 0; j < jmax; j++)
                {
                    dx = av1[i1 + j].X - av2[i2 + j].X;
                    dy = av1[i1 + j].Y - av2[i2 + j].Y;
                    dz = av1[i1 + j].Z - av2[i2 + j].Z;
                    r2 = dx * dx + dy * dy + dz * dz;
                    e += 1.0 / (1.0 + r2 * r2 * r2 * R0r6);
                }
            }
            e /= (Double)(imax * jmax);
            return R0 * Math.Pow((1.0 / e - 1.0), 1.0 / 6.0);
        }

        #endregion

        private Vector3[] _r;
        public Vector3[] R
        {
            get { return _r; }
        }

        private Vector3 _rmp;
        public Vector3 Rmp
        {
            get { return _rmp; }
        }
        private Byte[] _density;
        public Byte[] Density
        {
            get
            {
                Byte[] _density8 = new Byte[_density.Length];
                Array.Copy(_density, _density8, _density.Length);
                return _density8;
            }
        }
        /// <summary>
        /// grid parameters
        /// </summary>
        private Grid _xGrid;
        public Grid XGrid
        {
            get { return _xGrid; }
        }
        private Grid _yGrid;
        public Grid YGrid
        {
            get { return _yGrid; }
        }
        private Grid _zGrid;
        public Grid ZGrid
        {
            get { return _zGrid; }
        }
        private AVGlobalParameters _avGrid;
        public AVGlobalParameters AVGridPapameters
        {
            get { return _avGrid; }
            set { _avGrid = value; }
        }

        /// <summary>
        /// Calculates the accesible volume for a given labeling position.
        /// </summary>
        /// <param name="lp">Labeling position</param>
        public void Calculate(LabelingPosition lp)
        {
            if (lp.AVData.AVType == AVSimlationType.SingleDyeR)
            {
                this.Calculate1R(lp.AVData.L, lp.AVData.W, lp.AVData.R,
                       Array.BinarySearch<Int32>(this.mol.OriginalAtomID, lp.AVData.AtomID));
            }
            else if (lp.AVData.AVType == AVSimlationType.ThreeDyeR)
            {
                this.Calculate3R(lp.AVData.L, lp.AVData.W, lp.AVData.R1, lp.AVData.R2, lp.AVData.R3,
                       Array.BinarySearch<Int32>(this.mol.OriginalAtomID, lp.AVData.AtomID));
            }
            else throw new ArgumentException("Cannot calculate AV for a labeling position of type " + lp.AVData.AVType.ToString());
        }

        /// <summary>
        /// Calculate the accesible volume.
        /// </summary>
        /// <param name="L">Linker length</param>
        /// <param name="W">Linker width (diameter)</param>
        /// <param name="R">Dye radius</param>
        /// <param name="atom_i">Atom number in xyz arrays (not the original number in the pdb file)</param>
        public void Calculate1R(Double L, Double W, Double R, Int32 atom_i)
        {

            // grid
            Double dg1 = Math.Min(L * _avGrid.GridSize, W * _avGrid.GridSize);
            Double dg2 = Math.Min(R * 2.0 * _avGrid.GridSize, dg1);
            Double dg = Math.Max(dg2, _avGrid.MinGridSize);

            Double x0 = mol.XLocal[atom_i], y0 = mol.YLocal[atom_i], z0 = mol.ZLocal[atom_i];
            Double dx, dy, dz;
            Int32 npm = (Int32)Math.Floor(L / dg);
            Int32 ng = 2 * npm + 1, ng3 = ng * ng * ng, n;
            Int32 offset;
            Double[] xg = new Double[ng];
            Double[] yg = new Double[ng];
            Double[] zg = new Double[ng];
            for (Int32 i = -npm; i <= npm; i++)
            {
                n = i + npm;
                xg[n] = i * dg; yg[n] = i * dg; zg[n] = i * dg;
            }

            _density = new Byte[ng3];

            n = FpsNativeWrapper.AvCalculate1R(L, W, R, atom_i, dg, mol.XLocal, mol.YLocal, mol.ZLocal, mol.vdWR, mol.NAtoms,
                AtomData.vdWRMax, _avGrid.LinkerInitialSphere, _avGrid.LinkSearchNodes, _density);

            // saves
            _r = new Vector3[n];
            Double rn = 1.0 / (Double)n;
            n = 0; dx = 0.0; dy = 0.0; dz = 0.0;
            x0 += mol.CM.X; y0 += mol.CM.Y; z0 += mol.CM.Z;
            for (Int32 ix = -npm; ix <= npm; ix++)
            {
                offset = ng * (ng * (ix + npm)) + npm;
                for (Int32 iy = -npm; iy <= npm; iy++)
                {
                    for (Int32 iz = -npm; iz <= npm; iz++)
                        if (_density[iz + offset] > 0)
                        {
                            _r[n].X = xg[ix + npm] + x0; dx += _r[n].X;
                            _r[n].Y = yg[iy + npm] + y0; dy += _r[n].Y;
                            _r[n].Z = zg[iz + npm] + z0; dz += _r[n++].Z;
                        }
                    offset += ng;
                }
            }

            // other saves
            _rmp.X = dx * rn; _rmp.Y = dy * rn; _rmp.Z = dz * rn;
            _xGrid.Min = xg[0] + x0; _yGrid.Min = yg[0] + y0; _zGrid.Min = zg[0] + z0;
            _xGrid.NNodes = ng; _yGrid.NNodes = ng; _zGrid.NNodes = ng;
            _xGrid.Step = dg; _yGrid.Step = dg; _zGrid.Step = dg;
        }

        /// <summary>
        /// Calculate the accesible volume with 3 dye radii
        /// </summary>
        /// <param name="L">Linker length</param>
        /// <param name="W">Linker width (diameter)</param>
        /// <param name="R1">Dye radius 1</param>
        /// <param name="R2">Dye radius 2</param>
        /// <param name="R3">Dye radius 3</param>
        /// <param name="atom_i">Atom ID (original number in the pdb file)</param>
        public void Calculate3R(Double L, Double W, Double R1, Double R2, Double R3, Int32 atom_i)
        {

            // common grid
            Double dg1 = Math.Min(L * _avGrid.GridSize, W * _avGrid.GridSize);
            Double dg2 = Math.Min(R1 * 2.0 * _avGrid.GridSize, dg1);
            dg1 = Math.Min(R2 * 2.0 * _avGrid.GridSize, dg2);
            dg2 = Math.Min(R3 * 2.0 * _avGrid.GridSize, dg1);
            Double dg = Math.Max(dg2, _avGrid.MinGridSize);
            Double Rmax = Math.Max(R1, R2); Rmax = Math.Max(Rmax, R3);

            Double x0 = mol.XLocal[atom_i], y0 = mol.YLocal[atom_i], z0 = mol.ZLocal[atom_i];
            Double dx, dy, dz;
            Int32 npm = (Int32)Math.Floor(L / dg);
            Int32 ng = 2 * npm + 1, ng3 = ng * ng * ng, n;
            Int32 offset;
            Double[] xg = new Double[ng];
            Double[] yg = new Double[ng];
            Double[] zg = new Double[ng];
            for (Int32 i = -npm; i <= npm; i++)
            {
                n = i + npm;
                xg[n] = i * dg; yg[n] = i * dg; zg[n] = i * dg;
            }

            _density = new Byte[ng3];

            n = FpsNativeWrapper.AvCalculate3R(L, W, R1, R2, R3, atom_i, dg, mol.XLocal, mol.YLocal, mol.ZLocal, mol.vdWR, mol.NAtoms,
                AtomData.vdWRMax, _avGrid.LinkerInitialSphere, _avGrid.LinkSearchNodes, _density);

            _r = new Vector3[n];
            Double rn = 1.0 / (Double)n;
            n = 0; dx = 0.0; dy = 0.0; dz = 0.0;
            x0 += mol.CM.X; y0 += mol.CM.Y; z0 += mol.CM.Z;
            for (Int32 ix = -npm; ix <= npm; ix++)
            {
                offset = ng * (ng * (ix + npm)) + npm;
                for (Int32 iy = -npm; iy <= npm; iy++)
                {
                    for (Int32 iz = -npm; iz <= npm; iz++)
                    {
                        for (Int32 dn = 0; dn < _density[iz + offset]; dn++)
                        {
                            _r[n].X = xg[ix + npm] + x0; dx += _r[n].X;
                            _r[n].Y = yg[iy + npm] + y0; dy += _r[n].Y;
                            _r[n].Z = zg[iz + npm] + z0; dz += _r[n++].Z;
                        }
                    }
                    offset += ng;
                }
            }

            // other saves
            _rmp.X = dx * rn; _rmp.Y = dy * rn; _rmp.Z = dz * rn;
            _xGrid.Min = xg[0] + x0; _yGrid.Min = yg[0] + y0; _zGrid.Min = zg[0] + z0;
            _xGrid.NNodes = ng; _yGrid.NNodes = ng; _zGrid.NNodes = ng;
            _xGrid.Step = dg; _yGrid.Step = dg; _zGrid.Step = dg;
        }
    }
}
