using System.Linq;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.TSP_TW_D;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW_D.EAX
{
    public class EAXSolverTests
    {
        [Fact]
        public void EAXSolver_Solve_NoWindows_ShouldSolveAsTSP()
        {
            var weights = WeightMatrixHelpers.BuildDirected(4, 10);
            weights.SetDirectedWeights(0, 1, 1);
            weights.SetDirectedWeights(1, 2, 1);
            weights.SetDirectedWeights(2, 3, 1);
            weights.SetDirectedWeights(3, 0, 1);
            var problem = new TSPTWDProblem(0, 0, weights, 
                2,new TimeWindow[4]);

            var result = Itinero.Optimization.Solvers.TSP_TW_D.EAX.
                EAXSolver.Default.Search(problem);
            Assert.Equal(4, result.Fitness);
            Assert.Equal(new[] { 0, 1, 2, 3 }, result.Solution.Select(DirectedHelper.ExtractVisit).ToArray());
        }
    }
}