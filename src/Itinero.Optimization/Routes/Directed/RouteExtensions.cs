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
using System;

namespace Itinero.Optimization.Routes.TurningWeights
{
    /// <summary>
    /// Extensions methods for a route containing turns.
    /// </summary>
    public static class RouteExtensions
    {
        /// <summary>
        /// Gets the directedId in the route for the given id. 
        /// </summary>
        public static int GetDirectedId(this Route route, int id)
        {
            var directed = DirectedHelper.BuildDirectedId(id, 0);
            if (route.Contains(directed))
            {
                return directed;
            }
            directed = DirectedHelper.BuildDirectedId(id, 1);
            if (route.Contains(directed))
            {
                return directed;
            }
            directed = DirectedHelper.BuildDirectedId(id, 2);
            if (route.Contains(directed))
            {
                return directed;
            }
            directed = DirectedHelper.BuildDirectedId(id, 3);
            if (route.Contains(directed))
            {
                return directed;
            }
            return Constants.NOT_SET;
        }
    }
}