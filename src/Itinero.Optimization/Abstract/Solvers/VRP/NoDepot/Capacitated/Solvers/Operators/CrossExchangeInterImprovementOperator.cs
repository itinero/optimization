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

namespace Itinero.Optimization.Solutions.VRP.NoDepot.Capacitated.Solvers.Operators 
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
    public class CrossExchangeInterImprovementOperator : IInterTourImprovementOperator
    {
        private const float E = 0.001f;
        private readonly int _maxWindowSize = 10;

        /// <summary>
        /// Creates a new improvement operator.
        /// </summary>
        /// <param name="maxWindowSize">The maximum window size to search for sequences to exchange.</param>
        public CrossExchangeInterImprovementOperator(int maxWindowSize = 10)
        {
            _maxWindowSize = maxWindowSize;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name => "CROSS";

        /// <summary>
        /// Returns true if it doesn't matter if tour indexes are switched.
        /// </summary>
        public bool IsSymmetric => true;

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
            // check if solution has at least two tours.
            if (solution.Count < 2)
            {
                delta = 0;
                return false;
            }

            // choose two random routes.
            var random = RandomGeneratorExtensions.GetRandom ();
            var tourIdx1 = random.Generate (solution.Count);
            var tourIdx2 = random.Generate (solution.Count - 1);
            if (tourIdx2 >= tourIdx1) {
                tourIdx2++;
            }

            return Apply(problem, objective, solution, tourIdx1, tourIdx2, out delta);
        }

        /// <summary>
        /// Applies this inter-improvement operator.
        /// </summary>
        public bool Apply (NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, 
            int tourIdx1, int tourIdx2, out float delta) 
        {
            int maxWindowSize = _maxWindowSize;
            var tour1 = solution.Tour (tourIdx1);
            var tour2 = solution.Tour (tourIdx2);

            var tour1Weight = solution.Contents[tourIdx1].Weight; //objective.Calculate(problem, solution, tourIdx1);
            var tour2Weight = solution.Contents[tourIdx2].Weight; //objective.Calculate(problem, solution, tourIdx2);
            
            // loop over all sequences of size 4->maxWindowSize. 
            // - A minimum of 4 because otherwise we exchange just one visit.
            // - The edge to be exchanged are also included.
            foreach (var s1 in tour1.SeqAndSmaller(4, _maxWindowSize, tour1.IsClosed(), false))
            {
                var existing1 = problem.Weights[s1[0]][s1[1]] + 
                    problem.Weights[s1[s1.Length - 2]][s1[s1.Length - 1]];
                foreach (var s2 in tour2.SeqAndSmaller(4, _maxWindowSize, tour2.IsClosed(), false))
                {
                    var existing2 = problem.Weights[s2[0]][s2[1]] + 
                        problem.Weights[s2[s2.Length - 2]][s2[s2.Length - 1]];
                    var new1To2 = problem.Weights[s1[0]][s2[1]] + 
                        problem.Weights[s2[s2.Length - 2]][s1[s1.Length - 1]];
                    var new2To1 = problem.Weights[s2[0]][s1[1]] + 
                        problem.Weights[s1[s1.Length - 2]][s2[s2.Length - 1]];

                    var localDelta = (existing1 + existing2) - (new1To2 + new2To1);
                    if (localDelta > E)
                    { // there is a potential improvement.
                        var newWeight1 = tour1Weight - existing1 + new1To2;
                        if (newWeight1 > problem.Capacity.Max)
                        {
                            continue;
                        }
                        var newWeight2 = tour2Weight - existing2 + new2To1;
                        if (newWeight2 > problem.Capacity.Max)
                        {
                            continue;
                        }
                        if (!problem.Capacity.ExchangeIsPossible(solution.Contents[tourIdx1], s1, s2))
                        {
                            continue;
                        }
                        if (!problem.Capacity.ExchangeIsPossible(solution.Contents[tourIdx2], s2, s1))
                        {
                            continue;
                        }

                        // exchange is possible and there is improvement, do the improvement.

                        // tour2 -> tour1
                        tour1.ReplaceEdgeFrom(s1[0], s2[1]);
                        for (var i = 1; i < s2.Length - 1; i++) {
                            tour1.ReplaceEdgeFrom(s2[i], s2[i + 1]);
                        }
                        tour1.ReplaceEdgeFrom(s2[s2.Length - 2], s1[s1.Length - 1]);

                        // tour1 -> tour2
                        tour2.ReplaceEdgeFrom(s2[0], s1[1]);
                        for (var i = 1; i < s1.Length - 1; i++) {
                            tour2.ReplaceEdgeFrom(s1[i], s1[i + 1]);
                        }
                        tour2.ReplaceEdgeFrom(s1[s1.Length - 2], s2[s2.Length - 1]);

                        // update content.
                        problem.Capacity.UpdateExchange(solution.Contents[tourIdx1], s1, s2);
                        problem.Capacity.UpdateExchange(solution.Contents[tourIdx1], s2, s1);
                        solution.Contents[tourIdx1].Weight = newWeight1;
                        solution.Contents[tourIdx2].Weight = newWeight2;

                        // automatically removed in release mode.
                        tour1.Verify(problem.Weights.Length);
                        tour2.Verify(problem.Weights.Length);

                        delta = localDelta;
                        return true;
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