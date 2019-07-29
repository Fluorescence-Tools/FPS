using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fps
{
    [Serializable]
    public struct ConversionParameters
    {
        public Double R0; // Foerster radius
        public Int32 PolynomOrder;
    }

    public partial class ConversionForm : Form
    {
        private Double[] _x = new Double[0];
        /// <summary>
        /// "X" data
        /// </summary>
        public Double[] X
        {
            set { _x = value; }
        }
        private Double[] _y = new Double[0];
        /// <summary>
        /// "Y" data
        /// </summary>
        public Double[] Y
        {
            set { _y = value; }
        }
        private Double[] _c;
        /// <summary>
        /// Fitted coefficients
        /// </summary>
        public Double[] C
        {
            get { return _c; }
        }
        private Boolean _inverse = false;
        /// <summary>
        /// X(Y) fitting
        /// </summary>
        public Boolean Inverse
        {
            get { return _inverse; }
            set 
            { 
                _inverse = value;
                inversecheckBox.Checked = value;
            }
        }

        private Int32 _order = 3;
        /// <summary>
        /// Polynom order
        /// </summary>
        public Int32 PolynomOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        private String _xname = "X";
        /// <summary>
        /// X data name
        /// </summary>
        public String XName
        {
            get { return _xname; }
            set { _xname = value; }
        }
        private String _yname = "Y";
        /// <summary>
        /// Y data name
        /// </summary>
        public String YName
        {
            get { return _yname; }
            set { _yname = value; }
        }

        private Int32 _nsamples = 100000;
        /// <summary>
        /// Number of points to calculate averaged R
        /// </summary>
        public Int32 NSamples
        {
            get { return _nsamples; }
            set { _nsamples = value; }
        }
        
        public ConversionForm()
        {
            InitializeComponent();
            NumericControls.NumericBoxDouble t = new NumericControls.NumericBoxDouble();
            t.IsIndicator = true;
            t.Format = "F6";
            t.Increment = 0.0;
            t.SpinBoxVisible = false;
            coeftsarray.TemplateElement = t;
            coeftsarray.ApplyTemplate();
        }
        private void ConversionForm_Load(object sender, EventArgs e)
        {
            xarray.Value = _x;
            yarray.Value = _y;
            orderbox.Value = _order;
            this.Text = _xname + " to " + _yname + " conversion";
            this.labelX.Text = _xname;
            this.labelY.Text = _yname;
            inversecheckBox.Checked = _inverse;
            if (_x != null && _x.Length > _order + 1 && _y != null && _y.Length > _order + 1) Convert();
        }

        private void closebutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Obtain polynomial coefficients
        /// </summary>
        public void Convert()
        {
            if (_x == null || _x.Length < _order + 1 || _y == null || _y.Length < _order + 1)
            {
                _c = new Double[] { 0.0, 1.0 };
                return;
            }
            Int32 n = orderbox.Value + 1;
            Int32 m = _x.Length;
            Mapack.Matrix A = new Mapack.Matrix(m, n);
            Mapack.Matrix B = new Mapack.Matrix(m, 1);
            if (_inverse)
                for (Int32 i = 0; i < m; i++)
                {
                    A[i, 0] = 1.0;
                    for (Int32 j = 1; j < n; j++)
                        A[i, j] = A[i, j - 1] * _y[i];
                    B[i, 0] = _x[i];
                }
            else
                for (Int32 i = 0; i < m; i++)
                {
                    A[i, 0] = 1.0;
                    for (Int32 j = 1; j < n; j++)
                        A[i, j] = A[i, j - 1] * _x[i];
                    B[i, 0] = _y[i];
                }
            Mapack.Matrix Csolve = A.Solve(B);
            _c = new Double[n];
            for (Int32 j = 0; j < n; j++) _c[j] = Csolve[j, 0];
            coeftsarray.Value = _c;

            // error
            Mapack.Matrix E = A * Csolve - B;
            errorBoxDouble.Value = E.FrobeniusNorm / Math.Sqrt((Double)m);
        }
        public void Convert(Boolean inverse)
        {
            _inverse = inverse;
            Convert();
        }

        private void orderbox_ValueChanged(object sender, EventArgs e)
        {
            _inverse = inversecheckBox.Checked;
            Convert();
        }

        private void copybutton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            String nformat = "F8";
            Int32 i;
            String s = inversecheckBox.Checked ? _yname + " -> " + _xname + '\n' : _xname + " -> " + _yname + '\n';
            s += _xname + '\t' + _yname + '\t' + "C0..Cmax\n";
            for (i = 0; i < _c.Length; i++) s += _x[i].ToString(nformat) + '\t' + _y[i].ToString(nformat)
                + '\t' + _c[i].ToString(nformat) + '\n';
            for (; i < _x.Length; i++) s += _x[i].ToString(nformat) + '\t' + _y[i].ToString(nformat) + '\n';
            Clipboard.SetText(s);
        }

        // calculate <R>
        public Double AveragedDistance(Vector3[] r1, Vector3[] r2, Double translation,
            Boolean FRETAveraged, Boolean shuffle, Double R0, Random rnd)
        {
            Vector3 rrand;
            Matrix3 Urand;
            Double t, uz, uxy, uphi, Emean, Rmean, R06 = Math.Pow(R0, 6.0);

            if (shuffle)
            {
                // random translation of 1
                uz = -1.0 + rnd.NextDouble() * 2.0;
                uxy = Math.Sqrt(1.0 - uz * uz);
                uphi = rnd.NextDouble() * 2.0 * Math.PI;
                rrand = new Vector3(uxy * Math.Cos(uphi), uxy * Math.Sin(uphi), uz);
                rrand = rrand * translation;

                // random rotation of 2
                uz = -1.0 + rnd.NextDouble() * 2.0;
                uxy = Math.Sqrt(1.0 - uz * uz);
                uphi = rnd.NextDouble() * 2.0 * Math.PI;
                Urand = Matrix3.Rotation(new Vector3(uxy * Math.Cos(uphi), uxy * Math.Sin(uphi), uz),
                    2.0 * Math.PI * rnd.NextDouble());
            }
            else
            {
                rrand = new Vector3();
                Urand = Matrix3.E;
            }

            if (FRETAveraged)
            {
                Emean = 0.0;
                for (Int32 j = 0; j < _nsamples; j++)
                {
                    t = Vector3.SquareNormDiff(r1[rnd.Next(r1.Length - 1)] + rrand,
                        Urand * r2[rnd.Next(r2.Length - 1)]);
                    Emean += 1.0 / (1.0 + t * t * t / R06);
                }
                Emean /= ((Double)_nsamples);
                Rmean = R0 * Math.Pow((1.0 / Emean - 1.0), 1.0 / 6.0);
            }
            else
            {
                Rmean = 0.0;
                for (Int32 j = 0; j < _nsamples; j++)
                    Rmean += Math.Sqrt(Vector3.SquareNormDiff(r1[rnd.Next(r1.Length - 1)] + rrand,
                        Urand * r2[rnd.Next(r2.Length - 1)]));
                Rmean /= ((Double)NSamples);
            }
            return Rmean;
        }

    }
}