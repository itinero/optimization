using System.Linq;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Sequences;
using Itinero.Optimization.Strategies.Random;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours.Sequences
{
    public class IReadOnlyTourExtensionTests
    {
        [Fact]
        public void RandomSequence_ShouldSelectRandomWithProperSize()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            for (var t = 0; t < 100; t++)
            {
                var size = RandomGenerator.Default.Generate(tour.Count - 1) + 1;
                var s = tour.RandomSequence(size);

                Assert.NotNull(s);
                Assert.Equal(size, s.Length);

                var index = tour.GetIndexOf(s[0]);
                for (var i = 0; i < s.Length; i++)
                {
                    var expected = tour.GetVisitAt((index + i) % tour.Count);
                    Assert.Equal(expected, s[i]);
                }
            }
        }
        
        [Fact]
        public void RandomSequence_ShouldSelectRandomBetweenSizes()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            for (var t = 0; t < 100; t++)
            {
                RandomGenerator.Default.Generate2(tour.Count, out var minSize, out var maxSize);
                if (minSize > maxSize)
                {
                    var m = minSize;
                    minSize = maxSize;
                    maxSize = m;
                }
                
                var s = tour.RandomSequence(minSize, maxSize);

                Assert.NotNull(s);
                Assert.True(s.Length >= minSize && s.Length <= maxSize);

                if (s.Length <= 0) continue;
                var index = tour.GetIndexOf(s[0]);
                for (var i = 0; i < s.Length; i++)
                {
                    var expected = tour.GetVisitAt((index + i) % tour.Count);
                    Assert.Equal(expected, s[i]);
                }
            }
        }

        [Fact]
        public void ReadOnlyTourRemovedSequence_ShouldRemoveSequenceAtBeginning()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);
            var s = new Sequence(new[] { 0, 1, 2});

            var tourReadonly = tour.WithoutSequence(s);
            Assert.NotNull(tourReadonly);
            Assert.Equal(3, tourReadonly.First);
            Assert.Equal(3, tourReadonly.Last);
            Assert.Equal(2, tourReadonly.Count);
            var removed = tourReadonly.ToArray();
            Assert.Equal(2, removed.Length);
            Assert.Equal(3, removed[0]);
            Assert.Equal(4, removed[1]);
        }

        [Fact]
        public void ReadOnlyTourRemovedSequence_ShouldRemoveSequenceAtEnd()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);
            var s = new Sequence(new[] { 2, 3, 4 });

            var tourReadonly = tour.WithoutSequence(s);
            Assert.NotNull(tourReadonly);
            Assert.Equal(0, tourReadonly.First);
            Assert.Equal(0, tourReadonly.Last);
            Assert.Equal(2, tourReadonly.Count);
            var removed = tourReadonly.ToArray();
            Assert.Equal(2, removed.Length);
            Assert.Equal(0, removed[0]);
            Assert.Equal(1, removed[1]);
        }

        [Fact]
        public void ReadOnlyTourRemovedSequence_ShouldRemoveSequenceInMiddle()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);
            var s = new Sequence(new[] { 1, 2, 3 });

            var tourReadonly = tour.WithoutSequence(s);
            Assert.NotNull(tourReadonly);
            Assert.Equal(0, tourReadonly.First);
            Assert.Equal(0, tourReadonly.Last);
            Assert.Equal(2, tourReadonly.Count);
            var removed = tourReadonly.ToArray();
            Assert.Equal(2, removed.Length);
            Assert.Equal(0, removed[0]);
            Assert.Equal(4, removed[1]);
        }

        [Fact]
        public void ReadOnlyTourRemovedSequence_ShouldRemoveSequenceOverFirst()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);
            var s = new Sequence(new[] { 4, 0, 1 });

            var tourReadonly = tour.WithoutSequence(s);
            Assert.NotNull(tourReadonly);
            Assert.Equal(2, tourReadonly.First);
            Assert.Equal(2, tourReadonly.Last);
            Assert.Equal(2, tourReadonly.Count);
            var removed = tourReadonly.ToArray();
            Assert.Equal(2, removed.Length);
            Assert.Equal(2, removed[0]);
            Assert.Equal(3, removed[1]);
        }

        [Fact]
        public void ReadOnlyTourRemovedSequence_ShouldRemoveEverything()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);
            var s = new Sequence(new[] { 0, 1, 2, 3, 4 });

            var tourReadonly = tour.WithoutSequence(s);
            Assert.Null(tourReadonly);
        }

        [Fact]
        public void CheapestInsertion_Sequence_ShouldCalculateCheapestLocationClosed()
        {
            var matrix = new [] {
                new float[] { 0, 1, 2, 3, 4 },
                new float[] { 4, 0, 1, 2, 3 },
                new float[] { 3, 4, 0, 1, 2 },
                new float[] { 2, 3, 4, 0, 1 },
                new float[] { 1, 2, 3, 4, 0 }};
            
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);
            var s = new Sequence(new[] { 4, 0, 1 });
            var tourReadOnly = tour.WithoutSequence(s);

            var result = tourReadOnly.CalculateCheapest((x, y) => matrix[x][y], new Sequence(new[] {1}));
            Assert.Equal(0, result.cost);
            Assert.Equal(3, result.location.From);
            Assert.Equal(2, result.location.To);
        }
    }
}