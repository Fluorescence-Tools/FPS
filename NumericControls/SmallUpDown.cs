using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NumericControls
{
    // Small up-down control for using within NumericBox-es

    public class SmallUpDown : Control
    {
        public SmallUpDown()
        {
            InitializeComponent();
        }
        ~SmallUpDown()
        {
            sbticker.Dispose();
            sbbrush.Dispose();
            sbpen.Dispose();
        }

        // used to generate UpButtonPressed and DownButtonPressed events when mouse button is pressed and hold
        private Timer sbticker = new Timer();

        // brushes
        private LinearGradientBrush sbbrush;
        private Pen sbpen;
        private Bitmap myimage;

        private void InitializeComponent()
        {
            this.ForeColor = SystemColors.ControlDarkDark;
            this.BackColor = SystemColors.Control;
            this.TabStop = false;
            this.sbticker.Interval = 400;
            this.sbticker.Tick += new EventHandler(sbticker_Tick);
            sbpen = new Pen(new SolidBrush(SystemColors.ControlDarkDark), 1.0F);
            Redraw();
            this.PerformLayout();
        }

        // regions which cause Up/DownButtonPressed events on click
        private Rectangle URectangle = new Rectangle();
        private Rectangle LRectangle = new Rectangle();

        // occur when Up or Down buttons are pressed
        public event EventHandler UpButtonPressed;
        public event EventHandler DownButtonPressed;
        protected void OnUpButtonPressed() { if (UpButtonPressed!=null) UpButtonPressed(this, new EventArgs()); }
        protected void OnDownButtonPressed() { if (DownButtonPressed != null) DownButtonPressed(this, new EventArgs()); }
      
        // linked to last pressed button
        protected delegate void LastButtonPressed();
        LastButtonPressed OnLastButtonPressed = null;
        private void sbticker_Tick(object sender, EventArgs e)
        {
            if (OnLastButtonPressed != null)
            {
                OnLastButtonPressed();
                sbticker.Interval = 80;
            }
        }

        protected override Size DefaultSize 
        {
            get { return new Size(11,20);  }  
        }

        protected override void OnResize(EventArgs e)
        {
            Redraw();
            base.OnResize(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (levent.AffectedProperty != "Visible")
            {
                if (this.Width % 2 == 0) this.Width++;
                this.URectangle.Location = new Point(0, 0);
                this.URectangle.Size = new Size(this.Width, this.Height / 2);
                this.LRectangle.Location = new Point(0, this.Height - this.Height / 2);
                this.LRectangle.Size = this.URectangle.Size;
            }
            base.OnLayout(levent);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (URectangle.Contains(e.Location)) OnLastButtonPressed = OnUpButtonPressed;
            if (LRectangle.Contains(e.Location)) OnLastButtonPressed = OnDownButtonPressed;
            OnLastButtonPressed();
            sbticker.Start();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            sbticker.Interval = 400;
            sbticker.Stop();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            sbticker.Interval = 400;
            sbticker.Stop();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.DrawImage(myimage, new Point(0, 0));

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void Redraw()
        {
            myimage = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(myimage);

            // Border
            if (Application.RenderWithVisualStyles)
                ControlPaint.DrawVisualStyleBorder(g, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            else ControlPaint.DrawBorder3D(g, new Rectangle(0, 0, this.Width, this.Height), Border3DStyle.RaisedOuter);

            // triangles
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // new brushes
            sbbrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(this.Width, this.Height),
                SystemColors.ControlLight, SystemColors.ControlDarkDark);

            // upper triangle
            Point u1 = new Point(2, this.Height / 2 - 2);
            Point u2 = new Point(this.Width - 3, this.Height / 2 - 2);
            Point u3 = new Point(this.Width / 2, 2);
            g.DrawPolygon(sbpen, new Point[] { u1, u2, u3 });
            g.FillPolygon(sbbrush, new Point[] { u1, u2, u3 });

            // lower triangle
            Point l1 = new Point(2, this.Height / 2 + 1);
            Point l2 = new Point(this.Width - 3, this.Height / 2 + 1);
            Point l3 = new Point(this.Width / 2, this.Height - 3);
            g.DrawPolygon(sbpen, new Point[] { l3, l1, l2 });
            g.FillPolygon(sbbrush, new Point[] { l1, l2, l3 });
        }

    }
}
