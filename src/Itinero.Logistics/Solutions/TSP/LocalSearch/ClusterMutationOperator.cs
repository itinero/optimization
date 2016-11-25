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
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// An operator that uses best-effort to transform the route into a route that contains no edge a->b while an edge a->c with weight 0 exists.
    /// </summary>
    public class ClusterMutationOperator<T> : IOperator<T, ITSP<T>, TSPObjective<T>, IRoute, float>
        where T : struct
    {
        private Dictionary<int, int> _shouldFollow;
        private ITSP<T> _problem;

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "CLST"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports(TSPObjective<T> objective)
        {
            return objective.Name == MinimumWeightObjective<T>.MinimumWeightObjectiveName;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSP<T> problem, TSPObjective<T> objective, IRoute solution, out float delta)
        {
            if (objective.Name != MinimumWeightObjective<T>.MinimumWeightObjectiveName) 
            { // check, because assumptions are made in this operator about the objective.
                throw new ArgumentOutOfRangeException(string.Format("{0} cannot handle objective {1}.", this.Name, 
                    objective.Name));
            }

            if(_shouldFollow == null || _problem != problem)
            {
                _problem = problem;
                _shouldFollow = new Dictionary<int, int>();
                for(int x = 0; x < problem.Weights.Length; x++)
                {
                    for(int y = 0; y < problem.Weights[x].Length; y++)
                    {
                        if(x != y && problem.WeightHandler.GetTime(problem.Weights[x][y]) == 0)
                        {
                            _shouldFollow[x] = y;
                        }
                    }
                }
            }

            delta = 0.0f;
            var weights = problem.Weights;
            foreach(var pair in _shouldFollow)
            {
                var insert = pair.Key;
                var customer = pair.Value;
                float localDelta;
                objective.ShiftAfter(problem, solution, customer, insert, out localDelta);
                delta = delta + localDelta;

                insert = customer;
                if (_shouldFollow.TryGetValue(insert, out customer))
                { // move again because the customer before was just moved.
                    objective.ShiftAfter(problem, solution, customer, insert, out localDelta);
                    delta = delta + localDelta;
                }
            }
            return delta < 0;
        }
    }
}