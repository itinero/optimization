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

using Itinero.Optimization.Models.Vehicles.Constraints;

namespace Itinero.Optimization.Models.Vehicles
{
    /// <summary>
    /// Represents a vehicle.
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Gets or sets the profile name.
        /// </summary>
        /// <returns></returns>
        public string Profile { get; set; }

        /// <summary>
        /// Gets or sets the departure location if fixed.
        /// </summary>
        /// <returns></returns>
        public int? Departure { get; set; }

        /// <summary>
        /// Gets or sets the arrival location if fixed.
        /// </summary>
        /// <returns></returns>
        public int? Arrival { get; set; }

        /// <summary>
        /// Gets or sets a turn penalty (if any).
        /// </summary>
        /// <returns></returns>
        public float TurnPentalty { get; set; }

        /// <summary>
        /// Gets or sets the capacity constraints.
        /// </summary>
        public CapacityConstraint[] CapacityConstraints { get; set; }
    }
}