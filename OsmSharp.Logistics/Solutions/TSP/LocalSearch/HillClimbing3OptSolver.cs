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

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSP.Random;
using OsmSharp.Logistics.Solvers;
using System;

namespace OsmSharp.Logistics.Solutions.TSP.LocalSearch
{
    /// <summary>
    /// A 3-opt solver.
    /// </summary>
    public class HillClimbing3OptSolver : SolverBase<ITSP, IRoute>, IOperator<ITSP, IRoute>
    {
        private readonly ISolver<ITSP, IRoute> _generator;
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
        public HillClimbing3OptSolver(ISolver<ITSP, IRoute> generator, bool nearestNeighbours, bool dontLook)
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
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSP problem, out double fitness)
        {
            // generate some route first.
            var route = _generator.Solve(problem);

            // calculate fitness.
            fitness = 0.0;
            foreach(var pair in route.Pairs())
            {
                fitness = fitness + problem.Weights[pair.From][pair.To];
            }

            // improve the existing solution.
            double difference;
            if(this.Apply(problem, route, out difference))
            { // improvement!
                fitness = fitness + difference;
            }

            return route;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(ITSP problem, IRoute solution, out double delta)
        {
            _dontLookBits = new bool[problem.Weights.Length];
            var weights = problem.Weights;

            // loop over all customers.
            bool anyImprovement = false;
            bool improvement = true;
            delta = -1;
            while (improvement)
            {
                improvement = false;
                foreach (int v1 in solution)
                {
                    if (!this.Check(problem, v1))
                    {
                        if (this.Try3OptMoves(problem, weights, solution, v1))
                        {
                            anyImprovement = true;
                            improvement = true;
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
        public bool Try3OptMoves(ITSP problem, double[][] weights, IRoute route, int v1)
        {
            // get v_2.
            var v2 = route.GetNeigbours(v1)[0];
            if (v2 < 0)
            {
                return false;
            }

            var betweenV2V1 = route.Between(v2, v1);
            var weightV1V2 = weights[v1][v2];
            int v3 = -1;
            INNearestNeighbours neighbours = null;
            if (_nearestNeighbours)
            {
                neighbours = problem.GetNNearestNeighbours(10, v1);
            }

            foreach (int v4 in betweenV2V1)
            {
                if (v3 >= 0 && v3 != v1)
                {
                    if (!_nearestNeighbours ||
                        neighbours.Contains(v4))
                    {
                        var weightV1V4 = weights[v1][v4];
                        var weightV1V2PlusV3V4 = weightV1V2 + weights[v3][v4];
                        var weights_3 = weights[v3];
                        if (this.Try3OptMoves(problem, weights, route, v1, v2, v3, weights_3, v4, weightV1V2PlusV3V4, weightV1V4))
                        {
                            return true;
                        }
                    }
                }
                v3 = v4;
            }
            return false;
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(ITSP problem, double[][] weights, IRoute route,
            int v1, int v2, double weightV1V2,
            int v3)
        {
            // get v_4.
            var v4 = route.GetNeigbours(v3)[0];
            var weightV1V2PlusV3V4 = weightV1V2 + weights[v3][v4];
            var weightV1V4 = weights[v1][v4];
            var weightsV3 = weights[v3];
            return this.Try3OptMoves(problem, weights, route, v1, v2, v3, weightsV3, v4, weightV1V2PlusV3V4, weightV1V4);
        }

        /// <summary>
        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
        /// </summary>
        /// <returns></returns>
        public bool Try3OptMoves(ITSP problem, double[][] weights, IRoute route,
            int v1, int v2, int v3, double[] weightsV3, int v4, double weightV1V2PlusV3V4, double weightV1V4)
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

                        // calculate the total weight of the 'new' arcs.
                        var weight_new = weightV1V4 +
                            weightsV3[v6] +
                            weights[v5][v2];

                        // calculate the total weights.
                        var weight = weightV1V2PlusV3V4 + weights[v5][v6];

                        if (weight - weight_new > _epsilon)
                        { // actually do replace the vertices.
                            int count_before = route.Count;

                            route.ReplaceEdgeFrom(v1, v4);
                            route.ReplaceEdgeFrom(v3, v6);
                            route.ReplaceEdgeFrom(v5, v2);

                            int count_after = route.Count;

                            if (count_before != count_after)
                            {
                                throw new Exception();
                            }
                            // set bits.
                            this.Set(problem, v3, false);
                            this.Set(problem, v5, false);

                            return true; // move succeeded.
                        }
                        v5 = v6;
                    }
                }
            }
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