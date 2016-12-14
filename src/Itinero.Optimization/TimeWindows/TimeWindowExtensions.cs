// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TimeWindows
{
    /// <summary>
    /// Contains extension methods related to time windows.
    /// </summary>
    public static class TimeWindowExtensions
    {
        /// <summary>
        /// Calculates the total minimum diff.
        /// </summary>
        public static float TotalMinDiff<T>(this Tour tour, float[][] times, TimeWindow[] windows)
            where T : struct
        {
            var total = 0f;
            var seconds = 0f;
            var previous = Constants.NOT_SET;
            var enumerator = tour.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    seconds += times[previous][current];
                }
                total += windows[current].MinDiff(seconds);
                previous = current;
            }
            return total;
        }
    }
}