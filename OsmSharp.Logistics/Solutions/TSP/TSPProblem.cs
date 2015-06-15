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
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSP
{
    /// <summary>
    /// A TSP.
    /// </summary>
    public class TSPProblem : ITSP
    {
        /// <summary>
        /// Creates a new TSP.
        /// </summary>
        public TSPProblem(int first, double[][] weights, bool isClosed)
        {
            this.First = first;
            this.Weights = weights;
            this.IsClosed = isClosed;
        }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if this TSP is supposed to be a tour.
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

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, INNearestNeighbours[]> _nearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public INNearestNeighbours GetNNearestNeighbours(int n, int customer)
        {
            if (_nearestNeighbours == null)
            { // not there yet, create.
                _nearestNeighbours = new Dictionary<int, INNearestNeighbours[]>();
            }
            INNearestNeighbours[] nearestNeighbours = null;
            if(!_nearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new INNearestNeighbours[this.Weights.Length];
                _nearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NNearestNeighboursAlgorithm.Forward(this.Weights, n, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

    }
}