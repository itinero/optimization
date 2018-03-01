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
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Solutions.TSP;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Abstract.Solvers
{
    /// <summary>
    /// A registry for all solvers.
    /// </summary>
    public static class SolverRegistry
    {
        private static List<SolverDetails> _solvers = new List<SolverDetails>(
            new SolverDetails[]
            {
                TSP.TSPSolverDetails.Default
            });

        /// <summary>
        /// Registers a new solver.
        /// </summary>
        /// <param name="name">The name of the solver.</param>
        /// <param name="canSolve">A callback to decide if this solver can solve the given model.</param>
        /// <param name="solve">A callback to run the solver.</param>
        public static void Register(string name, SolverDetails.CanSolveDelegate canSolve, Func<AbstractModel, IList<ITour>> solve)
        {
            _solvers.Add(new SolverDetails()
            {
                Name = name,
                CanSolve = canSolve,
                Solve = solve
            });
        }

        /// <summary>
        /// Attempts to use the registered solvers to solve the given model.
        /// </summary>
        /// <param name="model">The model to solve.</param>
        public static IList<ITour> Solve(AbstractModel model)
        {
            var reasonsWhy = new StringBuilder();
            for (var i = 0; i < _solvers.Count; i++)
            {
                string reasonWhy;
                if (_solvers[i].CanSolve(model, out reasonWhy))
                {
                    return _solvers[i].Solve(model);
                }
                else
                {
                    reasonsWhy.Append(reasonWhy);
                    reasonsWhy.Append(Environment.NewLine);
                }
            }
            throw new Exception("The given model cannot be solved by any of the registered solvers: " + reasonsWhy.ToString());
        }
    }
}