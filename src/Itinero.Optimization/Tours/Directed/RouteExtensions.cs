/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using Itinero.Optimization.Algorithms.Directed;
using System;

namespace Itinero.Optimization.Tours.TurningWeights
{
    /// <summary>
    /// Extensions methods for a route containing turns.
    /// </summary>
    public static class RouteExtensions
    {
        /// <summary>
        /// Gets the directedId in the route for the given id. 
        /// </summary>
        public static int GetDirectedId(this Tour tour, int id)
        {
            var directed = DirectedHelper.BuildDirectedId(id, 0);
            if (tour.Contains(directed))
            {
                return directed;
            }
            directed = DirectedHelper.BuildDirectedId(id, 1);
            if (tour.Contains(directed))
            {
                return directed;
            }
            directed = DirectedHelper.BuildDirectedId(id, 2);
            if (tour.Contains(directed))
            {
                return directed;
            }
            directed = DirectedHelper.BuildDirectedId(id, 3);
            if (tour.Contains(directed))
            {
                return directed;
            }
            return Constants.NOT_SET;
        }
    }
}