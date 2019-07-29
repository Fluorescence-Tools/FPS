using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NumericControls
{
    // pre-defined non-generic
    public class NumericArrayDouble : NumericArray<Double>
    {
        protected override NumericBox<Double> NumericBox()
        {
            return new NumericBoxDouble();
        } 
    }
    public class NumericArrayInt32 : NumericArray<Int32>
    {
        protected override NumericBox<Int32> NumericBox()
        {
            return new NumericBoxInt32();
        }
    }

    /// <summary>
    /// Generic numeric array inspired by LabView
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public abstract class NumericArray<T> : UserControl
        where T : IConvertible, IComparable, new()
    {
        /// <summary>
        /// Default constructor, produces an empty array 
        /// </summary>
        public NumericArray()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Constructor with an initial value
        /// </summary>
        /// <param name="value">Initial value</param>
        public NumericArray(T[] value)
        {
            this.Value = value;
            InitializeComponent();
        }

        protected abstract NumericBox<T> NumericBox();


        #region Appearance

        /// <summary>
        /// Template for all elements
        /// </summary>
        public NumericBox<T> TemplateElement;

        // child controls
        private NumericUpDown index = new NumericUpDown();
        private NumericBox<T>[] elements = new NumericBox<T>[1];

        // to avoid circular reference with SizeAndLocation
        private Boolean resize_handled = false;

        private Int32 na_elementsvisible = 1;
        /// <summary>
        /// Determines how many elements are visible
        /// </summary>
        public Int32 ElementsVisible
        {
            get { return na_elementsvisible; }
            set
            {
                resize_handled = true;
                this.SuspendLayout();
                if (value < na_elementsvisible)
                {
                    for (Int32 i = value; i < na_elementsvisible; i++)
                        elements[i].Dispose();
                }
                Array.Resize(ref elements, value);
                for (Int32 i = na_elementsvisible; i < value; i++)
                {
                    elements[i] = NumericBox();
                    CloneElement(elements[i]);
                    if (na_flow == Orientation.Vertical) elements[i].Location = elements[i - 1].Location + new Size(0, TemplateElement.Height + 1);
                    else elements[i].Location = elements[i - 1].Location + new Size(TemplateElement.Width + 1, 0);
                    elements[i].ValueChanged += new EventHandler(NumericArray_ValueChanged);
                    this.Controls.Add(elements[i]);
                }
                na_elementsvisible = value;
                PopulateControl();
                this.ResumeLayout(true);
                resize_handled = false;
            }
        }

        private Orientation na_flow = Orientation.Vertical;
        /// <summary>
        /// Vertical of horisontal flow
        /// </summary>
        public Orientation ElementsFlow
        {
            get { return na_flow; }
            set { na_flow = value; this.ElementsVisible = na_elementsvisible; }
        }

        private Boolean _emptyelsvisible = true;
        /// <summary>
        /// Determines whether to show empty elements
        /// </summary>
        public Boolean EmptyElementsVisible
        {
            get { return _emptyelsvisible; }
            set { _emptyelsvisible = value; this.ElementsVisible = na_elementsvisible; }
        }

	
        #endregion
	
        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.index.UpDownAlign = LeftRightAlignment.Left;
            this.index.TextAlign = HorizontalAlignment.Right;
            this.index.Size = new Size(50,20);
            this.index.Location = new Point(0, 0);
            this.index.Minimum = (Decimal)(0);
            this.index.Maximum = (Decimal)(Int32.MaxValue);
            this.index.ValueChanged += new EventHandler(delegate { PopulateControl(); });
            MenuItem na_empty = new MenuItem("Empty Array", new EventHandler(EmptyArray));
            this.index.ContextMenu = new ContextMenu();
            this.index.ContextMenu.MenuItems.Add(na_empty);
            this.Controls.Add(index);

            TemplateElement = NumericBox();
            TemplateElement.Width = 50;
            TemplateElement.SpinBoxVisible = false;
            elements[0] = NumericBox();
            elements[0].Location = new Point(index.Width + 1, 0);
            CloneElement(elements[0]);
            elements[0].ValueChanged += new EventHandler(NumericArray_ValueChanged);
            this.Controls.Add(elements[0]);
            PopulateControl();

            this.ResumeLayout(true);
        }

        /// <summary>
        /// Sets some properties to be equal to those of TemplateElement 
        /// </summary>
        /// <param name="new_element">Control to be modified</param>
        private void CloneElement(NumericBox<T> new_element)
        {
            new_element.Size = TemplateElement.Size;
            new_element.SpinBoxVisible = TemplateElement.SpinBoxVisible;
            new_element.SpinBoxAlignment = TemplateElement.SpinBoxAlignment;
            new_element.IsIndicator = TemplateElement.IsIndicator;
            new_element.BackColor = TemplateElement.BackColor;
            new_element.ForeColor = TemplateElement.ForeColor;
            new_element.MinValue = TemplateElement.MinValue;
            new_element.MaxValue = TemplateElement.MaxValue;
            new_element.Increment = TemplateElement.Increment;
            new_element.StrictIncrement = TemplateElement.StrictIncrement;
            new_element.Format = TemplateElement.Format;
        }
        /// <summary>
        /// Applies CloneElement to all elements
        /// </summary>
        public void ApplyTemplate()
        {
            foreach (NumericBox<T> e in elements) CloneElement(e);
            PopulateControl();
        }

        #region Size and Location

        /// <summary>
        /// Default size of NumericArray
        /// </summary>
        protected override Size DefaultSize
        {
            get { return new Size(101, this.index.Height);}
        }
        /// <summary>
        /// Size of the indexer (to hide, set width = 0)
        /// </summary>
        public Size IndexSize
        {
            get { return this.index.Size; }
            set { this.index.Size = value; }
        }
		
        /// <summary>
        /// Arrange all elements and adjust the total size.
        /// </summary>
        protected void SizeAndLocation()
        {
            if (this.na_flow == Orientation.Vertical)
            {
                TemplateElement.Width = this.Width - index.Width - 1;
                foreach (NumericBox<T> e in elements)
                {
                    e.Width = TemplateElement.Width;
                    e.Location = new Point(index.Width + 1, e.Location.Y);
                }
                if (!resize_handled)
                {
                    Int32 new_elements_visible = Math.Max(1, (this.Height + 1) / (TemplateElement.Height + 1));
                    this.ElementsVisible = new_elements_visible;
                }
                this.Height = na_elementsvisible * (TemplateElement.Height + 1) - 1;
            }
            else
            {
                TemplateElement.Width = (this.Width - index.Width) / (na_elementsvisible) - 1;
                for (Int32 i = 0; i < elements.Length; i++)
                {
                    elements[i].Width = TemplateElement.Width;
                    elements[i].Location = new Point(index.Width + 1 + i * (TemplateElement.Width + 1), 0);
                }
                this.Height = TemplateElement.Height;
            }

        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (levent.AffectedProperty != "Visible") SizeAndLocation();
            base.OnLayout(levent);
        }

        #endregion

        #region Value

        // default value = empty array
        private T[] na_value = new T[0];
        /// <summary>
        /// Get or set all elements
        /// </summary>
        public T[] Value
        {
            get { return na_value; }
            set { na_value = value; PopulateControl(); }
        }

        /// <summary>
        /// Get or set one element
        /// </summary>
        /// <param name="na_index">index</param>
        /// <returns>value</returns>
        public T this[Int32 na_index]
        {
            get { return this.na_value[na_index]; }
            set 
            {
                if (na_index >= na_value.Length)
                {
                    Int32 old_length = na_value.Length;
                    Array.Resize(ref na_value, na_index + 1);
                    for (Int32 i = old_length; i < na_index; i++) na_value[i] = this.TemplateElement.Value;
                }
                this.na_value[na_index] = value;
                PopulateControl();
            }
        }

        /// <summary>
        /// Shows values
        /// </summary>
        private void PopulateControl()
        {
            Int32 i;
            for (i = 0; i < Math.Min(na_elementsvisible, na_value.Length - index.Value); i++)
            {
                elements[i].Value = na_value[i + (Int32)index.Value];
                elements[i].BackColor = TemplateElement.BackColor;
                elements[i].Visible = true;
            }
            for (; i < na_elementsvisible; i++)
            {
                elements[i].Value = TemplateElement.Value;
                elements[i].BackColor = Color.LightGray;
                elements[i].Visible = _emptyelsvisible;
            }
        }
        /// <summary>
        /// Occurs when a USER changes elements' values
        /// </summary>
        public event EventHandler ValueChanged;
        protected void OnValueChanged() { if (ValueChanged != null) ValueChanged(this, new EventArgs()); }

        private void NumericArray_ValueChanged(object sender, EventArgs e)
        {
            Int32 i = Array.IndexOf(elements, sender) + (Int32)this.index.Value;
            this[i] = elements[i - (Int32)this.index.Value].Value;
            OnValueChanged();
        }
        /// <summary>
        /// Empty array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmptyArray(object sender, EventArgs e)
        {
            this.Value = new T[0];
            OnValueChanged();
        }
        #endregion


    }
}
