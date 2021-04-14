using System.Threading;
using Itinero.Optimization.Solvers.Shared.Random1Shift;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.VNS;

namespace Itinero.Optimization.Solvers.TSP_TW_D.Operators
{
    /// <summary>
    /// An operator that runs the random 1 shift operation, remove a random visit and place it after another random visit.
    /// </summary>
    internal class Random1ShiftPerturber : Perturber<Candidate<TSPTWDProblem, Tour>>
    {
        public override string Name => "RAN_1SHIFT";
        
        public override bool Apply(Candidate<TSPTWDProblem, Tour> candidate, int level)
        {
            while (level >= 0)
            {
                candidate.Solution.DoRandom1Shift();
                level--;
            }

            var fitnessBefore = candidate.Fitness;
            candidate.Fitness = candidate.Solution.Fitness(candidate.Problem);

            return candidate.Fitness < fitnessBefore;
        }
        
        private static readonly ThreadLocal<Random1ShiftPerturber> DefaultLazy = new ThreadLocal<Random1ShiftPerturber>(() => new Random1ShiftPerturber());
        public static Random1ShiftPerturber Default => DefaultLazy.Value;
    }
}