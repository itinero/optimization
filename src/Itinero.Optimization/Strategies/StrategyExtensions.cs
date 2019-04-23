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

namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// Extension methods related to the strategies.
    /// </summary>
    public static class StrategyExtensions
    {
        /// <summary>
        /// Creates a new strategy that uses the given strategy a given number of times and returns the best candidate.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        /// <param name="n">The number of time to repeat.</param>
        /// <param name="stop">The function to determine the stopping condition, if any.</param>
        /// <typeparam name="TProblem">The problem.</typeparam>
        /// <typeparam name="TCandidate">The candidate.</typeparam>
        /// <param name="useParallel">Flag to control parallelism.</param>
        /// <returns></returns>
        public static Func<TProblem, TCandidate> Iterate<TProblem, TCandidate>(this Func<TProblem, TCandidate> strategy, int n, bool useParallel = false, Func<TCandidate, bool> stop = null)
        {
            return (p) => Iterative.IterativeStrategy<TProblem, TCandidate>.Iterate(strategy, p, n, stop, useParallel);
        }

        /// <summary>
        /// Creates a new strategy that uses the given strategy a given number of times and returns the best candidate.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        /// <param name="n">The number of time to repeat.</param>
        /// <param name="stop">The function to determine the stopping condition, if any.</param>
        /// <typeparam name="TProblem">The problem.</typeparam>
        /// <typeparam name="TCandidate">The candidate.</typeparam>
        /// <param name="useParallel">Flag to control parallelism.</param>
        /// <returns></returns>
        public static Strategy<TProblem, TCandidate> Iterate<TProblem, TCandidate>(this Strategy<TProblem, TCandidate> strategy, int n, bool useParallel = false, Func<TCandidate, bool> stop = null)
        {
            return new Iterative.IterativeStrategy<TProblem, TCandidate>(strategy, n, stop, useParallel);
        }

        /// <summary>
        /// Creates a new strategy that uses the given strategy and applies the given operator after.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        /// <param name="op">The operator to apply after.</param>
        /// <typeparam name="TProblem">The problem.</typeparam>
        /// <typeparam name="TCandidate">The candidate.</typeparam>
        /// <returns></returns>
        public static Strategy<TProblem, TCandidate> ApplyAfter<TProblem, TCandidate>(
            this Strategy<TProblem, TCandidate> strategy, Operator<TCandidate> op)
            where TCandidate : class
        {
            return new StrategyAndOperatorStrategy<TProblem, TCandidate>(strategy, op);
        }
    }
}