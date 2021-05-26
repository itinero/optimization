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
using System.Diagnostics;
using Itinero.Optimization.Solvers.Shared.Directed;

namespace Itinero.Optimization.Tests.Solvers
{
    /// <summary>
    /// Helper methods to create test weight matrices.
    /// </summary>
    internal static class WeightMatrixHelpers
    {
        /// <summary>
        /// Builds a weight matrix with a default weight.
        /// </summary>
        /// <param name="size">The size of the weight matrix.</param>
        /// <param name="defaultWeight">The default weight.</param>
        /// <returns>Returns a matrix with the given size and with default weight except on the diagonal '0'</returns>
        internal static float[][] Build(int size, float defaultWeight)
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
        
        /// <summary>
        /// Creates a new directed weight matrix filled with a default weight.
        /// </summary>
        internal static float[][] BuildDirected(int size, float defaultWeight)
        {
            var weights = new float[size * 2][];
            for (var x = 0; x < size * 2; x++)
            {
                weights[x] = new float[size * 2];
                var xDirection = (x % 2);
                for (var y = 0; y < size * 2; y++)
                {
                    var yDirection = (y % 2);
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                    else
                    {
                        weights[x][y] = defaultWeight;
                    }
                }
            }
            return weights;
        }
        
        /// <summary> 
        /// Creates a new directed weight matrix filled with a default weight.
        /// </summary>
        /// <param name="size">The size of the weight matrix.</param>
        /// <param name="defaultSame">The weight for when arrival and destination directions match.</param>
        /// <param name="defaultDifferent">The weight for when arrival and destination directions don't match.</param>
        /// <returns>The resulting weight matrix.</returns>
        internal static float[][] BuildDirected(int size, float defaultSame, float defaultDifferent)
        {
            var weights = new float[size * 2][];
            for (var x = 0; x < size * 2; x++)
            {
                weights[x] = new float[size * 2];
                var xDirection = (x % 2);
                for (var y = 0; y < size * 2; y++)
                {
                    var yDirection = (y % 2);
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                    else
                    {
                        if ((xDirection == 0 &&
                            yDirection == 0) ||
                            (xDirection == 1) &&
                            yDirection == 1)
                        {
                            weights[x][y] = defaultSame;
                        }
                        else
                        {
                            weights[x][y] = defaultDifferent;
                        }
                    }
                }
            }
            return weights;
        }
        
        /// <summary>
        /// Builds a function for access to the given matrix.
        /// </summary>
        internal static Func<int, int, float> ToFunc(this float[][] matrix)
        {
            return (x, y) =>
            {
                Debug.Assert(x >= 0 && x < matrix.Length &&
                             y >= 0 && y < matrix[x].Length);
                return matrix[x][y];
            };
        }
        
        /// <summary>
        /// Builds a function for access to the given array.
        /// </summary>
        internal static Func<int, float> ToFunc(this float[] array)
        {
            return (x) => array[x];
        }
        
        /// <summary>
        /// Builds a function for access to the given array.
        /// </summary>
        internal static Func<TurnEnum, float> ToTurnPenaltyFunc(this float[] array)
        {
            return (x) => array[(int)x];
        }
        
        /// <summary>
        /// Sets all 4 directed weights.
        /// </summary>
        /// <param name="weights">The directed weights.</param>
        /// <param name="from">The from visit.</param>
        /// <param name="to">The to visit.</param>
        /// <param name="weight">The weight.</param>
        internal static void SetDirectedWeights(this float[][] weights, int from, int to, float weight)
        {
            weights[from * 2 + 0][to * 2 + 0] = weight;
            weights[from * 2 + 0][to * 2 + 1] = weight;
            weights[from * 2 + 1][to * 2 + 0] = weight;
            weights[from * 2 + 1][to * 2 + 1] = weight;
        }
        
        /// <summary>
        /// Sets all 4 directed weights.
        /// </summary>
        /// <param name="weights">The directed weights.</param>
        /// <param name="from">The from visit.</param>
        /// <param name="to">The to visit.</param>
        /// <param name="bb">The weight.</param>
        /// <param name="bf">The weight.</param>
        /// <param name="fb">The weight.</param>
        /// <param name="ff">The weight.</param>
        internal static void SetDirectedWeights(this float[][] weights, int from, int to, float? bb = null, float? bf = null, float? fb = null, float? ff = null)
        {
            if (ff.HasValue) weights[from * 2 + 0][to * 2 + 0] = ff.Value;
            if (fb.HasValue) weights[from * 2 + 0][to * 2 + 1] = fb.Value;
            if (bf.HasValue) weights[from * 2 + 1][to * 2 + 0] = bf.Value;
            if (bb.HasValue) weights[from * 2 + 1][to * 2 + 1] = bb.Value;
        }
    }
}