using Itinero.Optimization.Solvers.CVRP_ND;
using Itinero.Optimization.Solvers.CVRP_ND.Construction;

namespace Itinero.Optimization.Tests.Functional.Solvers.CVRP_ND.Construction
{
    public class SeededConstructionHeuristicTest :  FunctionalTest<CVRPNDCandidate, CVRPNDProblem>
    {
        /// <summary>
        /// Gets the default test.
        /// </summary>
        public static SeededConstructionHeuristicTest Default => new SeededConstructionHeuristicTest();
        
        protected override CVRPNDCandidate Execute(CVRPNDProblem input)
        {
            return SeededConstructionHeuristic.Default.Search(input);
        }
    }
}