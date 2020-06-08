using System.Linq;
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

        [Fact]
        public void TimeWindow_Enumerator_Empty_ShouldBeEmpty()
        {
            var tw = new TimeWindow();

            var twList = tw.ToList();
            Assert.Empty(twList);
        }

        [Fact]
        public void TimeWindow_Enumerator_OneValue_ShouldReturn1RangeToInfinite()
        {
            var tw = new TimeWindow(new []  { 60f });

            var twList = tw.ToList();
            Assert.Single(twList);
            Assert.Equal(60.0, twList[0].start, 7);
            Assert.Equal(float.MaxValue, twList[0].end, 7);
        }

        [Fact]
        public void TimeWindow_Enumerator_2Values_ShouldReturn1Range()
        {
            var tw = new TimeWindow(new []  { 60f, 120f });

            var twList = tw.ToList();
            Assert.Single(twList);
            Assert.Equal(60.0, twList[0].start, 7);
            Assert.Equal(120.0, twList[0].end, 7);
        }

        [Fact]
        public void TimeWindow_Enumerator_3Values_ShouldReturn2RangesToInfinite()
        {
            var tw = new TimeWindow(new []  { 60f, 120f, 125f });

            var twList = tw.ToList();
            Assert.Equal(2, twList.Count);
            Assert.Equal(60.0, twList[0].start, 7);
            Assert.Equal(120.0, twList[0].end, 7);
            Assert.Equal(125.0, twList[1].start, 7);
            Assert.Equal(float.MaxValue, twList[1].end, 7);
        }

        [Fact]
        public void TimeWindow_Enumerator_4Values_ShouldReturn2Ranges()
        {
            var tw = new TimeWindow(new []  { 60f, 120f, 125f, 150f });

            var twList = tw.ToList();
            Assert.Equal(2, twList.Count);
            Assert.Equal(60.0, twList[0].start, 7);
            Assert.Equal(120.0, twList[0].end, 7);
            Assert.Equal(125.0, twList[1].start, 7);
            Assert.Equal(150.0, twList[1].end, 7);
        }
    }
}