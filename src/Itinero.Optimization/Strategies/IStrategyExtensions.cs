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
    public static class IStrategyExtensions
    {
        /// <summary>
        /// Creates a new strategy that uses the given strategy a given number of times and returns the best candidate.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        /// <param name="n">The number of time to repeat.</param>
        /// <typeparam name="TProblem">The problem.</typeparam>
        /// <typeparam name="TCandidate">The candidate.</typeparam>
        /// <returns></returns>
        public static Func<TProblem, TCandidate> Iterate<TProblem, TCandidate>(this Func<TProblem, TCandidate> strategy, int n)
        {
            return (p) => 
            {
                return Iterative.IterativeStrategy<TProblem, TCandidate>.Iterate(strategy, p, n);
            };
        }

        /// <summary>
        /// Creates a new strategy that uses the given strategy a given number of times and returns the best candidate.
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        /// <param name="n">The number of time to repeat.</param>
        /// <typeparam name="TProblem">The problem.</typeparam>
        /// <typeparam name="TCandidate">The candidate.</typeparam>
        /// <returns></returns>
        public static IStrategy<TProblem, TCandidate> Iterate<TProblem, TCandidate>(this IStrategy<TProblem, TCandidate> strategy, int n)
        {
            return new Iterative.IterativeStrategy<TProblem, TCandidate>(strategy, n);
        }
    }
}