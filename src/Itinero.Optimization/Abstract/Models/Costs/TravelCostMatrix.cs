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

namespace Itinero.Optimization.Abstract.Models.Costs
{
    /// <summary>
    /// Represents a travel cost matrix, the costs between visits, this could be distance, time or some custom metric.
    /// </summary>
    public class TravelCostMatrix
    {
        /// <summary>
        /// Gets or sets the name of the type of metric used.
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; } = Itinero.Optimization.Models.Metrics.Time;

        /// <summary>
        /// Gets or sets the cost matrix.
        /// </summary>
        public float[][] Costs { get;set; }

        /// <summary>
        /// Gets or sets the directed flag.
        /// </summary>
        /// <remarks>If this is true there are 4 costs per visit pair, for each direction to leave/arrive.</remarks>
        public bool Directed { get; set; }
    }
}