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
    public static class OperatorExtensions
    {        
        /// <summary>
        /// Converts the given operator into an operator that runs applies the given operator until it fails.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Operator<TCandidate> ApplyUntil<TCandidate>(this Operator<TCandidate> oper)
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
            return (candidate) => Iterative.IterativeOperator<TCandidate>.Iterate(oper, candidate, int.MaxValue, true);
        }
        
        /// <summary>
        /// Converts the given operator into an operator that runs applies the given operator until it fails but maximum for a given number of times.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="n">The number of times to iterate.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Operator<TCandidate> ApplyUntil<TCandidate>(this Operator<TCandidate> oper, int n)
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
            return (candidate) => Iterative.IterativeOperator<TCandidate>.Iterate(oper, candidate, n, true);
        }
        
        /// <summary>
        /// Converts the given operator into an operator that runs the given operator for a given number of times.
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="n">The number of times to iterate.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Operator<TCandidate> Iterate<TCandidate>(this Operator<TCandidate> oper, int n)
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
        
        /// <summary>
        /// Converts the given operator into an operator that also applies a set of other given operators..
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="operators">The other operators to apply after.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Operator<TCandidate> ApplyAfter<TCandidate>(this Operator<TCandidate> oper, params Operator<TCandidate>[] operators)
            where TCandidate : class
        {
            return new CombinedOperator<TCandidate>(oper, operators);
        }
        
        /// <summary>
        /// Converts the given operator into an operator that also applies a set of other given operators..
        /// </summary>
        /// <param name="oper">The operator.</param>
        /// <param name="operators">The other operators to apply after.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>>
        public static Func<TCandidate, bool> Iterate<TCandidate>(this Func<TCandidate, bool> oper, params Func<TCandidate, bool>[] operators)
            where TCandidate : class
        {
            return (candidate) =>
            {
                var success = oper(candidate);

                if (operators == null) return success;
                for (var i = 0; i < operators.Length; i++)
                {
                    if (operators[i](candidate))
                    {
                        success = true;
                    }
                }

                return success;
            };
        }
        
        /// <summary>
        /// Converts the given operators into one operator that applies any of the given operators after randomly selecting it.
        /// </summary>
        /// <param name="operators">The operators.</param>
        /// <typeparam name="TCandidate">The candidate type.</typeparam>
        /// <returns>The new operator.</returns>
        public static Operator<TCandidate> ApplyRandom<TCandidate>(this Operator<TCandidate>[] operators)
            where TCandidate : class
        {
            return new Random.RandomSelectedOperator<TCandidate>(operators);
        }
    }
}