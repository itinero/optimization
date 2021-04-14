using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_TW_D;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_TW_D
{
    public class TSPTWDFitnessTests
    {
        [Fact]
        public void TSPTWDFitness_ShouldTakeIntoAccountTripToFirst()
        {
            // create problem.
            var problem = new TSPTWDProblem(0, 0, WeightMatrixHelpers.BuildDirected(5, 10),
                2, TimeWindowHelpers.Unlimited(5));
            var tour = new Tour(new[]
            {
                DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(1, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(2, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(3, TurnEnum.ForwardForward),
                DirectedHelper.BuildVisit(4, TurnEnum.ForwardForward)
            }, DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward));
            
            Assert.Equal(50, tour.Fitness(problem));
        }

        [Fact]
        public void TSPTWDFitness_ShouldNotApplyPenaltiesWhenFeasible()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            var problem = new TSPTWDProblem(0, 0, weights, 2, windows);
            
            // create a feasible route.
            var tour = new Tour(new int[] { 0, 2, 4, 1, 3 }, 0);

            // apply the 1-shift local search.
            var fitness = tour.Fitness(problem);
            Assert.Equal(10, fitness);
        }

        [Fact]
        public void TSPTWDFitness_ShouldApplyPenaltiesWhenUnfeasible()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[2] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            var problem = new TSPTWDProblem(0, 0, weights, 2, windows);

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            var fitness = tour.Fitness(problem, timeWindowViolationPenaltyFactor: 1000000);
            Assert.Equal(1000010, fitness);
        }

        [Fact]
        public void TSPTWDFitness_LastViolated_ShouldApplyPenalty()
        {
            var weights = WeightMatrixHelpers.Build(5, 2);
            var windows = new TimeWindow[5];
            windows[4] = new TimeWindow()
            {
                Times = new[] {1f, 3f}
            };
            var problem = new TSPTWDProblem(0, 0, weights, 2, windows);

            // create a route with one shift.
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);

            var fitness = tour.Fitness(problem, timeWindowViolationPenaltyFactor: 1000000);
            Assert.Equal(5000010, fitness);
        }
    }
}