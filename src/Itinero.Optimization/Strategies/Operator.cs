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
using System.Threading;

namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// Abstract definition of an operator.
    /// </summary>
    public abstract class Operator<TCandidate>
        where TCandidate : class
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public abstract string Name { get; }

        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate give should be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public abstract bool Apply(TCandidate candidate);

        /// <summary>
        /// Define an implicit type conversion from a function to a operator instance.
        /// </summary>
        /// <param name="func">The function to use.</param>
        /// <returns>The operator instance.</returns>
        public static implicit operator Operator<TCandidate>(Func<TCandidate, bool> func)
        {
            return new FuncOperator<TCandidate>(func);
        }

        /// <summary>
        /// Define an implicit type conversion from a function to an operator instance.
        /// </summary>
        /// <param name="op">The operator instance.</param>
        /// <returns>The function.</returns>
        public static implicit operator Func<TCandidate, bool>(Operator<TCandidate> op)
        {
            return op.Apply;
        }
    }

    /// <summary>
    /// An operator that does nothing.
    /// </summary>
    public sealed class EmptyOperator<TCandidate> : Operator<TCandidate>
        where TCandidate : class
    {
        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        /// <returns></returns>
        public override string Name => "EMPTY";

        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        public override bool Apply(TCandidate candidate)
        {
            return false;
        }
        
        private static readonly ThreadLocal<EmptyOperator<TCandidate>> DefaultLazy = new ThreadLocal<EmptyOperator<TCandidate>>(() => new EmptyOperator<TCandidate>());
        
        /// <summary>
        /// Gets the default empty operator.
        /// </summary>
        public static EmptyOperator<TCandidate> Default => DefaultLazy.Value;
    }
}