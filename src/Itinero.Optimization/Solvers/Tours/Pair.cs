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

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Represents a pair (or a connection between two adjacent customers).
    /// </summary>
    public struct Pair
    {
        /// <summary>
        /// Creates a new pair.
        /// </summary>
        public Pair(int from, int to)
            : this()
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Returns the to customer.
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// Returns a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^
                   this.To.GetHashCode();
        }

        /// <summary>
        /// Returns true if the other object is equal.
        /// </summary>
        /// <param name="obj">The other.</param>
        /// <returns>True if they represent the same pair.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair pair)
            {
                return pair.From == this.From &&
                       pair.To == this.To;
            }
            return false;
        }

        /// <summary>
        /// Returns a description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.From} -> {this.To}";
        }
    }
}