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

using Itinero.Logistics.Weights;

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// Represents the default TSP fitness calculation.
    /// </summary>
    public class MinimumWeightObjective<T> : ISTSPObjective<T>
        where T : struct
    {
        private readonly WeightHandler<T> _weightHandler;

        /// <summary>
        /// Creates a new objective.
        /// </summary>
        public MinimumWeightObjective(WeightHandler<T> weightHandler)
        {
            _weightHandler = weightHandler;
        }

        /// <summary>
        /// The default name for this objective.
        /// </summary>
        public const string MinimumWeightObjectiveName = "MIN_WEIGHT";

        /// <summary>
        /// Returns the name of this fitness type.
        /// </summary>
        public string Name
        {
            get { return MinimumWeightObjectiveName; }
        }

        /// <summary>
        /// Calculates the fitness of a given solution based on the given problem definitions.
        /// </summary>
        public float Calculate(ISTSP<T> problem, Routes.IRoute solution)
        {
            var weight = this.CalculateWeight(problem, solution);
            return this.Calculate(problem, solution, weight);
        }

        /// <summary>
        /// Calculates the fitness of a given solution based on the given problem definitions.
        /// </summary>
        public float Calculate(ISTSP<T> problem, Routes.IRoute solution, float weight)
        {
            var nonUsed = problem.Weights.Length - solution.Count;
            return weight * nonUsed;
        }

        /// <summary>
        /// Calculates the fitness of a given solution based on the given problem definitions.
        /// </summary>
        public float CalculateWeight(ISTSP<T> problem, Routes.IRoute solution)
        {
            var fitness = 0f;
            foreach (var pair in solution.Pairs())
            {
                fitness = fitness + _weightHandler.GetTime(problem.Weights[pair.From][pair.To]);
            }

            if (fitness > _weightHandler.GetTime(problem.Max))
            { // oeps, constraint violated.
                fitness = 0;
            }

            return fitness;
        }

        /// <summary>
        /// Calculates the difference between the solution before the shift and after the shift.
        /// </summary>
        public bool ShiftAfter(ISTSP<T> problem, Routes.IRoute route, int customer, int before, out float difference)
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

            difference = -_weightHandler.GetTime(weights[oldBefore][customer])
                    - _weightHandler.GetTime(weights[customer][oldAfter])
                    + _weightHandler.GetTime(weights[oldBefore][oldAfter])
                    - _weightHandler.GetTime(weights[before][newAfter])
                    + _weightHandler.GetTime(weights[before][customer])
                    + _weightHandler.GetTime(weights[customer][newAfter]);
            return true;
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public float IfShiftAfter(ISTSP<T> problem, Routes.IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
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
            
            return -_weightHandler.GetTime(weights[oldBefore][customer])
                    - _weightHandler.GetTime(weights[customer][oldAfter])
                    + _weightHandler.GetTime(weights[oldBefore][oldAfter])
                    - _weightHandler.GetTime(weights[before][newAfter])
                    + _weightHandler.GetTime(weights[before][customer])
                    + _weightHandler.GetTime(weights[customer][newAfter]);
        }
    }
}
