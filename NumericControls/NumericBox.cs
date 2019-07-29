using System;
using System.Drawing;
using System.Windows.Forms;

namespace NumericControls
{
    // Pre-defined non-generic
    /// <summary>
    /// Numeric Box representing a Double number
    /// </summary>
    public class NumericBoxDouble : NumericBox<Double>
    {
        protected override Double TAdd(Double a, Double b) { return a + b; }
        protected override Double TMultiplyByInt(Double a, Int32 c) { return a * (Double)c; }
        protected override Int32 TIntDivide(Double a, Double b) { return (Int32)Math.Round(a / b); }
        protected override Boolean TTryParse(String s, out Double v) { return Double.TryParse(s, out v); }
    }

    /// <summary>
    /// Numeric Box representing an Int32 number
    /// </summary>
    public class NumericBoxInt32 : NumericBox<Int32>
    {
        protected override Int32 TAdd(Int32 a, Int32 b) { return a + b; }
        protected override Int32 TMultiplyByInt(Int32 a, Int32 c) { return a * c; }
        protected override Int32 TIntDivide(Int32 a, Int32 b) { return (Int32)Math.Round((Double)a / (Double)b); }
        protected override Boolean TTryParse(String s, out Int32 v) { return Int32.TryParse(s, out v); }
    }

    // ... Define for other types if you like



    //**************** Generic implementation ******************//
    /// <summary>
    /// Generic Numeric Box
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public abstract class NumericBox<T> : UserControl
        where T : IConvertible, IComparable, new()

    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NumericBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with initialization
        /// </summary>
        /// <param name="InitialValue">initial value</param>
        public NumericBox(T InitialValue)
        {
            this.ncValue = InitialValue;
            this.valuebox.Text = String.Format(ncFormatString, InitialValue);
            this.OldText = this.valuebox.Text;
            InitializeComponent();
        }

        protected abstract T TAdd(T a, T b);
        protected abstract T TMultiplyByInt(T a, Int32 c);
        protected abstract Int32 TIntDivide(T a, T b);
        protected abstract Boolean TTryParse(String s, out T v);
      
  
        // Child controls: a TextBox and a SmallUpDown
        private TextBox valuebox = new TextBox();
        private SmallUpDown spinbox = new SmallUpDown();

        #region Defaults

        // Some defaults
        private T ncValue = default(T);
        private T ncIncrement = default(T);
        private Boolean ncStrictIncrement = false;
        private Boolean ncIsIndicator = false;
        private T ncMaxValue = (T)typeof(T).GetField("MaxValue").GetValue(null);
        private T ncMinValue = (T)typeof(T).GetField("MinValue").GetValue(null);

        private String OldText = default(T).ToString();
        private String ncFormatString = "{0:G}";
        private Boolean ncSpinBoxVisible = true;
        private LeftRightAlignment ncSpinBoxAlignment = LeftRightAlignment.Right;

        private String dec_separator = Application.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private String minus_sign = Application.CurrentCulture.NumberFormat.NegativeSign;

        #endregion

        private void InitializeComponent()
        {

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.SuspendLayout();

            // Default behaviour of a numeric box
            this.valuebox.AcceptsTab = false;
            this.valuebox.AcceptsReturn = false;
            this.valuebox.Multiline = false;
            this.valuebox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.valuebox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.valuebox.Text = this.OldText;

            SizeAndLocation();

            this.Controls.Add(valuebox);
            this.Controls.Add(spinbox);

            this.ResumeLayout(true);

            // Add events handlers
            this.Validated += new System.EventHandler(this.CheckIfNumber);
            this.Click += new System.EventHandler(this.CheckIfNumber);
            this.valuebox.Click += new System.EventHandler(this.CheckIfNumber);
            this.MouseWheel += new MouseEventHandler(NumericBox_MouseWheel);
            this.valuebox.KeyDown += new KeyEventHandler(valuebox_KeyDown);
            this.valuebox.KeyPress += new KeyPressEventHandler(valuebox_KeyPress);

            spinbox.UpButtonPressed += new EventHandler(delegate { if (!this.ncIsIndicator) AddIncrement(1); });
            spinbox.DownButtonPressed += new EventHandler(delegate { if (!this.ncIsIndicator) AddIncrement(-1); });

        }

        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, new EventArgs()); }

        #region Properties

        /// <summary>
        /// Numeric value
        /// </summary>
        public T Value
        {
            get { return ncValue; }
            set
            {
                T new_value;
                if (value.CompareTo(this.ncMinValue) < 0) new_value = ncMinValue;
                else if (value.CompareTo(this.ncMaxValue) > 0) new_value = ncMaxValue;
                else new_value = value;
                if (ncStrictIncrement) new_value = TMultiplyByInt(ncIncrement, TIntDivide(new_value, ncIncrement));

                ncValue = new_value;
                this.valuebox.Text = String.Format(ncFormatString, ncValue);
                OldText = this.valuebox.Text;
            }
        }
        /// <summary>
        /// Increment on SmallUpDown, arrows, or mousewheel
        /// </summary>
        public T Increment
        {
            get { return ncIncrement; }
            set { ncIncrement = value; }
        }
        /// <summary>
        /// Determines if the value is rounded towards n*<paramref name="Increment"/>Increment</paramref>
        /// </summary>
        public Boolean StrictIncrement
        {
            get { return ncStrictIncrement; }
            set { ncStrictIncrement = value; }
        }
        /// <summary>
        /// If true, the value is read-only
        /// </summary>
        public Boolean IsIndicator
        {
            get { return ncIsIndicator; }
            set { ncIsIndicator = value; }
        }

        /// <summary>
        /// Min Value. Default is T.MinValue
        /// </summary>
        public T MinValue
        {
            get { return ncMinValue; }
            set { ncMinValue = value; }
        }
        /// <summary>
        /// MaxValue. Default is T.MaxValue
        /// </summary>
        public T MaxValue
        {
            get { return ncMaxValue; }
            set { ncMaxValue = value; }
        }

        /// <summary>
        /// Format of text string on output
        /// </summary>
        public String Format
        {
            get { return ncFormatString.Substring(3, ncFormatString.Length - 4); }
            set
            {
                ncFormatString = "{0:" + value + "}";
                this.valuebox.Text = String.Format(ncFormatString, this.Value);
            }
        }
        /// <summary>
        /// Position of the spinbox
        /// </summary>
        public LeftRightAlignment SpinBoxAlignment
        {
            get { return ncSpinBoxAlignment; }
            set
            {
                if (value != ncSpinBoxAlignment)
                {
                    ncSpinBoxAlignment = value;
                    SizeAndLocation();
                }
            }
        }
        /// <summary>
        /// Determines whether the spinbox is visible
        /// </summary>
        public Boolean SpinBoxVisible
        {
            get { return ncSpinBoxVisible; }
            set
            {
                if (value != ncSpinBoxVisible)
                {
                    ncSpinBoxVisible = value;
                    spinbox.Visible = value;
                    SizeAndLocation();
                }
            }
        }
        /// <summary>
        /// Gets or sets BackColor (applies also to valuebox)
        /// </summary>
        public override Color BackColor
        {
            get { return valuebox.BackColor; }
            set
            {
                valuebox.BackColor = value;
                base.BackColor = value;
            }
        }
        /// <summary>
        /// Gets or sets ForeColor (applies also to valuebox)
        /// </summary>
        public override Color ForeColor
        {
            get { return valuebox.ForeColor; }
            set
            {
                valuebox.ForeColor = value;
                base.ForeColor = value;
            }
        }

        #endregion

        #region Size and Location
        /// <summary>
        /// Default size of NumericBox
        /// </summary>
        protected override Size DefaultSize
        {
            get { return new Size(valuebox.Width + spinbox.Width, valuebox.Height); }
        }
        /// <summary>
        /// Calculates relative positions of the textbox and spinbox depending on spinbox alignment
        /// </summary>
        protected void SizeAndLocation()
        {
            // Size
            if (ncSpinBoxVisible) valuebox.Width = this.Width - spinbox.Width;
            else valuebox.Width = this.Width;

            valuebox.Height = this.Height;
            spinbox.Height = this.Height;

            // Location
            if (this.ncSpinBoxAlignment == LeftRightAlignment.Left)
            {
                spinbox.Location = new Point(0, 0);
                if (ncSpinBoxVisible) valuebox.Location = new Point(spinbox.Width - 1, 0);
                else valuebox.Location = new Point(0, 0);
            }
            else
            {
                valuebox.Location = new Point(0, 0);
                spinbox.Location = new Point(valuebox.Width - 1, 0);
            }

        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (levent.AffectedProperty != "Visible") SizeAndLocation();
            base.OnLayout(levent);
        }

        #endregion

        #region Value <-> Text

        /// <summary>
        /// Indicates whether the text entered by user has been checked
        /// </summary>
        private Boolean CheckedIfNumber = false;

        /// <summary>
        /// Checks if entered text can be converted to a number of type T. If no, old value is kept
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CheckIfNumber(object sender, EventArgs e)
        {
            // trying to convert new text to a number of type T
            T new_value;
            if (TTryParse(this.valuebox.Text, out new_value))
            {
                if (!new_value.Equals(ncValue))
                {
                    this.Value = new_value;
                    OnValueChanged();
                }
            }
            else this.valuebox.Text = OldText;
            CheckedIfNumber = true;
        }
        /// <summary>
        /// Adds Increment * ifactor to the Value
        /// </summary>
        /// <param name="ifactor"></param>
        protected void AddIncrement(Int32 ifactor)
        {
            T new_value;
            if (!CheckedIfNumber && TTryParse(this.valuebox.Text, out new_value))
                new_value = TAdd(new_value, TMultiplyByInt(Increment, ifactor));
            else new_value = TAdd(ncValue, TMultiplyByInt(Increment, ifactor));
            this.CheckedIfNumber = true;

            if (!new_value.Equals(ncValue))
            {
                this.Value = new_value;
                OnValueChanged();
            }
        }

        #endregion

        #region Keyboard events

        /// <summary>
        /// Interceps KeyDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void valuebox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.ncIsIndicator) { e.Handled = true; return; } // Disable all user input

            // intercept arrows and PgUp + PgDown
            switch (e.KeyCode)
            {
                case Keys.Up: { AddIncrement(1); break; }
                case Keys.Down: { AddIncrement(-1); break; }
                case Keys.PageUp: { AddIncrement(10); break; }
                case Keys.PageDown: { AddIncrement(-10); break; }
                case Keys.Enter: { CheckIfNumber(new object(), new EventArgs()); break; }
            }

        }
        /// <summary>
        /// Filters invalid keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void valuebox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.ncIsIndicator) { e.Handled = true; return; } 

            // Ctrl or Alt pressed -> do nothing special
            if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) != 0) return;

            String c = e.KeyChar.ToString();
            this.CheckedIfNumber = false;

            // Typical keys
            if (Char.IsDigit(e.KeyChar) || c.Equals(dec_separator) || c.Equals(minus_sign) ||
                 e.KeyChar == 'e' || e.KeyChar == 'E' || e.KeyChar == '\b') return;

            e.Handled = true;
        }

        #endregion

        /// <summary>
        /// Invokes AddIncrement on MouseWheel event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NumericBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ncIsIndicator) return;
            AddIncrement(Math.Sign(e.Delta));
        }
    }


}