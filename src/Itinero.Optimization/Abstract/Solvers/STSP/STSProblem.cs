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

using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Solutions.STSP.Solvers;
using Itinero.Optimization.Solutions.STSP.Solvers.Operators;
using System.Collections.Generic;

namespace Itinero.Optimization.Solutions.STSP
{
    /// <summary>
    /// The default STSP profile definition.
    /// </summary>
    public sealed class STSProblem
    {
        /// <summary>
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private STSProblem()
        {

        }

        /// <summary>
        /// Creates a new STSP 'open' STSP with only a start customer.
        /// </summary>
        public STSProblem(int first, float[][] weights, float max)
        {
            this.First = first;
            this.Last = null;
            this.Weights = weights;
            this.Max = max;

            for (var x = 0; x < this.Weights.Length; x++)
            {
                this.Weights[x][first] = 0;
            }
        }

        /// <summary>
        /// Creates a new STSP, 'closed' when first equals last.
        /// </summary>
        public STSProblem(int first, int last, float[][] weights, float max)
        {
            this.First = first;
            this.Last = last;
            this.Weights = weights;
            this.Max = max;

            this.Weights[first][last] = 0;
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
        /// Gets or sets the max.
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, NearestNeighbours[]> _forwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbours GetNNearestNeighboursForward(int n, int customer)
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
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighbours.Forward(this.Weights, n, customer);
                nearestNeighbours[customer] = result;
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
        public NearestNeighbours GetNNearestNeighboursBackward(int n, int customer)
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
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighbours.Backward(this.Weights, n, customer);
                nearestNeighbours[customer] = result;
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
        public SortedNearestNeighbours GetNearestNeighboursForward(float weight, int customer)
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
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = SortedNearestNeighbours.Forward(this.Weights, weight, customer);
                nearestNeighbours[customer] = result;
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
        public SortedNearestNeighbours GetNearestNeighboursBackward(float weight, int customer)
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
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = SortedNearestNeighbours.Backward(this.Weights, weight, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Solvers this problem using the default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            return this.Solve(new IterativeSolver<float, STSProblem, STSPObjective, Tour, STSPFitness>(
                new RandomSolver(), 10, new CheapestInsertionOperator(3, false), new CheapestInsertionOperator(3, true)));
        }

        /// <summary>
        /// Solves this problem using the given solver.
        /// </summary>
        public Tour Solve(SolverBase<float, STSProblem, STSPObjective, Tour, STSPFitness> solver)
        {
            return solver.Solve(this, new STSPObjective());
        }

        /// <summary>
        /// Creates an initial empty tour, add fixed first and/or last customer.
        /// </summary>
        /// <returns></returns>
        public Tour CreateEmptyTour()
        {
            if (!this.Last.HasValue)
            {
                return new Tours.Tour(new int[] { this.First }, null);
            }
            else
            {
                if (this.Last == this.First)
                {
                    return new Tours.Tour(new int[] { this.First }, this.First);
                }
                return new Tour(new int[] { this.First, this.Last.Value }, this.Last);
            }
        }
    }
}
