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
    /// A mockup of a TSP.
    /// </summary>
    public class TSPProblemMock : ITSP
    {
        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public TSPProblemMock(int first, int size, double defaultWeight)
        {
            this.Weights = new double[size][];
            for (int x = 0; x < size; x++)
            {
                this.Weights[x] = new double[size];
                for (int y = 0; y < size; y++)
                {
                    this.Weights[x][y] = defaultWeight;
                }
            }
            this.First = first;
            this.Last = null;
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public TSPProblemMock(int first, int last, int size, double defaultWeight)
        {
            this.Weights = new double[size][];
            for (int x = 0; x < size; x++)
            {
                this.Weights[x] = new double[size];
                for (int y = 0; y < size; y++)
                {
                    this.Weights[x][y] = defaultWeight;
                }
            }
            this.First = first;
            this.Last = last;
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public TSPProblemMock(int first, double[][] weights)
        {
            this.Weights = weights;
            this.First = first;
            this.Last = null;
        }

        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public TSPProblemMock(int first, int last, double[][] weights)
        {
            this.Weights = weights;
            this.First = first;
            this.Last = last;
        }

        /// <summary>
        /// Gets or sets the first customer.
        /// </summary>
        public int First
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the last customer.
        /// </summary>
        public int? Last
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public double[][] Weights
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the n-nearest neighbours.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public Logistics.Solutions.INNearestNeighbours GetNNearestNeighbours(int n, int customer)
        {
            return NNearestNeighboursAlgorithm.Forward(this.Weights, n, customer);
        }

        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        public ITSP ToClosed()
        {
            return new TSPProblemMock(this.First, this.First, this.Weights);
        }
    }
}