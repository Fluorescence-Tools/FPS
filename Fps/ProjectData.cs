// here ara data definitions which are loaded/saved in project files
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fps
{
    public class ProjectData
    {
        public static string[] DockModes = new string[] { "Dock", "Refine", "Error estimation", "Sample" };
        public static string[] FilterModes = new string[] { "Screening" };

        public FPSMode ProjectFPSMode { get; set; }
        public string[] MoleculesPaths { get; set; }
        public string LabelingPositionsPath { get; set; }
        public string DistancesPath { get; set; }

        private Dictionary<string, FPSParameters> _fpsparameters;

        public FPSParameters DockParameters
        {
            get { return _fpsparameters["Dock"]; }
            set { _fpsparameters["Dock"] = value; }
        }

        public FPSParameters RefineParameters
        {
            get { return _fpsparameters["Refine"]; }
            set { _fpsparameters["Refine"] = value; }
        }

        public FPSParameters ErrorEstimationParameters
        {
            get { return _fpsparameters["Error estimation"]; }
            set { _fpsparameters["Error estimation"] = value; }
        }

        public FPSParameters SampleParameters
        {
            get { return _fpsparameters["Sample"]; }
            set { _fpsparameters["Sample"] = value; }
        }

        public FPSParameters ScreeningParameters
        {
            get { return _fpsparameters["Screening"]; }
            set { _fpsparameters["Screening"] = value; }
        }

        private ConversionParameters _conversionParameters;
        public ConversionParameters ConversionParameters
        {
            get { return _conversionParameters; }
            set { _conversionParameters = value; }
        }

        public Boolean[] SelectedDistances { get; set; }
        public AVGlobalParameters AVGlobalParameters { get; set; }

        // default values
        public ProjectData()
        {
            ProjectFPSMode = FPSMode.None;
            this._fpsparameters = new Dictionary<string, FPSParameters>(DockModes.Length + FilterModes.Length);

            // default search parameters
            FPSParameters dockParameters = new FPSParameters()
            {
                ViscosityFactor = 1.0,
                TimeStepFactor = 1.0,
                MaxIterations = 200000,
                MaxForce = 400.0,
                ClashTolerance = 1.0,
                rkT = 10,
                ETolerance = 100,
                KTolerance = 0.001,
                FTolerance = 0.001,
                TTolerance = 0.02,
                OptimizeSelected = OptimizeSelected.Selected
            };
            this._fpsparameters.Add("Dock", dockParameters);

            // default refine parameters
            FPSParameters refineParameters = new FPSParameters()
            {
                ViscosityFactor = 0.7,
                TimeStepFactor = 0.5,
                MaxIterations = 500000,
                MaxForce = 10000.0,
                ClashTolerance = 0.5,
                rkT = 10,
                ETolerance = 100,
                KTolerance = 0.0005,
                FTolerance = 0.0005,
                TTolerance = 0.01,
                OptimizeSelected = OptimizeSelected.All
            };
            this._fpsparameters.Add("Refine", refineParameters);

            // default error estimation parameters
            FPSParameters errorParameters = new FPSParameters()
            {
                ViscosityFactor = 0.7,
                TimeStepFactor = 1.0,
                MaxIterations = 100000,
                MaxForce = 10000.0,
                ClashTolerance = 0.5,
                rkT = 10,
                ETolerance = 100,
                KTolerance = 0.001,
                FTolerance = 0.001,
                TTolerance = 0.02,
                OptimizeSelected = OptimizeSelected.All
            };
            this._fpsparameters.Add("Error estimation", errorParameters);

            // default error estimation parameters
            FPSParameters sampleParameters = new FPSParameters()
            {
                ViscosityFactor = 1.0,
                TimeStepFactor = 1.0,
                MaxIterations = 8000,
                MaxForce = 400.0,
                ClashTolerance = 1.0,
                rkT = 10,
                ETolerance = 100,
                KTolerance = 0.001,
                FTolerance = 0.001,
                TTolerance = 0.02,
                OptimizeSelected = OptimizeSelected.Selected
            };
            this._fpsparameters.Add("Sample", sampleParameters);

            // default screening parameters
            FPSParameters screeningParameters = new FPSParameters()
            {
                ViscosityFactor = 1.0,
                TimeStepFactor = 1.0,
                MaxIterations = 200000,
                MaxForce = 400.0,
                ClashTolerance = 1.0,
                rkT = 10,
                ETolerance = 100,
                KTolerance = 0.001,
                FTolerance = 0.001,
                TTolerance = 0.02,
                OptimizeSelected = OptimizeSelected.Selected
            };
            this._fpsparameters.Add("Screening", screeningParameters);

            // default conversion parameters
            _conversionParameters.R0 = 52.0;
            _conversionParameters.PolynomOrder = 3;

            // default AV parameters
            AVGlobalParameters _avGlobalParameters = new AVGlobalParameters()
            {
                GridSize = 0.2,
                MinGridSize = 0.4,
                LinkerInitialSphere = 0.5,
                LinkSearchNodes = 3,
                ESamples = 200000
            };
            this.AVGlobalParameters = _avGlobalParameters;
        }

        public void SetFpsParameters(string key, FPSParameters value)
        {
            this._fpsparameters[key] = value;
        }

        public void SetFpsParameters(Dictionary<string, FPSParameters> value)
        {
            this._fpsparameters = new Dictionary<string, FPSParameters>(value);
        }

        public FPSParameters GetFpsParameters(string key)
        {
            return this._fpsparameters[key];
        }

        public Dictionary<string, FPSParameters> GetFpsParameters()
        {
            return new Dictionary<string, FPSParameters>(this._fpsparameters);
        }
    }


}