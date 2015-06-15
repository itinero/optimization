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

namespace OsmSharp.Logistics.Solutions
{
    /// <summary>
    /// Abstract representation of n-nearest neighbours.
    /// </summary>
    public interface INNearestNeighbours : IEnumerable<int>
    {   
        /// <summary>
        /// Gets the # of nearest neighbours.
        /// </summary>
        int N { get; }

        /// <summary>
        /// Gets the maximum weight.
        /// </summary>
        double Max { get; }

        /// <summary>
        /// Determines whether this collection contains the specified element.
        /// </summary>
        /// <returns></returns>
        bool Contains(int customer);
    }
}