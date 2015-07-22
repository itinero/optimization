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

namespace OsmSharp.Logistics.Solutions.TSP
{
    /// <summary>
    /// Abstract representation of a TSP.
    /// </summary>
    public interface ITSP : IMatrixWeights, ICloneable
    {
        /// <summary>
        /// Gets the first customer.
        /// </summary>
        int First { get; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        int? Last { get; }

        /// <summary>
        /// Returns the backward n-nearest neighbours.
        /// </summary>
        /// <returns></returns>
        INearestNeighbours GetNNearestNeighboursForward(int n, int customer);

        /// <summary>
        /// Returns the forward n-nearest neighbours.
        /// </summary>
        /// <returns></returns>
        INearestNeighbours GetNNearestNeighboursBackward(int n, int customer);

        /// <summary>
        /// Returns the forward nearest neighbours based on weight.
        /// </summary>
        /// <returns>Customers are sorted based on their weight.</returns>
        ISortedNearestNeighbours GetNearestNeighboursForward(double weight, int customer);

        /// <summary>
        /// Returns the backward nearest neighbours based on weight.
        /// </summary>
        /// <returns>Customers are sorted based on their weight.</returns>
        ISortedNearestNeighbours GetNearestNeighboursBackward(double weight, int customer);

        /// <summary>
        /// Returns an equivalent closed version.
        /// </summary>
        /// <returns></returns>
        ITSP ToClosed();
    }
}