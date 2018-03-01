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

using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Models
{
    /// <summary>
    /// Represents a mapped model.
    /// </summary>
    public abstract class MappedModel
    {        
        /// <summary>
        /// Builds the abstract model.
        /// </summary>
        /// <returns></returns>
        public abstract Abstract.Models.AbstractModel BuildAbstract();

        /// <summary>
        /// Builds a real-world route based on the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        public abstract Route BuildRoute(ITour tour);

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns></returns>
        public Model Model { get; protected set; }
    }
}