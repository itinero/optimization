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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Tours.TurningWeights;

namespace Itinero.Optimization.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// An operator to execute n random 1-shift* relocations.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere.</remarks>
    public class Random1Shift<TObjective> : IPerturber<float, TSPTWProblem, TObjective, Tour, float>
        where TObjective : ObjectiveBase<TSPTWProblem, Tour, float>
    {
        private readonly RandomGenerator _random = RandomGeneratorExtensions.GetRandom();

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "RAN_1SHFT"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TObjective objective, Tour tour, out float difference)
        {
            return this.Apply(problem, objective, tour, 1, out difference);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TObjective objective, Tour tour, int level, out float difference)
        {
            var original = objective.Calculate(problem, tour);

            if (problem.Times.Length == 1)
            {
                difference = 0;
                return false;
            }

            difference = 0;
            while (level > 0)
            {
                // remove random customer after another random customer.
                var customer = _random.Generate(problem.Times.Length / 2);
                if (customer == problem.First || 
                    customer == problem.Last)
                { // don't move unmovable customers.
                    continue;
                }
                var before = _random.Generate((problem.Times.Length / 2) - 1);
                if (before >= customer)
                { // customer is the same of after.
                    before++;
                }
                if (problem.First != problem.Last &&
                    problem.Last.HasValue &&
                    problem.Last == before)
                { // don't move after a fixed last customer.
                    continue;
                }

                // get the actual id's of the customers.
                var directedCustomer = tour.GetDirectedId(customer);
                var directedBefore = tour.GetDirectedId(before);

                // do the best possible shift after.
                tour.ShiftAfterBestTurns(problem.Times, problem.TurnPenalties, directedCustomer, directedBefore);

                // calculate new fitness.
                var afterShift = objective.Calculate(problem, tour);
                var shiftDiff = afterShift - original;
                difference += shiftDiff;

                // decrease level.
                level--;
            }
            return difference < 0;
        }
    }
}
