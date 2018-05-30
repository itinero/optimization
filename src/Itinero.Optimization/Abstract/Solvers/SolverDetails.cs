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
using System.Collections.Generic;
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers
{
    /// <summary>
    /// Represents solver details, hooks custom solver implementations to the solver registry.
    /// </summary>
    public class SolverDetails
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

        /// <summary>
        /// A delegate to define try solve calls.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="intermediateResult">A callback to report on intermediate events if found.</param>
        /// <returns></returns>
        public delegate Result<IList<ITour>> TrySolveDelegate(MappedModel model, Action<IList<ITour>> intermediateResult);

        /// <summary>
        /// Gets or sets the try solve function.
        /// </summary>
        /// <returns></returns>
        public TrySolveDelegate TrySolve { get; set; }
    }
}