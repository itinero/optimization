// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solvers;

namespace Itinero.Logistics.Solutions.STSP.LocalSearch
{
    /// <summary>
    /// A local 1-Shift search for the STSP using the TSP local 1-Shift implementation.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere also called reinsertion heuristic.</remarks>
    public class Local1Shift<T> : IOperator<T, ISTSP<T>, STSPObjective<T>, IRoute, float>
        where T : struct
    {
        private readonly TSP.LocalSearch.Local1Shift<T> _local1shift = new TSP.LocalSearch.Local1Shift<T>();

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get
            {
                return _local1shift.Name;
            }
        }

        /// <summary>
        /// Returns true if the given object is supported.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ISTSP<T> problem, STSPObjective<T> objective, IRoute solution, out float delta)
        {
            var before = objective.Calculate(problem, solution);
            if (_local1shift.Apply(problem.ToTSP(), objective.ToTSPObjective(), solution, out delta))
            {
                var after = objective.Calculate(problem, solution);
                delta = before - after;
                return true;
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Supports(STSPObjective<T> objective)
        {
            return _local1shift.Supports(objective.ToTSPObjective());
        }
    }
}