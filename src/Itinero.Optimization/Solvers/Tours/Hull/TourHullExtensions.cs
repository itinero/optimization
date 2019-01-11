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

using System.Collections.Generic;
using Itinero.LocalGeo;

namespace Itinero.Optimization.Solvers.Tours.Hull
{
    /// <summary>
    /// Contains extensions related to the tour hull.
    /// </summary>
    public static class TourHullExtensions
    {
        /// <summary>
        /// Converts the given hull a polygon.
        /// </summary>
        /// <param name="tourHull">The tour hull.</param>
        /// <returns>A polygon.</returns>
        public static Polygon ToPolygon(this TourHull tourHull)
        {
            var polygon = new Polygon()
            {
                ExteriorRing = new List<Coordinate>()
            };
            for (var i = 0; i < tourHull.Count; i++)
            {
                polygon.ExteriorRing.Add(tourHull[i].location);
            }

            polygon.ExteriorRing.Add(tourHull[0].location);

            return polygon;
        }
    }
}