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
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Solutions.TSP
{
    /// <summary>
    /// A TSP.
    /// </summary>
    public class TSPProblem : ITSP
    {
        /// <summary>
        /// Creates a new TSP 'open' TSP with only a start customer.
        /// </summary>
        public TSPProblem(int first, double[][] weights)
            : this(first, weights, new MinimumWeightObjective())
        {

        }

        /// <summary>
        /// Creates a new TSP 'open' TSP with only a start customer.
        /// </summary>
        public TSPProblem(int first, double[][] weights, ITSPObjective fitness)
        {
            this.First = first;
            this.Last = null;
            this.Weights = weights;
            this.Objective = fitness;

            for (var x = 0; x < this.Weights.Length; x++)
            {
                this.Weights[x][first] = 0;
            }
        }

        /// <summary>
        /// Creates a new TSP, 'closed' when first equals last.
        /// </summary>
        public TSPProblem(int first, int last, double[][] weights)
            : this(first, last, weights, new MinimumWeightObjective())
        {

        }

        /// <summary>
        /// Creates a new TSP, 'closed' when first equals last.
        /// </summary>
        public TSPProblem(int first, int last, double[][] weights, ITSPObjective fitness)
        {
            this.First = first;
            this.Last = last;
            this.Weights = weights;
            this.Objective = fitness;

            this.Weights[first][last] = 0;
        }

        /// <summary>
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private TSPProblem()
        {

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
        /// Gets the last customer.
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
        /// Gets the fitness type.
        /// </summary>
        public ITSPObjective Objective
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

        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        public ITSP ToClosed()
        {
            if(this.Last == null)
            { // 'open' problem, just set weights to first to 0.
                // REMARK: weights already set in constructor.
                return new TSPProblem(this.First, this.First, this.Weights, this.Objective);
            }
            else if(this.First != this.Last)
            { // 'open' problem but with fixed weights.
                var weights = new double[this.Weights.Length - 1][];
                for (var x = 0; x < this.Weights.Length; x++)
                {
                    if (x == this.Last)
                    { // skip last edge.
                        continue;
                    }
                    var xNew = x;
                    if (x > this.Last)
                    { // decrease new index.
                        xNew = xNew - 1;
                    }

                    weights[xNew] = new double[this.Weights[x].Length - 1];

                    for (var y = 0; y < this.Weights[x].Length; y++)
                    {
                        if (y == this.Last)
                        { // skip last edge.
                            continue;
                        }
                        var yNew = y;
                        if (y > this.Last)
                        { // decrease new index.
                            yNew = yNew - 1;
                        }

                        if(yNew == xNew)
                        { // make not sense to keep values other than '0' and to make things easier to understand just use '0'.
                            weights[xNew][yNew] = 0;
                        }
                        else if (y == this.First)
                        { // replace -> first with -> last.
                            weights[xNew][yNew] = this.Weights[x][this.Last.Value];
                        }
                        else
                        { // nothing special about this connection, yay!
                            weights[xNew][yNew] = this.Weights[x][y];
                        }
                    }
                }
                return new TSPProblem(this.First, this.First, weights, this.Objective);
            }
            return this; // problem already closed with first==last.
        }

        /// <summary>
        /// Creates a deep-copy of this problem.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var weights = new double[this.Weights.Length][];
            for (var i = 0; i < this.Weights.Length; i++)
            {
                weights[i] = this.Weights[i].Clone() as double[];
            }
            var clone = new TSPProblem();
            clone.First = this.First;
            clone.Last = this.Last;
            clone.Weights = this.Weights;
            clone.Objective = this.Objective;
            return clone;
        }
    }
}