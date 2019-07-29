using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fps
{
    public class FilterJobManager : JobManagerBase
    {
        /// <summary>
        /// A filtering job that will be used to create other jobs by cloning.
        /// </summary>
        public FilterEngine FilterPrototype { get; set; }

        /// <summary>
        /// States to filter.
        /// </summary>
        public FilteringResult[] StatesToFilter { get; set; }

        public FilterJobManager() : base()
        {
        }

        protected override void DoJob()
        {
            // preparations
            int nfinal = this.StatesToFilter == null ? -1 : this.StatesToFilter.Length;
            int chunk = Math.Max(nfinal / 400, 1);
            int njobs = Math.Min(this.NThreads, nfinal);
            CancellationToken ctoken = cts.Token;
            FilterEngine[] jobslocal = new FilterEngine[njobs];
            Task<int>[] taskslocal = new Task<int>[njobs];
            for (int i = 0; i < jobslocal.Length; i++)
            {
                jobslocal[i] = (FilterEngine)FilterPrototype.Clone();
            }

            // main loop
            int currentstructure = 0;
            int completedtaskindex;
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
                    }
                    // something done, save and reuse this task 
                    else
                    {
                        completedtaskindex = Task.WaitAny(taskslocal, ctoken);
                        StructuresDone += taskslocal[completedtaskindex].Result;
                        this.OnProgressChanged(StructuresDone, nfinal);
                    }

                    // restart
                    if (currentstructure < nfinal)
                    {
                        taskslocal[completedtaskindex] = Task.Factory.StartNew<int>(p =>
                        {
                            var ptyped = p as Tuple<FilterEngine, int, int>; // job, start, stop
                            int i = ptyped.Item2;
                            for (; i < ptyped.Item3; i++) ptyped.Item1.CalculateChi2(ref this.StatesToFilter[i]);
                            return i - ptyped.Item2;
                        },
                        new Tuple<FilterEngine, int, int>(jobslocal[completedtaskindex], currentstructure, Math.Min(currentstructure + chunk, nfinal)),
                        ctoken);
                        currentstructure += chunk;
                    }
                    // almost done, queue nothing
                    else taskslocal[completedtaskindex] = new Task<int>(() => { return 0; }); // this task never starts
                }
            }
            finally
            {
                this.SimulationCompleted = true;
            }
        }
    }
}
