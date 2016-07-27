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

using Itinero.Logistics.Fitness;

namespace Itinero.Logistics.Solutions.TSP
{
    /// <summary>
    /// Represents the default TSP fitness calculation.
    /// </summary>
    public sealed class MinimumWeightObjective<T> : TSPObjective<T>
        where T : struct
    {
        private readonly DefaultFitnessHandler _fitnessHandler = new DefaultFitnessHandler();

        /// <summary>
        /// The default name for this objective.
        /// </summary>
        public const string MinimumWeightObjectiveName = "MIN_WEIGHT";


        /// <summary>
        /// Returns the name of this fitness type.
        /// </summary>
        public override sealed string Name
        {
            get { return MinimumWeightObjective<T>.MinimumWeightObjectiveName; }
        }

        /// <summary>
        /// Gets the fitness handler.
        /// </summary>
        public sealed override FitnessHandler<float> FitnessHandler
        {
            get
            {
                return _fitnessHandler;
            }
        }

        /// <summary>
        /// Calculates the fitness of a given solution based on the given problem definitions.
        /// </summary>
        /// <returns></returns>
        public override sealed float Calculate(ITSP<T> problem, Routes.IRoute solution)
        {
            var fitness = 0f;
            foreach (var pair in solution.Pairs())
            {
                fitness = fitness + problem.WeightHandler.GetTime(problem.Weights[pair.From][pair.To]);
            }
            return fitness;
        }

        /// <summary>
        /// Calculates the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public override sealed bool ShiftAfter(ITSP<T> problem, Routes.IRoute route, int customer, int before, out float difference)
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

            difference = -problem.WeightHandler.GetTime(weights[oldBefore][customer])
                    - problem.WeightHandler.GetTime(weights[customer][oldAfter])
                    + problem.WeightHandler.GetTime(weights[oldBefore][oldAfter])
                    - problem.WeightHandler.GetTime(weights[before][newAfter])
                    + problem.WeightHandler.GetTime(weights[before][customer])
                    + problem.WeightHandler.GetTime(weights[customer][newAfter]);
            return true;
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public override sealed float IfShiftAfter(ITSP<T> problem, Routes.IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
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

            return -problem.WeightHandler.GetTime(weights[oldBefore][customer])
                    - problem.WeightHandler.GetTime(weights[customer][oldAfter])
                    + problem.WeightHandler.GetTime(weights[oldBefore][oldAfter])
                    - problem.WeightHandler.GetTime(weights[before][newAfter])
                    + problem.WeightHandler.GetTime(weights[before][customer])
                    + problem.WeightHandler.GetTime(weights[customer][newAfter]);
        }
    }
}
