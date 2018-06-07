///*
// *  Licensed to SharpSoftware under one or more contributor
// *  license agreements. See the NOTICE file distributed with this work for 
// *  additional information regarding copyright ownership.
// * 
// *  SharpSoftware licenses this file to you under the Apache License, 
// *  Version 2.0 (the "License"); you may not use this file except in 
// *  compliance with the License. You may obtain a copy of the License at
// * 
// *       http://www.apache.org/licenses/LICENSE-2.0
// * 
// *  Unless required by applicable law or agreed to in writing, software
// *  distributed under the License is distributed on an "AS IS" BASIS,
// *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// *  See the License for the specific language governing permissions and
// *  limitations under the License.
// */
//
//using System;
//using Itinero.Optimization.Solvers.Tours;
//
//namespace Itinero.Optimization.Solvers.TSP.HillClimbing3Opt
//{
//    /// <summary>
//    /// Contains the core of the 3-OPT hill climbing algorithm. 
//    /// </summary>
//    internal static class HillClimbing3Opt
//    {
//         /// <summary>
//        /// Returns true if there was an improvement, false otherwise.
//        /// </summary>
//        /// <returns></returns>
//        internal static bool Apply(string name, TSProblem problem, Candidate<TSProblem, Tour> candidate)
//         {
//             var solution = candidate.Solution;
//            if (!problem.Last.HasValue)
//            {
//                Itinero.Logging.Logger.Log(name, Logging.TraceEventType.Warning,
//                    "Cannot apply this operator to an open problem, skipping.");
//                return false;
//            }
//            if (solution.First != solution.Last)
//            {
//                Itinero.Logging.Logger.Log(name, Logging.TraceEventType.Warning,
//                    "Cannot apply this operator to an open fixed end problem, skipping.");
//                return false;
//            }
//            if ((solution.First == solution.Last) != problem.Last.HasValue) { throw new ArgumentException("Route and problem have to be both closed."); }
//            
//            _dontLookBits = new bool[problem.Count];
//
//            // loop over all customers.
//            var anyImprovement = false;
//            var improvement = true;
//            var delta = 0f;
//            while (improvement)
//            {
//                improvement = false;
//                var moveDelta = 0.0f;
//                foreach (int v1 in solution)
//                {
//                    if (!this.Check(problem, v1))
//                    {
//                        if (this.Try3OptMoves(problem, solution, v1, out moveDelta))
//                        {
//                            anyImprovement = true;
//                            improvement = true;
//                            delta = delta + moveDelta;
//                            break;
//                        }
//                        this.Set(problem, v1, true);
//                    }
//                }
//            }
//            return anyImprovement;
//        }
//
//        /// <summary>
//        /// Tries all 3Opt Moves for the neighbourhood of v1.
//        /// </summary>
//        /// <returns></returns>
//        private static bool Try3OptMoves(TSProblem problem, Tour tour, int v1, out float delta)
//        {
//            // get v_2.
//            var v2 = tour.GetNeigbour(v1);
//            if (v2 < 0)
//            {
//                delta = 0;
//                return false;
//            }
//
//            var betweenV2V1 = tour.Between(v2, v1);
//            var weightV1V2 = problem.Weight(v1, v2); // weights[v1][v2];
//            if (v2 == problem.First && !problem.Last.HasValue)
//            { // set to zero if not closed.
//                weightV1V2 = 0;
//            }
//            var v3 = -1;
//            NearestNeighbours neighbours = null;
//            if (_nearestNeighbours)
//            {
//                neighbours = problem.NearestNeighbourCache.GetNNearestNeighboursForward(10, v1);
//            }
//
//            foreach (int v4 in betweenV2V1)
//            {
//                if (v3 >= 0 && v3 != v1)
//                {
//                    if (!_nearestNeighbours ||
//                        neighbours.Contains(v4))
//                    {
//                        var weightV1V4 = problem.Weight(v1, v4); // weights[v1][v4];
//                        var weightV3V4 = problem.Weight(v3, v4); // weights[v3][v4];
//                        if (v4 == problem.First && !problem.Last.HasValue)
//                        { // set to zero if not closed.
//                            weightV1V4 = 0;
//                            weightV3V4 = 0;
//                        }
//                        var weightV1V2PlusV3V4 = weightV1V2 + weightV3V4;
//                        // var weightsV3 = weights[v3];
//                        if (this.Try3OptMoves(problem, tour, v1, v2, v3, v4, weightV1V2PlusV3V4, weightV1V4, out delta))
//                        // if (this.Try3OptMoves(problem, weights, tour, v1, v2, v3, weightsV3, v4, weightV1V2PlusV3V4, weightV1V4, out delta))
//                        {
//                            return true;
//                        }
//                    }
//                }
//                v3 = v4;
//            }
//            delta = 0;
//            return false;
//        }
//
//        /// <summary>
//        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
//        /// </summary>
//        /// <returns></returns>
//        private static  Try3OptMoves(ITSProblem problem, ITour tour,
//            int v1, int v2, float weightV1V2,
//            int v3, out float delta)
//        {
//            // get v_4.
//            var v4 = tour.GetNeigbour(v3);
//            var weightV1V2PlusV3V4 = weightV1V2 + problem.Weight(v3, v4); // weights[v3][v4];
//            var weightV1V4 = problem.Weight(v1, v4); // weights[v1][v4];
//            if (v4 == problem.First && !problem.Last.HasValue)
//            { // set to zero if not closed.
//                weightV1V4 = 0;
//            }
//            //var weightsV3 = weights[v3];
//            return this.Try3OptMoves(problem, tour, v1, v2, v3, v4, weightV1V2PlusV3V4, weightV1V4, out delta);
//        }
//
//        /// <summary>
//        /// Tries all 3Opt Moves for the neighbourhood of v_1 containing v_3.
//        /// </summary>
//        /// <returns></returns>
//        public bool Try3OptMoves(ITSProblem problem, ITour tour,
//            int v1, int v2, int v3, int v4, float weightV1V2PlusV3V4, float weightV1V4, out float delta)
//        {
//            var betweenV4V1Enumerator = tour.Between(v4, v1).GetEnumerator();
//            if (betweenV4V1Enumerator.MoveNext())
//            {
//                var v5 = betweenV4V1Enumerator.Current;
//                if (v5 != v1)
//                {
//                    while (betweenV4V1Enumerator.MoveNext())
//                    {
//                        var v6 = betweenV4V1Enumerator.Current;
//                        var weightV3V6 = problem.Weight(v3, v6); // weightsV3[v6];
//                        var weightV5V2 = problem.Weight(v5, v2); // weights[v5][v2];
//                        var weightV5V6 = problem.Weight(v5, v6); // weights[v5][v6];
//                        if (v6 == problem.First && !problem.Last.HasValue)
//                        { // set to zero if not closed.
//                            weightV3V6 = 0;
//                            weightV5V6 = 0;
//                        }
//                        if (v2 == problem.First && !problem.Last.HasValue)
//                        { // set to zero if not closed.
//                            weightV5V2 = 0;
//                        }
//
//                        // calculate the total weight of the 'new' arcs.
//                        var weightNew = weightV1V4 + weightV3V6 + weightV5V2;
//
//                        // calculate the total weights.
//                        var weight = weightV1V2PlusV3V4 + weightV5V6;
//
//                        if (weight - weightNew > _epsilon)
//                        { // actually do replace the vertices.
//                            var countBefore = tour.Count;
//                            delta = weightNew - weight;
//
//                            tour.ReplaceEdgeFrom(v1, v4);
//                            tour.ReplaceEdgeFrom(v3, v6);
//                            tour.ReplaceEdgeFrom(v5, v2);
//
//                            int count_after = tour.Count;
//
//                            // set bits.
//                            this.Set(problem, v3, false);
//                            this.Set(problem, v5, false);
//
//                            return true; // move succeeded.
//                        }
//                        v5 = v6;
//                    }
//                }
//            }
//            delta = 0;
//            return false;
//        }
//
//        #region Don't Look
//
//        private bool[] _dontLookBits;
//
//        private bool Check(ITSProblem problem, int customer)
//        {
//            if (_dontLook)
//            {
//                return _dontLookBits[customer];
//            }
//            return false;
//        }
//
//        private void Set(ITSProblem problem, int customer, bool value)
//        {
//            if (_dontLook)
//            {
//                _dontLookBits[customer] = value;
//            }
//        }
//
//        #endregion
//    }
//}