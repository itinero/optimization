// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Solutions.Algorithms;
using OsmSharp.Logistics.Solutions.TSP;

namespace OsmSharp.Logistics.Tests.Solutions.TSP
{
    /// <summary>
    /// A few helper methods for creating TSP test problems.
    /// </summary>
    public static class TSPHelper
    {
        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static ITSP CreateTSP(int first, int size, float defaultWeight)
        {
            var weights = new float[size][];
            for (int x = 0; x < size; x++)
            {
                weights[x] = new float[size];
                for (int y = 0; y < size; y++)
                {
                    weights[x][y] = defaultWeight;
                    if(x == y)
                    {
                        weights[x][y] = 0;
                    }
                }
            }
            return new TSPProblem(0, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public static ITSP CreateTSP(int first, int last, int size, float defaultWeight)
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
            return new TSPProblem(first, last, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static ITSP CreateTSP(int first, float[][] weights)
        {
            return new TSPProblem(first, weights);
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        /// <returns></returns>
        public static ITSP CreateTSP(int first, int last, float[][] weights)
        {
            return new TSPProblem(first, last, weights);
        }
    }
}