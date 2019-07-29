using System;

namespace Fps
{
    public class MetropolisSampler : SpringEngine
    {
        private double Elast; // energy after last iteration

        public MetropolisSampler(MoleculeList ms, LabelingPositionList ls, DistanceList ds) : base(ms, ls, ds)
        {
            this.IsContinuous = true;
        }

        public MetropolisSampler(SpringEngine se) : base(se.Molecules, se.LabelingPositions, se.Distances)
        {
            this.SimulationParameters = se.SimulationParameters;
            this.IsContinuous = true;
        }

        public override SimulationResult Simulate()
        {
            // initial energy
            Elast = ForceAndTorque();
            CheckForClashes(true);
            Elast += Eclash;

            int ntotal = 0; // current number of iterations
            int nsuccessfull = 0; // number of successfull iterations
            //const int nsuccessfull_max = 2000; // required number of successfull iterations between snapshots

            // simulate: main loop
            do
            {
                nsuccessfull += this.Iterate() ? 1 : 0;
                ntotal++;

            } while (nsuccessfull < this.SimulationParameters.MaxIterations);//while (ntotal < this.SimulationParameters.MaxIterations && nsuccessfull < nsuccessfull_max);

            // save result
            SimulationResult sr = new SimulationResult();
            for (int i = 0; i < translation.Length; i++) this.rotation[i] = Matrix3.RepairRotation(rotation[i]);
            sr.E = ForceAndTorque();
            CheckForClashes(true);
            sr.E += Eclash;
            sr.ParentStructure = _originalstate.InternalNumber;
            sr.Eclash = Eclash;
            sr.Ebond = Ebond;
            sr.Converged = (ntotal < this.SimulationParameters.MaxIterations);
            sr.SimulationMethod = _originalstate.SimulationMethod | SimulationMethods.MetropolisSampling;
            sr.Translation = new Vector3[translation.Length];
            sr.Rotation = new Matrix3[translation.Length];
            for (int i = 0; i < translation.Length; i++)
            {
                sr.Translation[i] = Matrix3.Transpose(rotation[0]) * (translation[i] - translation[0])
                    - _molecules[i].CM + _molecules[0].CM;
                this.rotation[i] = Matrix3.RepairRotation(rotation[i]);
                sr.Rotation[i] = Matrix3.RepairRotation(Matrix3.Transpose(rotation[0]) * rotation[i]);
            }
            sr.Molecules = _molecules;
            sr.CalculateBestFitTranslation(false);
            return sr;
        }

        private bool Iterate()
        {
            // random subunit
            int i = _rnd.Next(this.translation.Length);

            // save initial state
            this.translationm1[i] = this.translation[i];
            this.rotationm1[i] = this.rotation[i];

            // random translation
            double translationstep = 0.05;
            this.translation[i].X += RandomNorm() * translationstep;
            this.translation[i].Y += RandomNorm() * translationstep;
            this.translation[i].Z += RandomNorm() * translationstep;

            // random rotation
            double rotationstep = 0.0017; // ~0.1° in rad

            // random rotation axis (anisotropic, normalized)
            double phi, rxy;
            double z0 = -1.0 + 2.0 * _rnd.NextDouble();
            rxy = Math.Sqrt(1 - z0 * z0);
            phi = _rnd.NextDouble() * 2.0 * Math.PI;
            Vector3 u = new Vector3(Math.Cos(phi) * rxy, Math.Sin(phi) * rxy, z0);

            // rotation matrix
            Matrix3 rot = Matrix3.Rotation(u, RandomNorm() * rotationstep);
            this.rotation[i] = rot * this.rotation[i];

            // recalculate "energy"
            double E = ForceAndTorque();
            CheckForClashes(true);
            E += Eclash;

            // accept or reject
            double recipr_kT = this.SimulationParameters.rkT;
            bool accepted = E < Elast || _rnd.NextDouble() < Math.Exp((Elast - E) * recipr_kT);

            // revert to previous state if rejected
            if (!accepted)
            {
                this.translation[i] = this.translationm1[i];
                this.rotation[i] = this.rotationm1[i];
            }
            else Elast = E;

            return accepted;
        }

        public override object Clone()
        {
            return new MetropolisSampler((SpringEngine)base.Clone());
        }
    }
}
