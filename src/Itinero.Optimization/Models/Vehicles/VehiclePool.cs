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
    /// Represents a pool of vehicles.
    /// </summary>
    public class VehiclePool
    {
        /// <summary>
        /// Gets or sets the vehicles.
        /// </summary>
        /// <returns></returns>
        public Vehicle[] Vehicles { get; set; }

        /// <summary>
        /// Gets or sets the reusable flag.
        /// </summary>
        public bool Reusable { get; set; }

        /// <summary>
        /// Creates a vehicle pool with one vehicle.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="departure">The departure visit.</param>
        /// <param name="arrival">The arrival visit, if any.</param>
        /// <param name="max">The maximum, defined in the metric of the profile (fastest means time, shortest, distance).</param>
        /// <param name="reusable">False if there is just one vehicle avialable, true if the vehicle is reusable.</param>
        /// <returns></returns>
        public static VehiclePool FromProfile(Itinero.Profiles.Profile profile, int departure = 0, int? arrival = 0, float max = float.MaxValue,
            bool reusable = false)
        {
            CapacityConstraint[] constraints = null;
            if (max < float.MaxValue)
            {
                constraints = new[]
                {
                    new CapacityConstraint()
                    {
                        Capacity = max,
                        Metric = profile.Metric.ToModelMetric()
                    }
                };
            }
            return new VehiclePool()
            {
                Reusable = reusable,
                Vehicles = new []
                {
                    new Vehicle()
                    {
                        Profile = profile.FullName,
                        Metric =  profile.Metric.ToModelMetric(),
                        Departure = departure,
                        Arrival = arrival,
                        CapacityConstraints = constraints
                    }
                }
            };
        }
    }
}