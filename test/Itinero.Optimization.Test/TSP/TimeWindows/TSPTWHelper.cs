// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

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