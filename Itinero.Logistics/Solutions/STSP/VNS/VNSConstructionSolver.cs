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
using Itinero.Logistics.Solutions.STSP.Random;
using Itinero.Logistics.Solvers.Iterative;
using Itinero.Logistics.Solvers.VNS;

namespace Itinero.Logistics.Solutions.STSP.VNS
{
    /// <summary>
    /// Implements a VNS-strategy to construct feasible solution for the STSP from random tours.
    /// </summary>
    public class VNSConstructionSolver : IterativeSolver<ISTSP, ISTSPObjective, IRoute>
    {
        /// <summary>
        /// Creates a new VNS construction solver.
        /// </summary>
        public VNSConstructionSolver()
            : this(10)
        {

        }

        /// <summary>
        /// Creates a new VNS construction solver.
        /// </summary>
        public VNSConstructionSolver(int maxIterations)
            : this(maxIterations, 1000)
        {

        }

        /// <summary>
        /// Creates a new VNS construction solver.
        /// </summary>
        public VNSConstructionSolver(int maxIterations, int levelMax)
            : base(new VNSSolver<ISTSP, ISTSPObjective, IRoute>(new RandomSolver(), new RandomExchange(),
                new LocalSearch.Local1Shift(), (i, l, p, o, r) =>
                {
                    if (l > levelMax)
                    {
                        return true;
                    }
                    return o.Calculate(p, r) == 0;
                }), maxIterations, (i, p, o, r) =>
                {
                    return o.Calculate(p, r) == 0;
                })
        {

        }
    }
}
