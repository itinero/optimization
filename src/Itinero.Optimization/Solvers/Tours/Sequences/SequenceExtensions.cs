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
using Itinero.Optimization.Solvers.Shared.Sequences;

namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    /// <summary>
    /// Contains extension methods for sequences.
    /// </summary>
    internal static class SequenceExtensions
    {
        /// <summary>
        /// Calculates the cost of a sequence.
        /// </summary>
        /// <param name="s">The sequence.</param>
        /// <param name="costFunc">The cost function.</param>
        /// <returns></returns>
        public static float Cost(this Sequence s, Func<int, float> costFunc)
        {
            if (costFunc == null)
            {
                return 0f;
            }
            
            var res = 0f;
            for (var i = 0; i < s.Length; i++)
            {
                res += costFunc(s[i]);
            }
            return res;
        }
        
        /// <summary>
        /// Calculates the weight of a part of a sequence.
        /// </summary>
        /// <param name="s">The sequence.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="start">The start in the sequence.</param>
        /// <param name="end">The end in the sequence.</param>
        /// <returns></returns>
        public static float WeightRange(this Sequence s, Func<int, int, float> weightFunc, int start, int end)
        {
            var res = 0f;
            for (var i = start + 1; i <= end; i++)
            {
                res += weightFunc(s[i - 1], s[i]);
            }
            return res;
        }
        
        /// <summary>
        /// Calculates the weight of a sequence.
        /// </summary>
        /// <param name="s">The sequence.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <returns></returns>
        public static float Weight(this Sequence s, Func<int, int, float> weightFunc)
        {
            var res = 0f;
            for (var i = 1; i < s.Length; i++)
            {
                res += weightFunc(s[i - 1], s[i]);
            }
            return res;
        }

        /// <summary>
        /// Calculates the weight of a sequence if it were reversed.
        /// </summary>
        /// <param name="s">The sequence.</param>
        /// <param name="weightFunc">The weight matrix.</param>
        /// <returns></returns>
        public static float WeightReversed(this Sequence s, Func<int, int, float> weightFunc)
        {
            var res = 0f;
            for (var i = 1; i < s.Length; i++)
            {
                res += weightFunc(s[i], s[i - 1]);
            }
            return res;
        }
    }
}