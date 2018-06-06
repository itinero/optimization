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
using System.Text;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Validation;

namespace Itinero.Optimization.Solvers
{
    /// <summary>
    /// A registry for all solvers.
    /// </summary>
    public static class SolverRegistry
    {
        private static readonly List<SolverHook> Solvers = new List<SolverHook>();
        
        /// <summary>
        /// A delegate to define try solve calls.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="intermediateResult">A callback to report on intermediate events if found.</param>
        /// <returns></returns>
        public delegate Result<IList<IEnumerable<int>>> TrySolveDelegate(Model model, 
            Action<IList<IEnumerable<int>>> intermediateResult);

        /// <summary>
        /// Registers a new solver.
        /// </summary>
        /// <param name="name">The name of the solver.</param>
        /// <param name="trySolve">A function to call the solver to try and solve the given model.</param>
        /// <param name="intermediateResult">A callback to report on intermediate results.</param>
        public static void Register(string name, TrySolveDelegate trySolve, Action<IList<IEnumerable<int>>> intermediateResult = null)
        {
            Solvers.Add(new SolverHook()
            {
                Name = name,
                TrySolve = trySolve
            });
        }

        /// <summary>
        /// Uses the registered solvers to solve the given model.
        /// </summary>
        /// <param name="model">The model to solve.</param>
        /// <param name="intermediateResult">A callback for intermediate results.</param>
        public static IList<IEnumerable<int>> Solve(Model model, Action<IList<IEnumerable<int>>> intermediateResult)
        {
            if(!model.IsValid(out var failReason)) 
            {
                throw new Exception($"The given model can never be solved: {failReason}");
            }

            var reasonsWhy = new StringBuilder();
            for (var i = 0; i < Solvers.Count; i++)
            {
                var result = Solvers[i].TrySolve(model, intermediateResult);
                if (!result.IsError)
                {
                    return result.Value;
                }
                else
                {
                    reasonsWhy.Append(result.ErrorMessage);
                    reasonsWhy.Append(Environment.NewLine);
                }
            }
            throw new Exception("The given model cannot be solved by any of the registered solvers: " + reasonsWhy.ToString());
        }
    }
}