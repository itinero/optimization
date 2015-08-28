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

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSPTW;
using OsmSharp.Logistics.Solvers;

namespace OsmSharp.Logistics.Tests.Routing.TSPTW
{
    /// <summary>
    /// A mock-solver that has a pre-determined order.
    /// </summary>
    public class TSPTWSolverMock : SolverBase<ITSPTW, ITSPTWObjective, IRoute>
    {
        private readonly IRoute _route;
        private readonly double _fitness;

        /// <summary>
        /// Creates a new mock-solver.
        /// </summary>
        public TSPTWSolverMock(IRoute route, double fitness)
        {
            _route = route;
            _fitness = fitness;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "MOCK"; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSPTW problem, ITSPTWObjective objective, out double fitness)
        {
            fitness = _fitness;
            return _route;
        }
    }
}