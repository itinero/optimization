using System;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    /// <summary>
    /// Extension methods related to tours & sequences.
    /// </summary>
    public static class TourExtensions
    {
        /// <summary>
        /// Selects a random sequence in the given size range from the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="maxSize">The maximum size.</param>
        /// <returns>A randomly selected sequence.</returns>
        public static Sequence RandomSequence(this IReadOnlyTour tour, int minSize, int maxSize)
        {
            if (tour == null) throw new ArgumentNullException(nameof(tour));
            if (minSize > maxSize) throw new ArgumentException($"Range for random selection [{minSize},{maxSize}] invalid: {nameof(minSize)} > {nameof(maxSize)}");
            
            // make sure the max is not > tour.count.
            if (tour.Count < maxSize)
            {
                maxSize = tour.Count;
            }
            
            // generate random size between min and amx..
            var size = minSize;
            if (minSize < maxSize)
            {
                size += RandomGenerator.Default.Generate(maxSize - minSize);
            }

            return tour.RandomSequence(size);
        }

        /// <summary>
        /// Selects a random sequence with the given size from the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="size">The size.</param>
        /// <returns>A randomly selected sequence.</returns>
        public static Sequence RandomSequence(this IReadOnlyTour tour, int size)
        {
            if (tour == null) throw new ArgumentNullException(nameof(tour));
            if (tour.Count == 0) throw new ArgumentException($"Cannot select a random sequence from a tour that has no visits.");
            if (tour.Count < size) throw new ArgumentException($"Cannot select a random sequence of size {size} from a tour with only {tour.Count} visits.");
            
            // if the tour is closed selection can overlap first.
            var maxPosition = tour.Count - size;
            if (tour.IsClosed())
            {
                maxPosition = tour.Count;
            }
            
            // select a random position.
            var position = RandomGenerator.Default.Generate(maxPosition);

            return tour.Sequence(position, size);
        }
        
        /// <summary>
        /// Gets a sequence starting at the given position with the given size.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        /// <returns>The sequence.</returns>
        public static Sequence Sequence(this IReadOnlyTour tour, int position, int size)
        {
            if (tour == null) throw new ArgumentNullException(nameof(tour));
            if (tour.Count < size) throw new ArgumentException($"Cannot select a sequence of size {size} from a tour with only {tour.Count} visits.");

            var visits = new int[size];
            var i = -1;
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    i++;

                    var pos = i - position;
                    if (pos >= size)
                    {
                        break;
                    }
                    if (pos >= 0)
                    {
                        visits[pos] = enumerator.Current;
                    }
                }

                var leftOver = (position + size) - tour.Count;
                if (leftOver > 0)
                {
                    enumerator.Reset();

                    while (enumerator.MoveNext())
                    {
                        var pos = size - leftOver;
                        visits[pos] = enumerator.Current;

                        leftOver--;

                        if (leftOver == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return new Sequence(visits);
        }
    }
}