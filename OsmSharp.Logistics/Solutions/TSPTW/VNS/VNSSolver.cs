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

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSPTW.Objectives;
using OsmSharp.Logistics.Solvers;
using OsmSharp.Logistics.Solvers.VNS;

namespace OsmSharp.Logistics.Solutions.TSPTW.VNS
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
            : base(new OsmSharp.Logistics.Solvers.SolverObjectiveWrapper<ITSPTW, ITSPTWObjective, IRoute>(
                new VNSConstructionSolver(), new FeasibleObjective(), (p, o, s) => o.Calculate(p, s)), new Random.Random1Shift(),
                    new VNDOperator(), (i, l, p, o, s) =>
                    {
                        return l > 1;
                    })
        {

        }
    }
}