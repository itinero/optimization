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

using Itinero.Optimization.Routes;

namespace Itinero.Optimization.TSP.TurningWeights
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
                this.Weights[x][first] = 0;
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
        /// Gets or sets the turn penalty.
        /// </summary>
        public float TurnPenalty { get; set; }

        /// <summary>
        /// Solves this TSP using a default solver.
        /// </summary>
        /// <returns></returns>
        public Route Solve()
        {
            return this.Solve(new Solvers.RandomSolver());
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Route Solve(Algorithms.Solvers.ISolver<float, TSProblem, TSPObjective, Route, float> solver)
        {
            return solver.Solve(this, new TSPObjective());
        }
    }
}