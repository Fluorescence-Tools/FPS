using System;
using System.Collections.Generic;
using System.IO;

namespace Fps
{
    public struct Distance
    {
        public String Position1;
        public String Position2;
        public Double R;
        public Double ErrPlus;
        public Double ErrMinus;
        public Boolean IsSelected;
        public Boolean IsBond;

        /// <summary>
        /// Convert Rmp - RDAE and so on using a polynomial conversion function
        /// </summary>
        /// <param name="convfun">Polynomial conversion function</param>
        /// <returns>Converted distance</returns>
        public Distance Convert(Double[] convfun)
        {
            Distance cd = this;
            Double e;
            cd.R = this.R * convfun[convfun.Length - 1] + convfun[convfun.Length - 2];
            for (Int32 i = convfun.Length - 3; i >= 0; i--) cd.R = this.R * cd.R + convfun[i];
            e = (this.R + this.ErrPlus) * convfun[convfun.Length - 1] + convfun[convfun.Length - 2];
            for (Int32 i = convfun.Length - 3; i >= 0; i--) e = (this.R + this.ErrPlus) * e + convfun[i];
            cd.ErrPlus = e - cd.R;
            e = (this.R - this.ErrMinus) * convfun[convfun.Length - 1] + convfun[convfun.Length - 2];
            for (Int32 i = convfun.Length - 3; i >= 0; i--) e = (this.R - this.ErrMinus) * e + convfun[i];
            cd.ErrMinus = cd.R - e;
            return cd;
        }
        public static Double Convert(Double R, Double[] convfun)
        {    
            Double Rc = R * convfun[convfun.Length - 1] + convfun[convfun.Length - 2];
            for (Int32 i = convfun.Length - 3; i >= 0; i--) Rc = R * Rc + convfun[i];
            return Rc;
        }
    }

    public enum DistanceDataType
    {
        Rmp, RDAMean, RDAMeanE
    }

    public class DistanceList : List<Distance>
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

        private DistanceDataType _datatype;
        /// <summary>
        /// Distance data type. Applied only if both LPs are D or A, otherwise Rmp is assumed
        /// </summary>
        public DistanceDataType DataType
        {
            get { return _datatype; }
            set { _datatype = value; }
        }
	
        private String _error = "";
        public String Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public DistanceList(String filename)
        {
            ReadDistances(filename);
        }
        public DistanceList()
        {
            this.Clear();
            this.Capacity = 500;
        }
        public DistanceList(Int32 capacity)
        {
            this.Clear();
            this.Capacity = capacity;
        }

        private void ReadDistances(String filename)
        {
            this.Clear();
            this.Capacity = 500;

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
            Double r, eplus, eminus;
            Char[] separator = new Char[] { ' ', '\t' };
            Int32 firstdataline = 1;
            // determine data type
            if (strdata[0].StartsWith(DistanceDataType.Rmp.ToString(), StringComparison.OrdinalIgnoreCase))
                _datatype = DistanceDataType.Rmp;
            else if (strdata[0].StartsWith(DistanceDataType.RDAMeanE.ToString(), StringComparison.OrdinalIgnoreCase))
                _datatype = DistanceDataType.RDAMeanE;
            else if (strdata[0].StartsWith(DistanceDataType.RDAMean.ToString(), StringComparison.OrdinalIgnoreCase))
                _datatype = DistanceDataType.RDAMean;
            else
            {
                _datatype = DistanceDataType.Rmp;
                firstdataline = 0;
            }
            // read values
            for (Int32 i = firstdataline; i < strdata.Length; i++)
            {
                tmpstr = strdata[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                // try to parse #3, 4, 5 to double
                if (tmpstr.Length < 5) continue;
                if (Double.TryParse(tmpstr[2], out r) &&
                    Double.TryParse(tmpstr[3], out eplus) && Double.TryParse(tmpstr[4], out eminus))
                {
                    Distance dis = new Distance();
                    dis.Position1 = tmpstr[0];
                    dis.Position2 = tmpstr[1];
                    dis.R = r; dis.ErrPlus = eplus; dis.ErrMinus = eminus;
                    dis.IsSelected = true;
                    this.Add(dis);
                }
                if (this.Count == 0) _error = "Unknown file format";
                else _error = "";
                this._fullPath = filename;
            }
        }
        public Distance Find(String pos1, String pos2)
        {
            return this.Find(delegate(Distance d) { return ((d.Position1 == pos1) && (d.Position2 == pos2)); });
        }
        public Int32 FindIndex(String pos1, String pos2)
        {
            return this.FindIndex(delegate(Distance d) { return ((d.Position1 == pos1) && (d.Position2 == pos2)); });
        }
        public void SetSelected(Int32 i, Boolean selected)
        {
            if (i >= 0 && i < this.Count)
            {
                Distance dtmp = this[i];
                dtmp.IsSelected = selected;
                this[i] = dtmp;
            }       
        }
    }
}
