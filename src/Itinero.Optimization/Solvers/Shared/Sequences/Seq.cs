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
using System.Text;
using Itinero.Optimization.Solvers.Tours.Sequences;

namespace Itinero.Optimization.Solvers.Shared.Sequences
{
    /// <summary>
    /// Represents a sequence.
    /// </summary>
    internal struct Seq
    {
        /// <summary>
        /// Creates a new sequence with the given visits.
        /// </summary>
        /// <param name="visits"></param>
        public Seq(Sequence visits)
        {
            this.Visits = visits;
            this.TotalOriginal = 0;
            this.Reversed = false;
            this.BetweenTravelCost = 0;
            this.BetweenVisitCost = 0;
            this.BetweenTravelCostReversed = 0;
        }

        /// <summary>
        /// Gets or sets the visits.
        /// </summary>
        /// <returns></returns>
        private Sequence Visits { get; set; }

        /// <summary>
        /// The weight including the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float BetweenTravelCost { get; set; }

        /// <summary>
        /// The weight including the travel cost from the first visit and to the last visit.
        /// </summary>
        /// <returns></returns>
        public float BetweenTravelCostReversed { get; set; }

        /// <summary>
        /// The visit cost of the visits between the first and last visit.
        /// </summary>
        public float BetweenVisitCost { get; set; }

        /// <summary>
        /// The total original weight.
        /// </summary>
        /// <returns></returns>
        public float TotalOriginal { get; set; }

        /// <summary>
        /// Gets or sets the reversed flag.
        /// </summary>
        public bool Reversed { get; set; }

        /// <summary>
        /// Gets the total cost between the first and last visit.
        /// </summary>
        public float Between
        {
            get
            {
                if (this.Reversed)
                {
                    return this.BetweenTravelCostReversed + this.BetweenVisitCost;
                }
                return this.BetweenTravelCost + this.BetweenVisitCost;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length => this.Visits.Length;

        /// <summary>
        /// Gets the visit at the given index (taking into account the reverse flag).
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int this[int i]
        {
            get
            {
                if (!this.Reversed) return this.Visits[i];
                
                if (i == 0)
                {
                    return this.Visits[0];
                }
                if (i == this.Visits.Length - 1)
                {
                    return this.Visits[this.Visits.Length - 1];
                }
                return this.Visits[this.Visits.Length - 1 - i];
            }
        }

        /// <summary>
        /// Reverses this sequence.
        /// </summary>
        /// <returns>The reverse equivalent.</returns>
        public Seq Reverse()
        {
            return new Seq()
            {
                Visits = this.Visits,
                TotalOriginal = this.TotalOriginal,
                Reversed = !this.Reversed,
                BetweenTravelCost = this.BetweenTravelCost,
                BetweenVisitCost = this.BetweenVisitCost,
                BetweenTravelCostReversed = this.BetweenTravelCostReversed
            };
        }

        /// <summary>
        /// Returns true if this sequence wraps the first.
        /// </summary>
        public bool Wraps => this.Visits.Wraps;

        /// <summary>
        /// Returns a proper description of this sequence.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var res = new StringBuilder();
            res.Append(this[0]);
            res.Append("->");
            res.Append("[");
            for (var i = 1; i < this.Length - 1; i++)
            {
                res.Append("->");
                res.Append(this[i]);
            }
            res.Append("]");
            res.Append("->");
            res.Append(this[this.Length - 1]);
            return res.ToString();
        }
    }
}