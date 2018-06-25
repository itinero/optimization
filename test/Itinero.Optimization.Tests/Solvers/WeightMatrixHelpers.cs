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

namespace Itinero.Optimization.Tests.Solvers
{
    /// <summary>
    /// Helper methods to create test weight matrices.
    /// </summary>
    public static class WeightMatrixHelpers
    {
        /// <summary>
        /// Builds a weight matrix with a default weight.
        /// </summary>
        /// <param name="size">The size of the weight matrix.</param>
        /// <param name="defaultWeight">The default weight.</param>
        /// <returns>Returns a matrix with the given size and with default weight except on the diagonal '0'</returns>
        public static float[][] Build(int size, float defaultWeight)
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

            return weights;
        }
    }
}