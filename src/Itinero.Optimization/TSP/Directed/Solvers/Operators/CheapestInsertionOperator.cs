// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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
using Itinero.Optimization.Tours;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Tours.TurningWeights;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Directed;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.Directed.Solvers.Operators
{
    /// <summary>
    /// An operator that executes a cheapest insertion operation.
    /// </summary>
    public class CheapestInsertionOperator : IOperator<float, TSProblem, TSPObjective, Tour, float>
    {
        private readonly int _n;

        /// <summary>
        /// Creates a new cheapest insertion operator.
        /// </summary>
        public CheapestInsertionOperator(int n = 1)
        {
            _n = n;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("CI_{0}");
            }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        public bool Supports(TSPObjective objective)
        {
            return true;
        }

        private RandomPool _pool;

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(TSProblem problem, TSPObjective objective, Tour solution, out float delta)
        {
            var before = objective.Calculate(problem, solution);
            var weights = problem.Weights;
            var turnPenalties = problem.TurnPenalties;
            delta = 0;

            // test switching directions in random order.
            if (_pool == null || solution.Count != _pool.Size)
            { // create a new pool.
                _pool = new RandomPool(solution.Count);
            }
            else
            { // just reset the existing one.
                _pool.Reset();
            }

            var i = _n;
            var toInsert = new List<int>();
            while (_pool.MoveNext() && i > 0)
            {
                i--;
                var currentId = _pool.Current;
                var current = solution.GetDirectedId(currentId);
                if (current != Constants.NOT_SET)
                {
                    if (current != solution.First &&
                        solution.Remove(current))
                    {
                        toInsert.Add(current);
                    }
                }
            }

            foreach(var current in toInsert)
            {
                CheapestInsertionDirectedHelper.InsertCheapestDirected(solution, weights, turnPenalties, 
                    DirectedHelper.ExtractId(current));
            }

            var after = objective.Calculate(problem, solution);
            delta = after - before;
            return delta < 0;
        }
    }
}