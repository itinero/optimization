using System.Linq;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.TSP_TW;
using Itinero.Optimization.Solvers.TSP_TW.EAX;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW.EAX
{
    public class EAXSolverTests
    {
        [Fact]
        public void EAXSolver_Solve_NoWindows_ShouldSolveAsTSP()
        {
            var problem = new TSPTWProblem(0, 0, new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 4, 0, 5, 6 },
                new float[] { 7, 8, 0, 9 },
                new float[] { 10, 11, 12, 0 }
            }, new TimeWindow[4]);

            var result = EAXSolver.Default.Search(problem);
            Assert.Equal(25, result.Fitness);
            Assert.Equal(new[] { 0, 1, 2, 3 }, result.Solution.ToArray());
        }

        [Fact]
        public void EAXSolver_Solve_OneSingleWindow_ShouldFollowWindow()
        {
            var problem = new TSPTWProblem(0, 0, new [] {
                new float[] { 0, 3, 4, 3 },
                new float[] { 3, 0, 3, 4 },
                new float[] { 4, 3, 0, 3 },
                new float[] { 3, 4, 3, 0 }
            }, new TimeWindow[4]
            {
                TimeWindow.Empty, 
                TimeWindow.Empty, 
                new TimeWindow()
                {
                    Times = new [] { 3f, 5 }
                }, 
                TimeWindow.Empty
            });

            var result = EAXSolver.Default.Search(problem);
            Assert.Equal(14, result.Fitness);
            Assert.Equal(new[] { 0, 2 }, result.Solution.Take(2).ToArray());
        }
        
        [Fact]
        public void EAXSolver_Solve_OneMultiWindow_ShouldFollowOptimalWindow()
        {
            var problem = new TSPTWProblem(0, 0, new [] {
                new float[] { 0, 3, 4, 3 },
                new float[] { 3, 0, 3, 4 },
                new float[] { 4, 3, 0, 3 },
                new float[] { 3, 4, 3, 0 }
            }, new TimeWindow[4]
            {
                TimeWindow.Empty, 
                TimeWindow.Empty, 
                new TimeWindow()
                {
                    Times = new [] { 3f, 5, 5, 6 }
                }, 
                TimeWindow.Empty
            });

            var result = EAXSolver.Default.Search(problem);
            Assert.Equal(12, result.Fitness);
            Assert.Equal(new[] { 0, 1, 2, 3 }, result.Solution.ToArray());
        }
    }
}