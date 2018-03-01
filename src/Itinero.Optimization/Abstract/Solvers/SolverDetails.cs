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
using Itinero.Optimization.Tours;

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
        /// A delegate to define can solve calls.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reasonIfNot">A human readable feedback on why this model can't be solved.</param>
        /// <returns></returns>
        public delegate bool CanSolveDelegate(AbstractModel model, out string reasonIfNot);

        /// <summary>
        /// The can solve call.
        /// </summary>
        public CanSolveDelegate CanSolve { get; set; }

        /// <summary>
        /// The solve call.
        /// </summary>
        /// <returns></returns>
        public Func<AbstractModel, IList<ITour>> Solve { get; set; }
    }
}