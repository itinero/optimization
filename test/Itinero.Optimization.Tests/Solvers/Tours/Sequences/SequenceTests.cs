using System.Linq;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Sequences;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours.Sequences
{
    public class SequenceTests
    {
        [Fact]
        public void Sequence_ShouldSelectAtPositionAndWithSize()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, null);

            var s = tour.Sequence(1, 2);

            Assert.NotNull(s);
            Assert.Equal(2, s.Length);
            Assert.Equal(1, s[0]);
            Assert.Equal(2, s[1]);
        }
        
        [Fact]
        public void Sequence_ShouldSelectAroundFirstWhenClosed()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            var s = tour.Sequence(4, 3);

            Assert.NotNull(s);
            Assert.Equal(3, s.Length);
            Assert.Equal(4, s[0]);
            Assert.Equal(0, s[1]);
            Assert.Equal(1, s[2]);
        }

        [Fact]
        public void Sequence_Enumerator_ShouldEnumerateEntireSequence()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            var s = tour.Sequence(4, 3).ToList();
            
            Assert.NotNull(s);
            Assert.Equal(3, s.Count);
            Assert.Equal(4, s[0]);
            Assert.Equal(0, s[1]);
            Assert.Equal(1, s[2]);
        }
    }
}