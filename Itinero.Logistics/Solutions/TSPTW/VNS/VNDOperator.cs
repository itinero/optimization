﻿// Itinero.Logistics - Route optimization for .NET
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
    /// A VND-operator/solver.
    /// </summary>
    public class VNDOperator<T> : VNSSolver<T, ITSPTW<T>, ITSPTWObjective<T>, IRoute>
        where T : struct
    {
        /// <summary>
        /// Creates a new VND-operator/solver.
        /// </summary>
        public VNDOperator()
            : base(new Itinero.Logistics.Solvers.SolverObjectiveWrapper<T, ITSPTW<T>, ITSPTWObjective<T>, IRoute>(
                new VNSConstructionSolver<T>(), new FeasibleObjective<T>(), (p, o, s) => o.Calculate(p, s)), 
                new OperatorAsPerturber<T,ITSPTW<T>, ITSPTWObjective<T>, IRoute>(
                new LocalSearch.Local1Shift<T>(true)),
                    new LocalSearch.Local2Opt<T>(), (i, l, p, o, s) =>
                    {
                        return l > 1;
                    })
        {

        }
    }
}