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

using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows;

namespace Itinero.Optimization.Test.TSP.TimeWindows
{
    /// <summary>
    /// A few helper methods for creating TSP test problems.
    /// </summary>
    public static class TSPTWHelper
    {
        /// <summary>
        /// Creates a new TSP-TW.
        /// </summary>
        public static TSPTWProblem CreateTSPTW(int first, int size, float defaultWeight)
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
            var windows = new TimeWindow[size];
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].Min = int.MinValue;
                windows[i].Max = int.MaxValue;
            }
            return new TSPTWProblem(0, weights, windows);
        }

        /// <summary>
        /// Creates a new TSP-TW.
        /// </summary>
        public static TSPTWProblem CreateTSPTW(int first, int last, int size, float defaultWeight)
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
            var windows = new TimeWindow[size];
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].Min = int.MinValue;
                windows[i].Max = int.MaxValue;
            }
            return new TSPTWProblem(first, last, weights, windows);
        }

        /// <summary>
        /// Creates a new TSP-TW.
        /// </summary>
        /// <returns></returns>
        public static TSPTWProblem CreateTSPTW(int first, float[][] weights)
        {
            var windows = new TimeWindow[weights.Length];
            for (int i = 0; i < windows.Length; i++)
            {
                windows[i].Min = int.MinValue;
                windows[i].Max = int.MaxValue;
            }
            return new TSPTWProblem(first, weights, windows);
        }

        /// <summary>
        /// Creates a new TSP-TW.
        /// </summary>
        /// <returns></returns>
        public static TSPTWProblem CreateTSPTW(int first, int last, float[][] weights)
        {
            var windows = new TimeWindow[weights.Length];
            for (var i = 0; i < windows.Length; i++)
            {
                windows[i].Min = int.MinValue;
                windows[i].Max = int.MaxValue;
            }
            return new TSPTWProblem(first, last, weights, windows);
        }

        /// <summary>
        /// Creates a new TSP-TW.
        /// </summary>
        /// <returns></returns>
        public static Optimization.TSP.TimeWindows.Directed.TSPTWProblem CreateDirectedTSPTW(int first, float[][] weights, TimeWindow[] windows, float turnPenalities)
        {
            return new Optimization.TSP.TimeWindows.Directed.TSPTWProblem(first, weights, windows, turnPenalities);
        }

        /// <summary>
        /// Creates a new TSP-TW.
        /// </summary>
        /// <returns></returns>
        public static Optimization.TSP.TimeWindows.Directed.TSPTWProblem CreateDirectedTSPTW(int first, int last, float[][] weights, TimeWindow[] windows, float turnPenalities)
        {
            return new Optimization.TSP.TimeWindows.Directed.TSPTWProblem(first, last, weights, windows, turnPenalities);
        }

        /// <summary>
        /// Creates a directed TSP-TW.
        /// </summary>
        public static Optimization.TSP.TimeWindows.Directed.TSPTWProblem CreateDirectedTSPTW(int first, int size, float defaultWeight, TimeWindow[] windows, float turnPenalities)
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
                            weights[x][y] = defaultWeight;
                        }
                    }
                }
            }
            return new Itinero.Optimization.TSP.TimeWindows.Directed.TSPTWProblem(0, weights, windows, turnPenalities);
        }


        /// <summary>
        /// Creates a directed TSP-TW.
        /// </summary>
        public static Optimization.TSP.TimeWindows.Directed.TSPTWProblem CreateDirectedTSPTW(int first, int size, float defaultWeight, float turnPenalities)
        {
            var windows = new TimeWindow[size];
            for (var i = 0; i < windows.Length; i++)
            {
                windows[i].Min = int.MinValue;
                windows[i].Max = int.MaxValue;
            }
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
            return new Itinero.Optimization.TSP.TimeWindows.Directed.TSPTWProblem(0, weights, windows, turnPenalities);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static Optimization.TSP.TimeWindows.Directed.TSPTWProblem CreateDirectedTSPTW(int first, int last, int size, float defaultWeight, float turnPenalities)
        {
            var windows = new TimeWindow[size];
            for (var i = 0; i < windows.Length; i++)
            {
                windows[i].Min = int.MinValue;
                windows[i].Max = int.MaxValue;
            }
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
            return new Optimization.TSP.TimeWindows.Directed.TSPTWProblem(first, last, weights, windows, turnPenalities);
        }
    }
}