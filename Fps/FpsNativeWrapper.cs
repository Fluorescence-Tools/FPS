using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fps
{
    static public class FpsNativeWrapper
    {
        public enum NativePlatformID
        {
            Win32, Win64, Linux32, Linux64, Other
        };

        public delegate int AvCalculate1RDelegate(double L, double W, double R, int atom_i, double dg,
            double[] XLocal, double[] YLocal, double[] ZLocal,
            double[] vdWR, int NAtoms, double vdWRMax,
            double linkersphere, int linknodes, byte[] density);

        public delegate int AvCalculate3RDelegate(double L, double W, double R1, double R2, double R3,
            int atom_i, double dg, double[] XLocal, double[] YLocal, double[] ZLocal,
            double[] vdWR, int NAtoms, double vdWRMax,
            double linkersphere, int linknodes, byte[] density);

        public delegate double RdaMeanFromAvDelegate(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
            int nsamples, int rndseed);

        public delegate double RdaMeanEFromAvDelegate(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
            int nsamples, int rndseed, double R0);

        public delegate double CheckForClashesDelegate(IntPtr mol1xyzv, IntPtr mol2xyzv, AtomCluster[] ac1, AtomCluster[] ac2,
            int nclusters1, int nclusters2, int[] mustbechecked1, int[] mustbechecked2, Matrix3 rotation2to1, Vector3 dcm,
            double kclash, ref Vector3 clashforce, ref Vector3 clashtorque1, ref Vector3 clashtorque2);

        public static AvCalculate1RDelegate AvCalculate1R;
        public static AvCalculate3RDelegate AvCalculate3R;
        public static RdaMeanFromAvDelegate RdaMeanFromAv;
        public static RdaMeanEFromAvDelegate RdaMeanEFromAv;
        public static CheckForClashesDelegate CheckForClashes;

        static private NativePlatformID _currentPlatform = NativePlatformID.Other;
        static public NativePlatformID CurrentPlatform
        {
            get { return _currentPlatform; }
            set
            {
                _currentPlatform = value;
                InitRoutines();
            }
        }

        static FpsNativeWrapper()
        {
            InitNativePlatform();
            InitRoutines();
        }

        #region managed prototypes

        internal class fpsnative_win64
        {
            [DllImport("fpsnative.win64.dll", EntryPoint = "?calculate1R@@YAHNNNHNPEAN000HNNHPEAE@Z")]
            public static extern int calculate1R(double L, double W, double R, int atom_i, double dg,
                double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("fpsnative.win64.dll", EntryPoint = "?calculate3R@@YAHNNNNNHNPEAN000HNNHPEAE@Z")]
            public static extern int calculate3R(double L, double W, double R1, double R2, double R3,
                int atom_i, double dg, double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("fpsnative.win64.dll", EntryPoint = "?rdamean@@YANPEAUVector3@@H0HHH@Z")]
            public static extern double rdamean(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed);

            [DllImport("fpsnative.win64.dll", EntryPoint = "?rdameanE@@YANPEAUVector3@@H0HHHN@Z")]
            public static extern double rdameanE(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed, double R0);

            [DllImport("fpsnative.win64.dll", EntryPoint = "?checkforclashes@@YANPEAT__m128@@0PEAUAtomCluster@@1HHPEAH2UMatrix3@@UVector3@@NAEAU4@55@Z")]
            public static extern double checkforclashes(IntPtr mol1xyzv, IntPtr mol2xyzv, AtomCluster[] ac1, AtomCluster[] ac2,
                int nclusters1, int nclusters2, int[] mustbechecked1, int[] mustbechecked2, Matrix3 rotation2to1, Vector3 dcm,
                double kclash, ref Vector3 clashforce, ref Vector3 clashtorque1, ref Vector3 clashtorque2);

            [DllImport("fpsnative.win64.dll", EntryPoint = "?testnative@@YAHXZ")]
            public static extern int testnative();
        }

        internal class fpsnative_win32
        {
            [DllImport("fpsnative.win32.dll", EntryPoint = "?calculate1R@@YAHNNNHNPAN000HNNHPAE@Z")]
            public static extern int calculate1R(double L, double W, double R, int atom_i, double dg,
                double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("fpsnative.win32.dll", EntryPoint = "?calculate3R@@YAHNNNNNHNPAN000HNNHPAE@Z")]
            public static extern int calculate3R(double L, double W, double R1, double R2, double R3,
                int atom_i, double dg, double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("fpsnative.win32.dll", EntryPoint = "?rdamean@@YANPAUVector3@@H0HHH@Z")]
            public static extern double rdamean(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed);

            [DllImport("fpsnative.win32.dll", EntryPoint = "?rdameanE@@YANPAUVector3@@H0HHHN@Z")]
            public static extern double rdameanE(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed, double R0);

            [DllImport("fpsnative.win32.dll", EntryPoint = "?checkforclashes@@YANPEAT__m128@@0PEAUAtomCluster@@1HHPEAH2UMatrix3@@UVector3@@NAEAU4@55@Z")]
            public static extern double checkforclashes(IntPtr mol1xyzv, IntPtr mol2xyzv, AtomCluster[] ac1, AtomCluster[] ac2,
                int nclusters1, int nclusters2, int[] mustbechecked1, int[] mustbechecked2, Matrix3 rotation2to1, Vector3 dcm,
                double kclash, ref Vector3 clashforce, ref Vector3 clashtorque1, ref Vector3 clashtorque2);

            [DllImport("fpsnative.win32.dll", EntryPoint = "?testnative@@YAHXZ")]
            public static extern int testnative();
        }

        internal class fpsnative_linux64
        {
            [DllImport("libfpsnative.Linux64.so", EntryPoint = "calculate1R")]
            public static extern int calculate1R(double L, double W, double R, int atom_i, double dg,
                double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("libfpsnative.Linux64.so", EntryPoint = "calculate3R")]
            public static extern int calculate3R(double L, double W, double R1, double R2, double R3,
                int atom_i, double dg, double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("libfpsnative.Linux64.so", EntryPoint = "rdamean")]
            public static extern double rdamean(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed);

            [DllImport("libfpsnative.Linux64.so", EntryPoint = "rdameanE")]
            public static extern double rdameanE(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed, double R0);

            [DllImport("libfpsnative.Linux64.so", EntryPoint = "testnative")]
            public static extern int testnative();
        }

        internal class fpsnative_linux32
        {
            [DllImport("libfpsnative.Linux32.so", EntryPoint = "calculate1R")]
            public static extern int calculate1R(double L, double W, double R, int atom_i, double dg,
                double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("libfpsnative.Linux32.so", EntryPoint = "calculate3R")]
            public static extern int calculate3R(double L, double W, double R1, double R2, double R3,
                int atom_i, double dg, double[] XLocal, double[] YLocal, double[] ZLocal,
                double[] vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes, [In, Out] byte[] density);

            [DllImport("libfpsnative.Linux32.so", EntryPoint = "rdamean")]
            public static extern double rdamean(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed);

            [DllImport("libfpsnative.Linux32.so", EntryPoint = "rdameanE")]
            public static extern double rdameanE(Vector3[] av1, int av1length, Vector3[] av2, int av2length,
                int nsamples, int rndseed, double R0);

            [DllImport("libfpsnative.Linux32.so", EntryPoint = "testnative")]
            public static extern int testnative();
        }

        #endregion

        static private void InitNativePlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    if (Environment.Is64BitProcess) CurrentPlatform = NativePlatformID.Win64;
                    else CurrentPlatform = NativePlatformID.Win32;
                    break;
                case PlatformID.Unix: // 32-bit .so not ready!!!
                    if (Environment.Is64BitProcess) CurrentPlatform = NativePlatformID.Linux64;
                    else CurrentPlatform = NativePlatformID.Linux32;
                    break;
                default: CurrentPlatform = NativePlatformID.Other; break; // i.e. use managed code
            }

            // try to run a simple native function, if failed revert to "other"
            try
            {
                if ((CurrentPlatform == NativePlatformID.Win32 && FpsNativeWrapper.fpsnative_win32.testnative() != 7) |
                    (CurrentPlatform == NativePlatformID.Win64 && FpsNativeWrapper.fpsnative_win64.testnative() != 7) |
                    (CurrentPlatform == NativePlatformID.Linux32 && FpsNativeWrapper.fpsnative_linux32.testnative() != 7) |
                    (CurrentPlatform == NativePlatformID.Linux64 && FpsNativeWrapper.fpsnative_linux64.testnative() != 7))
                    throw new Exception("Cannot load dll");
            }
            catch
            {
                CurrentPlatform = NativePlatformID.Other;
                MessageBox.Show("Problem loading compiled FPS library.\nOn Windows install VC++ runtime for optimal performance",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        static private void InitRoutines()
        {
            switch (CurrentPlatform)
            {
                case NativePlatformID.Win32:
                    AvCalculate1R = fpsnative_win32.calculate1R;
                    AvCalculate3R = fpsnative_win32.calculate3R;
                    RdaMeanFromAv = fpsnative_win32.rdamean;
                    RdaMeanEFromAv = fpsnative_win32.rdameanE;
                    CheckForClashes = fpsnative_win32.checkforclashes;
                    break;
                case NativePlatformID.Win64:
                    AvCalculate1R = fpsnative_win64.calculate1R;
                    AvCalculate3R = fpsnative_win64.calculate3R;
                    RdaMeanFromAv = fpsnative_win64.rdamean;
                    RdaMeanEFromAv = fpsnative_win64.rdameanE;
                    CheckForClashes = fpsnative_win64.checkforclashes;
                    break;
                case NativePlatformID.Linux32:
                    AvCalculate1R = fpsnative_linux32.calculate1R;
                    AvCalculate3R = fpsnative_linux32.calculate3R;
                    RdaMeanFromAv = fpsnative_linux32.rdamean;
                    RdaMeanEFromAv = fpsnative_linux32.rdameanE;
                    break;
                case NativePlatformID.Linux64:
                    AvCalculate1R = fpsnative_linux64.calculate1R;
                    AvCalculate3R = fpsnative_linux64.calculate3R;
                    RdaMeanFromAv = fpsnative_linux64.rdamean;
                    RdaMeanEFromAv = fpsnative_linux64.rdameanE;
                    break;
                case NativePlatformID.Other:
                default:
                    AvCalculate1R = AVEngine.AvCalculate1R;
                    AvCalculate3R = AVEngine.AvCalculate3R;
                    RdaMeanFromAv = AVEngine.RdaMeanFromAv;
                    RdaMeanEFromAv = AVEngine.RdaMeanEFromAv;
                    break;
            }
        }

        // ensure 16-bit alignment
        static public IntPtr Aligned16(IntPtr original)
        {
            return original + (int)((original.ToInt64() % 16 == 0 ? 0 : 16 - original.ToInt64() % 16));
        }
    }
}
