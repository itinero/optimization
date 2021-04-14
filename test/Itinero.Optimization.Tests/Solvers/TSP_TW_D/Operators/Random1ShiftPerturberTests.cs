using System.Collections.Generic;
using Itinero.Optimization.Solvers;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_TW_D;
using Itinero.Optimization.Solvers.TSP_TW_D.Operators;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW_D.Operators
{
    public class Random1ShiftPerturberTests
    {
        /// <summary>
        /// Random 1 shift should just use random 1 shift operation, apply the same tests.
        /// </summary>
        [Fact]
        public void Random1ShiftPerturber_ShouldJustUseRandom1ShiftOperation()
        {
            var problem = new TSPTWDProblem(0, null, WeightMatrixHelpers.Build(5, 10),
                2, TimeWindowHelpers.Unlimited(5));
            var tour = new Tour(new[]
            {
                DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(1, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(2, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(3, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(4, TurnEnum.ForwardForward)
            }, 0);
            var count = tour.Count;

            var candidate = new Candidate<TSPTWDProblem, Tour>()
            {
                Fitness = tour.Fitness(problem),
                Solution = tour,
                Problem = problem
            };
            for (var i = 0; i < 100; i++)
            {
                var fitness = candidate.Fitness;
                if (Random1ShiftPerturber.Default.Apply(candidate))
                {
                    Assert.True(fitness > candidate.Fitness);
                }
                else
                {
                    Assert.True(fitness <= candidate.Fitness);
                }
                Assert.Equal(count, tour.Count);
                
                var content = new HashSet<int>(tour);
                Assert.Contains(0, content);
                Assert.Contains(1, content);
                Assert.Contains(2, content);
                Assert.Contains(3, content);
                Assert.Contains(4, content);
            }
        }
    }
}