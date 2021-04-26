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
using Itinero.Optimization.Models.Mapping;

namespace Itinero.Optimization.Solvers
{
    /// <summary>
    /// A registry for all solvers.
    /// </summary>
    public class SolverRegistry
    {
        private static readonly Lazy<SolverRegistry> DefaultLazy = new Lazy<SolverRegistry>(() => new SolverRegistry());
        
        /// <summary>
        /// Gets the default solver registry.
        /// </summary>
        public static SolverRegistry Default => DefaultLazy.Value;
        
        private readonly List<SolverHook> Solvers = new List<SolverHook>(new []
        {
            CVRP_ND.CVRPNDSolverHook.Default,
            CVRP.CVRPSolverHook.Default,
            STSP.STSPSolverHook.Default,
            TSP_TW_D.TSPTWDSolverHook.Default,
            TSP_TW.TSPTWSolverHook.Default,
            TSP_D.TSPDSolverHook.Default,
            TSP.TSPSolverHook.Default
        });
        
        /// <summary>
        /// A delegate to define try solve calls.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="intermediateResult">A callback to report on intermediate events if found.</param>
        /// <returns></returns>
        public delegate Result<IEnumerable<(int vehicle, IEnumerable<int>)>> TrySolveDelegate(MappedModel model, 
            Action<IEnumerable<(int vehicle, IEnumerable<int>)>> intermediateResult);

        /// <summary>
        /// Registers a new solver.
        /// </summary>
        /// <param name="name">The name of the solver.</param>
        /// <param name="trySolve">A function to call the solver to try and solve the given model.</param>
        /// <param name="intermediateResult">A callback to report on intermediate results.</param>
        public void Register(string name, TrySolveDelegate trySolve, Action<IList<IEnumerable<int>>> intermediateResult = null)
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
        public IEnumerable<(int vehicle, IEnumerable<int> tour)> Solve(MappedModel model, 
            Action<IEnumerable<(int vehicle, IEnumerable<int> tour)>> intermediateResult)
        {
            if(!model.IsValid(out var failReason)) 
            {
                throw new Exception($"The given model can never be solved: {failReason}");
            }

            var reasonsWhy = new StringBuilder();
            for (var i = Solvers.Count - 1; i >= 0; i--)
            {
                var result = Solvers[i].TrySolve(model, intermediateResult);
                if (!result.IsError)
                {
                    return result.Value;
                }

                reasonsWhy.Append(result.ErrorMessage);
                reasonsWhy.Append(Environment.NewLine);
            }

            throw new Exception("The given model cannot be solved by any of the registered solvers or an unhandled exception occurred: " + reasonsWhy.ToString());
        }
        
        /// <summary>
        /// Represents 
        /// </summary>
        public class SolverHook
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <returns></returns>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the try solve function.
            /// </summary>
            /// <returns></returns>
            public SolverRegistry.TrySolveDelegate TrySolve { get; set; }

            /// <summary>
            /// Returns a description.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"SolverHook -> {this.Name}";
            }
        }
    }
}