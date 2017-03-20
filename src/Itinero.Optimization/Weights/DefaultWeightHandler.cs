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