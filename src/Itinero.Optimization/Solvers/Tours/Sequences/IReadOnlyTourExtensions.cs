using System;
using System.Collections.Generic;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    /// <summary>
    /// Extension methods related to tours and sequences.
    /// </summary>
    public static class IReadOnlyTourExtensions
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
        
        /// <summary>
        /// Calculates the weight for a sequence that is removed.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="remove">The sequence that is removed.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <returns>The difference in weight:
        ///   - negative: weight would be reduced in the new tour.
        ///   - positive: weight would be increase in the new tour.
        /// </returns>
        public static float CalculateRemove(this IReadOnlyTour tour, Sequence remove, Func<int, int, float> weightFunc)
        {
            if (remove.Length == 0) return 0;
            
            // calculate the diff weight as if the sequence would be removed.
            // TODO: this can be optimized, but it can be O(1) if we convert the tour to a double linked list as planned so we leave this for now.
            var diff = 0f;
            var after = -1;
            var before = -1;
            if (tour.Count == 1)
            {
                before = tour.First;
            }
            else
            { // loop over all pairs and figure out the 'before'.
                foreach (var pair in tour.Pairs())
                {
                    if (pair.To == remove.First)
                    {
                        before = pair.From;
                    }

                    if (pair.From == remove.Last)
                    {
                        after = pair.To;
                    }
                }
            }
            if (before == -1) throw new ArgumentOutOfRangeException(nameof(remove), "Sequence was not found in tour.");
            diff -= weightFunc(before, remove.First);
            if (after != -1)
            {
                diff -= weightFunc(remove.Last, after);
                diff += weightFunc(before, after);
            }
            
            // remove the weight of the sequence itself.    
            diff -= remove.Weight(weightFunc);

            return diff;
        }

        /// <summary>
        /// Calculates the best position to insert a given sequence.
        /// </summary>
        /// <param name="tourEnumerable">The tour to insert into.</param>
        /// <param name="weightFunc">The function to get the travel weights.</param>
        /// <param name="sequence">The visits to add.</param>
        /// <returns>Everything that is need to add the sequence and decide to execute:
        /// - The increase/decrease in weight:
        ///    - The weight difference from adding the new sequence.
        /// - The location and the location to place the new sequence:
        ///    - A pair to insert the sequence in between.
        /// </returns>
        public static (float cost, Pair location) CalculateCheapest(this IReadOnlyTour tourEnumerable, Func<int, int, float> weightFunc,
            Sequence sequence)
        {
            (float cost, Pair location) best = (float.MaxValue, new Pair(int.MaxValue, int.MaxValue));

            if (sequence.Length == 0)
            {
                return (0, new Pair(int.MaxValue, int.MaxValue));
            }

            if (tourEnumerable.Count == 1)
            {
                var first = tourEnumerable.First;
                if (tourEnumerable.IsClosed())
                {
                    // place between first and first.
                    var cost = (weightFunc(first, sequence.First) +
                                weightFunc(sequence.Last, first));
                    var location = new Pair(first, first);
                    return (cost, location);
                }
                else
                {
                    // place after first.
                    var cost = weightFunc(first, sequence.First);
                    var location = new Pair(first, first);
                    return (cost, location);
                }
            }

            foreach (var pair in tourEnumerable.Pairs())
            {
                var cost = weightFunc(pair.From, sequence.First) +
                           weightFunc(sequence.Last, pair.To) -
                           weightFunc(pair.From, pair.To);
                if (cost < best.cost)
                {
                    best = (cost, pair);
                }
            }

            return best;
        }

        /// <summary>
        /// Gets an enumerable that enables enumeration of the given tour with the given sequence removed.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="remove">The sequence to remove.</param>
        /// <returns>An enumerable for the tour but without the given sequence.</returns>
        public static IReadOnlyTourWithoutSequence WithoutSequence(this IReadOnlyTour tour, Sequence remove)
        {
            if (tour.Count == remove.Length)
            { // an empty tour cannot exist.
                return null;
            }
            return new ReadOnlyTourWithoutSequence(tour, remove);
        }

        /// <summary>
        /// Enumerates all sequences of the given size in the tour. 
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="size">The size of the sequences.</param>
        /// <param name="loopAround">Loop around the start of the tour if it's closed.</param>
        /// <returns>An enumerable of all the sequences.</returns>
        public static IEnumerable<int[]> Sequences(this IReadOnlyTour tour, int size, bool loopAround = true)
        {
            return new SequenceEnumerable(tour, tour.IsClosed(), size);
        }
    }
}