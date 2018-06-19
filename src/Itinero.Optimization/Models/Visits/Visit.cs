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

using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Models.Visits
{
    /// <summary>
    /// Represents a visit, a location and associated costs.
    /// </summary>
    public class Visit
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the timewindow.
        /// </summary>
        public TimeWindow TimeWindow { get; set; }
        
        /// <summary>
        /// Gets or sets the visit costs.
        /// </summary>
        public VisitCost[] VisitCosts { get; set; }
    }
}