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
using OsmSharp.Logistics.Solvers;
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// An operator that uses best-effort to transform the route into a route that contains no edge a->b while an edge a->c with weight 0 exists.
    /// </summary>
    public class ClusterMutationOperator : IOperator<ITSP, IRoute>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "CLST"; }
        }

        private Dictionary<int, int> _shouldFollow;
        private ITSP _problem;

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(ITSP problem, IRoute solution, out double delta)
        {
            if (problem.Objective.Name != MinimumWeightObjective.MinimumWeightObjectiveName) 
            { // check, because assumptions are made in this operator about the objective.
                throw new ArgumentOutOfRangeException(string.Format("{0} cannot handle objective {1}.", this.Name, 
                    problem.Objective.Name));
            }

            if(_shouldFollow == null || _problem != problem)
            {
                _problem = problem;
                _shouldFollow = new Dictionary<int, int>();
                for(int x = 0; x < problem.Weights.Length; x++)
                {
                    for(int y = 0; y < problem.Weights[x].Length; y++)
                    {
                        if(x != y && problem.Weights[x][y] == 0)
                        {
                            _shouldFollow[x] = y;
                        }
                    }
                }
            }

            delta = 0.0;
            var weights = problem.Weights;
            foreach(var pair in _shouldFollow)
            {
                var insert = pair.Key;
                var customer = pair.Value;
                double localDelta;
                problem.Objective.ShiftAfter(problem, solution, customer, insert, out localDelta);
                delta = delta + localDelta;

                insert = customer;
                if (_shouldFollow.TryGetValue(insert, out customer))
                { // move again because the customer before was just moved.
                    problem.Objective.ShiftAfter(problem, solution, customer, insert, out localDelta);
                    delta = delta + localDelta;
                }
            }
            return delta < 0;
        }
    }
}