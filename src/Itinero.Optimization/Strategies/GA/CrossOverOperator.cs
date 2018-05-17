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
    /// Abstract representation of a cross over operator.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public abstract class CrossOverOperator<TCandidate>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Applies this operator to the two candidates and returns a new candidate.
        /// </summary>
        /// <param name="candidate1">The first candidate.</param>
        /// <param name="candidate2">The second candidate.</param>
        /// <returns>The new candidate.</returns>
        public abstract TCandidate Apply(TCandidate candidate1, TCandidate candidate2);

        /// <summary>
        /// Define an implicit type conversion from a function to an operator instance.
        /// </summary>
        /// <param name="func">The function to use.</param>
        /// <returns>The operator instance.</returns>
        public static implicit operator CrossOverOperator<TCandidate>(Func<TCandidate, TCandidate, TCandidate> func)
        {
            return new FuncCrossOverOperator<TCandidate>(func);
        }

        /// <summary>
        /// Define an implicit type conversion from a function to an operator instance.
        /// </summary>
        /// <param name="op">The operator instance.</param>
        /// <returns>The function.</returns>
        public static implicit operator Func<TCandidate, TCandidate, TCandidate>(CrossOverOperator<TCandidate> op)
        {
            return op.Apply;
        }
    }
}