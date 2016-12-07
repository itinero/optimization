// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.STSP.Solvers.Operators
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
                    var current = solution.GetCustomerAt(index);
                    if (current != Constants.NOT_SET)
                    {
                        if (current != solution.First &&
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
                var cost = CheapestInsertion.CalculateCheapest(solution, weights, current, out position);
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