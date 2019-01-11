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

using System;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.BoundingBox
{
    /// <summary>
    /// Contains functions to help with the calculation of bounding boxes.
    /// </summary>
    public static class BoundingBoxHelper
    {
        /// <summary>
        /// Calculates a bounding box.
        /// </summary>
        /// <param name="tour"></param>
        /// <param name="visitLocationFunc"></param>
        /// <returns></returns>
        public static Box? BoundingBox(this Tour tour, Func<int, Coordinate?> visitLocationFunc)
        {
            Box? box = null;
            foreach (var visit in tour)
            {
                var visitLocation = visitLocationFunc(visit);
                if (!visitLocation.HasValue) break; // if any visit doesn't have a location then the result could be invalid, assume the worst.
                
                box = box?.ExpandWith(visitLocation.Value.Latitude, visitLocation.Value.Longitude) ?? 
                      new Box(visitLocation.Value, visitLocation.Value);
            }
            return box;
        }
    }
}