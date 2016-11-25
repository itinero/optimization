// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using Itinero.Logistics.Routes;

namespace Itinero.Logistics.Solutions.TSPTW
{
    /// <summary>
    /// Contains extension methods for IRoute and TSPTW.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Calculates the total minimum diff.
        /// </summary>
        public static float TotalMinDiff<T>(this IRoute route, ITSPTW<T> problem)
            where T : struct
        {
            var total = 0f;
            var seconds = 0f;
            var previous = Constants.NOT_SET;
            var enumerator = route.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if(previous != Constants.NOT_SET)
                { // keep track of time.
                    seconds += problem.WeightHandler.GetTime(problem.Weights[previous][current]);
                }
                total += problem.Windows[current].MinDiff(seconds);
                previous = current;
            }
            return total;
        }
    }
}
