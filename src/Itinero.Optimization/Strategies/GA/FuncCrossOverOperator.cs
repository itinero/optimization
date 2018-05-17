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

namespace Itinero.Optimization.Strategies.GA
{
    /// <summary>
    /// An operator defined by a function.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public sealed class FuncCrossOverOperator<TCandidate> : CrossOverOperator<TCandidate>
    {
        private readonly Func<TCandidate, TCandidate, TCandidate> _func;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="func">The function to use.</param>
        public FuncCrossOverOperator(Func<TCandidate, TCandidate, TCandidate> func)
        {
            _func = func;
            Name = Constants.ANONYMOUS;
        }

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="func">The function to use.</param>
        /// <param name="name">The name of this strategy.</param>
        public FuncCrossOverOperator(Func<TCandidate, TCandidate, TCandidate> func, string name)
        {
            _func = func;
            Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override string Name { get; }

        /// <summary>
        /// Applies this operator to the two candidates and returns a new candidate.
        /// </summary>
        /// <param name="candidate1">The first candidate.</param>
        /// <param name="candidate2">The second candidate.</param>
        /// <returns>The new candidate.</returns>
        public override TCandidate Apply(TCandidate candidate1, TCandidate candidate2)
        {
            return _func(candidate1, candidate2);
        }
    }
}