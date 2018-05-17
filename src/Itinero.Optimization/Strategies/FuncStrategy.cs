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
    /// A strategy defined by a function.
    /// </summary>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public sealed class FuncStrategy<TProblem, TCandidate> : Strategy<TProblem, TCandidate>
    {
        private readonly Func<TProblem, TCandidate> _func;

        /// <summary>
        /// Creates a new strategy.
        /// </summary>
        /// <param name="func">The function to use.</param>
        public FuncStrategy(Func<TProblem, TCandidate> func)
        {
            _func = func;
            Name = Constants.ANONYMOUS;
        }

        /// <summary>
        /// Creates a new strategy.
        /// </summary>
        /// <param name="func">The function to use.</param>
        /// <param name="name">The name of this strategy.</param>
        public FuncStrategy(Func<TProblem, TCandidate> func, string name)
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
        /// Runs this strategy on the given problem and returns the best candidate.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A candidate.</returns>
        public override TCandidate Search(TProblem problem)
        {
            return _func(problem);
        }
    }

    /// <summary>
    /// Extension methods related to the function strategy.
    /// </summary>
    public static class FuncStrategyExtensions
    {
        /// <summary>
        /// Converts the function into a strategy instance.
        /// </summary>
        /// <param name="func">The function to use as a strategy.</param>
        /// <returns>A strategy based on the given function.</returns>
        public static FuncStrategy<TProblem, TCandidate> ToStrategy<TProblem, TCandidate>(this Func<TProblem, TCandidate> func)
        {
            return new FuncStrategy<TProblem, TCandidate>(func);
        }
        
        /// <summary>
        /// Converts the function into a strategy instance.
        /// </summary>
        /// <param name="func">The function to use as a strategy.</param>
        /// <param name="name">The name of the strategy.</param>
        /// <returns>A strategy based on the given function.</returns>
        public static FuncStrategy<TProblem, TCandidate> ToStrategy<TProblem, TCandidate>(this Func<TProblem, TCandidate> func, string name)
        {
            return new FuncStrategy<TProblem, TCandidate>(func, name);
        }
    }
}