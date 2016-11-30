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

using Itinero.Optimization.Algorithms.Directed;

namespace Itinero.Routing.Optimization.Directed
{
    /// <summary>
    /// Contains extension methods related to turning weights and the algorithm calculating them.
    /// </summary>
    public static class DirectedExtensions
    {
        /// <summary>
        /// Converts a directed customer to a direction location index.
        /// </summary>
        public static int DirectedLocationIndexOf(this DirectedBidirectionalDykstra algorithm, int directedId)
        {
            int arrivalId, departuredId, id, turn;
            DirectedHelper.ExtractAll(directedId, out arrivalId, out departuredId, out id, out turn);
            var index = algorithm.LocationIndexOf(id);
            return DirectedHelper.BuildDirectedId(index, turn);
        }

        /// <summary>
        /// Converts a directed customer to a weight index.
        /// </summary>
        public static int DirectedIndexOf(this DirectedBidirectionalDykstra algorithm, int directedId)
        {
            int arrivalId, departuredId, id, turn;
            DirectedHelper.ExtractAll(directedId, out arrivalId, out departuredId, out id, out turn);
            var index = algorithm.IndexOf(id);
            return DirectedHelper.BuildDirectedId(index, turn);
        }
    }
}
