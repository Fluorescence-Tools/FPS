using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace Fps
{
    /// <summary>
    /// Provides atom names, masses, and van der Waals radii
    /// </summary>
    static public class AtomData
    {
        static private SortedList<String, Double> massList;
        static private SortedList<String, Double> vdWRList;

        /// <summary>
        /// Min. van der Waals radius
        /// </summary>
        static public Double vdWRMax { get; private set; }

        /// <summary>
        /// max. van der Waals radius
        /// </summary>
        static public Double vdWRMin { get; private set; }

        /// <summary>
        /// van der Waals radius which replaces original vdwr for "ATOM" type LPs
        /// </summary>
        static public Double vdWRNoClash { get; private set; }

        static AtomData()
        {
            vdWRMax = 0.0; vdWRMin = 100.0; vdWRNoClash = 0.4;
            String thispath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            String[] strdata = File.ReadAllLines(thispath +
                Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "vdW.txt");
            massList = new SortedList<String, Double>(strdata.Length + 1);
            massList.Add("", 0.0);
            vdWRList = new SortedList<String, Double>(strdata.Length + 1);
            vdWRList.Add("", 0.0);
            String[] tmpstr;
            Double r = 0.0;
            for (Int32 i = 0; i < strdata.Length; i++)
            {
                tmpstr = strdata[i].Split('\t');
                massList.Add(tmpstr[1], Double.Parse(tmpstr[3]));
                r = Double.Parse(tmpstr[5]);
                vdWRList.Add(tmpstr[1], r);
                vdWRMin = (r < vdWRMin) ? r : vdWRMin;
                vdWRMax = (r > vdWRMax) ? r : vdWRMax;
            }
        }

        /// <summary>
        /// Adds data specific for each atom
        /// </summary>
        /// <param name="atomname"> Atom name from a pdb file </param>
        /// <param name="standardName"> Standard atom name </param>
        /// <param name="weight"> Mass </param>
        /// <param name="vdWR"> van der Waals radius </param>
        static public void AtomParameters(String atomname,
            out String standardName, out Double weight, out Double vdWR)
        {
            String c;
            if (String.IsNullOrEmpty(atomname)) c = "";
            else if (massList.ContainsKey(atomname)) c = atomname;
            else if ((atomname.Length > 1) && massList.ContainsKey(atomname.Substring(0, 2)))
                c = atomname.Substring(0, 2);
            else if ((atomname.Length > 2) && massList.ContainsKey(atomname.Substring(1, 2)))
                c = atomname.Substring(1, 2);
            else if (massList.ContainsKey(atomname.Substring(0, 1))) c = atomname.Substring(0, 1);
            else if (massList.ContainsKey(atomname.Substring(1, 1))) c = atomname.Substring(1, 1);
            else c = "";

            standardName = c;
            weight = massList[c];
            vdWR = vdWRList[c];
        }
    }

    /// <summary>
    /// Default dye and linker parameters
    /// </summary>
    static public class LinkerData
    {
        static public readonly String[] LinkerList;
        static public readonly String[] DorA;
        /// <summary>
        /// Length from the attachment point to the center of the dye
        /// </summary>
        static public readonly Double[] LList;
        /// <summary>
        /// Linker width (diameter)
        /// </summary>
        static public readonly Double[] WList;
        /// <summary>
        /// Dye radius for simple AV
        /// </summary>
        static public readonly Double[] RList;
        /// <summary>
        /// Dye radius 1 of 3
        /// </summary>
        static public readonly Double[] R1List;
        /// <summary>
        /// Dye radius 2 of 3
        /// </summary>
        static public readonly Double[] R2List;
        /// <summary>
        /// Dye radius 3 of 3
        /// </summary>
        static public readonly Double[] R3List;

        static LinkerData()
        {
            String thispath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            String[] strdata = File.ReadAllLines(thispath +
                Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "linker.txt");
            LinkerList = new String[strdata.Length];
            DorA = new String[strdata.Length];
            LList = new Double[strdata.Length];
            WList = new Double[strdata.Length];
            RList = new Double[strdata.Length];
            R1List = new Double[strdata.Length];
            R2List = new Double[strdata.Length];
            R3List = new Double[strdata.Length];
            String[] tmpstr;
            for (Int32 i = 0; i < strdata.Length; i++)
            {
                tmpstr = strdata[i].Split('\t');
                LinkerList[i] = tmpstr[0];
                DorA[i] = tmpstr[1];
                LList[i] = Double.Parse(tmpstr[2]);
                WList[i] = Double.Parse(tmpstr[3]);
                RList[i] = Double.Parse(tmpstr[4]);
                R1List[i] = Double.Parse(tmpstr[5]);
                R2List[i] = Double.Parse(tmpstr[6]);
                R3List[i] = Double.Parse(tmpstr[7]);
            }
        }
    }

    static public class DebugData
    {
        // parameters for debugging
        static public Boolean SaveAtomClusters { get; set; }
        static public Boolean SaveTrajectories { get; set; }
    }

    static public class MiscData
    {
        // parameters for debugging
        static private Double _ENotCalculated = -999.9;
        static public Double ENotCalculated
        {
            get { return _ENotCalculated; }
        }
    }
}