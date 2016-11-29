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

using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Routes;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.Directed
{
    /// <summary>
    /// The default TSP profile definition.
    /// </summary>
    public sealed class TSProblem
    {
        /// <summary>ra
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private TSProblem()
        {

        }

        /// <summary>
        /// Creates a new TSP 'open' TSP with only a start customer.
        /// </summary>
        public TSProblem(int first, float[][] weights, float turnPenalty)
        {
            this.First = first;
            this.Last = null;
            this.Weights = weights;
            this.TurnPenalty = turnPenalty;

            for (var x = 0; x < this.Weights.Length; x++)
            {
                this.Weights[x][first * 2 + 0] = 0;
                this.Weights[x][first * 2 + 1] = 0;
            }
        }

        /// <summary>
        /// Creates a new TSP, 'closed' when first equals last.
        /// </summary>
        public TSProblem(int first, int last, float[][] weights, float turnPenalty)
        {
            this.First = first;
            this.Last = last;
            this.Weights = weights;
            this.TurnPenalty = turnPenalty;

            this.Weights[first * 2 + 0][last * 2 + 0] = 0;
            this.Weights[first * 2 + 1][last * 2 + 0] = 0;
            this.Weights[first * 2 + 0][last * 2 + 1] = 0;
            this.Weights[first * 2 + 1][last * 2 + 1] = 0;
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights { get; set; }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        public int? Last { get; set; }

        /// <summary>
        /// Gets or sets the turn penalty.
        /// </summary>
        public float TurnPenalty { get; set; }

        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public Route Solve()
        {
            return this.Solve(new Solvers.HillClimbing3OptSolver());
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Route Solve(Algorithms.Solvers.ISolver<float, TSProblem, TSPObjective, Route, float> solver)
        {
            return solver.Solve(this, new TSPObjective());
        }
        
        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, NearestNeighbours[]> _forwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbours GetNNearestNeighboursForward(int n, int id)
        {
            if (_forwardNearestNeighbours == null)
            { // not there yet, create.
                _forwardNearestNeighbours = new Dictionary<int, NearestNeighbours[]>();
            }
            NearestNeighbours[] nearestNeighbours = null;
            if (!_forwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new NearestNeighbours[this.Weights.Length];
                _forwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighbours.ForwardDirected(this.Weights, n, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, NearestNeighbours[]> _backwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbours GetNNearestNeighboursBackward(int n, int id)
        {
            if (_backwardNearestNeighbours == null)
            { // not there yet, create.
                _backwardNearestNeighbours = new Dictionary<int, NearestNeighbours[]>();
            }
            NearestNeighbours[] nearestNeighbours = null;
            if (!_backwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new NearestNeighbours[this.Weights.Length];
                _backwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighbours.BackwardDirected(this.Weights, n, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<float, SortedNearestNeighbours[]> _forwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public SortedNearestNeighbours GetNearestNeighboursForward(float weight, int id)
        {
            if (_forwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _forwardSortedNearestNeighbours = new Dictionary<float, SortedNearestNeighbours[]>();
            }
            SortedNearestNeighbours[] nearestNeighbours = null;
            if (!_forwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new SortedNearestNeighbours[this.Weights.Length];
                _forwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = SortedNearestNeighbours.ForwardDirected(this.Weights, weight, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<float, SortedNearestNeighbours[]> _backwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public SortedNearestNeighbours GetNearestNeighboursBackward(float weight, int id)
        {
            if (_backwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _backwardSortedNearestNeighbours = new Dictionary<float, SortedNearestNeighbours[]>();
            }
            SortedNearestNeighbours[] nearestNeighbours = null;
            if (!_backwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new SortedNearestNeighbours[this.Weights.Length];
                _backwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = SortedNearestNeighbours.BackwardDirected(this.Weights, weight, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }
    }
}