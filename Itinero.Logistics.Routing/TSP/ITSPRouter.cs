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

using System.Collections.Generic;

namespace Itinero.Logistics.Routing.TSP
{
    /// <summary>
    /// Abstract representation of a TSP-router.
    /// </summary>
    public interface ITSPRouter : IAlgorithm
    {
        /// <summary>
        /// Builds the resulting route.
        /// </summary>
        /// <returns></returns>
        Route BuildRoute();

        /// <summary>
        /// Builds the result route in segments divided by routes between customers.
        /// </summary>
        /// <returns></returns>
        List<Route> BuildRoutes();

        /// <summary>
        /// Gets the raw route representing the order of the locations.
        /// </summary>
        Routes.IRoute RawRoute { get; }
    }
}
