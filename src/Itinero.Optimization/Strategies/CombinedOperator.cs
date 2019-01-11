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

namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// An operator that combines several operators into one.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    internal sealed class CombinedOperator<TCandidate> : Operator<TCandidate>
        where TCandidate : class
    {
        private readonly Operator<TCandidate> _op;
        private readonly Operator<TCandidate>[] _operators;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="op">The first operator.</param>
        /// <param name="ops">The operators.</param>
        public CombinedOperator(Operator<TCandidate> op, params Operator<TCandidate>[] ops)
        {
            _op = op;
            _operators = ops;

            this.Name = _op.Name;
            if (_operators == null) return;
            foreach (var t in _operators)
            {
                this.Name += $"_{t}";
            }
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
            return ApplyAll(_op, _operators, candidate);
        }

        internal static bool ApplyAll(Operator<TCandidate> op, Operator<TCandidate>[] operators, TCandidate candidate)
        {
            var success = op.Apply(candidate);

            if (operators == null) return success;
            for (var i = 0; i < operators.Length; i++)
            {
                if (operators[i].Apply(candidate))
                {
                    success = true;
                }
            }

            return success;
        }
    }
}