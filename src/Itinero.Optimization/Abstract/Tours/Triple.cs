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

using System;
using Itinero.Optimization.Abstract.Tours.Sequences;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;

namespace Itinero.Optimization.Abstract.Tours
{
    /// <summary>
    /// Represents a triple (or a connection between three adjacent customers).
    /// </summary>
    public struct Triple
    {
        /// <summary>
        /// Creates a new triple.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="along"></param>
        /// <param name="to"></param>
        public Triple(int from, int along, int to)
            : this()
        {
            _raw = new int[3] { from, along, to };
            this._seq = new Solvers.VRP.Operators.Seq(new Sequence(_raw));
            this.From = from;
            this.Along = along;
            this.To = to;
        }

        private Solvers.VRP.Operators.Seq _seq;

        private int[] _raw;

        /// <summary>
        /// Returns the triple as a sequence.
        /// </summary>
        public Solvers.VRP.Operators.Seq AsSequence => _seq;

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public int From
        {
            get
            {
                return _raw[0];
            }
            set
            {
                _raw[0] = value;
            }
        }


        /// <summary>
        /// Returns the along customer.
        /// </summary>
        public int Along { get { return _raw[1]; } set { _raw[1] = value; } }

        /// <summary>
        /// Returns the to customer.
        /// </summary>
        public int To { get { return _raw[2]; } set { _raw[2] = value; } }

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
            if (obj is Triple)
            {
                return ((Triple)obj).From == this.From &&
                    ((Triple)obj).Along == this.Along &&
                    ((Triple)obj).To == this.To;
            }
            return false;
        }
    }
}