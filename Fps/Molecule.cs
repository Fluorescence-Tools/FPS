using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Fps
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AtomCluster
    {
        public int NAtoms;
        public int StartIndexInClusterArrays;
        public Vector3 ClusterCenter;
        public double ClusterRadius;

        public static Vector3 operator -(AtomCluster c1, AtomCluster c2)
        {
            return c1.ClusterCenter - c2.ClusterCenter;
        }
        public static implicit operator Vector3(AtomCluster c)
        {
            return c.ClusterCenter;
        }
    }

    public class Molecule : IDisposable
    {
        public Molecule(string filename)
        {
            this.ReadPDB(filename);
        }

        /// <summary>
        /// Combines units into one "molecule" (mainly to be able to redo AV). Only a few properties are copied
        /// </summary>
        /// <param name="sr">Structure</param>
        public Molecule(SimulationResult sr)
        {
            MoleculeList units = sr.Molecules;
            Prepared = false;
            Name = "Structure " + sr.InternalNumber.ToString();
            FullFileName = "";
            Error = "";

            NAtoms = 0;
            for (int i = 0; i < units.Count; i++) NAtoms += units[i].NAtoms;

            AtomMass = new double[NAtoms];
            vdWR = new double[NAtoms];

            OriginalAtomID = new int[NAtoms];
            Atoms = new string[NAtoms];
            AtomsStandard = new string[NAtoms];
            Mass = 0.0;
            CM = new Vector3();
            Molecule m, m0 = units[0];
            int n = 0;
            Vector3 r;
            // local coordinates
            XLocal = new double[NAtoms];
            YLocal = new double[NAtoms];
            ZLocal = new double[NAtoms];
            
            for (int i = 0; i < units.Count; i++)
            {
                m = units[i];
                for (int j = 0; j < m.NAtoms; j++)
                {
                    r = sr.Rotation[i] * (new Vector3(m.XLocal[j], m.YLocal[j], m.ZLocal[j])) 
                        - m0.CM + m.CM + sr.Translation[i];
                    XLocal[n] = r.X;
                    YLocal[n] = r.Y;
                    ZLocal[n] = r.Z;
                    AtomMass[n] = m.AtomMass[j];
                    vdWR[n] = m.vdWR[j];
                    OriginalAtomID[n] = m.OriginalAtomID[j];
                    Atoms[n] = m.Atoms[j];
                    AtomsStandard[n] = m.AtomsStandard[j];
                    n++;
                }
            }
        }

        /// <summary>
        /// Molecule name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full path to the PDB file
        /// </summary>
        public string FullFileName { get; set; }

        public int NAtoms { get; private set; }
        public string[] Atoms { get; private set; }
        public string[] AtomsStandard { get; private set; }

        private double[] _xOriginal;
        private double[] _yOriginal;
        private double[] _zOriginal;

        // with respect to the c.o.m, same order as originally
        public double[] XLocal { get; private set; }
        public double[] YLocal { get; private set; }
        public double[] ZLocal { get; private set; }

        public double[] AtomMass { get; private set; }
        public double[] vdWR { get; private set; }

        public int[] OriginalAtomID { get; private set; }
        public int MaxOriginalAtomID { get; private set; }
        public string[] OriginalPDBLines { get; private set; }
        public string Error { get; private set; }
        public Boolean Selected { get; set; }
	
        // calculated parameters

        /// <summary>
        /// Center of mass
        /// </summary>
        public Vector3 CM { get; private set; }

        /// <summary>
        /// Molecular mass
        /// </summary>
        public double Mass { get; private set; }

        /// <summary>
        /// Simplified moment of inertia (max of ixx, iyy, izz; assumed to be the same for all axes)
        /// </summary>
        public double SimpleI { get; private set; }

        /// <summary>
        /// More realistic moment of inertia
        /// </summary>
        public Matrix3 FullI { get; private set; }

        /// <summary>
        /// Max. radius of projections to YZ, XZ, and XY planes
        /// </summary>
        public Vector3 MaxRadius { get; private set; }

        // clusters
        public AtomCluster[] AtomClusters { get; private set; }
        public int MaxAtomsInCluster { get; private set; }
        public Vector3[] ClusteredAtomXYZ { get; private set; }
        public double[] ClusteredAtomvdwR { get; private set; }
        public int[] ClusteredAtomOriginalIndex { get; private set; }

        /// <summary>
        /// Packed and clustered (float x, float y, float z, float vdwR) for native code
        /// </summary>
        public IntPtr XYZvdwRVectorArray { get; private set; }

        /// <summary>
        /// Determines whether all additional parameters have been caclulated
        /// </summary>
        public bool Prepared { get; private set; }



        /// <summary>
        /// Read PDB file
        /// </summary>
        /// <param name="filename">PDB file name</param>
        private void ReadPDB(string filename)
        {
            Prepared = false;
            FullFileName = filename;
            string[] strdata;
            try
            {
                strdata = File.ReadAllLines(filename);
            }
            catch (Exception e)
            {
                Error = e.Message;
                return;
            }
            this.OriginalPDBLines = strdata;

            _xOriginal = new double[strdata.Length];
            _yOriginal = new double[strdata.Length];
            _zOriginal = new double[strdata.Length];
            AtomMass = new double[strdata.Length];
            vdWR = new double[strdata.Length];

            OriginalAtomID = new int[strdata.Length];
            Atoms = new string[strdata.Length];
            AtomsStandard = new string[strdata.Length];

            Mass = 0.0;
            Vector3 _cm = new Vector3();

            string[] tmpstr;
            Char[] separator = new Char[] {' '};
            double x, y, z, m;
            int n = 0, nmax = -1;
            Name = Path.GetFileNameWithoutExtension(filename);
            for (int i = 0; i < strdata.Length; i++)
            {
                if (strdata[i].Length > 54 && strdata[i].Substring(0, 4) == "ATOM")
                {
                    tmpstr = strdata[i].Split(separator, 4, StringSplitOptions.RemoveEmptyEntries);
                    OriginalAtomID[n] = int.Parse(tmpstr[1]);
                    Atoms[n] = tmpstr[2];
                    if (nmax < OriginalAtomID[n]) nmax = OriginalAtomID[n];
                    AtomData.AtomParameters(tmpstr[2], out AtomsStandard[n], out m, out vdWR[n]);
                    if (double.TryParse(strdata[i].Substring(30, 8), out x) &&
                        double.TryParse(strdata[i].Substring(38, 8), out y) && 
                        double.TryParse(strdata[i].Substring(46, 8), out z))
                    {
                        _xOriginal[n] = x; _yOriginal[n] = y; _zOriginal[n] = z;
                        AtomMass[n] = m;
                        Mass += m;
                        _cm.X += x * m; _cm.Y += y * m; _cm.Z += z * m;
                        n++;                    
                    }
                }
            }
            NAtoms = n;
            _cm.X /= Mass; _cm.Y /= Mass; _cm.Z /= Mass;
            CM = _cm;
            MaxOriginalAtomID = nmax;

            Array.Resize<double>(ref _xOriginal, n);
            Array.Resize<double>(ref _yOriginal, n);
            Array.Resize<double>(ref _zOriginal, n);
            int[] _originalAtomID = OriginalAtomID;
            Array.Resize<int>(ref _originalAtomID, n);
            OriginalAtomID = _originalAtomID;

            // local coordinates
            XLocal = new double[NAtoms];
            YLocal = new double[NAtoms];
            ZLocal = new double[NAtoms];
            for (int i = 0; i < NAtoms; i++)
            {
                XLocal[i] = _xOriginal[i] - CM.X;
                YLocal[i] = _yOriginal[i] - CM.Y;
                ZLocal[i] = _zOriginal[i] - CM.Z;
            }

            if (NAtoms == 0) Error = "Unknown file format";
            else Error = "";           
        }

        /// <summary>
        /// Calculate some additional parameters needed for simulations
        /// </summary>
        public void Prepare()
        {
            if (Error != "") return;

            // min/max values of local coordinates
            double xmin = 1.0e6, xmax = -1.0e6, ymin = 1.0e6, ymax = -1.0e6, zmin = 1.0e6, zmax = -1.0e6;
            for (int i = 0; i < NAtoms; i++)
            {
                xmin = Math.Min(xmin, XLocal[i]);
                xmax = Math.Max(xmax, XLocal[i]);
                ymin = Math.Min(ymin, YLocal[i]);
                ymax = Math.Max(ymax, YLocal[i]);
                zmin = Math.Min(zmin, ZLocal[i]);
                zmax = Math.Max(zmax, ZLocal[i]);
                MaxRadius = new Vector3(Math.Max(MaxRadius.X, Math.Sqrt(YLocal[i] * YLocal[i] + ZLocal[i] * ZLocal[i]) + vdWR[i]),
                            Math.Max(MaxRadius.Y, Math.Sqrt(XLocal[i] * XLocal[i] + ZLocal[i] * ZLocal[i]) + vdWR[i]),
                            Math.Max(MaxRadius.Z, Math.Sqrt(XLocal[i] * XLocal[i] + YLocal[i] * YLocal[i]) + vdWR[i]));
            }

            // moment(s) of inertia
            double ixx = 0.0, iyy = 0.0, izz = 0.0, ixy = 0.0, ixz = 0.0, iyz = 0.0;
            double m = 0.0, x2, y2, z2;
            for (int i = 0; i < NAtoms; i++)
            {
                m = AtomMass[i];
                x2 = XLocal[i] * XLocal[i]; y2 = YLocal[i] * YLocal[i]; z2 = ZLocal[i] * ZLocal[i];
                ixx += m * (y2 + z2);
                iyy += m * (x2 + z2);
                izz += m * (x2 + y2);
                ixy -= m * XLocal[i] * YLocal[i];
                ixz -= m * XLocal[i] * ZLocal[i];
                iyz -= m * YLocal[i] * ZLocal[i];
            }
            SimpleI = Math.Max(Math.Max(ixx, ixy), ixz);
            FullI = new Matrix3(ixx, ixy, ixz, ixy, iyy, iyz, ixz, iyz, izz);

            // clusters of clustersize^3 angstrom
            const double clustersize = 10.0;
            int nx, ny, nz, nxoff, nyoff, nzoff;
            nx = (int)(Math.Ceiling(xmax / clustersize) + Math.Ceiling(-xmin / clustersize));
            ny = (int)(Math.Ceiling(ymax / clustersize) + Math.Ceiling(-ymin / clustersize));
            nz = (int)(Math.Ceiling(zmax / clustersize) + Math.Ceiling(-zmin / clustersize));
            nxoff = (int)Math.Ceiling(-xmin / clustersize);
            nyoff = (int)Math.Ceiling(-ymin / clustersize);
            nzoff = (int)Math.Ceiling(-zmin / clustersize);
            int nclusters = nx * ny * nz;
            AtomCluster[] atomcluster_tmp = new AtomCluster[nx * ny * nz];

            // sort atoms
            // first run: count atoms in each cluster
            int nc, cNAtomsmax = 0;
            double rcsize = 1.0 / clustersize;
            for (int i = 0; i < NAtoms; i++)
            {
                nc = (int)(ZLocal[i] * rcsize + nzoff) + nz * ((int)(YLocal[i] * rcsize + nyoff)
                    + ny * ((int)(XLocal[i] * rcsize + nxoff)));
                atomcluster_tmp[nc].NAtoms++;
                cNAtomsmax = Math.Max(cNAtomsmax, atomcluster_tmp[nc].NAtoms);
            }
            // allocate index arrays
            int ncfull = 0;
            int[][] originalatomindex = new int[nclusters][];
            for (nc = 0; nc < nclusters; nc++)
                if (atomcluster_tmp[nc].NAtoms > 0)
                {
                    originalatomindex[nc] = new int[atomcluster_tmp[nc].NAtoms];
                    ncfull++;
                }

            // fill
            int[] atomcounters = new int[nclusters];
            for (int i = 0; i < NAtoms; i++)
            {
                nc = (int)(ZLocal[i] * rcsize + nzoff) + nz * ((int)(YLocal[i] * rcsize + nyoff)
                    + ny * ((int)(XLocal[i] * rcsize + nxoff)));
                originalatomindex[nc][atomcounters[nc]++] = i;
            }
            // remove empty
            AtomClusters = new AtomCluster[ncfull];
            ncfull = 0;
            for (nc = 0; nc < nclusters; nc++)
                if (atomcluster_tmp[nc].NAtoms > 0)
                {
                    originalatomindex[ncfull] = originalatomindex[nc];
                    AtomClusters[ncfull++] = atomcluster_tmp[nc];
                }
            // optimize each
            double rmax, cx, cy, cz;
            int ilocal;
            Vector3 rlocal;
            for (nc = 0; nc < AtomClusters.Length; nc++)
            {
                // get the center
                cx = 0.0; cy = 0.0; cz = 0.0;
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                {
                    ilocal = originalatomindex[nc][i];
                    cx += XLocal[ilocal]; cy += YLocal[ilocal]; cz += ZLocal[ilocal];
                }
                cx /= (double)AtomClusters[nc].NAtoms;
                cy /= (double)AtomClusters[nc].NAtoms;
                cz /= (double)AtomClusters[nc].NAtoms;
                AtomClusters[nc].ClusterCenter = new Vector3(cx, cy, cz);

                // find the most distant atom
                rmax = -1.0;
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                {
                    ilocal = originalatomindex[nc][i];
                    rlocal = new Vector3(cx - XLocal[ilocal], cy - YLocal[ilocal], cz - ZLocal[ilocal]);
                    rmax = Math.Max(Vector3.Abs(rlocal) + vdWR[ilocal], rmax);
                }
                AtomClusters[nc].ClusterRadius = rmax;
            }
            // try an alternative method : find a pair of most distant atoms for each dimension, try as a center
            int jlocal;
            double absr;
            for (nc = 0; nc < AtomClusters.Length; nc++)
            {
                rmax = -1.0;
                rlocal = AtomClusters[nc];
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                {
                    ilocal = originalatomindex[nc][i];
                    cx = XLocal[ilocal]; cy = YLocal[ilocal]; cz = ZLocal[ilocal];
                    for (int j = i + 1; j < AtomClusters[nc].NAtoms; j++)
                    {
                        jlocal = originalatomindex[nc][j];
                        rlocal = new Vector3(XLocal[jlocal] - cx, YLocal[jlocal] - cy, ZLocal[jlocal] - cz);
                        absr = Vector3.Abs(rlocal);
                        if (rmax < absr + vdWR[ilocal] + vdWR[jlocal])
                        {
                            rmax = absr + vdWR[ilocal] + vdWR[jlocal];
                            rlocal = (absr == 0.0) ? new Vector3(cx, cy, cz) :
                                 new Vector3(cx, cy, cz) + rlocal * ((0.5 * rmax - vdWR[ilocal]) / absr);
                        }
                    }
                }
                // find the most distant atom
                rmax = -1.0;
                cx = rlocal.X; cy = rlocal.Y; cz = rlocal.Z;
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                {
                    ilocal = originalatomindex[nc][i];
                    rlocal = new Vector3(cx - XLocal[ilocal], cy - YLocal[ilocal], cz - ZLocal[ilocal]);
                    rmax = Math.Max(Vector3.Abs(rlocal) + vdWR[ilocal], rmax);
                }
                if (rmax < AtomClusters[nc].ClusterRadius)
                {
                    AtomClusters[nc].ClusterCenter = new Vector3(cx, cy, cz);
                    AtomClusters[nc].ClusterRadius = rmax;
                }
            }

#if DEBUG
            if (DebugData.SaveAtomClusters)
            {
                // for debugging: save clusters xyz
                using (StreamWriter sw = new StreamWriter(this.Name + ".cluster.xyz", false))
                {
                    sw.WriteLine(AtomClusters.Length.ToString());
                    sw.WriteLine("cluster");
                    for (int i = 0; i < AtomClusters.Length; i++)
                        sw.WriteLine("L" + i.ToString() + "  " + AtomClusters[i].ClusterCenter.X.ToString("F3") + "  "
                            + AtomClusters[i].ClusterCenter.Y.ToString("F3") + "  " + AtomClusters[i].ClusterCenter.Z.ToString("F3"));
                    sw.Close();
                }
                using (StreamWriter sw = new StreamWriter(this.Name + ".cluster.pml", false))
                {
                    sw.WriteLine("select all");
                    sw.WriteLine("translate [" + CM.X.ToString("F3") + "," + CM.Y.ToString("F3")
                        + "," + CM.Z.ToString("F3") + "]");
                    sw.WriteLine("deselect");
                    for (int i = 0; i < AtomClusters.Length; i++)
                        sw.WriteLine("alter (name L" + i.ToString() + "), vdw=" + AtomClusters[i].ClusterRadius.ToString("F3"));
                    sw.Close();
                }
            }
#endif

            /////////////////////////////////////////////////////////////////////////////////////////

            // remove atoms which can never be approached
            int nnbr, nblocked = 0;
            double[] xLocalnbr = new double[NAtoms];
            double[] yLocalnbr = new double[NAtoms];
            double[] zLocalnbr = new double[NAtoms];
            double[] rthresh2 = new double[NAtoms];
            double[] z2plus = new double[NAtoms];
            double[] z2minus = new double[NAtoms];
            double x0, y0, z0, rmin = AtomData.vdWRMin;
            Boolean[] atom_blocked = new Boolean[NAtoms];
            Boolean accesible, zplus_blocked, zminus_blocked;

            double xg, yg, zg, rg, rxy, drmax, dtheta, dphi;

            for (int i = 0; i < NAtoms; i++)
            {
                accesible = false;
                x0 = XLocal[i]; y0 = YLocal[i]; z0 = ZLocal[i];
                rmax = 2.0 * rmin + vdWR[i];
                // select neighbours within r1 + r2 + 2rmin
                nnbr = 0;
                for (int j = 0; j < NAtoms; j++)
                {
                    x2 = (XLocal[j] - x0) * (XLocal[j] - x0);
                    y2 = (YLocal[j] - y0) * (YLocal[j] - y0);
                    z2 = (ZLocal[j] - z0) * (ZLocal[j] - z0);
                    if (x2 + y2 + z2 < (rmax + vdWR[j]) * (rmax + vdWR[j]))
                    {
                        xLocalnbr[nnbr] = XLocal[j] - x0;
                        yLocalnbr[nnbr] = YLocal[j] - y0;
                        zLocalnbr[nnbr] = ZLocal[j] - z0;
                        rthresh2[nnbr++] = (0.9 * rmin + vdWR[j]) * (0.9 * rmin + vdWR[j]);
                    }
                }
                // spherical grid
                rg = vdWR[i] + rmin;
                drmax = 0.1 * rmin / rg;
                nz = (int)Math.Ceiling(Math.PI / drmax);
                dtheta = Math.PI / nz;
                for (double theta = Math.PI / 2.0; theta >= 0.0; theta -= dtheta)
                {
                    zg = rg * Math.Sin(theta);
                    for (int j = 0; j < nnbr; j++)
                    {
                        z2plus[j] = (zLocalnbr[j] - zg) * (zLocalnbr[j] - zg);
                        z2minus[j] = (zLocalnbr[j] + zg) * (zLocalnbr[j] + zg);
                    }
                    rxy = rg * Math.Cos(theta);
                    dphi = (rxy > 0.05 * rmin / Math.PI) ? 0.1 * rmin / rxy : 2.0 * Math.PI;
                    for (double phi = 0.0; phi <= 2.0 * Math.PI; phi += dphi)
                    {
                        xg = rxy * Math.Cos(phi);
                        yg = rxy * Math.Sin(phi);

                        // check all neighbours at zg and -zg
                        zplus_blocked = false;
                        zminus_blocked = false;
                        for (int j = 0; j < nnbr; j++)
                        {
                            x2 = (xLocalnbr[j] - xg) * (xLocalnbr[j] - xg);
                            y2 = (yLocalnbr[j] - yg) * (yLocalnbr[j] - yg);
                            zplus_blocked = zplus_blocked || (x2 + y2 + z2plus[j] < rthresh2[j]);
                            zminus_blocked = zminus_blocked || (x2 + y2 + z2minus[j] < rthresh2[j]);
                            if (zplus_blocked && zminus_blocked) break;
                        }
                        accesible = accesible || (!zplus_blocked) || (!zminus_blocked);
                        if (accesible) break;
                    }
                    if (accesible) break;
                }
                atom_blocked[i] = !accesible;
                nblocked = accesible ? nblocked : nblocked + 1;
            }

            // finally remove blocked atoms and reorder
            int ai = 0, unblocked = NAtoms - nblocked;
            MaxAtomsInCluster = 0;
            for (nc = 0; nc < AtomClusters.Length; nc++)
            {
                // first run: count blocked atoms
                nblocked = 0; // now in cluster nc
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                    nblocked = atom_blocked[originalatomindex[nc][i]] ? nblocked + 1 : nblocked;

                // second run: fill 
                ai = 0;
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                {
                    ilocal = originalatomindex[nc][i];
                    if (!atom_blocked[ilocal]) originalatomindex[nc][ai++] = ilocal;
                }
                AtomClusters[nc].NAtoms -= nblocked;
                MaxAtomsInCluster = Math.Max(MaxAtomsInCluster, AtomClusters[nc].NAtoms);
            }

            // fill cluster arrays
            this.ClusteredAtomXYZ = new Vector3[unblocked];
            this.ClusteredAtomvdwR = new double[unblocked];
            this.ClusteredAtomOriginalIndex = new int[unblocked];
            if (XYZvdwRVectorArray != IntPtr.Zero) Marshal.FreeHGlobal(XYZvdwRVectorArray);
            XYZvdwRVectorArray = Marshal.AllocHGlobal((unblocked + 1) * sizeof(float) * 4);
            float[] XYZvdwRdata = new float[unblocked * 4];
            int idata = 0, idata4 = 0, oi;
            for (nc = 0; nc < AtomClusters.Length; nc++)
            {
                AtomClusters[nc].StartIndexInClusterArrays = idata;
                for (int i = 0; i < AtomClusters[nc].NAtoms; i++)
                {
                    oi = originalatomindex[nc][i];
                    ClusteredAtomXYZ[idata] = new Vector3(XLocal[oi], YLocal[oi], ZLocal[oi]);
                    ClusteredAtomvdwR[idata] = vdWR[oi];
                    ClusteredAtomOriginalIndex[idata++] = oi;
                    XYZvdwRdata[idata4++] = (float)XLocal[oi];
                    XYZvdwRdata[idata4++] = (float)YLocal[oi];
                    XYZvdwRdata[idata4++] = (float)ZLocal[oi];
                    XYZvdwRdata[idata4++] = (float)vdWR[oi];
                }
            }
            Marshal.Copy(XYZvdwRdata, 0, FpsNativeWrapper.Aligned16(XYZvdwRVectorArray), XYZvdwRdata.Length);
            Prepared = true;
        }

        #region Dispose

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                if (XYZvdwRVectorArray != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(XYZvdwRVectorArray);
                    XYZvdwRVectorArray = IntPtr.Zero;
                }

                disposed = true;
            }
        }

        ~Molecule()
        {
            Dispose(false);
        }

        #endregion
    }

    public class MoleculeList : List<Molecule>
    {
        public MoleculeList(int capacity)
        {
            this.Clear();
            this.Capacity = capacity;
        }
        public int FindIndex(string name)
        {
            return this.FindIndex(delegate(Molecule m) { return (m.Name == name); });
        }
        public int FindLastIndex(string name)
        {
            return this.FindLastIndex(delegate(Molecule m) { return (m.Name == name); });
        }
    }

}
