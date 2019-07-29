using System;

namespace Fps
{
    public enum OptimizeSelected
    {
        Selected, All, SelectedThenAll
    }

    [Serializable]
    public struct FPSParameters
    {
        public double ViscosityFactor;
        public double TimeStepFactor;
        public int MaxIterations;
        public double MaxForce;
        public double ClashTolerance;
        public double rkT;

        public double ETolerance;
        public double KTolerance;
        public double FTolerance;
        public double TTolerance;

        public OptimizeSelected OptimizeSelected;
    }

    /// <summary>
    /// A base class for all types of simulations (docking, refinement, metropolis, ...).
    /// Implements or declares only the minimum set of features needed for the job manager.
    /// </summary>
    public class SimulationBase : ICloneable
    {
        protected Random _rnd;
        protected MoleculeList _molecules;
        protected LabelingPositionList _labelingpositions;
        protected DistanceList _distances;
        protected SimulationResult _originalstate;

        public FPSParameters SimulationParameters { get; set; }

        public MoleculeList Molecules
        {
            get { return _molecules; }
            set
            {
                _molecules = new MoleculeList(value.Count);
                _molecules.AddRange(value);
            }
        }

        public LabelingPositionList LabelingPositions
        {
            get { return _labelingpositions; }
            set
            {
                _labelingpositions = new LabelingPositionList(value.Count);
                _labelingpositions.AddRange(value);
            }
        }

        public DistanceList Distances
        {
            get { return _distances; }
            set
            {
                _distances = new DistanceList(value.Count);
                _distances.AddRange(value);
            }
        }

        /// <summary>
        /// Indicates whether simmilation must run continuously (e.g. as in sampling).
        /// Set to true to prevent the job manager from calling SetState() between simulations.
        /// </summary>
        public virtual bool IsContinuous { get; protected set; }

        public SimulationBase()
        {
            InitRnd();
        }

        public SimulationBase(MoleculeList ms, LabelingPositionList lps, DistanceList ds)
        {
            this.Molecules = ms;
            this.LabelingPositions = lps;
            this.Distances = ds;
            InitRnd();
        }

        public virtual object Clone()
        {
            SimulationBase sb = new SimulationBase(this.Molecules, this.LabelingPositions, this.Distances);
            sb.SimulationParameters = this.SimulationParameters;
            return sb;
        }

        /// <summary>
        /// Sets an initial random state of the simulation. Must be overriden in the derived class if required.
        /// </summary>
        /// <param name="sr">Starting structure.</param>
        /// <returns>"Energy".</returns>
        public virtual double SetState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets an initial state of the simulation from an intermediate result. Must be overriden in the derived class.
        /// </summary>
        /// <param name="sr">Starting structure.</param>
        /// <returns>"Energy".</returns>
        public virtual double SetState(SimulationResult sr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs a simulation. Must be overriden in the derived class.
        /// </summary>
        /// <returns>Simulation result.</returns>
        public virtual SimulationResult Simulate()
        {
            throw new NotImplementedException();
        }

        private void InitRnd()
        {
            this._rnd = new Random((Int32)(DateTime.Now.Ticks) ^ this.GetHashCode());
        }
    }
}
