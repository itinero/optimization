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
using OsmSharp.Logistics.Solutions.TSP.GA.Operators;
using OsmSharp.Logistics.Solutions.TSP.LocalSearch;
using OsmSharp.Logistics.Solvers;
using OsmSharp.Logistics.Solvers.GA;

namespace OsmSharp.Logistics.Solutions.TSP.GA.EAX
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    public class EAXSolver : GASolver<ITSP, IRoute>
    {
        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(GASettings settings)
            : base(new HillClimbing3OptSolver(), new EdgeAssemblyCrossover(30, EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom, true),
            new TournamentSelectionOperator<ITSP, IRoute>(10, 0.5), new EmptyOperator<ITSP, IRoute>(), settings)
        {

        }
    }
}
