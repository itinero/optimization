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
using Itinero.Logistics.Solutions.TSPTW.Objectives;
using Itinero.Logistics.Solvers;
using Itinero.Logistics.Solvers.VNS;

namespace Itinero.Logistics.Solutions.TSPTW.VNS
{
    /// <summary>
    /// A VNS-solver for the TSP-TW problem.
    /// </summary>
    public class VNSSolver : VNSSolver<ITSPTW, ITSPTWObjective, IRoute>
    {
        /// <summary>
        /// Creates a new VNS-solver.
        /// </summary>
        public VNSSolver()
            : base(new Itinero.Logistics.Solvers.SolverObjectiveWrapper<ITSPTW, ITSPTWObjective, IRoute>(
                new VNSConstructionSolver(), new FeasibleObjective(), (p, o, s) => o.Calculate(p, s)), new Random.Random1Shift(),
                    new VNDOperator(), (i, l, p, o, s) =>
                    {
                        return l > 1;
                    })
        {

        }
    }
}