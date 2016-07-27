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
    /// Represents a default fitness handler.
    /// </summary>
    public sealed class DefaultFitnessHandler : FitnessHandler<float>
    {
        /// <summary>
        /// Gets a fitness value that represent the highest possible value.
        /// </summary>
        public override float Infinite
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Gets a fitness value that represent zero.
        /// </summary>
        public override float Zero
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds two fitness value.
        /// </summary>
        public override float Add(float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }

        /// <summary>
        /// Compares fitness1 to fitness2 and returns -1 if fitness1 is better, 0 if equal and 1 if fitness2 is better.
        /// </summary>
        public override int CompareTo(float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public override bool IsZero(float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Substracts a fitness value from another.
        /// </summary>
        public override float Subtract(float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }
    }
}
