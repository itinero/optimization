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
        public TSPProblemMock(int first, int size, double defaultWeight, bool isClosed)
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
            this.IsClosed = isClosed;
            this.First = first;
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
        /// Gets or sets the IsClosed flag.
        /// </summary>
        public bool IsClosed
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
    }
}