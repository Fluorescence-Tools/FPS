using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fps
{
    public partial class SimParametersForm : Form
    {
        private Dictionary<string, FPSParameters> _param;
        private Dictionary<string, FPSParameters> _lastsaved;
        private string _lastconfig;

        private ConversionParameters _convp;  // global for all configurations
        private ConversionParameters _convp_lastsaved;  // global for all configurations
        private AVGlobalParameters _avp;
        private AVGlobalParameters _avp_lastsaved;

        public ConversionParameters ConversionParameters
        {
            get { return _convp; }
            set { _convp = value; _convp_lastsaved = value; }
        }
        public AVGlobalParameters AVGridParameters
        {
            get { return _avp; }
            set { _avp = value; _avp_lastsaved = value; }
        }

        private void StoreCurrentValues(ref FPSParameters sp)
        {
            sp.ViscosityFactor = this.viscosity_numericBoxDouble.Value;
            sp.TimeStepFactor = this.dt_numericBoxDouble.Value;
            sp.MaxIterations = (Int32)(this.niter_numericBoxDouble.Value * 1000.0);
            sp.MaxForce = this.maxF_numericBoxDouble.Value;
            sp.ClashTolerance = this.clashtol_numericBoxDouble.Value;
            sp.rkT = this.rkt_numericBoxDouble.Value;
            sp.ETolerance = this.etol_numericBoxDouble.Value;
            sp.KTolerance = this.ktol_numericBoxDouble.Value;
            sp.FTolerance = this.ftol_numericBoxDouble.Value;
            sp.TTolerance = this.ttol_numericBoxDouble.Value;
            sp.OptimizeSelected = (OptimizeSelected)this.selR_comboBox.SelectedIndex;
            _convp.R0 = this.R0_numericBoxDouble.Value;
            _convp.PolynomOrder = this.polorder_numericBoxInt32.Value;
            _avp.ESamples = esamples_numericBoxInt32.Value * 1000;
            _avp.GridSize = avgrid_rel_numericBoxDouble.Value;
            _avp.MinGridSize = avgrid_min_numericBoxDouble.Value;
            _avp.LinkerInitialSphere = av_linkersphere_numericBoxDouble.Value;
            _avp.LinkSearchNodes = av_linksearchorder_numericBoxInt32.Value;
        }

        private void DisplayStoredValues(FPSParameters sp)
        {
            this.viscosity_numericBoxDouble.Value = sp.ViscosityFactor;
            this.dt_numericBoxDouble.Value = sp.TimeStepFactor;
            this.niter_numericBoxDouble.Value = sp.MaxIterations / 1000.0;
            this.maxF_numericBoxDouble.Value = sp.MaxForce;
            this.clashtol_numericBoxDouble.Value = sp.ClashTolerance;
            this.rkt_numericBoxDouble.Value = sp.rkT;
            this.etol_numericBoxDouble.Value = sp.ETolerance;
            this.ktol_numericBoxDouble.Value = sp.KTolerance;
            this.ftol_numericBoxDouble.Value = sp.FTolerance;
            this.ttol_numericBoxDouble.Value = sp.TTolerance;
            this.selR_comboBox.SelectedIndex = (Int32)sp.OptimizeSelected;
            this.R0_numericBoxDouble.Value = _convp.R0;
            this.polorder_numericBoxInt32.Value = _convp.PolynomOrder;
            this.avgrid_rel_numericBoxDouble.Value = _avp.GridSize;
            this.avgrid_min_numericBoxDouble.Value = _avp.MinGridSize;
            this.av_linkersphere_numericBoxDouble.Value = _avp.LinkerInitialSphere;
            this.av_linksearchorder_numericBoxInt32.Value = _avp.LinkSearchNodes;
            this.esamples_numericBoxInt32.Value = (Int32)(_avp.ESamples / 1000);
        }
	
        public SimParametersForm(ProjectData pd)
        {
            InitializeComponent();
            this._param = pd.GetFpsParameters();
            this._lastsaved = pd.GetFpsParameters();
            config_comboBox.Items.AddRange(ProjectData.DockModes);
            config_comboBox.Items.AddRange(ProjectData.FilterModes);
            config_comboBox.SelectedIndex = 0;
            clashtol_comboBox.SelectedIndex = 0;
            selR_comboBox.SelectedIndex = 0;
            _lastconfig = config_comboBox.SelectedItem.ToString();
        }

        public void SetFpsParameters(ProjectData pd)
        {
            this._param = pd.GetFpsParameters();
            this._lastsaved = pd.GetFpsParameters();
        }

        public Dictionary<string, FPSParameters> GetFpsParameters()
        {
            return new Dictionary<string, FPSParameters>(this._param);
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            // revert to saved values
            _param = _lastsaved;
            _convp = _convp_lastsaved;
            _avp = _avp_lastsaved;
        }

        private void SimParameters_Load(object sender, EventArgs e)
        {
            DisplayStoredValues(_param[config_comboBox.SelectedItem.ToString()]);
        }

        private void config_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // store current parameters, display new
            if (_lastconfig != null)
            {
                FPSParameters plocal = new FPSParameters();
                StoreCurrentValues(ref plocal);
                _param[_lastconfig] = plocal;
            }

            _lastconfig = config_comboBox.SelectedItem.ToString();
            DisplayStoredValues(_param[_lastconfig]);

            groupBox1.Enabled = (config_comboBox.SelectedIndex < ProjectData.DockModes.Length);
            groupBox2.Enabled = (config_comboBox.SelectedIndex < ProjectData.DockModes.Length);
            rkt_numericBoxDouble.Enabled = (config_comboBox.SelectedIndex == 3);
                
        }

        private void applybutton_Click(object sender, EventArgs e)
        {
            FPSParameters plocal = new FPSParameters();
            StoreCurrentValues(ref plocal);
            _param[config_comboBox.SelectedItem.ToString()] = plocal;

            // permanently save current values
            _lastsaved = new Dictionary<string, FPSParameters>(_param);
            _convp_lastsaved = _convp;
            _avp_lastsaved = _avp;
        }
    }
}