// Itinero.Logistics - Route optimization for .NET
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

using System;

namespace Itinero.Logistics.Solutions.TSP
{
    /// <summary>
    /// Represents the default TSP fitness calculation.
    /// </summary>
    public class MinimumWeightObjective : ITSPObjective
    {
        /// <summary>
        /// The default name for this objective.
        /// </summary>
        public const string MinimumWeightObjectiveName = "MIN_WEIGHT";

        /// <summary>
        /// Returns the name of this fitness type.
        /// </summary>
        public string Name
        {
            get { return MinimumWeightObjective.MinimumWeightObjectiveName; }
        }

        /// <summary>
        /// Calculates the fitness of a given solution based on the given problem definitions.
        /// </summary>
        /// <returns></returns>
        public double Calculate(ITSP problem, Routes.IRoute solution)
        {
            var fitness = 0.0;
            foreach (var pair in solution.Pairs())
            {
                fitness = fitness + problem.Weights[pair.From][pair.To];
            }
            return fitness;
        }

        /// <summary>
        /// Calculates the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public bool ShiftAfter(ITSP problem, Routes.IRoute route, int customer, int before, out double difference)
        {
            var weights = problem.Weights;

            // shift after and keep all info.
            int oldBefore, oldAfter, newAfter;
            if (!route.ShiftAfter(customer, before, out oldBefore, out oldAfter, out newAfter))
            { // shift did not succeed.
                difference = 0;
                return false;
            } 
            
            if (oldAfter == Constants.END)
            {
                oldAfter = route.First;
            }
            if (newAfter == Constants.END)
            {
                newAfter = route.First;
            }

            difference = - weights[oldBefore][customer]
                    - weights[customer][oldAfter]
                    + weights[oldBefore][oldAfter]
                    - weights[before][newAfter]
                    + weights[before][customer]
                    + weights[customer][newAfter];
            return true;
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public double IfShiftAfter(ITSP problem, Routes.IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
        {
            var weights = problem.Weights;

            if (oldAfter == Constants.END)
            {
                oldAfter = route.First;
            }
            if (newAfter == Constants.END)
            {
                newAfter = route.First;
            }

            return -weights[oldBefore][customer]
                    - weights[customer][oldAfter]
                    + weights[oldBefore][oldAfter]
                    - weights[before][newAfter]
                    + weights[before][customer]
                    + weights[customer][newAfter];
        }
    }
}
