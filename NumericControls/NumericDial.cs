using System;
using System.Drawing;
using System.Windows.Forms;

namespace NumericControls
{
    public class NumericDial : Control
    {
        public NumericDial()
        {
            InitializeComponent();
        }

        private System.Drawing.Drawing2D.GraphicsPath thispath = new System.Drawing.Drawing2D.GraphicsPath();

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ForeColor = SystemColors.ControlDarkDark;
            this.BackColor = SystemColors.Control;
            this.TabStop = true;
            this.DoubleBuffered = true;
            R = this.Width / 2.0;

            this.ResumeLayout(true);
        }
        /// <summary>
        /// Occurs when the Value is changed by user
        /// </summary>
        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, new EventArgs()); }
        
        protected override Size DefaultSize
        {
            get { return new Size(80, 80); }
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (this.Height != this.Width) this.Height = this.Width;
            R = this.Width / 2.0;
            center_shift.Width = (Int32)R; center_shift.Height = (Int32)R;

            thispath.Reset();
            thispath.AddEllipse(new Rectangle(-1, -1, this.Width + 2, this.Height + 2));
            this.Region = new Region(thispath);

            base.OnLayout(levent);
        }
        
        // Defaults
        private Double ndValue = 5.0;
        private Double ndMinValue = 0.0;
        private Double ndMaxValue = 10.0;
        private Double ndMinMaxAngle = Math.PI * 3.0 / 4.0;

        private Color ndLEDColor = Color.OrangeRed;
        private Double ndAngle = 0;
        private Double R;
        private Size center_shift = new Size(0, 0);
        private Double dAngle0;
        private Double mouseAngle;
        private Double new_value;

        #region Properties

        /// <summary>
        /// Min value
        /// </summary>
        public Double MinValue
        {
            get { return ndMinValue; }
            set { ndMinValue = value; this.Value = ndValue; }
        }
        /// <summary>
        /// Max value
        /// </summary>
        public Double MaxValue
        {
            get { return ndMaxValue; }
            set { ndMaxValue = value; this.Value = ndValue; }
        }
        /// <summary>
        /// Min and max angle in radians (0 = vertical)
        /// </summary>
        public Double MinMaxAngle
        {
            get { return ndMinMaxAngle; }
            set { ndMinMaxAngle = value; this.Value = ndValue; }
        }

        /// <summary>
        /// Color of the "LED" indicator
        /// </summary>
        public Color LEDColor
        {
            get { return ndLEDColor; }
            set { ndLEDColor = value; this.Invalidate(true); }
        }

        /// <summary>
        /// Numeric value
        /// </summary>
        public Double Value
        {
            get { return ndValue; }
            set 
            { 
                ndValue = value;
                ndAngle = -ndMinMaxAngle + (value - ndMinValue) / (ndMaxValue - ndMinValue) * 2.0 * ndMinMaxAngle;
                this.Invalidate(true);
            }
        }

        #endregion

        // Handling mouse moves
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            dAngle0 = ndAngle - Math.Atan2(e.X - R, R - e.Y);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                mouseAngle = Math.Atan2(e.X - R, R - e.Y);
                ndAngle = mouseAngle + dAngle0;
                if (ndAngle < -ndMinMaxAngle) ndAngle = -ndMinMaxAngle;
                if (ndAngle > ndMinMaxAngle) ndAngle = ndMinMaxAngle;
                new_value = ndMinValue + (ndMaxValue - ndMinValue) * (ndAngle + ndMinMaxAngle) / 2.0 / ndMinMaxAngle;
                if (new_value != ndValue)
                {
                    ndValue = new_value;
                    OnValueChanged();
                    this.Invalidate(true);
                }
            }
        }

        // Drawing NumericDial
        protected override void OnPaint(PaintEventArgs pe)
        {
            Brush ndbrush = new SolidBrush(this.ForeColor);
            Pen ndpen = new Pen(ndbrush, 2.0F);
            pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            pe.Graphics.DrawEllipse(ndpen, new Rectangle(1, 1, this.Width - 3, this.Height - 3));
            ndbrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Point (0,0), new Point(this.Width,this.Height),
                SystemColors.ControlLight, SystemColors.ControlDark);
            pe.Graphics.FillEllipse(ndbrush, new Rectangle(1, 1, this.Width - 3, this.Height - 3));

            // Drawing line
            Point ndstart = new Point((Int32)(Math.Sin(ndAngle) * R * 0.4), (Int32)(-Math.Cos(ndAngle) * R * 0.4));
            Point ndstop = new Point((Int32)(Math.Sin(ndAngle) * (R - 4D)), (Int32)(-Math.Cos(ndAngle) * (R - 4D)));
            ndstart += center_shift; ndstop += center_shift;
            ndpen.Color = ndLEDColor;
            pe.Graphics.DrawLine(ndpen, ndstart, ndstop);

            ndbrush.Dispose();
            ndpen.Dispose();

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
    }
}
