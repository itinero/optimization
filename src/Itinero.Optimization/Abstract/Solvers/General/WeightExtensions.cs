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
 
namespace Itinero.Optimization.General
{
    /// <summary>
    /// Contains helper functions related to weights.
    /// </summary>
    public static class WeightExtensions
    {
        /// <summary>
        /// Calculates the weight of a sequence.
        /// </summary>
        /// <param name="weights">The weight matrix.</param>
        /// <param name="visits">The sequence.</param>
        /// <returns></returns>
        public static float Seq(this float[][] weights, params int[] visits)
        {
            var res = 0f;
            for (var i = 1; i < visits.Length; i++)
            {
                res += weights[visits[i - 1]][visits[i]];
            }
            return res;
        }

        /// <summary>
        /// Calculates the weight of a sequence.
        /// </summary>
        /// <param name="weights">The weight matrix.</param>
        /// <param name="start">The start in the sequence.</param>
        /// <param name="end">The end in the sequence.</param>
        /// <param name="visits">The sequence.</param>
        /// <returns></returns>
        public static float SeqRange(this float[][] weights, int start, int end, params int[] visits)
        {
            var res = 0f;
            for (var i = start + 1; i <= end; i++)
            {
                res += weights[visits[i - 1]][visits[i]];
            }
            return res;
        }

        /// <summary>
        /// Calculates the weight of a sequence if it were reversed.
        /// </summary>
        /// <param name="weights">The weight matrix.</param>
        /// <param name="start">The start in the sequence.</param>
        /// <param name="end">The end in the sequence.</param>
        /// <param name="visits">The sequence.</param>
        /// <returns></returns>
        public static float SeqReversed(this float[][] weights, int start, int end, params int[] visits)
        {
            var res = 0f;
            for (var i = start + 1; i <= end; i++)
            {
                res += weights[visits[i]][visits[i - 1]];
            }
            return res;
        }

        /// <summary>
        /// Calculates the cost of a sequence.
        /// </summary>
        /// <param name="costs">The costs.</param>
        /// <param name="start">The start in the sequence.</param>
        /// <param name="end">The end in the sequence.</param>
        /// <param name="visits">The sequence.</param>
        /// <returns></returns>
        public static float Seq(this float[] costs, int start, int end, params int[] visits)
        {
            if (costs == null)
            {
                return 0f;
            }
            
            var res = 0f;
            for (var i = start; i <= end; i++)
            {
                res += costs[visits[i]];
            }
            return res;
        }
    }
}