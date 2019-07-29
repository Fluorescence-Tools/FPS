using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace GPFileTools
{

    public partial class OptionsManager : GPFileToolsBase
    {
       
        [Serializable]
        public struct Option
        {
            public object Value;
            public String Description;
            public String HelpText;
            public Boolean Display;
            public Boolean Enabled;

            public Option(object v, String d, Boolean ds)
            {
                this.Value = v;
                this.Description = d;
                this.HelpText = d;
                this.Display = ds;
                this.Enabled = true;
            }

            public Option(object v, String d, Boolean ds, Boolean enabled)
            {
                this.Value = v;
                this.Description = d;
                this.HelpText = d;
                this.Display = ds;
                this.Enabled = enabled;
            }
        }

        private Dictionary<String, Dictionary<String, Option>> allopts;
        private Dictionary<String, Dictionary<String, Option>> allopts_buffer;
        private Dictionary<String, Dictionary<String, object>> origin;
        private Dictionary<String, String> relevant_props;
        private Byte[] app_name;
        private Byte[] app_ver;
        private const Int32 app_name_ver_length = 64;
        private const String versiontag = "#VERSION "; 

        public OptionsManager()
        {
            this.ApplicationName = Application.ProductName;
            this.ApplicationVersion = Application.ProductVersion;

            this.Extension = "bin";
            this.FileFilter = "Binary files (*.bin)|*.bin|Text files (*.txt, *.dat)|*.txt;*.dat|All files (*.*)|*.*";
            allopts = new Dictionary<String, Dictionary<String, Option>>();
            origin = new Dictionary<String, Dictionary<String, object>>();

            this.OverwritePrompt = false;

            relevant_props = new Dictionary<String, String>();
            relevant_props.Add("CheckBox", "Checked");
            relevant_props.Add("ComboBox", "SelectedItem");
            relevant_props.Add("NumericUpDown", "Value");
            relevant_props.Add("NumericBoxInt32", "Value");
            relevant_props.Add("NumericBoxDouble", "Value");
            relevant_props.Add("NumericArrayInt32", "Value");
            relevant_props.Add("NumericArrayDouble", "Value");
        }
        ~OptionsManager() { }

        private Boolean omOldFileWarning = true;
        /// <summary>
        /// Determines if a warning should be issued when loaded file is not fully compatible
        /// </summary>
        public Boolean OldFileWarning
        {
            get { return omOldFileWarning; }
            set { omOldFileWarning = value; }
        }
        /// <summary>
        /// Application name to be added to saved files
        /// </summary>
        public String ApplicationName
        {
            get { return Encoding.Unicode.GetString(app_name).Trim(new Char[] { '\0' }); }
            set
            {
                this.app_name = Encoding.Unicode.GetBytes(value.ToCharArray());
                Array.Resize(ref app_name, app_name_ver_length);
            }
        }
        /// <summary>
        /// Application version to be added to saved files
        /// </summary>
        public String ApplicationVersion
        {
            set
            {
                this.app_ver = Encoding.Unicode.GetBytes((versiontag + value).ToCharArray());
                Array.Resize(ref app_ver, app_name_ver_length);
            }
        }

        #region Add and Remove

        public void AddCategory(String categoryname)
        {
            if (!allopts.ContainsKey(categoryname))
            {
                allopts.Add(categoryname, new Dictionary<String, Option>());
                origin.Add(categoryname, new Dictionary<String, object>());
            }
        }
        public void RemoveCategory(String categoryname)
        {
            if (allopts.ContainsKey(categoryname))
            {
                allopts.Remove(categoryname);
                origin.Remove(categoryname);
            }
        }

        /// <summary>
        /// Adds all object's properties
        /// </summary>
        /// <param name="org">any object</param>
        /// <param name="categoryname">to which category the properties should be added</param>
        public void AddObject(object o, String categoryname)
        {
            if (!allopts.ContainsKey(categoryname)) this.AddCategory(categoryname);
            Boolean default_display = !(o is Control);
            System.Reflection.PropertyInfo[] p = o.GetType().GetProperties();
            for (Int32 i = 0; i < p.Length; i++)
            {
                if (p[i].PropertyType.IsSerializable)
                {
                    origin[categoryname].Add(p[i].Name, o);
                    allopts[categoryname].Add(p[i].Name, new Option(p[i].GetValue(o, null), p[i].Name, default_display));
                }
            }
        }

        /// <summary>
        /// Adds a selected object's property
        /// </summary>
        /// <param name="org">any object</param>
        /// <param name="categoryname">to which category the property should be added</param>
        /// <param name="propname">property name</param>
        /// <param name="display">whether to display this option in "normal" options window</param>
        public void AddProperty(object o, String categoryname, String propname, Boolean display)
        {
            if (!allopts.ContainsKey(categoryname)) this.AddCategory(categoryname);

            System.Reflection.PropertyInfo p = o.GetType().GetProperty(propname);
            origin[categoryname].Add(propname, o);
            allopts[categoryname].Add(propname, new Option(p.GetValue(o, null), propname, display));
        }
        public void AddProperty(object o, String categoryname, String propname)
        {
            AddProperty(o, categoryname, propname, true);
        }

        public void RemoveProperty(String categoryname, String propname)
        {
            if (!allopts.ContainsKey(categoryname)) return;
            if (allopts[categoryname].ContainsKey(propname)) allopts[categoryname].Remove(propname);
            if (origin[categoryname].ContainsKey(propname)) origin[categoryname].Remove(propname);
        }

        /// <summary>
        /// Adds a relevant property of control (default = "Text")
        /// </summary>
        /// <param name="c">Control</param>
        /// <param name="categoryname">Category name</param>
        public void AddControl(Control c, String categoryname)
        {
            if (!allopts.ContainsKey(categoryname)) this.AddCategory(categoryname);

            String propname = "";
            if (!relevant_props.TryGetValue(c.GetType().Name, out propname)) propname = "Text";

            System.Reflection.PropertyInfo p = c.GetType().GetProperty(propname);
            origin[categoryname].Add(c.Name, c);
            allopts[categoryname].Add(c.Name, new Option(p.GetValue(c, null), c.Name, false, c.Enabled));
        }

        public void AddControlRange(Control[] cs, String categoryname)
        {
            foreach (Control c in cs) AddControl(c, categoryname);
        }
        public void AddControlRange(Control.ControlCollection cs, String categoryname)
        {
            foreach (Control c in cs) AddControl(c, categoryname);
        }

        /// <summary>
        /// Adds all child controls. Category name is the same as Control's name
        /// </summary>
        /// <param name="c">Control</param>
        public void AddCompositeControl(Control c)
        {
            Control[] c_children = new Control[c.Controls.Count];
            Int32[] tab_indices = new Int32[c.Controls.Count];
            Int32 i = 0;
            foreach (Control c_child in c.Controls)
            {
                tab_indices[i] = c_child.TabIndex;
                c_children[i++] = c_child;
            }
            Array.Sort(tab_indices, c_children);
            AddControlRange(c_children, c.Name);
        }
        /// <summary>
        /// Adds help text which appears as a tooltip
        /// </summary>
        /// <param name="categoryname">Category name</param>
        /// <param name="propertyname">Property name</param>
        /// <param name="helptext">New help text</param>
        public void AddHelp(String categoryname, String propertyname, String helptext)
        {
            if (allopts.ContainsKey(categoryname) && allopts[categoryname].ContainsKey(propertyname))
            {
                Option o = allopts[categoryname][propertyname];
                o.HelpText = helptext;
                allopts[categoryname][propertyname] = o;
            }
        }

        #endregion


        #region Load, Save, Export

        /// <summary>
        /// Updates all option's values before saving, exporting or displaying window.
        /// </summary>
        public void Update()
        {
            System.Reflection.PropertyInfo p;
            Option onew;
            Type otype;
            foreach (KeyValuePair<String, Dictionary<String, object>> orgs in origin)
                foreach (KeyValuePair<String, object> org in orgs.Value)
                {
                    otype = org.Value.GetType();
                    onew = allopts[orgs.Key][org.Key];
                    p = otype.GetProperty(org.Key);
                    if ((p == null) && (org.Value is Control))
                    {
                        if (relevant_props.ContainsKey(otype.Name))
                            p = otype.GetProperty(relevant_props[otype.Name]);
                        else p = otype.GetProperty("Text");
                        onew.Enabled = ((Control)org.Value).Enabled;
                    }
                    if (p != null)
                    {
                        onew.Value = p.GetValue(org.Value, null);
                        allopts[orgs.Key][org.Key] = onew;
                    }
                }
        }

        /// <summary>
        /// Save options to a file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="allopts_local">Options list</param>
        /// <returns>true if successful</returns>
        public Boolean Save(String filename, Dictionary<String, Dictionary<String, Option>> allopts_local)
        {
            this.Update();
            filename = ValidFileSave(filename);
            if (filename == null) return false;

            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            GZipStream gzfs = new GZipStream(fs, CompressionMode.Compress);

            try
            {
                fs.Write(this.app_name, 0, app_name_ver_length);
                fs.Write(this.app_ver, 0, app_name_ver_length);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(gzfs, allopts_local);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving file" + filename + ": " + e.Message, 
                    "GPFileTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                gzfs.Close();
                fs.Close();
            }
            return true;
        }
        /// <summary>
        /// Save local options to a file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <returns>true if successful</returns>
        public Boolean Save(String filename)
        {
            return this.Save(filename, this.allopts);
        }
        /// <summary>
        /// Apply options
        /// </summary>
        /// <param name="allopts_local">Set of options</param>
        /// <returns>true if successfull</returns>
        public Boolean ApplyOptions(Dictionary<String, Dictionary<String, Option>> allopts_local)
        {
            Type otype;
            foreach (KeyValuePair<String, Dictionary<String, Option>> os in allopts_local)
                foreach (KeyValuePair<String, Option> o in os.Value)
                {
                    otype = origin[os.Key][o.Key].GetType();
                    System.Reflection.PropertyInfo p = otype.GetProperty(o.Key);
                    if ((p == null) && (origin[os.Key][o.Key] is Control))
                    {
                        if (relevant_props.ContainsKey(otype.Name))
                            p = otype.GetProperty(relevant_props[otype.Name]);
                        else p = otype.GetProperty("Text");
                        ((Control)origin[os.Key][o.Key]).Enabled = o.Value.Enabled;
                    }
                    if (p != null && p.CanWrite)
                        try
                        {
                            p.SetValue(origin[os.Key][o.Key], o.Value.Value, null);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Error processing " + os.Key + "." +
                                o.Key + " = " + o.Value.Value.ToString() + ": \n" + e.Message,
                                "GPFileTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                } 

            return true;      
        }

        /// <summary>
        /// Load options from file and apply (if called locally)
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="allopts_local">Options list</param>
        /// <returns>true if successful</returns>
        public Boolean Load(String filename, ref Dictionary<String, Dictionary<String, Option>> allopts_local)
        {
            Boolean external_call = (allopts_local != this.allopts);
            // read data to buffer first
            filename = ValidFileOpen(filename);
            if (filename == null) return false;

            FileStream fs = new FileStream(filename, FileMode.Open);
            GZipStream gzfs = new GZipStream(fs, CompressionMode.Decompress);
            Byte[] file_app_name = new Byte[app_name_ver_length];
            Byte[] file_app_ver = new Byte[app_name_ver_length];
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                fs.Read(file_app_name, 0, app_name_ver_length);
                fs.Read(file_app_ver, 0, app_name_ver_length);
                String file_versionstring = Encoding.Unicode.GetString(file_app_ver);
                if (!file_versionstring.StartsWith(versiontag, StringComparison.Ordinal))  // old format
                    fs.Seek(-app_name_ver_length, SeekOrigin.Current);
                allopts_buffer = (Dictionary<String, Dictionary<String, Option>>)bf.Deserialize(gzfs);

                if (external_call) this.ApplicationName = Encoding.Unicode.GetString(file_app_name);
                if (Encoding.Unicode.GetString(app_name) != Encoding.Unicode.GetString(file_app_name))
                {
                    DialogResult dr = MessageBox.Show("File \"" + filename + "\" seems to be created by another application.\n Continue anyway?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dr.Equals(DialogResult.No)) return false;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file " + filename + ": " + e.Message,
                    "GPFileTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                gzfs.Close();
                fs.Close();
            }

            if (external_call)
            {
                allopts_local = allopts_buffer;
                return true;
            }

            // copy to allopts
            Int32 mismatches = 0;
            foreach (KeyValuePair<String, Dictionary<String, Option>> os in allopts_buffer)
            {
                if (allopts_local.ContainsKey(os.Key))
                {
                    mismatches += allopts_local[os.Key].Count;
                    foreach (KeyValuePair<String, Option> o in os.Value)
                        if (allopts_local[os.Key].ContainsKey(o.Key))
                        {
                            allopts_local[os.Key][o.Key] = o.Value;
                            mismatches--;
                        }
                        else mismatches++;
                }
                else mismatches += os.Value.Count;
            }

            // Warning if wrong file
            if (omOldFileWarning && (mismatches > 0))
            {
                DialogResult dr = MessageBox.Show("Old or incompatible file, " + mismatches.ToString() + " mismatches found. Continue anyway?",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr.Equals(DialogResult.No)) return false;
            }

            return this.ApplyOptions(allopts_local);
        }

        /// <summary>
        /// Load options from file and apply
        /// </summary>
        /// <param name="filename">File name</param>
        /// <returns>true if successful</returns>
        public Boolean Load(String filename)
        {
            return this.Load(filename, ref this.allopts);
        }

        /// <summary>
        /// Export as text file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="allopts_local">Options list</param>
        /// <returns>True if successful</returns>
        public Boolean Export(String filename, Dictionary<String, Dictionary<String, Option>> allopts_local)
        {
            this.Update();
            this.FileFilter = "Text files (*.txt, *.dat)|*.txt;*.dat|Binary files (*.bin)|*.bin|All files (*.*)|*.*";

            filename = ValidFileSave(filename);
            this.FileFilter = "Binary files (*.bin)|*.bin|Text files (*.txt, *.dat)|*.txt;*.dat|All files (*.*)|*.*";

            if (filename == null) return false;

            using (StreamWriter sr = new StreamWriter(filename, false))
            {
                sr.WriteLine("# " + this.ApplicationName + " v. " + Application.ProductVersion);
                sr.WriteLine("# " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                sr.WriteLine();

                foreach (KeyValuePair<String, Dictionary<String, Option>> os in allopts_local)
                {
                    sr.WriteLine("# " + os.Key);
                    foreach (KeyValuePair<String, Option> o in os.Value)
                        if ((o.Value.Value != null) && o.Value.Enabled)
                        {
                            sr.WriteLine(o.Key + " = " + o.Value.Value.ToString());
                            if (o.Value.Value.GetType().IsArray)
                            {
                                foreach (object ov in (o.Value.Value as System.Collections.IEnumerable))
                                    sr.Write(" " + ov.ToString());
                                sr.WriteLine();
                            }
                            else if (o.Value.Value.GetType().IsValueType && !o.Value.Value.GetType().IsEnum)  // most likely structure
                            {
                                foreach (System.Reflection.FieldInfo fi in o.Value.Value.GetType().GetFields())
                                    sr.WriteLine(" " + fi.Name + " = " + fi.GetValue(o.Value.Value).ToString());
                            }
                           
                        }
                        else if (o.Value.Enabled) sr.WriteLine(o.Key + " = (null)");
                    sr.WriteLine();
                }
            }
            return true;
        }

        /// <summary>
        /// Export as text file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <returns>True if successful</returns>
        public Boolean Export(String filename)
        {
            return this.Export(filename, this.allopts);
        }

        /// <summary>
        /// Save or export options, depending on file extension
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="allopts_local">Options list</param>
        /// <returns>true if successful</returns>
        public Boolean SaveOrExport(String filename, Dictionary<String, Dictionary<String, Option>> allopts_local)
        {
            filename = ValidFileSave(filename);
            if (filename == null) return false;
            FileInfo fi = new FileInfo(filename);
            if ((fi.Extension == ".txt") || (fi.Extension == ".dat")) return this.Export(filename, allopts_local);
            else return this.Save(filename, allopts_local);
        }

        #endregion

        /// <summary>
        /// Shows "Options" window
        /// </summary>
        /// <param name="advview">If true, shows advanced options</param>
        public void ShowOptionsWindow(Boolean advview)
        {
            this.Update();
            allopts_buffer = allopts;
            OptionsWindow ow = new OptionsWindow(allopts_buffer);
            ow.AdvancedView = advview;
            DialogResult dr = ow.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                allopts = allopts_buffer;
                ApplyOptions(this.allopts);
            }
        }
        public void ShowOptionsWindow() { ShowOptionsWindow(false); }

    }
}
