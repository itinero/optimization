// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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

using Itinero.Logistics.Solutions.Algorithms;
using Itinero.Logistics.Weights;
using System.Collections.Generic;
using System;

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// A STSP.
    /// </summary>
    public class STSPProblem<T> : ISTSP<T>
        where T : struct
    {
        private readonly WeightHandler<T> _weightHandler;

        /// <summary>
        /// Creates a new empty problem.
        /// </summary>
        protected STSPProblem(WeightHandler<T> weightHandler)
        {
            _weightHandler = weightHandler;
        }

        /// <summary>
        /// Creates a new STSP 'open' STSP with only a start customer.
        /// </summary>
        public STSPProblem(WeightHandler<T> weightHandler, int first, T[][] weights, T max)
        {
            _weightHandler = weightHandler;

            this.First = first;
            this.Last = null;
            this.Weights = weights;
            this.Max = max;

            for (var x = 0; x < this.Weights.Length; x++)
            {
                this.Weights[x][first] = _weightHandler.Zero;
            }
        }

        /// <summary>
        /// Creates a new STSP, 'closed' when first equals last.
        /// </summary>
        public STSPProblem(WeightHandler<T> weightHandler, int first, int last, T[][] weights, T max)
        {
            _weightHandler = weightHandler;

            this.First = first;
            this.Last = last;
            this.Weights = weights;
            this.Max = max;

            this.Weights[first][last] = _weightHandler.Zero;
        }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the last customer.
        /// </summary>
        public int? Last
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the maximum weight.
        /// </summary>
        public T Max
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public T[][] Weights
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the weight handler.
        /// </summary>
        public WeightHandler<T> WeightHandler
        {
            get
            {
                return _weightHandler;
            }
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, INearestNeighbours<T>[]> _forwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public INearestNeighbours<T> GetNNearestNeighboursForward(int n, int customer)
        {
            if (_forwardNearestNeighbours == null)
            { // not there yet, create.
                _forwardNearestNeighbours = new Dictionary<int, INearestNeighbours<T>[]>();
            }
            INearestNeighbours<T>[] nearestNeighbours = null;
            if (!_forwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new INearestNeighbours<T>[this.Weights.Length];
                _forwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm<T>.Forward(this.WeightHandler, this.Weights, n, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, INearestNeighbours<T>[]> _backwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        public INearestNeighbours<T> GetNNearestNeighboursBackward(int n, int customer)
        {
            if (_backwardNearestNeighbours == null)
            { // not there yet, create.
                _backwardNearestNeighbours = new Dictionary<int, INearestNeighbours<T>[]>();
            }
            INearestNeighbours<T>[] nearestNeighbours = null;
            if (!_backwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new INearestNeighbours<T>[this.Weights.Length];
                _backwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm<T>.Backward(this.WeightHandler, this.Weights, n, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<T, ISortedNearestNeighbours<T>[]> _forwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public ISortedNearestNeighbours<T> GetNearestNeighboursForward(T weight, int customer)
        {
            if (_forwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _forwardSortedNearestNeighbours = new Dictionary<T, ISortedNearestNeighbours<T>[]>();
            }
            ISortedNearestNeighbours<T>[] nearestNeighbours = null;
            if (!_forwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new ISortedNearestNeighbours<T>[this.Weights.Length];
                _forwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm<T>.Forward(this.WeightHandler, this.Weights, weight, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<T, ISortedNearestNeighbours<T>[]> _backwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public ISortedNearestNeighbours<T> GetNearestNeighboursBackward(T weight, int customer)
        {
            if (_backwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _backwardSortedNearestNeighbours = new Dictionary<T, ISortedNearestNeighbours<T>[]>();
            }
            ISortedNearestNeighbours<T>[] nearestNeighbours = null;
            if (!_backwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new ISortedNearestNeighbours<T>[this.Weights.Length];
                _backwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm<T>.Backward(this.WeightHandler, this.Weights, weight, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        public virtual ISTSP<T> ToClosed()
        {
            if (this.Last == null)
            { // 'open' problem, just set weights to first to 0.
                // REMARK: weights already set in constructor.
                return new STSPProblem<T>(_weightHandler, this.First, this.First, this.Weights, this.Max);
            }
            else if (this.First != this.Last)
            { // 'open' problem but with fixed weights.
                var weights = new T[this.Weights.Length - 1][];
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

                    weights[xNew] = new T[this.Weights[x].Length - 1];

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

                        if (yNew == xNew)
                        { // make not sense to keep values other than '0' and to make things easier to understand just use '0'.
                            weights[xNew][yNew] = _weightHandler.Zero;
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
                return new STSPProblem<T>(_weightHandler, this.First, this.First, weights, this.Max);
            }
            return this; // problem already closed with first==last.
        }

        /// <summary>
        /// Creates a deep-copy of this problem.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var weights = new T[this.Weights.Length][];
            for (var i = 0; i < this.Weights.Length; i++)
            {
                weights[i] = this.Weights[i].Clone() as T[];
            }
            var clone = new STSPProblem<T>(this.WeightHandler);
            clone.First = this.First;
            clone.Last = this.Last;
            clone.Weights = this.Weights;
            clone.Max = this.Max;
            return clone;
        }
    }
}