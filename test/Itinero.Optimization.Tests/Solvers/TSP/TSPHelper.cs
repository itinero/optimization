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
using Itinero.Optimization.Solvers.TSP;

namespace Itinero.Optimization.Tests.Solvers.TSP
{
    /// <summary>
    /// A few helper methods for creating TSP test problems.
    /// </summary>
    internal static class TSPHelper
    {
        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TSProblem CreateTSP(int first, int size, float defaultWeight, IEnumerable<int> visits = null)
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
            return new TSProblem(0, null, weights, visits);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TSProblem CreateTSP(int first, int last, int size, float defaultWeight, IEnumerable<int> visits = null)
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
            return new TSProblem(first, last, weights, visits);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static TSProblem CreateTSP(int first, float[][] weights, IEnumerable<int> visits = null)
        {
            return new TSProblem(first, null, weights, visits);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static TSProblem CreateTSP(int first, int last, float[][] weights, IEnumerable<int> visits = null)
        {
            return new TSProblem(first, last, weights, visits);
        }
    }
}