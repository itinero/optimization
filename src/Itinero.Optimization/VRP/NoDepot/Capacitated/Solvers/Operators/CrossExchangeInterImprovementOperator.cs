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

using System;
using System.Collections.Generic;
using System.Text;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers.Operators 
{
    /// <summary>
    /// An improvement operator that tries to exchange parts of routes.
    /// </summary>
    /// <remarks>
    /// This follows a 'stop on first'-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 random tours.
    /// - Loop over all edge-ranges in tour1.
    ///   - Loop over all edge-ranges in tour2.
    ///     - Check if a swap between tours improves things.
    /// 
    /// The search stops from the moment any improvement is found.
    /// </remarks>
    public class CrossExchangeInterImprovementOperator : IOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> 
    {
        private const float E = 0.001f;

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name => "CROSS";

        /// <summary>
        /// Returns true if this operator supports the given objective.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports (NoDepotCVRPObjective objective) 
        {
            return true;
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply (NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, out float delta) 
        {
            int maxWindowSize = 50;
            var max = problem.Max;

            // choose two random routes.
            var random = RandomGeneratorExtensions.GetRandom ();
            var tourIdx1 = random.Generate (solution.Count);
            var tourIdx2 = random.Generate (solution.Count - 1);
            if (tourIdx2 >= tourIdx1) {
                tourIdx2++;
            }
            var tour1 = solution.Tour (tourIdx1);
            var tour2 = solution.Tour (tourIdx2);

            var tour1Weight = objective.Calculate(problem, solution, tourIdx1);
            var tour2Weight = objective.Calculate(problem, solution, tourIdx2);
            var totalBefore =  tour1Weight + tour2Weight;

            var route1Cumul = objective.CalculateCumul (problem, solution, tourIdx1);
            var route2Cumul = objective.CalculateCumul (problem, solution, tourIdx2);

            // build all edge weights.
            var route1Edges = new List<Pair>(tour1.Pairs());
            var route2Edges = new List<Pair>(tour2.Pairs());
            var route1Weights = new float[route1Edges.Count];
            for (int idx = 0; idx < route1Edges.Count; idx++) {
                var edge = route1Edges[idx];
                route1Weights[idx] = problem.Weights[edge.From][edge.To];
            }
            var route2Weights = new float[route2Edges.Count];
            for (int idx = 0; idx < route2Edges.Count; idx++) {
                var edge = route2Edges[idx];
                route2Weights[idx] = problem.Weights[edge.From][edge.To];
            }

            var route2Pairs = new List<EdgePair> ();
            for (int iIdx = 0; iIdx < route2Edges.Count - 2; iIdx++) {
                var i = route2Edges[iIdx];
                var iWeight = route2Weights[iIdx];
                var weightBeforeI = route2Cumul[iIdx];

                int kIdxMax = route2Edges.Count;
                if (kIdxMax > iIdx + 2 + maxWindowSize) {
                    kIdxMax = iIdx + 2 + maxWindowSize;
                }
                for (var kIdx = iIdx + 2; kIdx < kIdxMax; kIdx++) {
                    var k = route2Edges[kIdx];
                    var kWeight = route2Weights[kIdx];
                    var weightAfterK = route2Cumul[route2Cumul.Length - 1] - route2Cumul[kIdx + 1];
                    var weighBetweenRoute = route2Cumul[kIdx] - route2Cumul[iIdx + 1];

                    // TODO: investigate if the between list is needed here.
                    route2Pairs.Add (new EdgePair () {
                        First = i,
                            FirstWeight = iWeight,
                            Second = k,
                            SecondWeight = kWeight,
                            Between = new List<int> (tour2.Between (i.To, k.From)),
                            WeightTotal = iWeight + kWeight,
                            WeightAfter = weightAfterK,
                            WeightBefore = weightBeforeI,
                            WeightBetween = weighBetweenRoute,
                            CustomersBetween = kIdx - iIdx
                    });
                }
            }

            // build all edge pairs.
            for (var iIdx = 0; iIdx < route1Edges.Count - 2; iIdx++) 
            {
                var i = route1Edges[iIdx];
                var iWeight = route1Weights[iIdx];
                var weightBeforeI = route1Cumul[iIdx];

                int kIdxMax = route1Edges.Count;
                if (kIdxMax > iIdx + 2 + maxWindowSize) 
                {
                    kIdxMax = iIdx + 2 + maxWindowSize;
                }
                for (var kIdx = iIdx + 2; kIdx < kIdxMax; kIdx++) 
                {
                    var k = route1Edges[kIdx];
                    var kWeight = route1Weights[kIdx];
                    var weightAfterK = route1Cumul[route1Cumul.Length - 1] - route1Cumul[kIdx + 1];
                    var weightBetweenRoute = route1Cumul[kIdx] - route1Cumul[iIdx + 1];

                    var pair1 = new EdgePair () {
                        First = i,
                        FirstWeight = iWeight,
                        Second = k,
                        SecondWeight = kWeight,
                        Between = new List<int> (tour1.Between (i.To, k.From)),
                        WeightTotal = iWeight + kWeight,
                        WeightAfter = weightAfterK,
                        WeightBefore = weightBeforeI,
                        WeightBetween = weightBetweenRoute,
                        CustomersBetween = kIdx - iIdx
                    };

                    foreach (var pair2 in route2Pairs) 
                    {
                        var existingWeight = pair1.WeightTotal + pair2.WeightTotal;

                        // get first route new.
                        var newWeight = problem.Weights[pair1.First.From][pair2.First.To];
                        if (newWeight > existingWeight - E) 
                        {
                            continue;
                        }

                        var firstRoute2New = problem.Weights[pair2.First.From][pair1.First.To];
                        newWeight = newWeight + firstRoute2New;
                        if (newWeight > existingWeight - E) 
                        {
                            continue;
                        }

                        var secondRoute1New = problem.Weights[pair1.Second.From][pair2.Second.To];
                        newWeight = newWeight + secondRoute1New;
                        if (newWeight > existingWeight - E) 
                        {
                            continue;
                        }

                        var secondRoute2New = problem.Weights[pair2.Second.From][pair1.Second.To];
                        newWeight = newWeight + secondRoute2New;

                        if (newWeight < existingWeight - E) 
                        { // there is a decrease in total weight; check bounds.
                            var route1Weight = pair1.WeightBefore + pair2.WeightBetween + pair1.WeightAfter;
                            var route2Weight = pair2.WeightBefore + pair1.WeightBetween + pair2.WeightAfter;

                            if (route1Weight < max && route2Weight < max) 
                            {
                                var route1Between = pair1.Between;
                                var route2Between = pair2.Between;

                                tour1.ReplaceEdgeFrom (pair1.First.From, pair1.Second.To);
                                tour2.ReplaceEdgeFrom (pair2.First.From, pair2.Second.To);

                                var previous = pair1.First.From;
                                for (var idx = 0; idx < route2Between.Count; idx++) {
                                    tour1.ReplaceEdgeFrom (previous, route2Between[idx]);
                                    previous = route2Between[idx];
                                }
                                tour1.ReplaceEdgeFrom (previous, pair1.Second.To);

                                previous = pair2.First.From;
                                for (int idx = 0; idx < route1Between.Count; idx++) {
                                    tour2.ReplaceEdgeFrom (previous, route1Between[idx]);
                                    previous = route1Between[idx];
                                }
                                tour2.ReplaceEdgeFrom (previous, pair2.Second.To);

                                var tour1WeightAfter = objective.Calculate(problem, solution, tourIdx1);
                                var tour2WeightAfter = objective.Calculate(problem, solution, tourIdx2);
                                var totalAfter = tour1WeightAfter +
                                    tour2WeightAfter;

                                delta = totalBefore - totalAfter;
                                return true;
                            }
                        }
                    }
                }
            }

            delta = 0;
            return false;
        }

        private class EdgePair {
            public Pair First { get; set; }

            public float FirstWeight { get; set; }

            public Pair Second { get; set; }

            public float SecondWeight { get; set; }

            public List<int> Between { get; set; }

            public float WeightTotal { get; set; }

            public float WeightBefore { get; set; }

            public float WeightAfter { get; set; }

            public float WeightBetween { get; set; }

            public int CustomersBetween { get; set; }
        }
    }
}