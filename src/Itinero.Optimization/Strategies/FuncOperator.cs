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
    /// An operator defined by a function.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public sealed class FuncOperator<TCandidate> : IOperator<TCandidate>
        where TCandidate : class
    {
        private readonly Func<TCandidate, bool> _func;
        private readonly string _name;

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="func">The function to use.</param>
        public FuncOperator(Func<TCandidate, bool> func)
        {
            _func = func;
            _name = Constants.ANONYMOUS;
        }

        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="func">The function to use.</param>
        /// <param name="name">The name of this strategy.</param>
        public FuncOperator(Func<TCandidate, bool> func, string name)
        {
            _func = func;
            _name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Applies this operator to the given candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        public bool Apply(TCandidate candidate)
        {
            return _func(candidate);
        }

        /// <summary>
        /// Define an implicit type conversion from a function to a strategy instance.
        /// </summary>
        /// <param name="func">The function to use as a strategy.</param>
        /// <returns>The function as a strategy instance.</returns>
        public static implicit operator FuncOperator<TCandidate>(Func<TCandidate, bool> func)
        {
            return new FuncOperator<TCandidate>(func);
        }
    }

    /// <summary>
    /// Extension methods related to the function operator.
    /// </summary>
    public static class FuncOperatorExtensions
    {
        /// <summary>
        /// Converts the function into an operator instance.
        /// </summary>
        /// <param name="func">The function to use as an operator.</param>
        /// <returns>An operator based on the given function.</returns>
        public static FuncOperator<TCandidate> ToOperator<TCandidate>(this Func<TCandidate, bool> func)
            where TCandidate : class
        {
            return new FuncOperator<TCandidate>(func);
        }
        
        /// <summary>
        /// Converts the function into an operator instance.
        /// </summary>
        /// <param name="func">The function to use as an operator.</param>
        /// <param name="name">The name of the operator.</param>
        /// <returns>An operator based on the given function.</returns>
        public static FuncOperator<TCandidate> ToOperator<TCandidate>(this Func<TCandidate, bool> func, string name)
            where TCandidate : class
        {
            return new FuncOperator<TCandidate>(func, name);
        }
    }
}