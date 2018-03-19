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
using System.Linq;
using System.Text;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.General;
using Itinero.Optimization.Abstract.Solvers.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers.Operators
{
    /// <summary>
    /// An improvement operator that tries to exchange parts of routes.
    /// </summary>
    /// <remarks>
    /// This follows a 'stop on first'-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 tours (random, given or all pairs once).
    /// - Loop over all edge-ranges in tour1.
    ///   - Loop over all edge-ranges in tour2.
    ///     - Check if a swap between tours improves things.
    /// 
    /// The search stops from the moment any improvement is found unless configured to keep testing all tour pairs.
    /// </remarks>
    public class CrossExchangeInterImprovementOperator : IInterTourImprovementOperator
    {
        private const float E = 0.001f;
        private readonly int _maxWindowSize = 8;
        private readonly bool _tryReversed = true;
        private readonly bool _tryAll = false;

        /// <summary>
        /// Creates a new improvement operator.
        /// </summary>
        /// <param name="maxWindowSize">The maximum window size to search for sequences to exchange.</param>
        /// <param name="tryReversed">True when exchanged sequenced also need to be reversed before testing.</param>
        /// <param name="tryAll">True when all tour pairs need to be tested.</param>
        public CrossExchangeInterImprovementOperator(int maxWindowSize = 8, bool tryReversed = true, bool tryAll = false)
        {
            _tryAll = tryAll;
            _maxWindowSize = maxWindowSize;
            _tryReversed = tryReversed;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                var name = string.Format("CROSS-MUL-{0}", _maxWindowSize);
                if (_tryAll)
                {
                    name += "_(ALL)";
                }
                if (_tryReversed)
                {
                    name += "_(REV)";
                }
                return name;
            }
        }

        /// <summary>
        /// Returns true if it doesn't matter if tour indexes are switched.
        /// </summary>
        public bool IsSymmetric => true;

        /// <summary>
        /// Returns true if this operator supports the given objective.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports(NoDepotCVRPObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, out float delta)
        {
            if (_tryAll)
            {
                delta = 0;
                //bool improved = true;
                //while (improved)
                //{
                //    improved = false;
                    for (var t1 = 0; t1 < solution.Count; t1++)
                    {
                        for (var t2 = 0; t2 < t1; t2++)
                        {
                            if (t1 == t2)
                            {
                                continue;
                            }

                            if (this.Apply(problem, objective, solution, t1, t2, out float localDelta))
                            { // success!
                                delta += localDelta;
                                //improved = true;
                            }
                        }
                    }
                //}

                return delta != 0;
            }
            else
            { // just choose random routes.
                // check if solution has at least two tours.
                if (solution.Count < 2)
                {
                    delta = 0;
                    return false;
                }

                // choose two random routes.
                var random = RandomGeneratorExtensions.GetRandom();
                var tourIdx1 = random.Generate(solution.Count);
                var tourIdx2 = random.Generate(solution.Count - 1);
                if (tourIdx2 >= tourIdx1)
                {
                    tourIdx2++;
                }

                return Apply(problem, objective, solution, tourIdx1, tourIdx2, out delta);
            }
        }

        /// <summary>
        /// Applies this inter-improvement operator.
        /// </summary>
        public bool Apply(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution,
            int tourIdx1, int tourIdx2, out float delta)
        {
            int maxWindowSize = _maxWindowSize;
            var tour1 = solution.Tour(tourIdx1);
            var tour2 = solution.Tour(tourIdx2);

            var tour1Weight = solution.Contents[tourIdx1].Weight;
            var tour2Weight = solution.Contents[tourIdx2].Weight;

            var tour1Enumerable = tour1.SeqAndSmaller(4, maxWindowSize + 2, tour1.IsClosed(), false);
            var tour2Enumerable = tour2.SeqAndSmaller(4, maxWindowSize + 2, tour2.IsClosed(), false);

            // loop over all sequences of size 4->maxWindowSize + 2. 
            // - A minimum of 4 because otherwise we exchange just one visit.
            // - The edge to be exchanged are also included.
            foreach (var s1 in tour1Enumerable)
            {
                // calculate existing weights for s1.
                var existing1 = problem.Weights[s1[0]][s1[1]] +
                    problem.Weights[s1[s1.Length - 2]][s1[s1.Length - 1]];
                var between1 = problem.Weights.SeqRange(1, s1.Length - 2, s1);
                var total1 = existing1 + between1;

                // switch s1.
                var between1Rev = 0f;
                int[] s1Rev = null;
                if (_tryReversed)
                { // only setup reversed data if needed.
                    between1Rev = problem.Weights.SeqReversed(1, s1.Length - 2, s1);
                    s1Rev = s1.Clone() as int[];
                    s1Rev.ReverseRange();
                }

                foreach (var s2 in tour2Enumerable)
                {
                    // calculate existing weights for s2.
                    var existing2 = problem.Weights[s2[0]][s2[1]] +
                        problem.Weights[s2[s2.Length - 2]][s2[s2.Length - 1]];
                    var between2 = problem.Weights.SeqRange(1, s2.Length - 2, s2);
                    var total2 = existing2 + between2;

                    // try exchanging without change order.
                    if (this.TryExchange(problem, objective, solution,
                            tourIdx1, tour1Weight, s1, total1, between1,
                            tourIdx2, tour2Weight, s2, total2, between2,
                            out delta))
                    {
                        return true;
                    }

                    if (!_tryReversed)
                    { // don't try the other options with sequences reversed.
                        continue;
                    }

                    // try reversing s1.
                    if (this.TryExchange(problem, objective, solution,
                            tourIdx1, tour1Weight, s1Rev, total1, between1Rev,
                            tourIdx2, tour2Weight, s2, total2, between2,
                            out delta))
                    {
                        return true;
                    }

                    // try reversing s2.
                    var between2Rev = problem.Weights.SeqReversed(1, s2.Length - 2, s2);
                    var s2Rev = s2; // WARNING: overwriting s2 to avoid cloning, we don't need it anymore.
                    s2Rev.ReverseRange();
                    if (this.TryExchange(problem, objective, solution,
                            tourIdx1, tour1Weight, s1, total1, between1,
                            tourIdx2, tour2Weight, s2Rev, total2, between2Rev,
                            out delta))
                    {
                        return true;
                    }

                    // try reversing both.
                    if (this.TryExchange(problem, objective, solution,
                            tourIdx1, tour1Weight, s1Rev, total1, between1Rev,
                            tourIdx2, tour2Weight, s2Rev, total2, between2Rev,
                            out delta))
                    {
                        return true;
                    }
                }
            }

            delta = 0;
            return false;
        }

        private bool TryExchange(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution,
            int tour1Idx, float tour1Weight, int[] s1, float total1, float between1New,
            int tour2Idx, float tour2Weight, int[] s2, float total2, float between2New,
            out float delta)
        {
            // calculate weights if exchange was made.
            var exchange1To2 = problem.Weights[s1[0]][s2[1]] +
                problem.Weights[s2[s2.Length - 2]][s1[s1.Length - 1]] +
                between2New;
            var exchange2To1 = problem.Weights[s2[0]][s1[1]] +
                problem.Weights[s1[s1.Length - 2]][s2[s2.Length - 1]] +
                between1New;

            var localDelta = (total1 + total2) - (exchange1To2 + exchange2To1);
            if (localDelta > E)
            { // there is a potential improvement.
                // calculate the visit weights for the exchange sequences if any.
                var visitWeights1 = problem.VisitCosts.Seq(1, s1.Length - 2, s1);
                var visitWeights2 = problem.VisitCosts.Seq(1, s2.Length - 2, s2);

                var newWeight1 = tour1Weight - visitWeights1 - total1 + exchange1To2 + visitWeights2;
                if (newWeight1 > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
                var newWeight2 = tour2Weight - visitWeights1 - total2 + exchange2To1 + visitWeights1;
                if (newWeight2 > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[tour1Idx], s1, s2))
                {
                    delta = 0;
                    return false;
                }
                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[tour2Idx], s2, s1))
                {
                    delta = 0;
                    return false;
                }

                Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Information,
                    "Improvement found {0}({1})->{2}({3})",
                        tour1Idx, s1.SeqToString(), tour2Idx, s2.SeqToString());

                // exchange is possible and there is improvement, do the improvement.
                var tour1 = solution.Tour(tour1Idx);
                var tour2 = solution.Tour(tour2Idx);

                // tour2 -> tour1
                tour1.ReplaceEdgeFrom(s1[0], s2[1]);
                for (var i = 1; i < s2.Length - 1; i++)
                {
                    tour1.ReplaceEdgeFrom(s2[i], s2[i + 1]);
                }
                tour1.ReplaceEdgeFrom(s2[s2.Length - 2], s1[s1.Length - 1]);

                // tour1 -> tour2
                tour2.ReplaceEdgeFrom(s2[0], s1[1]);
                for (var i = 1; i < s1.Length - 1; i++)
                {
                    tour2.ReplaceEdgeFrom(s1[i], s1[i + 1]);
                }
                tour2.ReplaceEdgeFrom(s1[s1.Length - 2], s2[s2.Length - 1]);

                // update content.
                problem.Capacity.UpdateExchange(solution.Contents[tour1Idx], s1, s2);
                problem.Capacity.UpdateExchange(solution.Contents[tour2Idx], s2, s1);
                solution.Contents[tour1Idx].Weight = newWeight1;
                solution.Contents[tour2Idx].Weight = newWeight2;

                // automatically removed in release mode.
                tour1.Verify(problem.Weights.Length);
                tour2.Verify(problem.Weights.Length);

                delta = localDelta;
                return true;
            }
            delta = 0;
            return false;
        }

        private struct LocalSeq
        {
            public int[] Seq { get; set; }

            public bool Forward { get; set; }
        }

    }
}