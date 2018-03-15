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

using Itinero.Algorithms.Matrices;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represent the capacity of a vehicle.
    /// </summary>
    public class Capacity
    {
        /// <summary>
        /// The maximum travel time.
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// A collection of custom constraints.
        /// </summary>
        public CapacityConstraint[] Constraints { get; set; }

        /// <summary>
        /// Scales this capacity by the given ratio [0-1] and returns the result.
        /// </summary>
        /// <param name="ratio">The ratio defined in the range [0-1].</param>
        /// <returns></returns>
        public Capacity Scale(float ratio)
        {
            if (this.Constraints == null)
            {
                return new Capacity()
                {
                    Max = this.Max * ratio
                };
            }
            var constraints = new CapacityConstraint[this.Constraints.Length];
            for (var i = 0; i < constraints.Length; i++)
            {
                constraints[i] = new CapacityConstraint()
                {
                    Max = this.Constraints[i].Max * ratio,
                    Values = this.Constraints[i].Values
                };
            }
            return new Capacity()
            {
                Max = this.Max * ratio,
                Constraints = constraints
            };
        }
}
}