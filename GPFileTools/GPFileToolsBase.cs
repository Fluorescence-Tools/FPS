using System;
using System.IO;
using System.Windows.Forms;

namespace GPFileTools
{
    /// <summary>
    /// Basic operations with files: dialogs, ...
    /// </summary>
    public class GPFileToolsBase
    {
        public GPFileToolsBase() { }
        ~GPFileToolsBase() { }

        private String Default_Ext = "txt";
        private String Default_Filter = "All files (*.*)|*.*|Text files (*.txt, *.dat)|*.txt;*.dat";
        private String Default_OpenTitle = "Open File:";
        private String Default_SaveTitle = "Save File:";
        private String Default_DirTitle = "Select a Directory:";
        private String Default_SelectTitle = "Select Files:";
        private Boolean Default_OWPrompt = true;

        #region Properties

        public String Extension
        {
            get { return Default_Ext; }
            set { Default_Ext = value; }
        }
        public String FileFilter
        {
            get { return Default_Filter; }
            set { Default_Filter = value; }
        }
        public String OpenTitle
        {
            get { return Default_OpenTitle; }
            set { Default_OpenTitle = value; }
        }
        public String SaveTitle
        {
            get { return Default_SaveTitle; }
            set { Default_SaveTitle = value; }
        }
        public String DirTitle
        {
            get { return Default_DirTitle; }
            set { Default_DirTitle = value; }
        }
        public String SelectTitle
        {
            get { return Default_SelectTitle; }
            set { Default_SelectTitle = value; }
        }
        public Boolean  OverwritePrompt
        {
            get { return Default_OWPrompt; }
            set { Default_OWPrompt = value; }
        }

	
        #endregion

        #region Open and save files

        public String ValidFileOpen(String start_fname)
        {
            String valid_fname = start_fname;

            if (!File.Exists(start_fname))
            {
                OpenFileDialog fdialog = new OpenFileDialog();
                if (Directory.Exists(Path.GetDirectoryName(start_fname))) fdialog.InitialDirectory = Path.GetDirectoryName(start_fname);
                else fdialog.InitialDirectory = Directory.GetCurrentDirectory();

                fdialog.CheckFileExists = true;
                fdialog.Multiselect = false;
                fdialog.Filter = Default_Filter;
                fdialog.DefaultExt = Default_Ext;
                fdialog.Title = Default_OpenTitle;

                DialogResult dr1 = fdialog.ShowDialog();
                if (dr1 != DialogResult.Cancel) valid_fname = fdialog.FileName;
                else valid_fname = null;
                fdialog.Dispose();
            }
            return valid_fname;
        }

        public String ValidFileSave(String start_fname)
        {
            String valid_fname = start_fname;
            DialogResult dr1;

            if (File.Exists(start_fname))
            {
                if (!Default_OWPrompt) return valid_fname;
                dr1 = MessageBox.Show("File " + start_fname + " already exists.\nDo you want to replace it?",
                    "Save File:", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr1 == DialogResult.Yes) return valid_fname;
            }

            SaveFileDialog fdialog = new SaveFileDialog();
            Stream tryopen;
            if (Directory.Exists(Path.GetDirectoryName(start_fname))) fdialog.InitialDirectory = Path.GetDirectoryName(start_fname);
            else fdialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (Directory.Exists(start_fname)) fdialog.FileName = String.Empty;
            else fdialog.FileName = Path.Combine(fdialog.InitialDirectory, Path.GetFileName(start_fname));

            if (!File.Exists(start_fname) && (fdialog.FileName!=String.Empty) && 
                !Directory.Exists(start_fname) && ((tryopen = fdialog.OpenFile()) != null))
            {
                tryopen.Close();
                tryopen.Dispose();
                fdialog.Dispose();
                return valid_fname;
            }
            fdialog.CheckFileExists = false;
            fdialog.OverwritePrompt = Default_OWPrompt;
            fdialog.Filter = Default_Filter;
            fdialog.DefaultExt = Default_Ext;
            fdialog.Title = Default_SaveTitle;

            dr1 = fdialog.ShowDialog();
            if (dr1 != DialogResult.Cancel) valid_fname = fdialog.FileName;
            else valid_fname = null;
            fdialog.Dispose();


            return valid_fname;
        }

        public String ValidDirectory(String start_dirname)
        {
            String valid_dirname = null;

            OpenFileDialog fdialog = new OpenFileDialog();
            if (Directory.Exists(Path.GetDirectoryName(start_dirname))) fdialog.InitialDirectory = Path.GetDirectoryName(start_dirname);
            else fdialog.InitialDirectory = Directory.GetCurrentDirectory();

            fdialog.CheckFileExists = false;
            fdialog.Multiselect = false;
            fdialog.Filter = "All files (*.*)|*.*";
            fdialog.Title = Default_DirTitle;
            fdialog.FileName = "tmp.txt";

            DialogResult dr1 = fdialog.ShowDialog();
            if (dr1 != DialogResult.Cancel) valid_dirname = Path.GetDirectoryName(fdialog.FileName);
            fdialog.Dispose();
            return valid_dirname;
        }

        public String[] ValidManyFiles(String start_fname)
        {
            String[] ManyFilesList = new String[0];

            OpenFileDialog fdialog = new OpenFileDialog();
            if (Directory.Exists(Path.GetDirectoryName(start_fname))) fdialog.InitialDirectory = Path.GetDirectoryName(start_fname);
            else fdialog.InitialDirectory = Directory.GetCurrentDirectory();

            fdialog.CheckFileExists = true;
            fdialog.Multiselect = true;
            fdialog.Filter = Default_Filter;
            fdialog.DefaultExt = Default_Ext;
            fdialog.Title = Default_SelectTitle;

            if (File.Exists(start_fname)) fdialog.FileName = start_fname;

            DialogResult dr1 = fdialog.ShowDialog();
            if (dr1 != DialogResult.Cancel) ManyFilesList = fdialog.FileNames;
            fdialog.Dispose();
            return ManyFilesList;
        }

        #endregion


        #region Find Parse and TryParse

        public delegate T ParseMethod<T>(String s);
        public delegate bool TryParseMethod<T>(String s, out T v);

        protected ParseMethod<T> FindParse<T>()
        {
            Type[] ParseArgs = { typeof(String) };
            System.Reflection.MethodInfo Parse_MI = typeof(T).GetMethod("Parse", ParseArgs);
            if (Parse_MI == null) throw new NotSupportedException("Parse is not a member of " + typeof(T).Name);
            return (ParseMethod<T>)Delegate.CreateDelegate(typeof(ParseMethod<T>), Parse_MI);
        }
        protected TryParseMethod<T> FindTryParse<T>()
        {
            Type[] TryParseArgs = { typeof(String), typeof(T).MakeByRefType() };
            System.Reflection.MethodInfo TryParse_MI = typeof(T).GetMethod("TryParse", TryParseArgs);
            if (TryParse_MI == null) throw new NotSupportedException("TryParse is not a member of " + typeof(T).Name);
            return (TryParseMethod<T>)Delegate.CreateDelegate(typeof(TryParseMethod<T>), TryParse_MI);
        }

        #endregion

    }

}
