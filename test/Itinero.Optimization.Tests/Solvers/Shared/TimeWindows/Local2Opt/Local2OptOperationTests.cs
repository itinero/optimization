using Itinero.Optimization.Solvers.Shared;
using Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.TimeWindows.Local2Opt
{
    public class Local2OptOperationTests
    {
        [Fact]
        public void Local2OptOperation_ShouldExecutePossibleMove()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            weights[3][1] = 100;
            var windows = new TimeWindow[5];
            
            var tour = new Tour(new int[] { 0, 3, 2, 1, 4 }, 0);

            // apply operator, should detect possible move.
            Assert.True(tour.Do2Opt((x, y) => weights[x][y], windows));

            // test result.
            Assert.Equal(new [] { 0, 1, 2, 3, 4 }, tour);
        }
        
        [Fact]
        public void Local2OptOperation_ShouldDoNothingOnImpossibleMove()
        {
            var weights = WeightMatrixHelpers.Build(5, 10);
            weights[0][1] = 1;
            weights[1][2] = 1;
            weights[2][3] = 1;
            weights[3][4] = 1;
            weights[4][0] = 1;
            weights[3][1] = 100;
            var windows = new TimeWindow[5];
            windows[3] = new TimeWindow()
            {
                Times = new[] {1f, 2f}
            };
            windows[2] = new TimeWindow()
            {
                Times = new[] {11f, 12f}
            };
            
            var tour = new Tour(new [] { 0, 3, 2, 1, 4 }, 0);

            // apply operator, should detect possible move.
            Assert.False(tour.Do2Opt((x, y) => weights[x][y], windows));

            // test result.
            Assert.Equal(new [] { 0, 3, 2, 1, 4 }, tour);
        }
    }
}