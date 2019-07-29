using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Fps
{
    // a base class for simulation- and filter job managers
    public class JobManagerBase
    {
        protected CancellationTokenSource cts;

        /// <summary>
        /// Number of threads to use.
        /// </summary>
        public int NThreads { get; set; }

        /// <summary>
        /// Indicates how many structures are already processed.
        /// </summary>
        public int StructuresDone { get; protected set; }

        /// <summary>
        /// Indicates whether filtering is completed (also set to true if cancelled or crashed).
        /// </summary>
        public bool SimulationCompleted { get; protected set; }

        public event ProgressChangedEventHandler ProgressChanged;

        public JobManagerBase()
        {
            this.SimulationCompleted = true;
        }

        public void StartJobAsync()
        {
            if (!SimulationCompleted) throw new ApplicationException("Simulation is already runnning");
            this.SimulationCompleted = false;
            StructuresDone = 0;
            cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => this.DoJob());
        }

        protected virtual void DoJob()
        {
            throw new NotImplementedException("Simulate() must be overriden in the derived class");
        }

        public void Cancel()
        {
            this.cts.Cancel();
        }

        // this mechanism is currently not in use!
        protected virtual void OnProgressChanged(int done, int total)
        {
            if (this.ProgressChanged != null)
            {
                ProgressChangedEventArgs e = new ProgressChangedEventArgs((done * 100) / total,
                    done.ToString() + " of " + total.ToString());
                ProgressChanged(this, e);
            }
        }
    }
}
