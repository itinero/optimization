// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

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