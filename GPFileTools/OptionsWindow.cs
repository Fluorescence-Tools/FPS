using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GPFileTools
{

    public partial class OptionsWindow : Form
    {
        private Dictionary<String, Dictionary<String, OptionsManager.Option>> allopts_local;
        // needed to call load/save methods
        private OptionsManager om_local = new OptionsManager();
        // original options list
        private Dictionary<String, Dictionary<String, OptionsManager.Option>> allopts_original_ref;
        private String previous_selection = "";

        private Boolean owAdvancedView = false;
        /// <summary>
        /// Determines whether to show Load and Save buttons, and whether to allow user to delete records
        /// </summary>
        public Boolean AdvancedView
        {
            get { return owAdvancedView; }
            set {
                if (!value)
                {
                    this.buttonLoad.Hide();
                    this.buttonSave.Hide();
                    this.optionsGridView.AllowUserToDeleteRows = false;
                }
                else
                {
                    this.buttonLoad.Show();
                    this.buttonSave.Show();
                    this.optionsGridView.AllowUserToDeleteRows = true;
                }
                owAdvancedView = value;
            }
        }


        public OptionsWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Constructor with an initial options list
        /// </summary>
        /// <param name="allopts_original">Initial options list</param>
        public OptionsWindow(Dictionary<String, Dictionary<String, OptionsManager.Option>> allopts_original)
        {
            InitializeComponent();

            allopts_local = new Dictionary<string, Dictionary<string, OptionsManager.Option>>(allopts_original);
            allopts_original_ref = allopts_original;
        }

        // on load, populate controls
        private void OptionsWindow_Load(object sender, EventArgs e)
        {
            this.AdvancedView = this.owAdvancedView;

            this.PopulateLeft();
            if (this.categorieslist.Items.Count > 0) this.categorieslist.SelectedIndex = 0;
        }
        private void OptionsWindow_Layout(object sender, LayoutEventArgs e)
        {
            this.SuspendLayout();

            this.categorieslist.Height = this.Height - 97;
            this.optionsGridView.Height = this.Height - 97;

            this.categorieslist.Width = this.Width / 3 - categorieslist.Location.X - categorieslist.Margin.Right;
            this.optionsGridView.Location = new Point(this.categorieslist.Right +
                categorieslist.Margin.Right + optionsGridView.Margin.Left, optionsGridView.Location.Y);
            this.optionsGridView.Width = 2 * this.Width / 3 - 20 - optionsGridView.Margin.Left;

            this.optionsGridView.Columns[0].Width = optionsGridView.Width / 3;
            this.optionsGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.ResumeLayout(true);
        }


        private System.Reflection.MethodInfo Parse_MI = null;
        private Boolean ToDisplay(OptionsManager.Option o) { return o.Display; }

        /// <summary>
        /// Populate left panel
        /// </summary>
        private void PopulateLeft()
        {
            if (allopts_local == null) return;
            OptionsManager.Option[] ovs;
            foreach (KeyValuePair<String, Dictionary<String, OptionsManager.Option>> os in allopts_local)
            {
                ovs = new OptionsManager.Option[os.Value.Count];
                os.Value.Values.CopyTo(ovs, 0);
                if(Array.Exists(ovs, ToDisplay) || this.owAdvancedView)
                    this.categorieslist.Items.Add(os.Key);
            }
        }

        /// <summary>
        /// Populate right panel
        /// </summary>
        /// <param name="orgs">Options list (one category)</param>
        private void PopulateRight(Dictionary<String,OptionsManager.Option> os)
        {
            this.optionsGridView.SuspendLayout();
            this.optionsGridView.Rows.Clear();
            foreach (OptionsManager.Option o in os.Values)
            {
                if ((!o.Display) && (!owAdvancedView)) continue; 

                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(this.optionsGridView);
                r.Cells[0].Value = o.Description;
                r.Cells[0].ToolTipText = o.HelpText;
                r.Cells[1].ToolTipText = o.HelpText;

                if (o.Value == null)
                {
                    r.Cells[1].Value = "(null)";
                    r.Cells[1].ReadOnly = true;
                    r.Cells[1].Style.BackColor = Color.Beige;
                }
                else if (o.Value.GetType().IsEnum)
                {
                    r.Cells[1] = new DataGridViewComboBoxCell();
                    ((DataGridViewComboBoxCell)r.Cells[1]).Items.AddRange(Enum.GetNames(o.Value.GetType()));
                    r.Cells[1].Value = Enum.GetName(o.Value.GetType(), o.Value);
                }
                else if (IsNumericType(o.Value.GetType(), ref Parse_MI) || (o.Value is String))
                {
                    r.Cells[1].Value = o.Value.ToString();
                }
                else if (o.Value is Boolean)
                {

                    r.Cells[1] = new DataGridViewCheckBoxCell();
                    r.Cells[1].Value = o.Value;
                }
                else
                {
                    r.Cells[1].Value = o.Value.ToString();
                    r.Cells[1].ReadOnly = true;
                    r.Cells[1].Style.BackColor = Color.Beige;
                }

                this.optionsGridView.Rows.Add(r);        
            }
            this.optionsGridView.ResumeLayout();
        }

        /// <summary>
        /// Copy user's settings to the local options list
        /// </summary>
        /// <param name="orgs">Options list (one category)</param>
        /// <returns>true if all values are valid</returns>
        private Boolean OApplyLocal(Dictionary<String, OptionsManager.Option> os)
        {
            Boolean parse_error = false;

            foreach (DataGridViewRow r in optionsGridView.Rows)
            {
                OptionsManager.Option o = os[r.Cells[0].Value.ToString()];

                if (o.Value == null) continue;
                try
                {

                    if (o.Value.GetType().IsEnum)
                        o.Value = Enum.Parse(o.Value.GetType(), r.Cells[1].Value.ToString(), true);
                    else if (IsNumericType(o.Value.GetType(), ref Parse_MI))
                        o.Value = Parse_MI.Invoke(o.Value, new object[] { r.Cells[1].Value.ToString() });
                    else if ((o.Value is Boolean) || (o.Value is String))
                        o.Value = r.Cells[1].Value;
                }
                catch
                {
                    r.Cells[1].Style.BackColor = Color.Red;
                    r.Cells[1].Style.SelectionBackColor = Color.Red;
                    parse_error = true; 
                    continue;
                }

                os[r.Cells[0].Value.ToString()] = o;
            }
            if (parse_error)
            {
                optionsGridView.Refresh();
                System.Threading.Thread.Sleep(500);
                foreach (DataGridViewRow r in this.optionsGridView.Rows)
                {
                    r.Cells[1].Style.BackColor = this.optionsGridView.DefaultCellStyle.BackColor;
                    r.Cells[1].Style.SelectionBackColor = this.optionsGridView.DefaultCellStyle.SelectionBackColor;
                }
                optionsGridView.Refresh();
            }

            return !parse_error;


        }
        /// <summary>
        /// Determine if an object is of a numeric type
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="Parse_MI">MethodInfo of "Parse" method of this type</param>
        /// <returns>true if numeric</returns>
        private Boolean IsNumericType(Type t, ref System.Reflection.MethodInfo Parse_MI)
        {
            switch (t.Name)
            {
                case "short":
                case "int":
                case "long":
                case "ushort":
                case "uint":
                case "ulong":
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                    Parse_MI = t.GetMethod("Parse", new Type[] { typeof(String) });
                    return true;
            }
            return false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (OApplyLocal(allopts_local[categorieslist.SelectedItem.ToString()]))
            {
                allopts_original_ref = allopts_local;
                if (this.owAdvancedView) buttonSave_Click(sender, e);
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (OApplyLocal(allopts_local[categorieslist.SelectedItem.ToString()])) allopts_original_ref = allopts_local;
        }

        private void categorieslist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categorieslist.SelectedItem == null) return;
            if (allopts_local.ContainsKey(previous_selection)) OApplyLocal(allopts_local[previous_selection]);

            previous_selection = categorieslist.SelectedItem.ToString();
            PopulateRight(allopts_local[categorieslist.SelectedItem.ToString()]);
        }


        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (om_local.Load(Application.StartupPath, ref allopts_local))
            {
                this.Text = om_local.ApplicationName + " options";
                this.categorieslist.Items.Clear();
                previous_selection = "";
                PopulateLeft();
                this.categorieslist.SelectedIndex = 0;
            } 
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!OApplyLocal(allopts_local[categorieslist.SelectedItem.ToString()])) return;
            om_local.SaveOrExport(Application.StartupPath, allopts_local);
        }

        // delete one option
        private void optionsGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            allopts_local[previous_selection].Remove(e.Row.Cells[0].Value.ToString());
        }

        // delete a category
        private void categorieslist_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode.Equals(Keys.Delete) || e.KeyCode.Equals(Keys.Back)) && this.owAdvancedView)
            {
                if (categorieslist.SelectedItem == null) return;
                String sr = categorieslist.SelectedItem.ToString();
                categorieslist.Items.Remove(categorieslist.SelectedItem);
                allopts_local.Remove(sr);
                if (categorieslist.Items.Count > 0) categorieslist.SelectedIndex = 0;
                else this.optionsGridView.Rows.Clear();
            }
        }

    }
}