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

using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Weights
{
    /// <summary>
    /// A weight handler to provide add/subtract/multiply and comparison algorithms.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WeightHandler<T> : IComparer<T>
        where T : struct
    {
        /// <summary>
        /// Adds weight1 and weight2.
        /// </summary>
        public abstract T Add(T weight1, T weight2);

        /// <summary>
        /// Subtracts weight2 from weight1.
        /// </summary>
        public abstract T Subtract(T weight1, T weight2);

        /// <summary>
        /// Multiplies the given weight with the given factor.
        /// </summary>
        public abstract T Multiply(T weight, float factor);

        /// <summary>
        /// Multiplies the given weight with the given divider.
        /// </summary>
        public abstract T Divide(T weight, float divider);

        /// <summary>
        /// Returns true if the given weight is bigger in any of it's fields compared to the other weight.
        /// </summary>
        public abstract bool IsLargerThanAny(T weight, T other);

        /// <summary>
        /// Returns true if the given weight is smaller than the other weight.
        /// </summary>
        public bool IsSmallerThan(T weight, T other)
        {
            return this.Compare(weight, other) < 0;
        }
        
        /// <summary>
        /// Returns true if the given weight is smaller than or equal to the other weight.
        /// </summary>
        public bool IsSmallerThanOrEqual(T weight, T other)
        {
            return this.Compare(weight, other) <= 0;
        }

        /// <summary>
        /// Returns true if the given weight is larger than the other weight.
        /// </summary>
        public bool IsLargerThan(T weight, T other)
        {
            return this.Compare(weight, other) > 0;
        }

        /// <summary>
        /// Returns true if the given weight is larger than or equal to the other weight.
        /// </summary>
        public bool IsLargerThanOrEqual(T weight, T other)
        {
            return this.Compare(weight, other) >= 0;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        public abstract int Compare(T x, T y);

        /// <summary>
        /// Gets the time component.
        /// </summary>
        public abstract float GetTime(T weight);

        /// <summary>
        /// Gets the weight that represents zero.
        /// </summary>
        public abstract T Zero
        {
            get;
        }

        /// <summary>
        /// Gets the weight that represents infinity.
        /// </summary>
        public abstract T Infinity
        {
            get;
        }
    }
}