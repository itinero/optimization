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
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.TSP.TimeWindows.Directed
{
    /// <summary>
    /// An objective that leads to feasible solutions for the TSP with TW.
    /// </summary>
    public class TSPTWFeasibleObjective : ObjectiveBase<TSPTWProblem, Tour, float>
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
        /// Gets the name of this objective.
        /// </summary>
        public override string Name
        {
            get
            {
                return "FEAS";
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
        public sealed override float Add(TSPTWProblem problem, float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }

        /// <summary>
        /// Compares the two fitness values.
        /// </summary>
        public sealed override int CompareTo(TSPTWProblem problem, float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public sealed override bool IsZero(TSPTWProblem problem, float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Subtracts the given fitness values.
        /// </summary>
        public sealed override float Subtract(TSPTWProblem problem, float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }

        /// <summary>
        /// Gets the non-lineair flag, affects using deltas.
        /// </summary>
        public override bool IsNonContinuous
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Calculates the fitness of a TSP solution.
        /// </summary>
        /// <returns></returns>
        public sealed override float Calculate(TSPTWProblem problem, Tour solution)
        {
            var times = problem.Times;

            var infeasible = 0f;
            var time = 0f;
            var wait = 0f;
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
                    time = time + times[previousFrom][arrivalId];
                }
                else
                {
                    firstTo = arrivalId;
                }

                // check the windows (before turn-penalties).
                var window = problem.Windows[id];
                if (window.Max < (time + wait))
                { // ok, unfeasible.
                    infeasible += (time + wait) - window.Max;
                }
                if (window.Min > (time + wait))
                { // wait here!
                    wait += (window.Min - (time + wait));
                }

                // add turn penalty.
                time += problem.TurnPenalties[turn];

                previousFrom = departureId;
            }

            // add the weight between last and first.
            if (previousFrom != int.MaxValue)
            {
                time = time + times[previousFrom][firstTo];
            }

            // TODO:there is need to seperate the violations, waiting time and weights.
            //      expand the logistics library to allow for pareto-optimal or other 
            //      more advanced weight comparisons.
            //      => this may not be needed anymore for the TSP-TW but for other problems it may be useful.
            return infeasible;
        }
    }
}
