// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Routes;

namespace OsmSharp.Logistics.Solutions.TSPTW
{
    /// <summary>
    /// Contains extension methods for IRoute and TSPTW.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Calculates the total minimum diff.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="problem"></param>
        /// <returns></returns>
        public static double TotalMinDiff(this IRoute route, ITSPTW problem)
        {
            var total = 0.0;
            var seconds = 0.0;
            var previous = Constants.NOT_SET;
            var enumerator = route.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if(previous != Constants.NOT_SET)
                { // keep track of time.
                    seconds += problem.Weights[previous][current];
                }
                total += problem.Windows[current].MinDiff(seconds);
                previous = current;
            }
            return total;
        }
    }
}
