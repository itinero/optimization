using System.Threading;
using Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.TSP_TW_D.Operators
{
    /// <summary>
    /// An operator that runs the local 2 opt operator.
    /// </summary>
    internal class Local2OptOperator : Operator<Candidate<TSPTWDProblem, Tour>>
    {
        public override string Name => "2OPT";

        public override bool Apply(Candidate<TSPTWDProblem, Tour> candidate)
        {
            if (!candidate.Solution.Do2Opt(candidate.Problem.Weight, candidate.Problem.Windows)) return false;
            
            candidate.Fitness = candidate.Solution.Fitness(candidate.Problem);
            return true;
        }
        
        private static readonly ThreadLocal<Local2OptOperator> DefaultLazy = new ThreadLocal<Local2OptOperator>(() => new Local2OptOperator());
        public static Local2OptOperator Default => DefaultLazy.Value;
    }
}