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

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// A generic max capacity constraint, this can be weight, # of items or cubic centimeters of example.
    /// </summary>
    public class CapacityConstraint
    {
        /// <summary>
        /// Gets or sets the name of this constraint. This is often a type, such as 'time', 'weight' of 'distance'
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// (e.g. a vehicle with a max loading of 100 where the name implies we are talking about weight (thus kg); or 60 where the name implies minutes (so at most a trip of 60 minutes )
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// This is an array of costs; which might be different on each visit.
        /// (e.g. the visit with ID 0 contains 10kg that should be picked up, stop 1 has a load of 25kg, .!--.!--.)
        /// </summary>
        public float[] Values { get; set; }
    }
}