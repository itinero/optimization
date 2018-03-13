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
    /// Represents a triple (or a connection between three adjacent customers).
    /// </summary>
    public struct Triple<T>
        where T : struct
    {
        /// <summary>
        /// Creates a new triple.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="along"></param>
        /// <param name="to"></param>
        public Triple(T from, T along, T to)
            : this()
        {
            this.From = from;
            this.Along = along;
            this.To = to;
        }

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public T From { get; set; }

        /// <summary>
        /// Returns the along customer.
        /// </summary>
        public T Along { get; set; }

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
            return string.Format("{0} -> {1} -> {2}", this.From, this.Along, this.To);
        }

        /// <summary>
        /// Returns a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^
                this.Along.GetHashCode() ^
                this.To.GetHashCode();
        }

        /// <summary>
        /// Returns true if the other object is equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Triple<T>)
            {
                return ((Triple<T>)obj).From.Equals(this.From) &&
                    ((Triple<T>)obj).Along.Equals(this.Along) &&
                    ((Triple<T>)obj).To.Equals(this.To);
            }
            return false;
        }
    }
}
