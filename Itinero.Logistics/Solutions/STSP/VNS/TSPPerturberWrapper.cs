// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solvers;

namespace Itinero.Logistics.Solutions.STSP.VNS
{
    /// <summary>
    /// A wrapper for a TSP-perturber that can also be used for the TSP-TW.
    /// </summary>
    public class TSPPerturberWrapper<T> : IPerturber<T, ISTSP<T>, STSPObjective<T>, IRoute, float>
        where T : struct
    {
        private readonly IPerturber<T, TSP.ITSP<T>, TSP.TSPObjective<T>, IRoute, float> _perturber;

        /// <summary>
        /// Creates a new wrapper.
        /// </summary>
        public TSPPerturberWrapper(IPerturber<T, TSP.ITSP<T>, TSP.TSPObjective<T>, IRoute, float> perturber)
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
        public bool Supports(STSPObjective<T> objective)
        { // it is assumed the objective will be supported, also for the perturber being wrapped.
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ISTSP<T> problem, STSPObjective<T> objective, IRoute solution, out float delta)
        {
            return _perturber.Apply(problem.ToTSP(), objective.ToTSPObjective(), solution, out delta);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ISTSP<T> problem, STSPObjective<T> objective, IRoute solution, int level, out float delta)
        {
            return _perturber.Apply(problem.ToTSP(), objective.ToTSPObjective(), solution, level, out delta);
        }
    }
}