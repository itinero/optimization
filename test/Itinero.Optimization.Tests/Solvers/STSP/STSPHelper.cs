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

using System.Collections.Generic;
using Itinero.Optimization.Solvers.STSP;

namespace Itinero.Optimization.Tests.Solvers.STSP
{
    /// <summary>
    /// A few helper methods for creating STSP test problems.
    /// </summary>
    internal static class STSPHelper
    {
        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        public static STSProblem Create(int first, int size, float defaultWeight, float max, IEnumerable<int> visits = null)
        {
            var weights = new float[size][];
            for (var x = 0; x < size; x++)
            {
                weights[x] = new float[size];
                for (var y = 0; y < size; y++)
                {
                    weights[x][y] = defaultWeight;
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                }
            }
            return new STSProblem(0, null, weights, max, visits);
        }

        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        public static STSProblem Create(int first, int last, int size, float defaultWeight, float max, IEnumerable<int> visits = null)
        {
            var weights = new float[size][];
            for (var x = 0; x < size; x++)
            {
                weights[x] = new float[size];
                for (var y = 0; y < size; y++)
                {
                    weights[x][y] = defaultWeight;
                }
            }
            return new STSProblem(first, last, weights, max, visits);
        }

        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        /// <returns></returns>
        public static STSProblem Create(int first, float[][] weights, float max, IEnumerable<int> visits = null)
        {
            return new STSProblem(first, null, weights, max, visits);
        }

        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        /// <returns></returns>
        public static STSProblem Create(int first, int last, float[][] weights, float max, IEnumerable<int> visits = null)
        {
            return new STSProblem(first, last, weights, max, visits);
        }
    }
}