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

namespace Itinero.Optimization.Strategies.Iterative
{
    /// <summary>
    /// An operator that repeats another operator a number of times.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    internal sealed class IterativeOperator<TCandidate> : Operator<TCandidate>
        where TCandidate : class
    {
        private readonly Operator<TCandidate> _operator;
        private readonly int _n;
        private readonly bool _stopAtFail;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="n">The numer of times to repeat.</param>
        /// <param name="stopAtFail">When true the iteration stops as soon as the operator fails.</param>
        public IterativeOperator(Operator<TCandidate> op, int n, bool stopAtFail = false)
        {
            _operator = op;
            _n = n;
            _stopAtFail = stopAtFail;

            this.Name = op.Name + "x" + _n.ToString();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        public override bool Apply(TCandidate candidate)
        {
            return Iterate(_operator, candidate, _n, _stopAtFail);
        }

        internal static bool Iterate(Func<TCandidate, bool> op, TCandidate candidate, int n, bool stopAtFail)
        {
            var success = false;
            var i = n;
            while (i > 0)
            {
                i--;

                if (!op(candidate))
                {
                    if (stopAtFail)
                    {
                        return true;
                    }
                }
                else
                {
                    success = true;
                }
            }
            return success;
        }
    }
}