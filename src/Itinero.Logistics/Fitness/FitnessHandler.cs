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

namespace Itinero.Logistics.Fitness
{
    /// <summary>
    /// Represents a fitness handler.
    /// </summary>
    public abstract class FitnessHandler<TFitness>
    {
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
        public abstract int CompareTo(TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Adds two fitness value.
        /// </summary>
        public abstract TFitness Add(TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Substracts a fitness value from another.
        /// </summary>
        public abstract TFitness Subtract(TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public abstract bool IsZero(TFitness fitness);
    }
}