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
using Itinero.Logistics.Solutions.TSP;

namespace Itinero.Logistics.Solutions.TSPTW
{
    /// <summary>
    /// A wrapper to use a TSPTW objective as a TSP objective.
    /// </summary>
    public class TSPObjectiveWrapper<T> : TSPObjective<T>
        where T : struct
    {
        private readonly TSPTWObjective<T> _objective;

        /// <summary>
        /// Creates a new objective wrapper.
        /// </summary>
        /// <param name="objective"></param>
        public TSPObjectiveWrapper(TSPTWObjective<T> objective)
        {
            _objective = objective;
        }

        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public override string Name
        {
            get
            {
                return string.Format("TSP({0})", _objective.Name);
            }
        }

        /// <summary>
        /// Calculates the fitness of the given solution.
        /// </summary>
        public override float Calculate(ITSP<T> problem, IRoute solution)
        {
            return _objective.CalculateTSP(problem, solution);
        }

        /// <summary>
        /// Calculates the change in fitness when a given customer would be shifted.
        /// </summary>
        public override float IfShiftAfter(ITSP<T> problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
        {
            return _objective.IfShiftAfterTSP(problem, route, customer, before, oldBefore, oldAfter, newAfter);
        }

        /// <summary>
        /// Calculates the change in fitness when a given customer would be shifted.
        /// </summary>
        public override bool ShiftAfter(ITSP<T> problem, IRoute route, int customer, int before, out float difference)
        {
            return _objective.ShiftAfterTSP(problem, route, customer, before, out difference);
        }
    }
}