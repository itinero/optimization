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

namespace Itinero.Optimization.Abstract.Tours.Typed
{
    /// <summary>
    /// Represents a pair (or a connection between two adjacent customers).
    /// </summary>
    public struct Pair<T>
        where T : struct
    {
        /// <summary>
        /// Creates a new pair.
        /// </summary>
        public Pair(T from, T to)
            : this()
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public T From { get; set; }

        /// <summary>
        /// Returns the to customer.
        /// </summary>
        public T To { get; set; }

        /// <summary>
        /// Returns a description of this edge.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(this.From.ToInvariantString());
            stringBuilder.Append("->");
            stringBuilder.Append(this.To.ToInvariantString());
            return stringBuilder.ToString();
        }

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
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair<T>)
            {
                return ((Pair<T>)obj).From.Equals(this.From) &&
                    ((Pair<T>)obj).To.Equals(this.To);
            }
            return false;
        }
    }
}