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

using System.Diagnostics;
using System.Threading;
using Itinero.Optimization.Solvers.Shared.Sequences;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.CVRP_ND.Operators
{
    /// <summary>
    /// A tour-aware operator that exchange sub sequences between tours.
    /// </summary>
    internal class ExchangeOperator : TourAwareOperator
    {
        private readonly int _maxWindowSize;
        private readonly int _minWindowSize;
        private readonly bool _tryAll;
        private readonly bool _bestImprovement;
        private readonly bool _onlyLast;
        
        /// <summary>
        /// Creates a new operator.
        /// </summary>
        /// <param name="minWindowSize">The minimum window size to search for sequences to exchange.</param>
        /// <param name="maxWindowSize">The maximum window size to search for sequences to exchange.</param>
        /// <param name="tryAll">When true all tour pairs are tried out.</param>
        /// <param name="onlyLast">When true only exchange between the last and other tours are considered.</param>
        /// <param name="bestImprovement">When true all options are tried before choosing the best.</param>
        public ExchangeOperator(int minWindowSize = 0, int maxWindowSize = 4, bool tryAll = false, bool bestImprovement = false,
            bool onlyLast = false)
        {
            _tryAll = tryAll;
            _bestImprovement = bestImprovement;
            _minWindowSize = minWindowSize;
            _maxWindowSize = maxWindowSize;
            _onlyLast = onlyLast;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public override string Name
        {
            get
            {
                if (_tryAll && _bestImprovement)
                {
                    return $"EX-{_minWindowSize}-{_maxWindowSize}_(ALL)_(BEST)";
                }
                if (_tryAll)
                {
                    return $"EX-{_minWindowSize}-{_maxWindowSize}_(ALL)";
                }
                if (_bestImprovement)
                {
                    return $"EX-{_minWindowSize}-{_maxWindowSize}_(BEST)";
                }
                return $"EX-{_minWindowSize}-{_maxWindowSize}";
            }
        }

        /// <summary>
        /// Applies this operator to the given candidate. Tries to exchange between any two tours.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate will be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public override bool Apply(CVRPNDCandidate candidate)
        {
            if (_onlyLast)
            {
                (bool success, Seq s1, Seq s2, float fitnessDelta, int t1, int t2) best = (false, new Seq(), new Seq(),
                    float.MaxValue, -1, -1);
                
                // try all pairs and keep the best exchange around if any.
                var t1 = candidate.Solution.Count - 1;
                for (var t2 = 0; t2 < t1; t2++)
                {
                    var exchange = this.Try(candidate, t1, t2);
                    if (!exchange.success || !(exchange.fitnessDelta < best.fitnessDelta)) continue;

                    best.fitnessDelta = exchange.fitnessDelta;
                    best.t1 = t1;
                    best.t2 = t2;
                    best.s1 = exchange.s1;
                    best.s2 = exchange.s2;
                    best.success = true;

                    if (!_bestImprovement)
                    {
                        // exchange was already done on the first improvement found. 
                        return true;
                    }
                }

                if (best.success)
                {
                    // do the exchange, this is the best one.
                    candidate.TrySwap(best.t1, best.t2, best.s1, best.s2);
                    return true;
                }
                
                return false;
            }
            else if (_tryAll)
            {
                Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                
                (bool success, Seq s1, Seq s2, float fitnessDelta, int t1, int t2) best = (false, new Seq(), new Seq(),
                    float.MaxValue, -1, -1);
                
                // try all pairs and keep the best exchange around if any.
                for (var t1 = 0; t1 < candidate.Solution.Count; t1++)
                for (var t2 = 0; t2 < t1; t2++)
                {
                    var exchange = this.Try(candidate, t1, t2);
                    if (!exchange.success || !(exchange.fitnessDelta < best.fitnessDelta)) continue;

                    best.fitnessDelta = exchange.fitnessDelta;
                    best.t1 = t1;
                    best.t2 = t2;
                    best.s1 = exchange.s1;
                    best.s2 = exchange.s2;
                    best.success = true;

                    if (!_bestImprovement)
                    {
                        // exchange was already done on the first improvement found. 
                        Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                        return true;
                    }
                }

                if (best.success)
                {
                    // do the exchange, this is the best one.
                    candidate.TrySwap(best.t1, best.t2, best.s1, best.s2);
                    Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                    return true;
                }
                return false;
            }
            else 
            {
                Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                
                // choose two random tours.
                Strategies.Random.RandomGenerator.Generate2(candidate.Solution.Count, out var t1, out var t2);

                var exchange = this.Try(candidate, t1, t2);
                if (!exchange.success) return false;
                
                if (!_bestImprovement)
                {
                    // exchange was already done on the first improvement found. 
                    Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                    return true;
                }
                // do the exchange, this is the best one.
                candidate.TrySwap(t1, t2, exchange.s1, exchange.s2);
                Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                return true;
            }
        }

        /// <summary>
        /// Applies this operator to the given candidate. Tries to exchange between the given tour and all the others.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="t">The tour to focus on.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate will be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public override bool Apply(CVRPNDCandidate candidate, int t)
        {
            if (_tryAll)
            { // try all second tours.
                (bool success, Seq s1, Seq s2, float fitnessDelta, int t2) best = (false, new Seq(), new Seq(),
                    float.MaxValue, -1);

                for (var t2 = 0; t2 < candidate.Solution.Count; t2++)
                {
                    if (t == t2) continue;

                    var exchange = this.Try(candidate, t, t2);
                    if (!exchange.success || !(exchange.fitnessDelta < best.fitnessDelta)) continue;

                    best.fitnessDelta = exchange.fitnessDelta;
                    best.t2 = t2;
                    best.s1 = exchange.s1;
                    best.s2 = exchange.s2;
                    best.success = true;

                    if (!_bestImprovement)
                    {
                        // exchange was already done on the first improvement found. 
                        Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                        return true;
                    }
                }

                if (best.success)
                {
                    // do the exchange, this is the best one.
                    candidate.TrySwap(t, best.t2, best.s1, best.s2);
                    Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                    return true;
                }

                return false;
            }
            else
            { // choose a random second tour.
                if (!RandomGenerator.GenerateSecond(candidate.Solution.Count, t, out var t2)) return false;

                var exchange = this.Try(candidate, t, t2);
                if (!exchange.success) return false;
                
                if (!_bestImprovement)
                {
                    // exchange was already done on the first improvement found. 
                    return true;
                }
                // do the exchange, this is the best one.
                candidate.TrySwap(t, t2, exchange.s1, exchange.s2);
            
                Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
                
                return true;
            }
        }

        /// <summary>
        /// Applies this operator to the given candidate. Tries to exchange between the two given tours.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns>True if an improvement was found.</returns>
        /// <remarks>The candidate will be modified in-place but this should *only* happen in the case when there is an improvement or this operator is explicitly used as a mutation operator.</remarks>
        public override bool Apply(CVRPNDCandidate candidate, int t1, int t2)
        {
            var exchange = this.Try(candidate, t1, t2);
            if (!exchange.success) return false;
            
            candidate.TrySwap(t1, t2, exchange.s1, exchange.s2);
            
            Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
            
            return true;
        }

        /// <summary>
        /// Tries exchanges between the two given tours.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns>All the info about the exchange that was found.</returns>
        private (bool success, Seq s1, Seq s2, float fitnessDelta) Try(CVRPNDCandidate candidate, int t1, int t2)
        {
            (bool success, Seq s1, Seq s2, float fitnessDelta) best = (false, new Seq(), new Seq(), 0);

            var tryReversed = true; // TODO: make this candidate-dependent.
            var tour1Enumerable = candidate.SeqAndSmaller(t1, _minWindowSize + 2, _maxWindowSize + 2);
            var tour2Enumerable = candidate.SeqAndSmaller(t2, _minWindowSize + 2, _maxWindowSize + 2);

            if (_bestImprovement)
            {
                // go over all combinations and choose the best.
                // loop over all sequences of size 4->maxWindowSize + 2. 
                // - A minimum of 4 because otherwise we exchange just one visit.
                // - The edge to be exchanged are also included.
                foreach (var s1 in tour1Enumerable)
                {
                    // switch s1.
                    var s1Rev = s1.Reverse();

                    foreach (var s2 in tour2Enumerable)
                    {
                        // try exchanging without change order.
                        var simResult = candidate.SimulateSwap(t1, t2, s1, s2);
                        if (simResult.success &&
                            simResult.fitnessDelta < best.fitnessDelta)
                        {
                            // exchange succeeded.
                            best.success = true;
                            best.fitnessDelta = simResult.fitnessDelta;
                            best.s1 = s1;
                            best.s2 = s2;
                        }

                        if (!tryReversed)
                        {
                            // don't try the other options with sequences reversed.
                            continue;
                        }

                        // try exchanging with s1 reversed.
                        if (s1.Length > 3)
                        {
                            simResult = candidate.SimulateSwap(t1, t2, s1Rev, s2);
                            if (simResult.success &&
                                simResult.fitnessDelta < best.fitnessDelta)
                            {
                                // exchange succeeded.
                                best.success = true;
                                best.fitnessDelta = simResult.fitnessDelta;
                                best.s1 = s1Rev;
                                best.s2 = s2;
                            }
                        }

                        // reverse s2.
                        if (s2.Length <= 3)
                        {
                            // not need to check reverse for single-visit sequences.
                            continue;
                        }

                        var s2Rev = s2.Reverse();

                        // try exchanging with s2 reversed.
                        simResult = candidate.SimulateSwap(t1, t2, s1, s2Rev);
                        if (simResult.success &&
                            simResult.fitnessDelta < best.fitnessDelta)
                        {
                            best.success = true;
                            best.fitnessDelta = simResult.fitnessDelta;
                            best.s1 = s1;
                            best.s2 = s2Rev;
                        }

                        // try exchanging with both reversed.
                        if (s1.Length > 3)
                        {
                            simResult = candidate.SimulateSwap(t1, t2, s1Rev, s2Rev);
                            if (simResult.success &&
                                simResult.fitnessDelta < best.fitnessDelta)
                            {
                                best.success = true;
                                best.fitnessDelta = simResult.fitnessDelta;
                                best.s1 = s1Rev;
                                best.s2 = s2Rev;
                            }
                        }
                    }
                }
            }
            else
            {
                // choose the first successful exchange.
                // loop over all sequences of size 4->maxWindowSize + 2. 
                // - A minimum of 4 because otherwise we exchange just one visit.
                // - The edge to be exchanged are also included.
                foreach (var s1 in tour1Enumerable)
                {
                    // switch s1.
                    var s1Rev = s1.Reverse();

                    foreach (var s2 in tour2Enumerable)
                    {
                        var trySwapResult = candidate.TrySwap(t1, t2, s1, s2);
                        if (trySwapResult.success)
                        {
                            best.fitnessDelta = trySwapResult.fitnessDelta;
                            best.s1 = s1;
                            best.s2 = s2;
                            best.success = true;
                            return best;
                        }

                        if (!tryReversed)
                        {
                            // don't try the other options with sequences reversed.
                            continue;
                        }

                        // try exchanging with s1 reversed.
                        if (s1.Length > 3)
                        {
                            trySwapResult = candidate.TrySwap(t1, t2, s1Rev, s2);
                            if (trySwapResult.success)
                            {
                                best.fitnessDelta = trySwapResult.fitnessDelta;
                                best.s1 = s1Rev;
                                best.s2 = s2;
                                best.success = true;
                                return best;
                            }
                        }

                        // reverse s2.
                        if (s2.Length <= 3)
                        {
                            // not need to check reverse for single-visit sequences.
                            continue;
                        }

                        var s2Rev = s2.Reverse();

                        // try exchanging with s2 reversed.
                        trySwapResult = candidate.TrySwap(t1, t2, s1, s2Rev);
                        if (trySwapResult.success)
                        {
                            best.fitnessDelta = trySwapResult.fitnessDelta;
                            best.s1 = s1;
                            best.s2 = s2Rev;
                            best.success = true;
                            return best;
                        }
                        // try exchanging with both reversed.
                        if (s1.Length > 3)
                        {
                            trySwapResult = candidate.TrySwap(t1, t2, s1Rev, s2Rev);
                            if (trySwapResult.success)
                            {
                                best.fitnessDelta = trySwapResult.fitnessDelta;
                                best.s1 = s1Rev;
                                best.s2 = s2Rev;
                                best.success = true;
                                return best;
                            }
                        }
                    }
                }
            }

            return best;
        }
        
        private static readonly ThreadLocal<ExchangeOperator> DefaultLazy = new ThreadLocal<ExchangeOperator>(() => new ExchangeOperator());
        public static ExchangeOperator Default => DefaultLazy.Value;
    }
}