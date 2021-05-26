using System.Linq;
using Itinero.Algorithms;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.LocalSearch.Local2Opt;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.LocalSearch.Local2Opt
{
    public class Local2OptDirectedOperationTests
    {
        [Fact]
        public void GetDisjointEdgePairsDirected_OpenTour4_1Pair()
        {
            var tour = new Tour(new [] {0, 1, 2, 3}, null);

            var result = tour.GetDisjointEdgePairsDirected().ToList();
            Assert.Single(result);
            Assert.Equal((new Quad(-1,0,1,2), new Quad(1,2,3,-1)), result[0]);
        }
        
        [Fact]
        public void GetDisjointEdgePairsDirected_OpenTour6_OneOption()
        {
            var tour = new Tour(new [] {0, 1, 2, 3, 4, 5}, null);

            var result = tour.GetDisjointEdgePairsDirected().ToList();
            Assert.Equal(6, result.Count);
            Assert.Equal((new Quad(-1,0,1,2), new Quad(1,2,3,4)), result[0]);
            Assert.Equal((new Quad(-1,0,1,2), new Quad(2,3,4,5)), result[1]);
            Assert.Equal((new Quad(-1,0,1,2), new Quad(3,4,5,-1)), result[2]);
            Assert.Equal((new Quad(0,1,2,3), new Quad(2,3,4,5)), result[3]);
            Assert.Equal((new Quad(0,1,2,3), new Quad(3,4,5,-1)), result[4]);
            Assert.Equal((new Quad(1,2,3,4), new Quad(3,4,5,-1)), result[5]);
        }

        [Fact]
        public void Get2OptImprovementsDirected_OpenTour6_NoImprovements()
        {
            var tour = new Tour(new [] {0, 1, 2, 3, 4, 5}, null);

            var result = tour.Get2OptImprovementsDirected(quad => 1).ToList();
            
            Assert.Empty(result);
        }

        [Fact]
        public void Get2OptImprovementsDirected_ClosedTour7_OnePossibleMove_ShouldReturn1Move()
        {
            var weights = WeightMatrixHelpers.BuildDirected(7, 10);
            weights.SetDirectedWeights(0,1, 1);
            weights.SetDirectedWeights(1,2, 1);
            weights.SetDirectedWeights(2,3, 1);
            weights.SetDirectedWeights(4,5, 1);
            weights.SetDirectedWeights(5,6, 1);
            weights.SetDirectedWeights(6,0, 1);
            weights.SetDirectedWeights(3,1, 100);
            
            var tour = new Tour(new [] { 0, 16, 12, 8, 4, 20, 24 }, 0);

            // apply operator, should detect possible move.
            var result = tour.Get2OptImprovementsDirected(
                q => weights[DirectedHelper.ExtractDepartureWeightId(q.Visit2)][DirectedHelper.ExtractDepartureWeightId(q.Visit3)]).ToList();
            
            Assert.Single(result);
            var expected = ((new Quad(24, 0, 16, 12), (TurnEnum.ForwardForward, TurnEnum.ForwardForward),
                new Quad(8, 4, 20, 24), (TurnEnum.ForwardForward, TurnEnum.ForwardForward)), 18f);
            Assert.Equal(expected, result[0]);
        }

        [Fact]
        public void Get2OptImprovementsDirected_ClosedTour4_BestPossibleMove_WithTurnUpdates_ShouldReturnOptimizedTurns()
        {
            var weights = WeightMatrixHelpers.BuildDirected(7, 5);
            weights.SetDirectedWeights(0,1, ff: 1);
            weights.SetDirectedWeights(1,2, ff: 1);
            weights.SetDirectedWeights(2,3, ff: 1);
            weights.SetDirectedWeights(4,0, ff: 1);
            weights.SetDirectedWeights(3,1, 100);
            
            var tour = new Tour(new [] { 0, 12, 8, 4, 16 }, 0);
        
            // apply operator, should detect possible moves, select the one with the max decrease.
            var result = tour.Get2OptImprovementsDirected(
                q => weights[DirectedHelper.ExtractDepartureWeightId(q.Visit2)][
                    DirectedHelper.ExtractDepartureWeightId(q.Visit3)]).Best();
            
            Assert.Equal(((new Quad(16,0,12,8), (TurnEnum.ForwardForward, TurnEnum.ForwardForward), 
                new Quad(8,4,16,0), (TurnEnum.ForwardForward, TurnEnum.ForwardForward)), 4f), result);
        }

        [Fact]
        public void Apply2OptDirected_ClosedTour7_ApplyPossibleMove_MoveShouldBeApplied()
        {
            var tour = new Tour(new [] { 0, 16, 12, 8, 4, 20, 24 }, 0);
            
            tour.Apply2OptDirected((new Quad(24, 0, 16, 12), (TurnEnum.ForwardForward, TurnEnum.ForwardForward),
                new Quad(8, 4, 20, 24), (TurnEnum.ForwardForward, TurnEnum.ForwardForward)));
            
            // test result.
            Assert.Equal(new [] { 0, 4, 8, 12, 16, 20, 24 }, tour);
        }

        [Fact]
        public void Apply2OptDirected_ClosedTour7_ApplyPossibleMove_WithNewTurns_MoveShouldBeApplied()
        {
            var tour = new Tour(new [] { 0, 16, 12, 8, 4, 20, 24 }, 0);
            
            tour.Apply2OptDirected((new Quad(24, 0, 16, 12), (TurnEnum.ForwardForward, TurnEnum.ForwardBackward),
                new Quad(8, 4, 20, 24), (TurnEnum.ForwardForward, TurnEnum.BackwardForward)));
            
            // test result.
            Assert.Equal(new [] { 0, 4, 8, 12, 17, 22, 24 }, tour);
        }
    }
}