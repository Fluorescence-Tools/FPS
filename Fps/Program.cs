using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace Fps
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // fixes the . <-> , problem
            CultureInfo ci = (CultureInfo)Application.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = ci;

            int nt;
            // independent AV interface
            if (args.Length >= 1 && args[0].Length > 1 && (args[0][0] == '/' || args[0][0] == '-') &&
                args[0].Substring(1).Equals("av", StringComparison.OrdinalIgnoreCase))
                Application.Run(new AVinterface());

            // Main program with user-defined number of threads
            else if (args.Length >= 1 && args[0].Length > 1 && (args[0][0] == '/' || args[0][0] == '-') &&
                int.TryParse(args[0].Substring(1), out nt))
                Application.Run(new SpringTheoryMain(nt));

            // Main program with default number of threads
            else Application.Run(new SpringTheoryMain());
        }
    }
}