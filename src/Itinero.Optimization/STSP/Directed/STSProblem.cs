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
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Optimization.STSP.Directed.Solvers.Operators;
using System.Collections.Generic;
using Itinero.Optimization.STSP.Directed.Solver.Operators;

namespace Itinero.Optimization.STSP.Directed
{
    /// <summary>
    /// The default STSP profile definition.
    /// </summary>
    public sealed class STSProblem
    {
        /// <summary>ra
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private STSProblem()
        {

        }

        /// <summary>
        /// Creates a new TSP 'open' TSP with only a start customer.
        /// </summary>
        public STSProblem(int first, float[][] weights, float turnPenalty, float max)
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
        public STSProblem(int first, int last, float[][] weights, float turnPenalty, float max)
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
        /// Gets or sets the maximum weight.
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            var solver = new IterativeSolver<float, STSProblem, STSPObjective, Tour, STSPFitness>(
                new STSP.Directed.Solver.RandomSolver(), 100, new IterativeOperator<float, STSProblem, STSPObjective, Tour, STSPFitness>(
                    new CheapestInsertionOperator(3, 4), 250), new DirectionLocalSearch());
            return this.Solve(solver);
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Tour Solve(Algorithms.Solvers.ISolver<float, STSProblem, STSPObjective, Tour, STSPFitness> solver)
        {
            return solver.Solve(this, new STSPObjective());
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
