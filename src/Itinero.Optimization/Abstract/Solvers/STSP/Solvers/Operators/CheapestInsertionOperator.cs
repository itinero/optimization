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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Abstract.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.STSP.Solvers.Operators
{
    /// <summary>
    /// An operator that executes a cheapest insertion operation.
    /// </summary>
    public class CheapestInsertionOperator : IOperator<float, STSProblem, STSPObjective, Tour, STSPFitness>
    {
        private readonly int _n;
        private readonly bool _insertNew;
        
        /// <summary>
        /// Creates a new cheapest insertion operator.
        /// </summary>
        public CheapestInsertionOperator(int n = 1, bool insertNew = false)
        {
            _n = n;
            _insertNew = insertNew;
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
        public bool Supports(STSPObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(STSProblem problem, STSPObjective objective, Tour solution, out STSPFitness delta)
        {
            var before = objective.Calculate(problem, solution);
            var weights = problem.Weights;
            delta = objective.Zero;

            var i = _n;
            var toInsert = new List<int>();
            if (!_insertNew)
            { // select existing customers, to reinsert.
                while (solution.Count > 1 && i > 0)
                {
                    i--;
                    var index = RandomGeneratorExtensions.GetRandom().Generate(solution.Count);
                    var current = solution.GetVisitAt(index);
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
            }
            else if (solution.Count < problem.Weights.Length)
            { // select random new customer to insert.
                while (solution.Count > 1 && i > 0)
                {
                    i--;
                    var current = RandomGeneratorExtensions.GetRandom().Generate(problem.Weights.Length);
                    if (!solution.Contains(current))
                    {
                        if (current != solution.First &&
                            current != solution.Last &&
                            solution.Remove(current))
                        {
                            toInsert.Add(current);
                        }
                    }
                }
            }

            var fitness = objective.Calculate(problem, solution);
            foreach (var current in toInsert)
            {
                // insert new.
                Pair position;
                var cost = CheapestInsertionHelper.CalculateCheapest(solution, weights, current, out position);
                if (cost + fitness.Weight < problem.Max)
                {
                    solution.InsertAfter(position.From, current);
                    fitness.Weight = fitness.Weight + cost;
                    fitness.Customers = fitness.Customers + 1;
                }
            }

            var after = objective.Calculate(problem, solution);
            delta = objective.Subtract(problem, after, before);
            return objective.CompareTo(problem, before, after) > 0;
        }
    }
}