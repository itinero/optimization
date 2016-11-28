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

namespace Itinero.Optimization.Weights
{
    /// <summary>
    /// A default weight handler.
    /// </summary>
    public sealed class DefaultWeightHandler : WeightHandler<float>
    {
        /// <summary>
        /// Gets the weight that represents infinity.
        /// </summary>
        public override float Infinity
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Gets the weight that represents zero.
        /// </summary>
        public override float Zero
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds weight1 and weight2.
        /// </summary>
        public override float Add(float weight1, float weight2)
        {
            return weight1 + weight2;
        }

        /// <summary>
        /// Returns true if the given weight is bigger in any of it's fields compared to the other weight.
        /// </summary>
        public override bool IsLargerThanAny(float weight, float other)
        {
            return weight > other;
        }

        /// <summary>
        /// Multiplies the given weight with the given factor.
        /// </summary>
        public override float Multiply(float weight, float factor)
        {
            return weight * factor;
        }

        /// <summary>
        /// Divides the given weight by the given divider.
        /// </summary>
        public override float Divide(float weight, float divider)
        {
            return weight / divider;
        }

        /// <summary>
        /// Subtracts weight2 from weight1.
        /// </summary>
        public override float Subtract(float weight1, float weight2)
        {
            return weight1 - weight2;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        public override int Compare(float x, float y)
        {
            return x.CompareTo(y);
        }
    }
}