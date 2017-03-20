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
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Algorithms.Directed;

namespace Itinero.Optimization.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// A local search procedure to optimize turns.
    /// </summary>
    public class Local1ShiftTurn : IOperator<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "LCL_1SHFT_TURN"; }
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(TSPTWProblem problem, TSPTWObjective objective, Tour solution, out float delta)
        {
            foreach(var c in solution.Triples())
            {
                int betterDirectedId;
                bool switchDepartureId, switchedArrivalId;
                if (DirectedHelper.SwitchToBestTurn(c.Along, c.From, c.To, problem.Times, problem.TurnPenalties, out betterDirectedId, 
                    out switchDepartureId, out switchedArrivalId, out delta))
                { // an improvement was found.
                    solution.Replace(c.Along, betterDirectedId);
                    if (switchDepartureId)
                    {
                        var newFromId = DirectedHelper.SwitchDepartureOffset(c.From);
                        solution.Replace(c.From, newFromId);
                    }
                    if (switchedArrivalId)
                    {
                        var newToId = DirectedHelper.SwitchArrivalOffset(c.To);
                        solution.Replace(c.To, newToId);
                    }
                    return true;
                }
            }
            delta = 0;
            return false;
        }

        public bool Supports(TSPTWObjective objective)
        {
            return true;
        }
    }
}