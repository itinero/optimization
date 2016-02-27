// Itinero - OpenStreetMap (OSM) SDK
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

namespace Itinero.Logistics.Solutions
{
    /// <summary>
    /// Abstract representation of nearest neighbours in a sorted collection.
    /// </summary>
    public interface ISortedNearestNeighbours : INearestNeighbours
    {
        /// <summary>
        /// Gets the customer at the given index.
        /// </summary>
        /// <returns></returns>
        int Get(int idx);
    }
}