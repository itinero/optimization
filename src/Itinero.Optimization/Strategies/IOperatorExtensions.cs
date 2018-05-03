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
    /// Contains extension methods related to operators.
    /// </summary>
    public static class IOperatorExtensions
    {        
        /// <summary>
        /// Converts the given operator into an operator that runs applies the given operator until it fails.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static IOperator<TCandidate> ApplyUntil<TCandidate>(this IOperator<TCandidate> oper)
            where TCandidate : class
        {
            return new Iterative.IterativeOperator<TCandidate>(oper, int.MaxValue, true);
        } 

        /// <summary>
        /// Converts the given operator into an operator that runs applies the given operator until it fails.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Func<TCandidate, bool> ApplyUntil<TCandidate>(this Func<TCandidate, bool> oper)
            where TCandidate : class
        {
            return (candidate) => 
            {
                return Iterative.IterativeOperator<TCandidate>.Iterate(oper, candidate, int.MaxValue, true);
            };
        }
        
        /// <summary>
        /// Converts the given operator into an operator that runs applies the given operator until it fails but for a given number of times.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="n">The number of times to iterate.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static IOperator<TCandidate> ApplyUntil<TCandidate>(this IOperator<TCandidate> oper, int n)
            where TCandidate : class
        {
            return new Iterative.IterativeOperator<TCandidate>(oper, n, true);
        }
        
        /// <summary>
        /// Converts the given operator into an operator that runs the given operator for a given number of times.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="n">The number of times to iterate.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Func<TCandidate, bool> ApplyUntil<TCandidate>(this Func<TCandidate, bool> oper, int n)
            where TCandidate : class
        {
            return (candidate) => 
            {
                return Iterative.IterativeOperator<TCandidate>.Iterate(oper, candidate, n, true);
            };
        }
        
        /// <summary>
        /// Converts the given operator into an operator that runs the given operator for a given number of times.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="n">The number of times to iterate.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static IOperator<TCandidate> Iterate<TCandidate>(this IOperator<TCandidate> oper, int n)
            where TCandidate : class
        {
            return new Iterative.IterativeOperator<TCandidate>(oper, n);
        }
        
        /// <summary>
        /// Converts the given operator into an operator that runs the given operator for a given number of times.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="n">The number of times to iterate.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Func<TCandidate, bool> Iterate<TCandidate>(this Func<TCandidate, bool> oper, int n)
            where TCandidate : class
        {
            return (candidate) => 
            {
                var success = false;
                var i = n;
                while (i > 0)
                {
                    i--;

                    if (oper(candidate))
                    {
                        success = true;
                    }
                }
                return success;
            };
        }
    }
}