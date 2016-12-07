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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.VNS;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.TSP.TimeWindows.Solvers.Operators
{
    /// <summary>
    /// A VND-operator/solver.
    /// </summary>
    public class VNDOperator : VNSSolver<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        /// <summary>
        /// Creates a new VND-operator/solver.
        /// </summary>
        public VNDOperator()
            : base(new SolverObjectiveWrapper<float, TSPTWProblem, TSPTWObjective, TSPTWFeasibleObjective, Tour, float>(
                new VNSConstructionSolver(), new TSPTWFeasibleObjective(), (p, o, s) => o.Calculate(p, s)),
                new OperatorAsPerturber<float, TSPTWProblem, TSPTWObjective, Tour, float>(
                    new Local1Shift<TSPTWObjective>(true)),
                    new Local2Opt(), (i, l, p, o, s) =>
                    {
                        return l > 1;
                    })
        {

        }
    }
}