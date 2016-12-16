// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.TimeWindows.Directed
{
    /// <summary>
    /// An abstract base class for all TSP-TW objectives.
    /// </summary>
    public abstract class TSPTWObjectiveBase : ObjectiveBase<TSPTWProblem, Tour, float>
    {
        /// <summary>
        /// Calculates the fitness value of the given tour, assumes the tour is closed.
        /// </summary>
        public abstract float Calculate(TSPTWProblem problem, IEnumerable<int> tour);

        /// <summary>
        /// Calculates the fitness value of the given tour, assumes the tour is closed.
        /// </summary>
        public abstract float Calculate(TSPTWProblem problem, IEnumerable<int> tour, out int violated, out float violatedTime, out float waitTime, out float time,
            ref bool[] validFlags);
    }
}