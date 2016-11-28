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

namespace Itinero.Optimization.Test.Functional
{
    /// <summary>
    /// A few helper methods for creating TSP test problems.
    /// </summary>
    public static class TSPHelper
    {
        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TSP.TSProblem CreateTSP(int first, int size, float defaultWeight)
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
            return new TSP.TSProblem(0, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TSP.TSProblem CreateTSP(int first, int last, int size, float defaultWeight)
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
            return new TSP.TSProblem(first, last, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static TSP.TSProblem CreateTSP(int first, float[][] weights)
        {
            return new TSP.TSProblem(first, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static TSP.TSProblem CreateTSP(int first, int last, float[][] weights)
        {
            return new TSP.TSProblem(first, last, weights);
        }
        
        /// <summary>
        /// Creates a directed TSP.
        /// </summary>
        public static TSP.TurningWeights.TSProblem CreateDirectedTSP(int first, int size, float defaultWeight, float turnPenalities)
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
            return new Itinero.Optimization.TSP.TurningWeights.TSProblem(0, weights, turnPenalities);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static TSP.TurningWeights.TSProblem CreateDirectedTSP(int first, int last, int size, float defaultWeight, float turnPenalities)
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
            return new TSP.TurningWeights.TSProblem(first, last, weights, turnPenalities);
        }
    }
}