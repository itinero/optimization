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

using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.Algorithms
{
    /// <summary>
    /// An enumerable containing n-nearest neighbours and some extra information like maximum weight and n.
    /// </summary>
    public class NNearestNeighbours : HashSet<int>, INNearestNeighbours
    {
        /// <summary>
        /// Creates a new nearest neighbours enumerable.
        /// </summary>
        /// <param name="n"></param>
        public NNearestNeighbours(int n)
        {
            this.N = n;
        }

        /// <summary>
        /// Gets the requested N.
        /// </summary>
        /// <remarks>It's possible this contains less than N if problem size is smaller than N for example.</remarks>
        public int N
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maximum weight of the furthest customer.
        /// </summary>
        public double Max
        {
            get;
            set;
        }
    }
}