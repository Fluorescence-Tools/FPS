using System;

namespace Fps
{
    public class Refinement : SpringEngine
    {
        private Molecule mt;
        private SimulationResult sr;
        private int[] molstart;
        private int[] molnatoms;

        public ProjectData ProjectDataCopy { get; set; }

        public Refinement(MoleculeList ms, LabelingPositionList ls, DistanceList ds) : base(ms, ls, ds)
        {
        }

        public Refinement(SpringEngine se) : base(se.Molecules, se.LabelingPositions, se.Distances)
        {
            this.SimulationParameters = se.SimulationParameters;
        }

        public override double SetState(SimulationResult sr0)
        {
            InitializeStructureForRefinement(sr0);
            if ((sr0.SimulationMethod & SimulationMethods.Refinement) == 0 || sr0.RefinedLabelingPositions == null)
                this.LabelingPositions = RedoAV();
            else this.LabelingPositions = sr0.RefinedLabelingPositions;
            return base.SetState(sr0);
        }

        public override SimulationResult Simulate()
        {
            SimulationResult srrefined = base.Simulate();
            srrefined.SimulationMethod |= SimulationMethods.Refinement;
            InitializeStructureForRefinement(srrefined);
            srrefined.RefinedLabelingPositions = RedoAV();
            srrefined.E = SetState(srrefined);
            return srrefined;
        }

        public override object Clone()
        {
            Refinement ref_new = new Refinement((SpringEngine)base.Clone());
            ref_new.ProjectDataCopy = this.ProjectDataCopy;
            return ref_new;
        }

        private void InitializeStructureForRefinement(SimulationResult sr0)
        {
            sr = sr0;
            mt = new Molecule(sr);
            molstart = new int[sr.Molecules.Count];
            molnatoms = new int[sr.Molecules.Count];
            molstart[0] = 0;
            for (int i = 1; i < sr.Molecules.Count; i++) 
                molstart[i] = molstart[i - 1] + sr.Molecules[i - 1].NAtoms;
            for (int i = 0; i < sr.Molecules.Count; i++)
                molnatoms[i] = sr.Molecules[i].NAtoms;
        }

        private LabelingPositionList RedoAV()
        {
            LabelingPositionList lps_local = new LabelingPositionList(this.LabelingPositions.Count);
            lps_local.AddRange(this.LabelingPositions);

            AVEngine av = new AVEngine(mt, this.ProjectDataCopy.AVGlobalParameters);
            LabelingPosition l;
            Vector3 rmp;
            int natom, nmol;
            for (Int32 i = 0; i < lps_local.Count; i++)
            {
                l = lps_local[i];
                if (l.AVData.AVType == AVSimlationType.None) continue;
                nmol = sr.Molecules.FindIndex(l.Molecule);
                natom = Array.BinarySearch<Int32>(mt.OriginalAtomID, molstart[nmol], molnatoms[nmol], l.AVData.AtomID);
                if (l.AVData.AVType == AVSimlationType.SingleDyeR)
                    av.Calculate1R(l.AVData.L, l.AVData.W, l.AVData.R, natom);
                else if (l.AVData.AVType == AVSimlationType.ThreeDyeR)
                    av.Calculate3R(l.AVData.L, l.AVData.W, l.AVData.R1, l.AVData.R2, l.AVData.R3, natom);
                rmp = Matrix3.Transpose(sr.Rotation[nmol]) * (av.Rmp - sr.Translation[nmol]
                    + sr.Molecules[0].CM - sr.Molecules[nmol].CM) + sr.Molecules[nmol].CM;
                l.X = rmp.X; l.Y = rmp.Y; l.Z = rmp.Z;
                lps_local[i] = l;
            }
            return lps_local;
        }

    }
}
