/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Solvers.TSP.Directed.Solvers.Operators;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Solvers.TSP.Directed
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
            this.TurnPenalties = new float[] {
                0,
                turnPenalty,
                turnPenalty,
                0
            };

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
            this.TurnPenalties = new float[] {
                0,
                turnPenalty,
                turnPenalty,
                0
            };

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
        /// Gets or sets the turn penalties per type of turn.
        /// 0: forward, forward.
        /// 1: forward, backward.
        /// 2: backward, forward.
        /// 3: backward, backward.
        /// </summary>
        public float[] TurnPenalties { get; set; }

        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            var solver = new IterativeSolver<float, TSProblem, TSPObjective, Tour, float>(
                new Solvers.HillClimbing3OptSolver(), 100, new CheapestInsertionOperator(2), new DirectionLocalSearch());
            return this.Solve(solver);
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Tour Solve(Algorithms.Solvers.ISolver<float, TSProblem, TSPObjective, Tour, float> solver)
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

        /// <summary>
        /// Creates an initial empty tour, add fixed first and/or last customer.
        /// </summary>
        /// <returns></returns>
        public Tour CreateEmptyTour()
        {
            var firstDirectedId = DirectedHelper.BuildDirectedId(this.First, 0);
            if (!this.Last.HasValue)
            {
                return new Tours.Tour(new int[] { firstDirectedId }, null);
            }
            else
            {
                if (this.Last == this.First)
                {
                    return new Tours.Tour(new int[] { firstDirectedId }, firstDirectedId);
                }
                var lastDirectedId = DirectedHelper.BuildDirectedId(this.Last.Value, 0);
                return new Tour(new int[] { firstDirectedId, lastDirectedId }, lastDirectedId);
            }
        }
    }
}