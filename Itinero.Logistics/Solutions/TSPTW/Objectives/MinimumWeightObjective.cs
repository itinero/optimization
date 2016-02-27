// Itinero - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSP;

namespace Itinero.Logistics.Solutions.TSPTW.Objectives
{
    /// <summary>
    /// An objective that leads to minimum weight solutions for the TSP with TW.
    /// </summary>
    public class MinimumWeightObjective : TSPTWObjectiveBase
    {
        /// <summary>
        /// The default name for this objective.
        /// </summary>
        public const string MinimumWeightObjectiveName = "MIN_WEIGHT_TW";

        /// <summary>
        /// Returns the name of this objective.
        /// </summary>
        public override string Name
        {
            get { return MinimumWeightObjective.MinimumWeightObjectiveName; }
        }

        /// <summary>
        /// Calculates the fitness of a TSP solution.
        /// </summary>
        /// <returns></returns>
        public override double Calculate(ITSPTW problem, Routes.IRoute solution)
        {
            var infeasible = 0.0;
            var weight = 0.0;
            var wait = 0.0;
            var previous = Constants.NOT_SET;
            var enumerator = solution.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    weight += problem.Weights[previous][current];
                }
                var window = problem.Windows[current];
                if (window.Max < (weight + wait))
                { // ok, unfeasible.
                    infeasible += (weight + wait) - window.Max;
                }
                if (window.Min > (weight + wait))
                { // wait here!
                    wait += (window.Min - (weight + wait));
                }
                previous = current;
            }
            if(solution.Last.HasValue && solution.First == solution.Last)
            { // also move from last->first
                var current = solution.Last.Value;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    weight += problem.Weights[previous][current];
                }
                if (solution.Last != solution.First)
                {
                    var window = problem.Windows[current];
                    if (window.Max < (weight + wait))
                    { // ok, unfeasible.
                        infeasible += (weight + wait) - window.Max;
                    }
                    if (window.Min > (weight + wait))
                    { // wait here!
                        wait += (window.Min - (weight + wait));
                    }
                }
            }
            // TODO:there is need to seperate the violations, waiting time and weights.
            //      expand the logistics library to allow for pareto-optimal or other 
            //      more advanced weight comparisons.
            //      => this may not be needed anymore for the TSP-TW but for other problems it may be useful.
            return weight + infeasible * 100 + wait / 100;
        }

        /// <summary>
        /// Executes the shift-after and returns the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public override bool ShiftAfter(ITSPTW problem, Routes.IRoute route, int customer, int before, out double difference)
        {
            var fitnessBefore = this.Calculate(problem, route);
            route.ShiftAfter(customer, before);
            var fitnessAfter = this.Calculate(problem, route);
            difference = fitnessBefore - fitnessAfter;
            return true;
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public override double IfShiftAfter(ITSPTW problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
        {
            var fitnessBefore = this.Calculate(problem, route);
            var clone = route.Clone() as IRoute;
            clone.ShiftAfter(customer, before);
            var fitnessAfter = this.Calculate(problem, route);
            return fitnessBefore - fitnessAfter;
        }
    }
}