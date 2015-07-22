// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSP;

namespace OsmSharp.Logistics.Solutions.TSPTW.Objectives
{
    /// <summary>
    /// An objective that leads to feasible solutions for the TSP with TW.
    /// </summary>
    public class FeasibleObjective : TSPTWObjectiveBase
    {
        /// <summary>
        /// The default name for this objective.
        /// </summary>
        public const string FeasibleObjectiveName = "FEAS";

        /// <summary>
        /// Returns the name of this objective.
        /// </summary>
        public override string Name
        {
            get { return FeasibleObjective.FeasibleObjectiveName; }
        }

        /// <summary>
        /// Calculates the fitness of a TSP solution.
        /// </summary>
        /// <returns></returns>
        public override double Calculate(ITSPTW problem, Routes.IRoute solution)
        {
            var fitness = 0.0;
            var time = 0.0;
            var previous = Constants.NOT_SET;
            var enumerator = solution.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    time += problem.Weights[previous][current];
                }
                var window = problem.Windows[current];
                if(window.Max < time)
                { // ok, unfeasible.
                    fitness += time - window.Max;
                }
                if(window.Min > time)
                { // wait here!
                    time = window.Min;
                }
                previous = current;
            }
            return fitness;
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
