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

namespace Itinero.Optimization.Test.TSP
{
    /// <summary>
    /// A few helper methods for creating TSP test problems.
    /// </summary>
    public static class TSPHelper
    {
        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TestTSPProblem CreateTSP(int first, int size, float defaultWeight)
        {
            var weights = new float[size][];
            for (int x = 0; x < size; x++)
            {
                weights[x] = new float[size];
                for (int y = 0; y < size; y++)
                {
                    weights[x][y] = defaultWeight;
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                }
            }
            return new TestTSPProblem(0, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TestTSPProblem CreateTSP(int first, int last, int size, float defaultWeight)
        {
            var weights = new float[size][];
            for (int x = 0; x < size; x++)
            {
                weights[x] = new float[size];
                for (int y = 0; y < size; y++)
                {
                    weights[x][y] = defaultWeight;
                }
            }
            return new TestTSPProblem(first, last, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static TestTSPProblem CreateTSP(int first, float[][] weights)
        {
            return new TestTSPProblem(first, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static TestTSPProblem CreateTSP(int first, int last, float[][] weights)
        {
            return new TestTSPProblem(first, last, weights);
        }

        /// <summary>
        /// Creates a directed TSP.
        /// </summary>
        public static Optimization.Solutions.TSP.Directed.TSProblem CreateDirectedTSP(int first, int size, float defaultWeight, float turnPenalities)
        {
            var weights = new float[size * 2][];
            for (int x = 0; x < size * 2; x++)
            {
                weights[x] = new float[size * 2];
                var xDirection = (x % 2);
                for (int y = 0; y < size * 2; y++)
                {
                    var yDirection = (y % 2);
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                    else
                    {
                        if (xDirection == yDirection)
                        {
                            weights[x][y] = defaultWeight;
                        }
                        else
                        {
                            weights[x][y] = defaultWeight + (int)(defaultWeight * 0.1);
                        }
                    }
                }
            }
            return new Itinero.Optimization.Solutions.TSP.Directed.TSProblem(0, weights, turnPenalities);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static Optimization.Solutions.TSP.Directed.TSProblem CreateDirectedTSP(int first, int last, int size, float defaultWeight, float turnPenalities)
        {
            var weights = new float[size * 2][];
            for (int x = 0; x < size * 2; x++)
            {
                weights[x] = new float[size * 2];
                var xDirection = (x % 2);
                for (int y = 0; y < size * 2; y++)
                {
                    var yDirection = (y % 2);
                    if (x == y)
                    {
                        weights[x][y] = 0;
                    }
                    else
                    {
                        if (xDirection == yDirection)
                        {
                            weights[x][y] = defaultWeight;
                        }
                        else
                        {
                            weights[x][y] = defaultWeight + (int)(defaultWeight * 0.1);
                        }
                    }
                }
            }
            return new Optimization.Solutions.TSP.Directed.TSProblem(first, last, weights, turnPenalities);
        }
    }
}
