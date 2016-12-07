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
using Itinero.Optimization.Algorithms.Solvers.VNS;
using Itinero.Optimization.Tours;
using Itinero.Optimization.TSP.TimeWindows.Solvers.Operators;

namespace Itinero.Optimization.TSP.TimeWindows.Solvers
{
    /// <summary>
    /// A VNS-solver for the TSP-TW problem.
    /// </summary>
    public class VNSSolver : VNSSolver<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        /// <summary>
        /// Creates a new VNS-solver.
        /// </summary>
        public VNSSolver()
            : base(new SolverObjectiveWrapper<float, TSPTWProblem, TSPTWObjective, TSPTWFeasibleObjective, Tour, float>(
                new VNSConstructionSolver(), new TSPTWFeasibleObjective(), (p, o, s) => o.Calculate(p, s)), new Random1Shift<TSPTWObjective>(),
                    new VNDOperator(), (i, l, p, o, s) =>
                    {
                        return l > 1;
                    })
        {

        }
    }
}