using System;

namespace Fps
{
    // Error estimation by bootstrapping
    public class ErrorEstimation : SpringEngine
    {
        public ErrorEstimation(MoleculeList ms, LabelingPositionList ls, DistanceList ds) : base(ms, ls, ds)
        {
        }

        public ErrorEstimation(SpringEngine se) : base(se.Molecules, se.LabelingPositions, se.Distances)
        {
            this.SimulationParameters = se.SimulationParameters;
        }

        public override double SetState(SimulationResult sr)
        {
            // set model distances as "true"
            Distance rmodeltmp;
            DistanceList rlisttmp = new DistanceList(this.Distances.Count);
            rlisttmp.AddRange(this.Distances);
            for (Int32 i = 0; i < rlisttmp.Count; i++)
            {
                rmodeltmp = rlisttmp[i];
                rmodeltmp.R = sr.ModelDistance(this.LabelingPositions.Find(rmodeltmp.Position1),
                    this.LabelingPositions.Find(rmodeltmp.Position2));
                rlisttmp[i] = rmodeltmp;
            }
            this.Distances = rlisttmp;

            return base.SetState(sr);
        }

        public override SimulationResult Simulate()
        {
            PerturbDistances(1.0);
            SimulationResult sr = base.Simulate();
            sr.SimulationMethod |= SimulationMethods.ErrorEstimation;
            return sr;
        }

        public override object Clone()
        {
            return new ErrorEstimation((SpringEngine)base.Clone());
        }

        private void PerturbDistances(double sigmafactor)
        {
            Distance dtmp;
            double rnorm;
            for (int i = 0; i < _distances.Count; i++)
            {
                if (!_distances[i].IsSelected) continue;
                dtmp = _distances[i];
                rnorm = RandomNorm() * sigmafactor;
                dtmp.R += (rnorm > 0) ? rnorm * dtmp.ErrPlus : rnorm * dtmp.ErrMinus;
                _distances[i] = dtmp;
            }
        }
    }
}
