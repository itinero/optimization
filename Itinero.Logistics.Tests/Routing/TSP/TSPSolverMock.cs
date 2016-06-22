// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSP;
using Itinero.Logistics.Solvers;

namespace Itinero.Logistics.Tests.Routing.TSP
{
    /// <summary>
    /// A mock-solver that has a pre-determined order.
    /// </summary>
    public class TSPSolverMock : SolverBase<float, ITSP<float>, ITSPObjective<float>, IRoute>
    {
        private readonly IRoute _route;
        private readonly float _fitness;

        /// <summary>
        /// Creates a new mock-solver.
        /// </summary>
        public TSPSolverMock(IRoute route, float fitness)
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
        public override IRoute Solve(ITSP<float> problem, ITSPObjective<float> objective, out float fitness)
        {
            fitness = _fitness;
            return _route;
        }
    }
}