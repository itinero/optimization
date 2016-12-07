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

            var toInsert = new List<int>();
            if (solution.Count < problem.Weights.Length &&
                _toInsert > 0)
            { // select random new customer to insert.
                var i = _toInsert;
                while (solution.Count > 1 && i > 0)
                {
                    i--;
                    var current = RandomGeneratorExtensions.GetRandom().Generate(problem.Weights.Length / 2);
                    var directedId = solution.GetDirectedId(current);
                    if (directedId == Constants.NOT_SET)
                    {
                        toInsert.Add(current);
                    }
                }
            }

            if (_toRemove > 0)
            { // select existing customers, to reinsert.
                var i = _toRemove;
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
                            toInsert.Add(DirectedHelper.ExtractId(current));
                        }
                    }
                }
            }

            toInsert.Shuffle();

            var fitness = objective.Calculate(problem, solution);
            foreach (var current in toInsert)
            {
                Pair location;
                int departureOffsetFrom, arrivalOffsetTo, turn;
                var cost = CheapestInsertion.CalculateCheapestDirected(solution, problem.Weights, problem.TurnPenalties, current, out location,
                    out departureOffsetFrom, out arrivalOffsetTo, out turn);
                if (cost + fitness.Weight < problem.Max)
                {
                    solution.InsertDirected(current, location, departureOffsetFrom, arrivalOffsetTo, turn);

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