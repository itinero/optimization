// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Algorithms.TurningWeights;

namespace Itinero.Routing.Optimization.TurningWeights
{
    /// <summary>
    /// Contains extension methods related to turning weights and the algorithm calculating them.
    /// </summary>
    public static class TurningWeightExtensions
    {
        /// <summary>
        /// Converts a directed customer to a direction location index.
        /// </summary>
        public static int DirectedLocationIndexOf(this TurningWeightBidirectionalDykstra algorithm, int id)
        {
            int originalId, direction;
            CustomerHelper.ExtractIdAndDirection(id, out originalId, out direction);
            var index = algorithm.LocationIndexOf(originalId);
            return CustomerHelper.DirectedIdFrom(index, direction);
        }

        /// <summary>
        /// Converts a directed customer to a weight index.
        /// </summary>
        public static int DirectedIndexOf(this TurningWeightBidirectionalDykstra algorithm, int id)
        {
            int originalId, direction;
            CustomerHelper.ExtractIdAndDirection(id, out direction, out originalId);
            var index = algorithm.IndexOf(originalId);
            return CustomerHelper.DirectedIdFrom(index, direction);
        }
    }
}
