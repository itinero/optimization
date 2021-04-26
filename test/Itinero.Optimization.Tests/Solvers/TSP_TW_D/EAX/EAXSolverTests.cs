using System.Linq;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.TSP_TW_D;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW_D.EAX
{
    public class EAXSolverTests
    {
        [Fact]
        public void EAXSolver_Solve_NoWindows_ShouldSolveAsTSP()
        {
            var problem = new TSPTWDProblem(0, 0, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, 2,new TimeWindow[4]);

            var result = Itinero.Optimization.Solvers.TSP_TW_D.EAX.
                EAXSolver.Default.Search(problem);
            Assert.Equal(25, result.Fitness);
            Assert.Equal(new[] { 0, 1, 2, 3 }, result.Solution.ToArray());
        }
    }
}