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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Sequence.Directed
{
    /// <summary>
    /// The default TSP objective.
    /// </summary>
    public class SequenceDirectedObjective : ObjectiveBase<SequenceDirectedProblem, Tour, float>
    {
        /// <summary>
        /// Gets the value that represents infinity.
        /// </summary>
        public sealed override float Infinite
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Returns true if the object is non-linear.
        /// </summary>
        public sealed override bool IsNonContinuous
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public sealed override string Name
        {
            get
            {
                return "DIR_SEQ";
            }
        }

        /// <summary>
        /// Gets the value that represents 0.
        /// </summary>
        public sealed override float Zero
        {
            get
            { 
                return 0;
            }
        }

        /// <summary>
        /// Adds the two given fitness values.
        /// </summary>
        public sealed override float Add(SequenceDirectedProblem problem, float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }

        /// <summary>
        /// Calculates the fitness value of the given solution.
        /// </summary>
        public sealed override float Calculate(SequenceDirectedProblem problem, Tour solution)
        {
            var weights = problem.Weights;
            var weight = 0f;
            var previousFrom = int.MaxValue;
            var firstTo = int.MaxValue;
            foreach (var directedId in solution)
            {
                // extract turns and stuff from directed id.
                int arrivalId, departureId, id, turn;
                DirectedHelper.ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);

                // add the weight from the previous customer to the current one.
                if (previousFrom != int.MaxValue)
                {
                    weight = weight + weights[previousFrom][arrivalId];
                }
                else
                {
                    firstTo = arrivalId;
                }

                // add turn penalty.
                weight += problem.TurnPenalties[turn];

                previousFrom = departureId;
            }

            // add the weight between last and first.
            if (previousFrom != int.MaxValue &&
                solution.First == solution.Last)
            {
                weight = weight + weights[previousFrom][firstTo];
            }
            return weight;
        }

        /// <summary>
        /// Compares the two fitness values.
        /// </summary>
        public sealed override int CompareTo(SequenceDirectedProblem problem, float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public sealed override bool IsZero(SequenceDirectedProblem problem, float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Subtracts the given fitness values.
        /// </summary>
        public sealed override float Subtract(SequenceDirectedProblem problem, float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }
    }
}