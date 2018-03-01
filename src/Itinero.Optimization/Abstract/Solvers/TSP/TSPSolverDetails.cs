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

using System.Collections.Generic;
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Solutions.TSP;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Abstract.Solvers.TSP
{
    /// <summary>
    /// Hooks the TSP solver up to the solver registry by defining solver details.
    /// </summary>
    public static class TSPSolverDetails
    {
        /// <summary>
        /// Gets the default solver details.
        /// </summary>
        /// <returns></returns>
        public static SolverDetails Default = new SolverDetails()
        {
            Name = "TSP",
            CanSolve = CanSolve,
            Solve = Solve
        };

        private static bool CanSolve(AbstractModel model, out string reasonIfNot)
        {
            return model.IsTSP(out reasonIfNot);
        }

        private static IList<ITour> Solve(AbstractModel model)
        {
            string reasonIfFailed;
            var problem = model.ToTSP(out reasonIfFailed);
            var solution = problem.Solve();

            return new List<ITour>(
                new ITour[]
                {
                    solution
                });
        }
    }
}