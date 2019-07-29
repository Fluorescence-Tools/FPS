using System;
using System.IO;
using System.Windows.Forms;

namespace GPFileTools
{
    public class SpreadsheetAscii : GPFileToolsBase
    {
        public SpreadsheetAscii() { }
        ~SpreadsheetAscii() { }

        private Boolean saAppend = false;
        private Boolean saTranspose = false;
        private String saFormat = "G";
        private Char[] saDelimeters = { '\t' };
        private Boolean saShowProgress = true;

        #region Properties

        public Boolean Append
        {
            get { return saAppend; }
            set { saAppend = value; }
        }

        public Boolean Transpose
        {
            get { return saTranspose; }
            set { saTranspose = value; }
        }

        public String Format
        {
            get { return saFormat; }
            set { saFormat = value; }
        }

        public Char[] Delimeters
        {
            get { return saDelimeters; }
            set { saDelimeters = value; }
        }

        public Boolean ShowProgress
        {
            get { return saShowProgress; }
            set { saShowProgress = value; }
        }

        #endregion

        #region Private readers

        // Read text before data
        private String ReadHeaderAndFooter(String[] strData, Int32 maxColN,
            out Int32 firstDataLine, out Int32 lastDataLine, out Int32 Ncolumns)
        {

            String[] tmpStr;
            Double tmpValue;
            Int32 i;
            Boolean isData;
            Ncolumns = 0;
            maxColN--;

            // reading header
            for (i = 0; i < strData.Length; i++)
            {
                tmpStr = strData[i].Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries);
                if (tmpStr.Length <= maxColN) continue;

                // trying to parse each element at least to Double. If successful, proceed with the data
                isData = true;
                for (Int32 k = 0; k < tmpStr.Length; k++) isData = isData & Double.TryParse(tmpStr[k], out tmpValue);
                if (isData)
                {
                    Ncolumns = tmpStr.Length;
                    break;
                }
            }
            firstDataLine = i;

            // reading footer
            Int32 j;
            for (j = strData.Length - 1; j >= 0; j--)
            {
                tmpStr = strData[j].Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries);
                if (tmpStr.Length <= maxColN) continue;

                // trying to parse each element at least to Double. If successful, proceed with the data
                isData = true;
                for (Int32 k = 0; k < tmpStr.Length; k++) isData = isData & Double.TryParse(tmpStr[k], out tmpValue);
                if (isData) break;
            }
            lastDataLine = j;

            // return header
            tmpStr = new String[j];
            Array.Copy(strData, 0, tmpStr, 0, i);
            return String.Concat(tmpStr);
        }
        // overload without Ncoulumns
        private String ReadHeaderAndFooter(String[] strData, Int32 maxColN,
            out Int32 firstDataLine, out Int32 lastDataLine)
        {
            Int32 Ncolumns;
            return ReadHeaderAndFooter(strData, maxColN, out firstDataLine, out lastDataLine, out Ncolumns);
        }


        // read one column
        private T[] ReadDataOne<T>(ref String[] strData, Int32 colY,
            ParseMethod<T> TParse, Int32 firstDataLine, Int32 lastdataline)
        {

            T[] dataY = new T[lastdataline - firstDataLine + 1];
            colY--;

            Int32 i0 = 0; Int32 i1; Int32 k;
            for (Int32 i = firstDataLine; i <= lastdataline; i++)
            {
                for (k = 0; k < colY; k++) i0 = strData[i].IndexOfAny(saDelimeters, 0);
                i1 = strData[i].IndexOfAny(saDelimeters, i0 + 1);
                if (i1 == -1) i1 = strData[i].Length;
                dataY[i - firstDataLine] = TParse(strData[i].Substring(i0, i1 - i0));
            }
            return dataY;
        }

        // read several columns
        private T[,] ReadDataMany<T>(ref String[] strData, Int32[] colNumbers,
            ParseMethod<T> TParse, Int32 firstDataLine, Int32 lastdataline)
        {
            T[,] dataZ = new T[lastdataline - firstDataLine + 1, colNumbers.Length];

            Int32 colNmax = 0;
            Int32 k = 0;
            for (k = 0; k < colNumbers.Length; k++)
                if (--colNumbers[k] > colNmax) colNmax = colNumbers[k];
            Int32[] si = new Int32[colNmax + 2]; si[0] = 0;

            for (Int32 i = firstDataLine; i <= lastdataline; i++)
            {
                for (k = 1; k <= colNmax + 1; k++) si[k] = strData[i].IndexOfAny(saDelimeters, si[k - 1] + 1);
                if (si[colNmax + 1] == -1) si[colNmax + 1] = strData[i].Length;
                for (k = 0; k < colNumbers.Length; k++)
                    dataZ[i - firstDataLine, k] = TParse(strData[i].Substring(si[colNumbers[k]], si[colNumbers[k] + 1] - si[colNumbers[k]]));
            }
            return dataZ;
        }

        // read all columns
        private T[,] ReadDataAll<T>(ref String[] strData,
            ParseMethod<T> TParse, Int32 firstDataLine, Int32 lastdataline)
        {
            Int32 colNmax = strData[firstDataLine].Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries).Length - 1;
            T[,] dataZ = new T[lastdataline - firstDataLine + 1, colNmax + 1];

            Int32 k = 0;
            Int32[] si = new Int32[colNmax + 2]; si[0] = 0;
            for (Int32 i = firstDataLine; i <= lastdataline; i++)
            {
                for (k = 1; k <= colNmax; k++) si[k] = strData[i].IndexOfAny(saDelimeters, si[k - 1] + 1);
                si[colNmax + 1] = strData[i].Length;
                for (k = 0; k <= colNmax; k++)
                    dataZ[i - firstDataLine, k] = TParse(strData[i].Substring(si[k], si[k + 1] - si[k]));
            }
            return dataZ;
        }

        #endregion

        #region File info
        public struct SpreadsheetInfo
        {
            public Int32 NRows;
            public Int32 NColumns;
        }

        public SpreadsheetInfo GetSpreadsheetInfo(String filename)
        {
            SpreadsheetInfo result = new SpreadsheetInfo();
            filename = ValidFileOpen(filename);
            if (filename == null) return result;

            Int32 firstData; Int32 lastData; Int32 Ncolumns;
            String[] strData = File.ReadAllLines(filename);

            ReadHeaderAndFooter(strData, 0, out firstData, out lastData, out Ncolumns);
            result.NRows = lastData - firstData + 1;
            result.NColumns = Ncolumns;

            return result;
        }

        #endregion

        #region ReadY + overloads, one file

        // column by number
        public Boolean ReadY<T>(String filename, ref T[] dataY, ref String fileheader, Int32 colY)
        {
            filename = ValidFileOpen(filename);
            if (filename == null) return false;

            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                String[] strData = File.ReadAllLines(filename);
                Int32 firstData; Int32 lastData;

                fileheader = ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                dataY = ReadDataOne<T>(ref strData, colY, TParse, firstData, lastData);
            }

            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public Boolean ReadY<T>(String filename, ref T[] dataY, Int32 colY)
        {
            String fileheader = "";
            return ReadY<T>(filename, ref dataY, ref fileheader, colY);
        }
        public Boolean ReadY<T>(String filename, ref T[] dataY)
        {
            return ReadY<T>(filename, ref dataY, 1);
        }

        // column by name
        public Boolean ReadY<T>(String filename, ref T[] dataY, ref String fileheader, String colYname)
        {
            filename = ValidFileOpen(filename);
            if (filename == null) return false;

            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                String[] strData = File.ReadAllLines(filename);
                Int32 firstData; Int32 lastData;

                fileheader = ReadHeaderAndFooter(strData, 1, out firstData, out lastData);
                String[] tmpStr = fileheader.Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries);
                Int32 colY = Array.IndexOf<String>(tmpStr, colYname) + 1;
                if (colY == 0) throw new ArgumentException("Column " + colYname + " not found");
                dataY = ReadDataOne<T>(ref strData, colY, TParse, firstData, lastData);
            }

            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public Boolean ReadY<T>(String filename, ref T[] dataY, String colYname)
        {
            String fileheader = "";
            return ReadY<T>(filename, ref dataY, ref fileheader, colYname);
        }

        #endregion

        #region ReadY + overloads, many files

        // column by number
        public Boolean ReadY<T>(String[] filenames, ref T[] dataY, ref String fileheader, Int32 colY)
        {
            if (filenames == null || (filenames.Length == 0)) filenames = ValidManyFiles(null);
            else if (filenames.Length == 1) filenames = ValidManyFiles(filenames[0]);

            if (filenames == null || (filenames.Length == 0)) return false;

            ProgressForm sa_pf = new ProgressForm(filenames);
            if (saShowProgress)
            {
                sa_pf.Show();
                sa_pf.Count = 0;
            }
            T[][] tmpData;
            String[] strData;

            Int32 i = 0;
            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                Int32 firstData; Int32 lastData;

                strData = File.ReadAllLines(filenames[0]);
                tmpData = new T[filenames.Length][];

                fileheader = ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                tmpData[0] = ReadDataOne<T>(ref strData, colY, TParse, firstData, lastData);

                Int32[] recordlengths = new Int32[filenames.Length];
                recordlengths[0] = lastData - firstData + 1;
                Int32 totallength = lastData - firstData + 1;

                for (i = 1; i < filenames.Length; i++)
                {
                    sa_pf.Count = i;
                    strData = File.ReadAllLines(filenames[i]);
                    ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                    tmpData[i] = ReadDataOne<T>(ref strData, colY, TParse, firstData, lastData);
                    recordlengths[i] = lastData - firstData + 1;
                    totallength += lastData - firstData + 1;
                }

                dataY = new T[totallength];
                Int32 j = 0;
                for (i = 0; i < filenames.Length; i++)
                {
                    Array.Copy(tmpData[i], 0, dataY, j, recordlengths[i]);
                    j += recordlengths[i];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filenames[i],
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (saShowProgress) sa_pf.Close();
                sa_pf.Dispose();
            }
            return true;
        }

        public Boolean ReadY<T>(String[] filenames, ref T[] dataY, Int32 colY)
        {
            String fileheader = "";
            return ReadY<T>(filenames, ref dataY, ref fileheader, colY);
        }
        public Boolean ReadY<T>(String[] filenames, ref T[] dataY)
        {
            return ReadY<T>(filenames, ref dataY, 1);
        }

        // column by name
        public Boolean ReadY<T>(String[] filenames, ref T[] dataY, ref String fileheader, String colYname)
        {
            if (filenames == null || (filenames.Length == 0)) filenames = ValidManyFiles(null);
            else if (filenames.Length == 1) filenames = ValidManyFiles(filenames[0]);

            if (filenames == null || (filenames.Length == 0)) return false;

            ProgressForm sa_pf = new ProgressForm(filenames);
            if (saShowProgress)
            {
                sa_pf.Show();
                sa_pf.Count = 0;
            }
            T[][] tmpData;
            String[] strData;

            Int32 i = 0;
            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                Int32 firstData; Int32 lastData;

                strData = File.ReadAllLines(filenames[0]);
                tmpData = new T[filenames.Length][];

                fileheader = ReadHeaderAndFooter(strData, 1, out firstData, out lastData);
                String[] tmpStr = fileheader.Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries);
                Int32 colY = Array.IndexOf<String>(tmpStr, colYname) + 1;
                if (colY == 0) throw new ArgumentException("Column " + colYname + " not found");
                tmpData[0] = ReadDataOne<T>(ref strData, colY, TParse, firstData, lastData);

                Int32[] recordlengths = new Int32[filenames.Length];
                recordlengths[0] = lastData - firstData + 1;
                Int32 totallength = lastData - firstData + 1;

                for (i = 1; i < filenames.Length; i++)
                {
                    sa_pf.Count = i;
                    strData = File.ReadAllLines(filenames[i]);
                    ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                    tmpData[i] = ReadDataOne<T>(ref strData, colY, TParse, firstData, lastData);
                    recordlengths[i] = lastData - firstData + 1;
                    totallength += lastData - firstData + 1;
                }

                dataY = new T[totallength];
                Int32 j = 0;
                for (i = 0; i < filenames.Length; i++)
                {
                    Array.Copy(tmpData[i], 0, dataY, j, recordlengths[i]);
                    j += recordlengths[i];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filenames[i],
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (saShowProgress) sa_pf.Close();
                sa_pf.Dispose();
            }
            return true;
        }

        public Boolean ReadY<T>(String[] filenames, ref T[] dataY, String colYname)
        {
            String fileheader = "";
            return ReadY<T>(filenames, ref dataY, ref fileheader, colYname);
        }

        #endregion 

        #region ReadXY + overloads, one file

        // columns by number
        public Boolean ReadXY<T>(String filename, ref T[] dataX, ref T[] dataY, ref String fileheader, Int32 colX, Int32 colY)
        {
            filename = ValidFileOpen(filename);
            if (filename == null) return false;

            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                String[] strData = File.ReadAllLines(filename);
                Int32 firstData; Int32 lastData;

                fileheader = ReadHeaderAndFooter(strData, colY, out firstData, out lastData);

                T[,] dataXY = ReadDataMany<T>(ref strData, new Int32[] { colX, colY }, TParse, firstData, lastData);

                dataX = new T[dataXY.GetLength(0)];
                dataY = new T[dataXY.GetLength(0)];
                for (Int32 i = 0; i < dataXY.GetLength(0); i++) dataX[i] = dataXY[i, 0];
                for (Int32 i = 0; i < dataXY.GetLength(0); i++) dataY[i] = dataXY[i, 1];
            }

            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public Boolean ReadXY<T>(String filename, ref T[] dataX, ref T[] dataY, Int32 colX, Int32 colY)
        {
            String fileheader = "";
            return ReadXY<T>(filename, ref dataX, ref dataY, ref fileheader, colX, colY);
        }
        public Boolean ReadXY<T>(String filename, ref T[] dataX, ref T[] dataY)
        {
            return ReadXY<T>(filename, ref dataX, ref dataY, 1, 2);
        }


        // columns by name
        public Boolean ReadXY<T>(String filename, ref T[] dataX, ref T[] dataY, ref String fileheader, String colXname, String colYname)
        {
            filename = ValidFileOpen(filename);
            if (filename == null) return false;

            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                String[] strData = File.ReadAllLines(filename);
                Int32 firstData; Int32 lastData;

                fileheader = ReadHeaderAndFooter(strData, 2, out firstData, out lastData);
                String[] tmpStr = fileheader.Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries);
                Int32 colX = Array.IndexOf<String>(tmpStr, colXname) + 1;
                if (colX == 0) throw new ArgumentException("Column \"" + colXname + "\" not found");
                Int32 colY = Array.IndexOf<String>(tmpStr, colYname) + 1;
                if (colY == 0) throw new ArgumentException("Column \"" + colYname + "\" not found");

                T[,] dataXY = ReadDataMany<T>(ref strData, new Int32[] { colX, colY }, TParse, firstData, lastData);

                dataX = new T[dataXY.GetLength(0)];
                dataY = new T[dataXY.GetLength(0)];
                for (Int32 i = 0; i < dataXY.GetLength(0); i++) dataX[i] = dataXY[i, 0];
                for (Int32 i = 0; i < dataXY.GetLength(0); i++) dataY[i] = dataXY[i, 1];
            }

            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public Boolean ReadXY<T>(String filename, ref T[] dataX, ref T[] dataY, String colXname, String colYname)
        {
            String fileheader = "";
            return ReadXY<T>(filename, ref dataX, ref dataY, ref fileheader, colXname, colYname);
        }

        #endregion

        #region ReadXY + overloads, many files

        // columns by number
        public Boolean ReadXY<T>(String[] filenames, ref T[] dataX, ref T[] dataY, ref String fileheader, Int32 colX, Int32 colY)
        {
            if (filenames == null || (filenames.Length == 0)) filenames = ValidManyFiles(null);
            else if (filenames.Length == 1) filenames = ValidManyFiles(filenames[0]);

            if (filenames == null || (filenames.Length == 0)) return false;

            ProgressForm sa_pf = new ProgressForm(filenames);
            if (saShowProgress)
            {
                sa_pf.Show();
                sa_pf.Count = 0;
            }
            T[][,] tmpData;
            String[] strData;

            Int32 i = 0;
            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                Int32 firstData; Int32 lastData;

                strData = File.ReadAllLines(filenames[0]);
                tmpData = new T[filenames.Length][,];

                fileheader = ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                tmpData[0] = ReadDataMany<T>(ref strData, new Int32[] { colX, colY }, TParse, firstData, lastData);

                Int32[] recordlengths = new Int32[filenames.Length];
                recordlengths[0] = lastData - firstData + 1;
                Int32 totallength = lastData - firstData + 1;

                for (i = 1; i < filenames.Length; i++)
                {
                    sa_pf.Count = i;
                    strData = File.ReadAllLines(filenames[i]);
                    ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                    tmpData[i] = ReadDataMany<T>(ref strData, new Int32[] { colX, colY }, TParse, firstData, lastData);
                    recordlengths[i] = lastData - firstData + 1;
                    totallength += lastData - firstData + 1;
                }

                dataX = new T[totallength];
                dataY = new T[totallength];
                Int32 j = 0;

                for (i = 0; i < filenames.Length; i++)
                {
                    for (Int32 k = 0; k < tmpData[i].GetLength(0); k++) dataX[k + j] = tmpData[i][k, 0];
                    for (Int32 k = 0; k < tmpData[i].GetLength(0); k++) dataY[k + j] = tmpData[i][k, 1];
                    j += recordlengths[i];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filenames[i],
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (saShowProgress) sa_pf.Close();
                sa_pf.Dispose();
            }
            return true;
        }
        public Boolean ReadXY<T>(String[] filenames, ref T[] dataX, ref T[] dataY, Int32 colX, Int32 colY)
        {
            String fileheader = "";
            return ReadXY<T>(filenames, ref dataX, ref dataY, ref fileheader, colX, colY);
        }
        public Boolean ReadXY<T>(String[] filenames, ref T[] dataX, ref T[] dataY)
        {
            return ReadXY<T>(filenames, ref dataX, ref dataY, 1, 2);
        }


        // columns by name
        public Boolean ReadXY<T>(String[] filenames, ref T[] dataX, ref T[] dataY, ref String fileheader, String colXname, String colYname)
        {
            if (filenames == null || (filenames.Length == 0)) filenames = ValidManyFiles(null);
            else if (filenames.Length == 1) filenames = ValidManyFiles(filenames[0]);

            if (filenames == null || (filenames.Length == 0)) return false;

            ProgressForm sa_pf = new ProgressForm(filenames);
            if (saShowProgress)
            {
                sa_pf.Show();
                sa_pf.Count = 0;
            }
            T[][,] tmpData;
            String[] strData;

            Int32 i = 0;
            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                Int32 firstData; Int32 lastData;

                strData = File.ReadAllLines(filenames[0]);
                tmpData = new T[filenames.Length][,];

                fileheader = ReadHeaderAndFooter(strData, 1, out firstData, out lastData);
                String[] tmpStr = fileheader.Split(saDelimeters, StringSplitOptions.RemoveEmptyEntries);
                Int32 colX = Array.IndexOf<String>(tmpStr, colXname) + 1;
                if (colX == 0) throw new ArgumentException("Column \"" + colXname + "\" not found");
                Int32 colY = Array.IndexOf<String>(tmpStr, colYname) + 1;
                if (colY == 0) throw new ArgumentException("Column \"" + colYname + "\" not found");

                tmpData[0] = ReadDataMany<T>(ref strData, new Int32[] { colX, colY }, TParse, firstData, lastData);

                Int32[] recordlengths = new Int32[filenames.Length];
                recordlengths[0] = lastData - firstData + 1;
                Int32 totallength = lastData - firstData + 1;

                for (i = 1; i < filenames.Length; i++)
                {
                    sa_pf.Count = i;
                    strData = File.ReadAllLines(filenames[i]);
                    ReadHeaderAndFooter(strData, colY, out firstData, out lastData);
                    tmpData[i] = ReadDataMany<T>(ref strData, new Int32[] { colX, colY }, TParse, firstData, lastData);
                    recordlengths[i] = lastData - firstData + 1;
                    totallength += lastData - firstData + 1;
                }

                dataX = new T[totallength];
                dataY = new T[totallength];
                Int32 j = 0;

                for (i = 0; i < filenames.Length; i++)
                {
                    for (Int32 k = 0; k < tmpData[i].GetLength(0); k++) dataX[k + j] = tmpData[i][k, 0];
                    for (Int32 k = 0; k < tmpData[i].GetLength(0); k++) dataY[k + j] = tmpData[i][k, 1];
                    j += recordlengths[i];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filenames[i],
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (saShowProgress) sa_pf.Close();
                sa_pf.Dispose();
            }
            return true;
        }
        public Boolean ReadXY<T>(String[] filenames, ref T[] dataX, ref T[] dataY, String colXname, String colYname)
        {
            String fileheader = "";
            return ReadXY<T>(filenames, ref dataX, ref dataY, ref fileheader, colXname, colYname);
        }

        #endregion

        #region ReadZ + overloads, one file

        // column by number
        public Boolean ReadZ<T>(String filename, ref T[,] dataZ, ref String fileheader)
        {
            filename = ValidFileOpen(filename);
            if (filename == null) return false;

            try
            {
                TryParseMethod<T> TTryParse = FindTryParse<T>();
                ParseMethod<T> TParse = FindParse<T>();

                String[] strData = File.ReadAllLines(filename);
                Int32 firstData; Int32 lastData;

                fileheader = ReadHeaderAndFooter(strData, 1, out firstData, out lastData);
                dataZ = ReadDataAll<T>(ref strData, TParse, firstData, lastData);
            }

            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nIn file: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public Boolean ReadZ<T>(String filename, ref T[,] dataZ)
        {
            String fileheader = "";
            return ReadZ<T>(filename, ref dataZ, ref fileheader);
        }

        #endregion


        #region WriteY + overloads

        public Boolean WriteY<T>(String filename, T[] dataY, String fileheader)
        {
            filename = ValidFileSave(filename);
            if (filename == null) return false;

            String formatstring = "{0:" + saFormat + "}";

            try
            {
                using (StreamWriter sasw = new StreamWriter(filename, this.saAppend))
                {
                    if (!fileheader.Equals(String.Empty)) sasw.WriteLine(fileheader);
                    for (Int32 i = 0; i < dataY.Length; i++)
                        sasw.WriteLine(formatstring, dataY[i]);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nFile: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public Boolean WriteY<T>(String filename, T[] dataY)
        {
            return WriteY<T>(filename, dataY, String.Empty);
        }

        #endregion

        #region WriteXY + overloads

        public Boolean WriteXY<T>(String filename, T[] dataX, T[] dataY, String fileheader)
        {
            filename = ValidFileSave(filename);
            if (filename == null) return false;

            String formatstring = "{0:" + saFormat + "}" + saDelimeters[0] + "{1:" + saFormat + "}";
            Int32 maxLength = Math.Min(dataX.Length, dataY.Length);

            try
            {
                using (StreamWriter sasw = new StreamWriter(filename, this.saAppend))
                {
                    if (!fileheader.Equals(String.Empty)) sasw.WriteLine(fileheader);
                    for (Int32 i = 0; i < maxLength; i++)
                        sasw.WriteLine(formatstring, dataX[i], dataY[i]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nFile: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public Boolean WriteXY<T>(String filename, T[] dataX, T[] dataY)
        {
            return WriteXY<T>(filename, dataX, dataY, String.Empty);
        }

        // write 1,2,3,.. instead of X
        public Boolean WriteXY<T>(String filename, T[] dataY, String fileheader)
        {
            filename = ValidFileSave(filename);
            if (filename == null) return false;

            String formatstring = "{0:" + saFormat + "}" + saDelimeters[0] + "{1:" + saFormat + "}";

            try
            {
                using (StreamWriter sasw = new StreamWriter(filename, this.saAppend))
                {
                    if (!fileheader.Equals(String.Empty)) sasw.WriteLine(fileheader);
                    for (Int32 i = 0; i < dataY.Length; i++)
                    {
                        sasw.WriteLine(formatstring, i + 1, dataY[i]);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nFile: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public Boolean WriteXY<T>(String filename, T[] dataY)
        {
            return WriteXY<T>(filename, dataY, String.Empty);
        }

        #endregion

        #region WriteZ + overloads

        public Boolean WriteZ<T>(String filename, T[,] dataZ, String fileheader)
        {
            filename = ValidFileSave(filename);
            if (filename == null) return false;

            String formatstring = "{0:" + saFormat + "}";
            String formatstringd = "{0:" + saFormat + "}" + saDelimeters[0];
            Int32 j;

            try
            {
                using (StreamWriter sasw = new StreamWriter(filename, this.saAppend))
                {
                    if (!fileheader.Equals(String.Empty)) sasw.WriteLine(fileheader);
                    for (Int32 i = 0; i < dataZ.GetLength(0); i++)
                    {
                        for (j = 0; j < dataZ.GetLength(1) - 1; j++)
                            sasw.Write(formatstringd, dataZ[i, j]);
                        sasw.WriteLine(formatstring, dataZ[i, j]);
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error in " + this.ToString() + ":\n" + e.Message + "\nFile: " + filename,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public Boolean WriteZ<T>(String filename, T[,] dataZ)
        {
            return WriteZ<T>(filename, dataZ, String.Empty);
        }

        #endregion


    }
}
