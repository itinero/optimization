// Itinero - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace Itinero.Logistics.Solvers
{
    /// <summary>
    /// Common delegates for solver implementations.
    /// </summary>
    public static class SolverDelegates
    {
        /// <summary>
        /// A delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate<TSolution>(TSolution result);

        /// <summary>
        /// A delegate for a stop condition.
        /// </summary>
        /// <returns></returns>
        public delegate bool StopConditionDelegate<TProblem, TObjective, TSolution>(int iteration, TProblem problem,
            TObjective objective, TSolution solution);

        /// <summary>
        /// A delegate for a stop condition.
        /// </summary>
        /// <returns></returns>
        public delegate bool StopConditionWithLevelDelegate<TProblem, TObjective, TSolution>(int iteration, int level,
            TProblem problem, TObjective objective, TSolution solution);
    }
}