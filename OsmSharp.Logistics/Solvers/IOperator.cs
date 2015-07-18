// OsmSharp - OpenStreetMap (OSM) SDK
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
    /// Represents a heuristic/solver operator that is applied to a single instance and may lead to better/worse solution.
    /// </summary>
    public interface IOperator<TProblem, TSolution>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness, when > 0 there was an improvement and a reduction in fitness.</param>
        /// <returns></returns>
        bool Apply(TProblem problem, TSolution solution, out double delta);
    }
}
