// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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

namespace Itinero.Optimization.Test.STSP
{
    /// <summary>
    /// A few helper methods for creating STSP test problems.
    /// </summary>
    public static class STSPHelper
    {
        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        public static Optimization.STSP.STSProblem CreateSTSP(int first, int size, float defaultWeight, float max)
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
            return new Optimization.STSP.STSProblem(0, weights, max);
        }

        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        public static Optimization.STSP.STSProblem CreateSTSP(int first, int last, int size, float defaultWeight, float max)
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
            return new Optimization.STSP.STSProblem(first, last, weights, max);
        }

        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        /// <returns></returns>
        public static Optimization.STSP.STSProblem CreateSTSP(int first, float[][] weights, float max)
        {
            return new Optimization.STSP.STSProblem(first, weights, max);
        }

        /// <summary>
        /// Creates a new STSP.
        /// </summary>
        /// <returns></returns>
        public static Optimization.STSP.STSProblem CreateSTSP(int first, int last, float[][] weights, float max)
        {
            return new Optimization.STSP.STSProblem(first, last, weights, max);
        }
    }
}