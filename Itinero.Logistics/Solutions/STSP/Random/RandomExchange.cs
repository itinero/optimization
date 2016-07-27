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

using System;
using Itinero.Logistics.Routes;
using Itinero.Logistics.Solvers;
using Itinero.Logistics.Collections;
using System.Linq;
using Itinero.Logistics.Solutions.Algorithms;

namespace Itinero.Logistics.Solutions.STSP.Random
{
    /// <summary>
    /// A random exchange operator for the STSP. Tries to remove customers and then cheapest insert other customers.
    /// </summary>
    public class RandomExchange<T> : IPerturber<T, ISTSP<T>, STSPObjective<T>, IRoute, float>
        where T : struct
    {
        private readonly int _max; // holds the maxium customers to be exchanged.

        /// <summary>
        /// Creates a new random exchange operator.
        /// </summary>
        public RandomExchange(int max = 3)
        {
            _max = max;
        }

        /// <summary>
        /// Returns the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                return "RAN_ADD_REMOVE";
            }
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(ISTSP<T> problem, STSPObjective<T> objective, IRoute solution, out float delta)
        {
            return this.Apply(problem, objective, solution, 1, out delta);
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(ISTSP<T> problem, STSPObjective<T> objective, IRoute solution, int level, out float delta)
        {
            var fitnessBefore = objective.Calculate(problem, solution);
            var rand = Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetRandom();

            // remove some customers from the current solution.
            var pool = new RandomSet<int>(solution);
            var count = System.Math.Min((rand.Generate(_max - 1) + 1) * level, solution.Count / 2);
            while (count > 0)
            {
                // remove random customer after another random customer.
                var customer = pool.RemoveRandom();
                while (customer == problem.First ||
                    customer == problem.Last)
                {
                    customer = pool.RemoveRandom();
                }

                // remove customer.
                solution.Remove(customer);

                // decrease level.
                count--;
            }

            // calculate current fitness 
            var weight = objective.CalculateWeight(problem, solution);

            // build pool of unused customers.
            pool = new RandomSet<int>(Enumerable.Range(0, problem.Weights.Length));
            pool.RemoveAll(solution);

            // insert random customers until there is no more space.
            // try every customer in the pool.
            while (pool.Count > 0)
            {
                var c = pool.RemoveRandom();
                Pair location;
                var cost = solution.CalculateCheapest(problem, c, out location);
                if (cost < float.MaxValue &&
                    cost + weight < problem.WeightHandler.GetTime(problem.Max))
                {
                    solution.InsertAfter(location.From, c);
                    weight += cost;
                }
            }
            delta = fitnessBefore - objective.Calculate(problem, solution, weight);
            return delta > 0;
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(STSPObjective<T> objective)
        {
            return objective.Name == MinimumWeightObjective<T>.MinimumWeightObjectiveName;
        }
    }
}