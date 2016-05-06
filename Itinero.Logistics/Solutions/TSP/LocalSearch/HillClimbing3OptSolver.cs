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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSP.Random;
using Itinero.Logistics.Solvers;
using System;

namespace Itinero.Logistics.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// A 3-opt solver.
    /// </summary>
    public class HillClimbing3OptSolver : SolverBase<ITSP, ITSPObjective, IRoute>, IOperator<ITSP, ITSPObjective, IRoute>
    {
        private readonly ISolver<ITSP, ITSPObjective, IRoute> _generator;
        private readonly bool _nearestNeighbours = false;
        private readonly bool _dontLook = false;
        private readonly double _epsilon = 0.001f;
        private bool[] _dontLookBits;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public HillClimbing3OptSolver()
            : this(new RandomSolver(), true, true)
        {

        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public HillClimbing3OptSolver(ISolver<ITSP, ITSPObjective, IRoute> generator, bool nearestNeighbours, bool dontLook)
        {
            _generator = generator;
            _dontLook = dontLook;
            _nearestNeighbours = nearestNeighbours;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                if (_nearestNeighbours && _dontLook)
                {
                    return "3OHC_(NN)_(DL)";
                }
                else if (_nearestNeighbours)
                {
                    return "3OHC_(NN)";
                }
                else if (_dontLook)
                {
                    return "3OHC_(DL)";
                }
                return "3OHC";
            }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(ITSPObjective objective)
        {
            return objective.Name == MinimumWeightObjective.MinimumWeightObjectiveName;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSP problem, ITSPObjective objective, out double fitness)
        {
            if (!problem.Last.HasValue) { throw new ArgumentException("OPT3 cannot be used on open TSP-problems."); }

            // generate some route first.
            var route = _generator.Solve(problem, objective);

            // calculate fitness.
            fitness = objective.Calculate(problem, route);

            // improve the existing solution.
            double difference;
            if(this.Apply(problem, objective, route, out difference))
            { // improvement!
                fitness = fitness + difference;
            }

            return route;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSP problem, ITSPObjective objective, IRoute solution, out double delta)
        {
            if (!problem.Last.HasValue) { throw new ArgumentException("3OPT operator cannot be used on open TSP-problems."); }
            if (solution.First != solution.Last) { throw new ArgumentException("3OPT operator cannot be used on open TSP-problems."); }
            if ((solution.First == solution.Last) != problem.Last.HasValue) { throw new ArgumentException("Route and problem have to be both closed."); }

            _dontLookBits = new bool[problem.Weights.Length];
            var weights = problem.Weights;

            // loop over all customers.
            bool anyImprovement = false;
            bool improvement = true;
            delta = 0;
            while (improvement)
            {
                improvement = false;
                var moveDelta = 0.0;
                foreach (int v1 in solution)
                {
                    if (!this.Check(problem, v1))
                    {
                        if (this.Try3OptMoves(problem, weights, solution, v1, out moveDelta))
                        {
                            anyImprovement = true;
                            improvement = true;
                            delta = delta + moveDelta;
                            break;
                        }
                        this.Set(problem, v1, true);
                    }
                }
            }
            return anyImprovement;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v1.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(ITSP problem, float[][] weights, IRoute route, int v1, out double delta)
        {
            // get v_2.
            var v2 = route.GetNeigbour(v1);
            if (v2 < 0)
            {
                delta = 0;
                return false;
            }

            var betweenV2V1 = route.Between(v2, v1);
            var weightV1V2 = weights[v1][v2];
            if (v2 == problem.First && !problem.Last.HasValue)
            { // set to zero if not closed.
                weightV1V2 = 0;
            }
            int v3 = -1;
            INearestNeighbours neighbours = null;
            if (_nearestNeighbours)
            {
                neighbours = problem.GetNNearestNeighboursForward(10, v1);
            }

            foreach (int v4 in betweenV2V1)
            {
                if (v3 >= 0 && v3 != v1)
                {
                    if (!_nearestNeighbours ||
                        neighbours.Contains(v4))
                    {
                        var weightV1V4 = weights[v1][v4];
                        var weightV3V4 = weights[v3][v4];
                        if (v4 == problem.First && !problem.Last.HasValue)
                        { // set to zero if not closed.
                            weightV1V4 = 0;
                            weightV3V4 = 0;
                        }
                        var weightV1V2PlusV3V4 = weightV1V2 + weightV3V4;
                        var weightsV3 = weights[v3];
                        if (this.Try3OptMoves(problem, weights, route, v1, v2, v3, weightsV3, v4, weightV1V2PlusV3V4, weightV1V4, out delta))
                        {
                            return true;
                        }
                    }
                }
                v3 = v4;
            }
            delta = 0;
            return false;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(ITSP problem, float[][] weights, IRoute route,
            int v1, int v2, double weightV1V2,
            int v3, out double delta)
        {
            // get v_4.
            var v4 = route.GetNeigbour(v3);
            var weightV1V2PlusV3V4 = weightV1V2 + weights[v3][v4];
            var weightV1V4 = weights[v1][v4];
            if (v4 == problem.First && !problem.Last.HasValue)
            { // set to zero if not closed.
                weightV1V4 = 0;
            }
            var weightsV3 = weights[v3];
            return this.Try3OptMoves(problem, weights, route, v1, v2, v3, weightsV3, v4, weightV1V2PlusV3V4, weightV1V4, out delta);
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(ITSP problem, float[][] weights, IRoute route,
            int v1, int v2, int v3, float[] weightsV3, int v4, double weightV1V2PlusV3V4, double weightV1V4, out double delta)
        {
            var betweenV4V1Enumerator = route.Between(v4, v1).GetEnumerator();
            if (betweenV4V1Enumerator.MoveNext())
            {
                var v5 = betweenV4V1Enumerator.Current;
                if (v5 != v1)
                {
                    while (betweenV4V1Enumerator.MoveNext())
                    {
                        var v6 = betweenV4V1Enumerator.Current;
                        var weightV3V6 = weightsV3[v6];
                        var weightV5V2 = weights[v5][v2];
                        var weightV5V6 = weights[v5][v6];
                        if (v6 == problem.First && !problem.Last.HasValue)
                        { // set to zero if not closed.
                            weightV3V6 = 0;
                            weightV5V6 = 0;
                        }
                        if (v2 == problem.First && !problem.Last.HasValue)
                        { // set to zero if not closed.
                            weightV5V2 = 0;
                        }

                        // calculate the total weight of the 'new' arcs.
                        var weightNew = weightV1V4 + weightV3V6 + weightV5V2;

                        // calculate the total weights.
                        var weight = weightV1V2PlusV3V4 + weightV5V6;

                        if (weight - weightNew > _epsilon)
                        { // actually do replace the vertices.
                            var countBefore = route.Count;
                            delta = weightNew - weight;

                            route.ReplaceEdgeFrom(v1, v4);
                            route.ReplaceEdgeFrom(v3, v6);
                            route.ReplaceEdgeFrom(v5, v2);

                            int count_after = route.Count;

                            // set bits.
                            this.Set(problem, v3, false);
                            this.Set(problem, v5, false);

                            return true; // move succeeded.
                        }
                        v5 = v6;
                    }
                }
            }
            delta = 0;
            return false;
        }

        #region Don't Look

        private bool Check(ITSP problem, int customer)
        {
            if (_dontLook)
            {
                return _dontLookBits[customer];
            }
            return false;
        }

        private void Set(ITSP problem, int customer, bool value)
        {
            if (_dontLook)
            {
                _dontLookBits[customer] = value;
            }
        }

        #endregion
    }
}