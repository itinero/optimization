using Itinero.Optimization.Models.TimeWindows;
using Xunit;

namespace Itinero.Optimization.Tests.Models.TimeWindows
{
    public class TimeWindowTests
    {
        [Fact]
        public void TimeWindow_ToString_Empty_ShouldReturnEmpty()
        {
            var tw = new TimeWindow();
            
            Assert.Equal("[0, ∞[", tw.ToInvariantString());
        }
        
        [Fact]
        public void TimeWindow_ToString_OneValue_ShouldReturn1RangeToInfinite()
        {
            var tw = new TimeWindow {Times = new[] {60f}};

            Assert.Equal("[60, ∞[", tw.ToInvariantString());
        }
        
        [Fact]
        public void TimeWindow_ToString_TwoValues_ShouldReturn1Range()
        {
            var tw = new TimeWindow {Times = new[] {60f, 120}};

            Assert.Equal("[60, 120]", tw.ToInvariantString());
        }
    }
}