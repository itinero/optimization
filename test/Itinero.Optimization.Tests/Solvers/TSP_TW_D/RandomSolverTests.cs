using System.Collections.Generic;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.TSP_TW_D;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW_D
{
    public class RandomSolverTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Fact]
        public void RandomSolver_NameShouldBeRAN()
        {
            Assert.Equal("RAN", RandomSolver.Default.Name);
        }

        /// <summary>
        /// Tests the solver on a 'open' tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnOpen()
        {
            // create problem.
            var problem = new TSPTWDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10),
                2, TimeWindowHelpers.Unlimited(5));

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, tour.First);
                Assert.Null(tour.Last);

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnClosed()
        {
            // create problem.
            var problem = new TSPTWDProblem(0, 0, WeightMatrixHelpers.BuildDirected(5, 10),
                2, TimeWindowHelpers.Unlimited(5));

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(50, fitness);
                Assert.Equal(problem.First, tour.First);
                Assert.Equal(problem.First, tour.Last);

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldGenerateRandomPermutationOnFixed()
        {
            // create problem.
            var problem = new TSPTWDProblem(0, 4, WeightMatrixHelpers.BuildDirected(5, 10),
                2, TimeWindowHelpers.Unlimited(5));

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, DirectedHelper.ExtractVisit(tour.First));
                Assert.Equal(problem.Last, DirectedHelper.ExtractVisit(tour.Last));

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 1)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 3)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.Empty(solutionList);
            }
        }

        /// <summary>
        /// Tests the solver on a closed tsp with custom visits.
        /// </summary>
        [Fact]
        public void RandomSolver_ShouldUseOnlyProvidedVisits()
        {
            // create problem.
            var problem = new TSPTWDProblem(0, 8, WeightMatrixHelpers.BuildDirected(10, 10),
                2, TimeWindowHelpers.Unlimited(10), new [] { 0, 2, 4, 6, 8 });

            for (var i = 0; i < 100; i++)
            {
                // generate solution.
                var candidate = RandomSolver.Default.Search(problem);
                var tour = candidate.Solution;
                var fitness = candidate.Fitness;

                // test contents.
                Assert.Equal(40, fitness);
                Assert.Equal(problem.First, DirectedHelper.ExtractVisit(tour.First));
                Assert.Equal(problem.Last, DirectedHelper.ExtractVisit(tour.Last));

                var solutionList = new List<int>(tour);
                Assert.Equal(0, solutionList[0]);
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 0)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 2)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 4)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 6)));
                Assert.True(solutionList.Remove(solutionList.Find(x => DirectedHelper.Extract(x).visit == 8)));
                Assert.Empty(solutionList);
            }
        }
    }
}