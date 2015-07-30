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
using OsmSharp.Logistics.Solvers;

namespace OsmSharp.Logistics.Solutions.TSPTW.Random
{
    /// <summary>
    /// A wrapper for a TSP-perturber that can also be used for the TSP-TW.
    /// </summary>
    public class TSPPerturberWrapper : IPerturber<ITSPTW, ITSPTWObjective, IRoute>
    {
        private readonly IPerturber<TSP.ITSP, TSP.ITSPObjective, IRoute> _perturber;

        /// <summary>
        /// Creates a new wrapper.
        /// </summary>
        public TSPPerturberWrapper(IPerturber<TSP.ITSP, TSP.ITSPObjective, IRoute> perturber)
        {
            _perturber = perturber;
        }
        
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return _perturber.Name; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(ITSPTWObjective objective)
        { // it is assumed the objective will be supported, also for the perturber being wrapped.
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSPTW problem, ITSPTWObjective objective, IRoute solution, out double delta)
        {
            return _perturber.Apply(problem, objective, solution, out delta);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSPTW problem, ITSPTWObjective objective, IRoute solution, int level, out double delta)
        {
            return _perturber.Apply(problem, objective, solution, level, out delta);
        }
    }
}