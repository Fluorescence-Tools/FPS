using System;
using System.Collections.Generic;
using System.IO;

namespace Fps
{
    public enum DyeType { Unknown = 0, Donor, Acceptor }

    [Serializable]
    public struct LabelingPosition
    {
        public String Name;
        public String Molecule;
        public Double X;
        public Double Y;
        public Double Z;
        public DyeType Dye;
        public AVSimlationData AVData;

        public static Vector3 operator -(LabelingPosition l1, LabelingPosition l2)
        {
            return new Vector3(l1.X - l2.X, l1.Y - l2.Y, l1.Z - l2.Z);
        }
        public static LabelingPosition operator -(LabelingPosition l, Vector3 v)
        {
            LabelingPosition l1 = l;
            l1.X = l.X - v.X;
            l1.Y = l.Y - v.Y;
            l1.Z = l.Z - v.Z;
            return l1;
        }
        public static implicit operator Vector3(LabelingPosition l)
        {
            return new Vector3(l.X, l.Y, l.Z);
        }
    }

    [Serializable]
    public class LabelingPositionList : List<LabelingPosition>
    {
        private String _fullPath;
        /// <summary>
        /// Full path to the data file
        /// </summary>
        public String FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        private String _error = "";
        public String Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public LabelingPositionList(String filename)
        {
            ReadLabelingPositions(filename);
        }

        public LabelingPositionList()
        {
            this.Clear();
            this.Capacity = 200;
        }

        public LabelingPositionList(int capacity)
        {
            this.Clear();
            this.Capacity = capacity;
        }

        /// <summary>
        /// reading LPs, old file format (not used at the moment)
        /// </summary>
        /// <param name="filename">Data file name</param>
        private void ReadLabelingPositionsOld(String filename)
        {
            // expected formats:
            // Name (tab) Molecule (tab) X (tab) Y (tab) Z (tab)
            // or:
            // Name (tab) Molecule (tab) D/A (tab) X (tab) Y (tab) Z (tab)
            this.Clear();
            this.Capacity = 200;

            String[] strdata;
            try
            {
                strdata = File.ReadAllLines(filename);
            }
            catch (Exception e)
            {
                _error = e.Message;
                return;
            }

            String[] tmpstr;
            Double x, y, z;
            Char[] separator = new Char[] { ' ', '\t' };
            for (Int32 i = 0; i < strdata.Length; i++)
            {
                tmpstr = strdata[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (tmpstr.Length < 5) continue;

                // format without D/A: try to parse #3, 4, 5 to double
                if (tmpstr.Length == 5 && Double.TryParse(tmpstr[2], out x) &&
                    Double.TryParse(tmpstr[3], out y) && Double.TryParse(tmpstr[4], out z))
                {
                    LabelingPosition l = new LabelingPosition();
                    l.Name = tmpstr[0];
                    l.Molecule = tmpstr[1];
                    l.Dye = DyeType.Unknown;
                    l.X = x; l.Y = y; l.Z = z;
                    l.AVData.AVType = AVSimlationType.None;
                    this.Add(l);
                }
                // format with D/A: try to parse #4, 5, 6 to double
                else if (tmpstr.Length == 6 && Double.TryParse(tmpstr[3], out x) &&
                    Double.TryParse(tmpstr[4], out y) && Double.TryParse(tmpstr[5], out z))
                {
                    LabelingPosition l = new LabelingPosition();
                    l.Name = tmpstr[0];
                    l.Molecule = tmpstr[1];
                    if (tmpstr[2].Equals("D", StringComparison.OrdinalIgnoreCase)) l.Dye = DyeType.Donor;
                    else if (tmpstr[2].Equals("A", StringComparison.OrdinalIgnoreCase)) l.Dye = DyeType.Acceptor;
                    else l.Dye = DyeType.Unknown;
                    l.X = x; l.Y = y; l.Z = z;
                    l.AVData.AVType = AVSimlationType.None;
                    this.Add(l);
                }
                if (this.Count == 0) _error = "Unknown file format";
                else _error = "";
                this._fullPath = filename;
            }
        }
        /// <summary>
        /// Read Labeling Positions data, new format
        /// </summary>
        /// <param name="filename">File name</param>
        private void ReadLabelingPositions(String filename)
        {
            // expected format:
            // Name (tab) Molecule (tab) D/A (tab) Type: [XYZ, AV1, AV3, AVS, EDF, ATOM] (tab) relevant data
            this.Clear();
            this.Capacity = 200;

            String[] strdata;
            try
            {
                strdata = File.ReadAllLines(filename);
            }
            catch (Exception e)
            {
                _error = e.Message;
                return;
            }

            String[] tmpstr;
            LabelingPosition l;
            Double data1, data2, data3, data4, data5; Int32 data6;
            Char[] separator = new Char[] { ' ', '\t' };

            for (Int32 i = 0; i < strdata.Length; i++)
            {
                tmpstr = strdata[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (tmpstr.Length < 5) continue;
                l = new LabelingPosition();
                l.Name = tmpstr[0];
                l.Molecule = tmpstr[1];
                if (tmpstr[2].Equals("D", StringComparison.OrdinalIgnoreCase)) l.Dye = DyeType.Donor;
                else if (tmpstr[2].Equals("A", StringComparison.OrdinalIgnoreCase)) l.Dye = DyeType.Acceptor;
                else l.Dye = DyeType.Unknown;

                if (tmpstr[3] == AVSimlationTypeShort.XYZ.ToString() && (tmpstr.Length >= 7))
                    if (Double.TryParse(tmpstr[4], out data1) && Double.TryParse(tmpstr[5], out data2) && Double.TryParse(tmpstr[6], out data3))
                    {
                        l.AVData.AVType = AVSimlationType.None; l.AVData.AVReady = true;
                        l.X = data1; l.Y = data2; l.Z = data3;
                    }
                    else continue;
                else if (tmpstr[3] == AVSimlationTypeShort.AV1.ToString() && (tmpstr.Length >= 8))
                    if (Double.TryParse(tmpstr[4], out data1) && Double.TryParse(tmpstr[5], out data2) && Double.TryParse(tmpstr[6], out data3)
                        && Int32.TryParse(tmpstr[7], out data6))
                    {
                        l.AVData.AVType = AVSimlationType.SingleDyeR; l.AVData.AVReady = false;
                        l.AVData.L = data1; l.AVData.W = data2; l.AVData.R = data3; l.AVData.AtomID = data6;
                    }
                    else continue;
                else if (tmpstr[3] == AVSimlationTypeShort.AV3.ToString() && (tmpstr.Length >= 10))
                    if (Double.TryParse(tmpstr[4], out data1) && Double.TryParse(tmpstr[5], out data2) && Double.TryParse(tmpstr[6], out data3)
                        && Double.TryParse(tmpstr[7], out data4) && Double.TryParse(tmpstr[8], out data5) && Int32.TryParse(tmpstr[9], out data6))
                    {
                        l.AVData.AVType = AVSimlationType.ThreeDyeR; l.AVData.AVReady = false;
                        l.AVData.L = data1; l.AVData.W = data2; l.AVData.R1 = data3;
                        l.AVData.R2 = data4; l.AVData.R3 = data5; l.AVData.AtomID = data6;
                    }
                    else continue;
                else if ((tmpstr[3] == "ATOM" && (tmpstr.Length >= 5)))
                    if (Int32.TryParse(tmpstr[4], out data6))
                    {
                        l.AVData.AVType = AVSimlationType.None; l.AVData.AVReady = false;
                        l.AVData.AtomID = data6;
                    }
                    else continue;
                else continue;

                this.Add(l);
            }
            if (this.Count == 0) _error = "Unknown file format";
            else _error = "";
            this._fullPath = filename;
        }

        /// <summary>
        /// Checks whether a string represents an LP data line
        /// </summary>
        /// <param name="s">String to check</param>
        /// <returns>Whether s contains valid LP data</returns>
        public static Boolean ValidLPLine(String s)
        {
            Char[] separator = new Char[] { ' ', '\t' };
            Double data1, data2, data3, data4, data5; Int32 data6;
            String[] tmpstr = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (tmpstr.Length < 7 || tmpstr[0] == "ATOM") return false; // for safety
            else return (tmpstr[3] == AVSimlationTypeShort.XYZ.ToString() && (tmpstr.Length >= 7) && Double.TryParse(tmpstr[4], out data1) &&
                Double.TryParse(tmpstr[5], out data2) && Double.TryParse(tmpstr[6], out data3)) ||
                (tmpstr[3] == AVSimlationTypeShort.AV1.ToString() && (tmpstr.Length >= 8) && Double.TryParse(tmpstr[4], out data1) &&
                Double.TryParse(tmpstr[5], out data2) && Double.TryParse(tmpstr[6], out data3) && Int32.TryParse(tmpstr[7], out data6)) ||
                (tmpstr[3] == AVSimlationTypeShort.AV3.ToString() && (tmpstr.Length >= 10) && Double.TryParse(tmpstr[4], out data1) && 
                Double.TryParse(tmpstr[5], out data2) && Double.TryParse(tmpstr[6], out data3) && Double.TryParse(tmpstr[7], out data4) && 
                Double.TryParse(tmpstr[8], out data5) && Int32.TryParse(tmpstr[9], out data6));
        }

        public Int32 FindIndex(String name)
        {
            return this.FindIndex(delegate(LabelingPosition l) { return (l.Name == name); });
        }
        public Int32 FindLastIndex(String name)
        {
            return this.FindLastIndex(delegate(LabelingPosition l) { return (l.Name == name); });
        }
        public LabelingPosition Find(String name)
        {
            return this.Find(delegate(LabelingPosition l) { return (l.Name == name); });
        }
    }

}
