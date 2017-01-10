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
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Tours.TurningWeights;
using System.Collections.Generic;

namespace Itinero.Optimization.STSP.Directed.Solvers.Operators
{
    /// <summary>
    /// An operator that executes a cheapest insertion operation.
    /// </summary>
    public class CheapestInsertionOperator : IOperator<float, STSProblem, STSPObjective, Tour, STSPFitness>
    {
        private readonly int _toRemove;
        private readonly int _toInsert;

        /// <summary>
        /// Creates a new cheapest insertion operator.
        /// </summary>
        public CheapestInsertionOperator(int toRemove = 1, int toInsert = 0)
        {
            _toInsert = toInsert;
            _toRemove = toRemove;
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

            // select random new customers to insert.
            var toInsert = new List<int>();
            if (solution.Count < problem.Weights.Length &&
                _toInsert > 0)
            { 
                var i = _toInsert;
                while (solution.Count > 1 && i > 0)
                {
                    i--;
                    var current = RandomGeneratorExtensions.GetRandom().Generate(problem.Weights.Length / 2);
                    var directedId = solution.GetDirectedId(current);
                    if (directedId == Constants.NOT_SET &&
                        !toInsert.Contains(current))
                    {
                        toInsert.Add(current);
                    }
                }
            }

            // select existing customers, to reinsert.
            if (_toRemove > 0)
            { 
                var i = _toRemove;
                while (solution.Count > 1 && i > 0)
                {
                    i--;
                    var index = RandomGeneratorExtensions.GetRandom().Generate(solution.Count);
                    var directedId = solution.GetCustomerAt(index);
                    if (directedId != Constants.NOT_SET)
                    {
                        var current = DirectedHelper.ExtractId(directedId);
                        if (directedId != solution.First &&
                            directedId != solution.Last &&
                            solution.Remove(directedId) &&
                            !toInsert.Contains(current))
                        {
                            toInsert.Add(current);
                        }
                    }
                }
            }

            // shuffle the customers to insert.
            toInsert.Shuffle();

            // insert all customers without exceeding max.
            var fitness = objective.Calculate(problem, solution);
            foreach (var current in toInsert)
            {
                var cost = solution.InsertCheapestDirected(problem.Weights, problem.TurnPenalties, current,
                    problem.Max - fitness.Weight);
                if (cost > 0)
                {
                    fitness.Weight += cost;
                    fitness.Customers++;
                }
            }

            var after = objective.Calculate(problem, solution);
            delta = objective.Subtract(problem, after, before);
            return objective.CompareTo(problem, before, after) > 0;
        }
    }
}