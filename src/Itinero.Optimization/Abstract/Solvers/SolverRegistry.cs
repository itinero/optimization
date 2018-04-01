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
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Abstract.Tours;

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
                TSP.TSPSolverDetails.Default,
                STSP.STSPSolverDetails.Default,
                TSP.TimeWindows.TSPTWSolverDetails.Default,
                TSP.Directed.TSPSolverDetails.Default,
                STSP.Directed.STSPSolverDetails.Default,
                TSP.TimeWindows.Directed.TSPTWSolverDetails.Default,
                VRP.Depot.Capacitated.DepotCVRPSolverDetails.Default,
         //       VRP.NoDepot.Capacitated.NoDepotCVRPSolverDetails.Default
            });

        /// <summary>
        /// Registers a new solver.
        /// </summary>
        /// <param name="name">The name of the solver.</param>
        /// <param name="trySolve">A callback to call the solver to try and solve the given model.</param>
        public static void Register(string name, SolverDetails.TrySolveDelegate trySolve)
        {
            _solvers.Add(new SolverDetails()
            {
                Name = name,
                TrySolve = trySolve
            });
        }

        /// <summary>
        /// Attempts to use the registered solvers to solve the given model.
        /// </summary>
        /// <param name="model">The model to solve.</param>
        public static IList<ITour> Solve(MappedModel model)
        {
            string failReason;
            List<int> faultyVisists;
            if(!model.BuildAbstract().HasSaneConstraints(out failReason, out faultyVisists)){
                throw new Exception("The given model can never be solved, it has insane constraints:\n"+failReason);
            }

            var reasonsWhy = new StringBuilder();
            for (var i = 0; i < _solvers.Count; i++)
            {
                var result = _solvers[i].TrySolve(model);
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