﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

namespace OsmSharp.Logistics.Solvers
{
    /// <summary>
    /// Abstract representation of a solver.
    /// </summary>
    public interface ISolver<TProblem, TSolution>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        TSolution Solve(TProblem problem);

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <param name="problem">The problem to solver.</param>
        /// <param name="fitness">The fitness value if the solution. Smaller is better.</param>
        /// <returns>The solution.</returns>
        TSolution Solve(TProblem problem, out double fitness);

        /// <summary>
        /// Stops the executing of the solving process.
        /// </summary>
        void Stop();

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        event SolverDelegates.IntermidiateDelegate<TSolution> IntermidiateResult;
    }
}