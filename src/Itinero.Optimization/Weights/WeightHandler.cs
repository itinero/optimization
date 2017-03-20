/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;

namespace Itinero.Optimization.Weights
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
        /// Divides the given weight with the given divider.
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
