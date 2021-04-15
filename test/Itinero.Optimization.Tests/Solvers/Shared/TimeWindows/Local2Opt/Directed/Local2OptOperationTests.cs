using System.Linq;
using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt.Directed;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.TimeWindows.Local2Opt.Directed
{
    public class Local2OptOperationTests
    {
        [Fact]
        public void Local2OptOperation_ShouldExecutePossibleMove()
        {
            var weights = WeightMatrixHelpers.BuildDirected(5, 10);
            weights.SetDirectedWeights(0,1,1);
            weights.SetDirectedWeights(1,2,1);
            weights.SetDirectedWeights(2,3,1);
            weights.SetDirectedWeights(3,4,1);
            weights.SetDirectedWeights(4,0,1);
            weights.SetDirectedWeights(3,1,100);
            var windows = new TimeWindow[5];
            var turnPenaltyFunc = new float[] { 0, 2, 2, 0 }.ToTurnPenaltyFunc();
            
            var tour = new Tour(new int[]
            {
                DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(3, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(2, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(1, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(4, TurnEnum.ForwardForward)
            }, DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward));

            // apply operator, should detect possible move.
            Assert.True(tour.Do2Opt((x, y) => weights[x][y], turnPenaltyFunc, windows));

            // test result.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, tour.Select(DirectedHelper.ExtractVisit));
        }
        
        [Fact]
        public void Local2OptOperation_ShouldDoNothingOnImpossibleMove()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights.SetDirectedWeights(0,1,1);
            weights.SetDirectedWeights(1,2,1);
            weights.SetDirectedWeights(2,3,1);
            weights.SetDirectedWeights(3,4,1);
            weights.SetDirectedWeights(4,0,1);
            weights.SetDirectedWeights(3,1,100);
            var windows = new TimeWindow[5];
            windows[3] = new TimeWindow()
            {
                Times = new[] {1f, 2f}
            };
            windows[2] = new TimeWindow()
            {
                Times = new[] {11f, 12f}
            };
            var turnPenaltyFunc = new float[] { 0, 2, 2, 0 }.ToTurnPenaltyFunc();

            var tour = new Tour(new int[]
            {
                DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(3, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(2, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(1, TurnEnum.ForwardForward), 
                DirectedHelper.BuildVisit(4, TurnEnum.ForwardForward)
            }, DirectedHelper.BuildVisit(0, TurnEnum.ForwardForward));

            // apply operator, should detect possible move.
            Assert.False(tour.Do2Opt((x, y) => weights[x][y], turnPenaltyFunc, windows));

            // test result.
            Assert.Equal(new [] { 0, 3, 2, 1, 4 }, tour.Select(DirectedHelper.ExtractVisit));
        }
    }
}