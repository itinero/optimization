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

namespace Itinero.Optimization.Algorithms.Solvers.Objective
{
    /// <summary>
    /// Represents an objective for an algoritm to work towards and is responsible for handling fitness calculations.
    /// </summary>
    public abstract class ObjectiveBase<TProblem, TSolution, TFitness>
    {
        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the non-continous flag, affects using deltas.
        /// </summary>
        public abstract bool IsNonContinuous
        {
            get;
        }

        /// <summary>
        /// Calculates the fitness of the given solution.
        /// </summary>
        public abstract TFitness Calculate(TProblem problem, TSolution solution);

        /// <summary>
        /// Gets a fitness value that represent zero.
        /// </summary>
        public abstract TFitness Zero
        {
            get;
        }

        /// <summary>
        /// Gets a fitness value that represent the highest possible value.
        /// </summary>
        public abstract TFitness Infinite
        {
            get;
        }

        /// <summary>
        /// Compares fitness1 to fitness2 and returns 1 if fitness1 is better, 0 if equal and -1 if fitness2 is better.
        /// </summary>
        public abstract int CompareTo(TProblem problem, TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Adds two fitness value.
        /// </summary>
        public abstract TFitness Add(TProblem problem, TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Substracts a fitness value from another.
        /// </summary>
        public abstract TFitness Subtract(TProblem problem, TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public abstract bool IsZero(TProblem problem, TFitness fitness);
    }
}
