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

namespace Itinero.Optimization.Strategies.Random
{    
    /// <summary>
    /// An operator that random applies another operator.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    internal sealed class RandomSelectedOperator<TCandidate> : Operator<TCandidate>
        where TCandidate : class
    {
        private readonly Operator<TCandidate>[] _operators;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="ops">The operators.</param>
        public RandomSelectedOperator(params Operator<TCandidate>[] ops)
        {
            _operators = ops;

            if (_operators == null) return;
            this.Name = "RAN";
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
            return ApplyRandom(_operators, candidate);
        }

        internal static bool ApplyRandom(Operator<TCandidate>[] operators, TCandidate candidate)
        {
            var o = RandomGenerator.Default.Generate(operators.Length);
            return operators[o].Apply(candidate);
        }
    }
}