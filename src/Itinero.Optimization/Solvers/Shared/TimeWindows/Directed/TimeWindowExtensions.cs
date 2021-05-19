using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.TimeWindows.Directed
{
    internal static class TimeWindowExtensions
    {
        /// <summary>
        /// Gets the violated visits.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The weight func.</param>
        /// <param name="windowFunc">The window func.</param>
        /// <returns>The visits that have their time windows violated.</returns>
        public static IEnumerable<(int visit, int position)> GetVisitsWithViolatedWindows(this Tour tour,
            Func<int, int, float> weightFunc, Func<int, TimeWindow> windowFunc)
        {
            var time = 0.0f;
            var position = 0;
            var previous = Tour.NOT_SET;
            using var enumerator = tour.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var currentVisit = DirectedHelper.ExtractVisit(current);
                if (previous != Tour.NOT_SET)
                {
                    // keep track of time.
                    time += weightFunc(previous, current);
                }

                var window = windowFunc(currentVisit);
                if (!window.IsEmpty)
                {
                    if (window.Max < time && position > 1)
                    {
                        // ok, unfeasible and customer is not the first 'moveable' customer.
                        if (currentVisit != DirectedHelper.ExtractVisit(tour.Last))
                        {
                            // when the last customer is fixed, don't try to relocate.
                            yield return (enumerator.Current, position);
                        }
                    }

                    if (window.Min > time)
                    {
                        // wait here!
                        time = window.Min;
                    }
                }

                // increase position.
                position++;
                previous = enumerator.Current;
            }
        }
    }
}