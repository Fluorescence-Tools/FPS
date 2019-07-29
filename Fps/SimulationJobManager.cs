using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fps
{
    public class SimulationJobManager : JobManagerBase
    {
        /// <summary>
        /// A simulation job that will be used to create other jobs by cloning.
        /// </summary>
        public SimulationBase SimulationPrototype { get; set; }

        /// <summary>
        /// Initial states (if any), e.g. for refinement or error estimation.
        /// </summary>
        public SimulationResult[] InitialStates { get; set; }

        /// <summary>
        /// Number of resulting structures per input (or number of repetitions if no inputs are required).
        /// </summary>
        public int FinalStatesPerInitialState { get; set; }

        /// <summary>
        /// Simulation results.
        /// </summary>
        public SimulationResult[] FinalStates { get; private set; }

        public SimulationJobManager() : base()
        {
        }

        protected override void DoJob()
        {
            bool noinitalstates = InitialStates == null || InitialStates.Length == 0;
            int nfinal = noinitalstates ? FinalStatesPerInitialState : FinalStatesPerInitialState * InitialStates.Length;
            int nrestarts = noinitalstates ? FinalStatesPerInitialState :
                SimulationPrototype.IsContinuous ? InitialStates.Length : FinalStatesPerInitialState * InitialStates.Length;
            this.FinalStates = new SimulationResult[nfinal];

            // preparations
            int njobs = this.SimulationPrototype.IsContinuous ? Math.Min(Math.Min(this.NThreads, nfinal), InitialStates.Length) :
                Math.Min(this.NThreads, nfinal);
            CancellationToken ctoken = cts.Token;
            SimulationBase[] jobslocal = new SimulationBase[njobs];
            Task<SimulationResult>[] taskslocal = new Task<SimulationResult>[njobs];
            int[] structuresdonebyjob = new int[jobslocal.Length];
            for (int i = 0; i < jobslocal.Length; i++)
            {
                jobslocal[i] = (SimulationBase)SimulationPrototype.Clone();
            }

            // main loop
            int currentstructure = 0;
            int completedtaskindex;
            int inputstructure = 0;
            bool alltasksinitialized = false;
           
            try
            {
                while (this.StructuresDone < nfinal)
                {
                    // try to find a task that never ran
                    alltasksinitialized = Array.TrueForAll(taskslocal, t => t != null);
                    if (!alltasksinitialized)
                    {
                        completedtaskindex = Array.IndexOf(taskslocal, null);
                        structuresdonebyjob[completedtaskindex] = FinalStatesPerInitialState; // signal to "restart"
                    }
                    // something done, save and reuse this task 
                    else
                    {
                        completedtaskindex = Task.WaitAny(taskslocal, ctoken);
                        this.FinalStates[StructuresDone++] = taskslocal[completedtaskindex].Result;
                        this.OnProgressChanged(StructuresDone, nfinal);
                        structuresdonebyjob[completedtaskindex]++;
                    }

                    // restart
                    // docking case: restart for each iteration
                    if (noinitalstates && currentstructure < nfinal)
                    {
                        taskslocal[completedtaskindex] = Task.Factory.StartNew<SimulationResult>(job =>
                            {
                                ((SimulationBase)job).SetState();
                                return ((SimulationBase)job).Simulate();
                            },
                            jobslocal[completedtaskindex],
                            ctoken);
                    }
                    // error estimation, refinement, or sampling: restart
                    else if ((!jobslocal[completedtaskindex].IsContinuous && currentstructure < nfinal)
                        || (jobslocal[completedtaskindex].IsContinuous && structuresdonebyjob[completedtaskindex] == FinalStatesPerInitialState && inputstructure < InitialStates.Length))
                    {
                        inputstructure = jobslocal[completedtaskindex].IsContinuous ? inputstructure : currentstructure / FinalStatesPerInitialState;
                        taskslocal[completedtaskindex] = Task.Factory.StartNew<SimulationResult>(jobp =>
                        {
                            var jobptyped = jobp as Tuple<SimulationBase, SimulationResult>;
                            jobptyped.Item1.SetState(jobptyped.Item2);
                            return jobptyped.Item1.Simulate();
                        },
                        new Tuple<SimulationBase, SimulationResult>(jobslocal[completedtaskindex], InitialStates[inputstructure++]),
                        ctoken);
                        structuresdonebyjob[completedtaskindex] = 0;
                    }
                    // sampling: continue
                    else if (jobslocal[completedtaskindex].IsContinuous && structuresdonebyjob[completedtaskindex] < FinalStatesPerInitialState)
                    {
                        taskslocal[completedtaskindex] = Task.Factory.StartNew<SimulationResult>(job => ((SimulationBase)job).Simulate(),
                            jobslocal[completedtaskindex], ctoken);
                    }
                    // almost done, queue nothing
                    else taskslocal[completedtaskindex] = new Task<SimulationResult>(() => { return new SimulationResult(); }); // this task never starts

                    currentstructure++;
                }
            }
            finally
            {
                this.SimulationCompleted = true;
            }
        }
    }
}
