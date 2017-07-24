/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Tours.TurningWeights;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Directed;
using System.Collections.Generic;
using System;

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
            if (problem.Weights.Length <= 2)
            {
                delta = 0;
                return false;
            }

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
                        current != solution.Last &&
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