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
using System.Diagnostics;
using System.Threading;
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Shared.HillClimbing3Opt;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.CVRP_ND.Operators
{
    /// <summary>
    /// A placement operator that uses cheapest insertion to insert unplaced visit in the last tour until no more visits can be placed.
    /// </summary>
    internal class CheapestInsertionPlacementOperator : PlacementOperator<CVRPNDCandidate>
    {
        private readonly float _improvementsThreshold; // the threshold for when the apply inter-tour improvements.
        private readonly Func<int, int, float> _insertionCostHeuristic;
        private readonly bool _lastOnly = false;
        private readonly Operator<CVRPNDCandidate> _improvementOperator;
        
        /// <summary>
        /// Creates a new placement operator.
        /// </summary>
        /// <param name="improvementOperator">The operator to apply at intermediate stages of the placement.</param>
        /// <param name="insertionCostHeuristic">The insertion cost heuristic in any.</param>
        /// <param name="lastOnly">When true inserts in the last tour only.</param>
        /// <param name="improvementsThreshold">The improvements threshold parameter.</param>
        public CheapestInsertionPlacementOperator(Operator<CVRPNDCandidate> improvementOperator = null, Func<int, int, float> insertionCostHeuristic = null,
            bool lastOnly = false, float improvementsThreshold = 0.05f)
        {
            _insertionCostHeuristic = insertionCostHeuristic;
            _lastOnly = lastOnly;
            _improvementsThreshold = improvementsThreshold;
            _improvementOperator = improvementOperator ?? (new ExchangeOperator(onlyLast: _lastOnly, bestImprovement: true, minWindowSize: 0, maxWindowSize: 20)).ApplyUntil();
        }

        public override string Name => "CI_PLACE";

        public override bool Apply(CVRPNDCandidate candidate)
        {
            var visits = candidate.GetUnplacedVisits();
            return this.Apply(candidate, visits);
        }

        public override bool Apply(CVRPNDCandidate candidate, ICollection<int> visits)
        {
            var relativeThreshold = (int) (_improvementsThreshold * candidate.Problem.Count);
            var insertedCount = 0;
            if (_lastOnly)
            {
                // try to place in the last tour.
                var t = candidate.Count - 1;
                var tour = candidate.Tour(t);
                Func<int, float> insertionCostHeuristic = null;
                if (_insertionCostHeuristic != null)
                {
                    insertionCostHeuristic = (v) => _insertionCostHeuristic(tour.First, v);
                }

                var cheapest = tour.CalculateCheapest(candidate.Problem.TravelWeight, visits,
                    insertionCostHeuristic, (travelCost, v) => candidate.CanInsert(t, v, travelCost));
                while (cheapest.cost < float.MaxValue)
                {
                    candidate.InsertAfter(t, cheapest.location.From, cheapest.visit);
                    visits.Remove(cheapest.visit);
                    insertedCount++;
                    if (visits.Count == 0)
                    {
                        break;
                    }
                    
                    if (insertedCount == relativeThreshold)
                    { // apply exchanges if needed.
                        var last = candidate.Solution.Tour(candidate.Solution.Count - 1);
                        // TODO: use nearest neighbours.
                        last.Do3Opt(candidate.Problem.TravelWeight, candidate.Problem.MaxVisit, candidate.Problem.NearestNeighbourCache.GetNNearestNeighbours(10));
                        
                        insertedCount = 0;
                        _improvementOperator?.Apply(candidate);
                    }

                    cheapest = tour.CalculateCheapest(candidate.Problem.TravelWeight, visits,
                        insertionCostHeuristic, (travelCost, v) => candidate.CanInsert(t, v, travelCost));
                    
                    // TODO: if placement fails, try 3OPT and exchange one last time.
                    // TODO: see if we can do 3opt and ex at seperate paces.
                }
            }
            else
            {
                (float cost, Pair location, int visit, int tour) best = (float.MaxValue, new Pair(), Tour.NOT_SET, int.MaxValue);
                while (true)
                {
                    for (var t = 0; t < candidate.Count; t++)
                    {
                        var tour = candidate.Tour(t);
                        Func<int, float> insertionCostHeuristic = null;
                        if (_insertionCostHeuristic != null)
                        {
                            insertionCostHeuristic = (v) => _insertionCostHeuristic(tour.First, v);
                        }

                        var cheapest = tour.CalculateCheapest(candidate.Problem.TravelWeight, visits,
                            insertionCostHeuristic, (travelCost, v) => candidate.CanInsert(t, v, travelCost));
                        if (cheapest.cost < best.cost)
                        {
                            best = (cheapest.cost, cheapest.location, cheapest.visit, t);
                        }
                    }

                    if (best.visit == Tour.NOT_SET)
                    { // none of the visits can be placed (anymore).
                        break;
                    }
                    
                    candidate.InsertAfter(best.tour, best.location.From, best.visit);
                    visits.Remove(best.visit);
                    if (visits.Count == 0)
                    {
                        break;
                    }

                    best = (float.MaxValue, new Pair(), Tour.NOT_SET, int.MaxValue);
                }
            }

            return false;
        }
        
        private static readonly ThreadLocal<CheapestInsertionPlacementOperator> DefaultLastOnlyLazy = new ThreadLocal<CheapestInsertionPlacementOperator>(
            () => new CheapestInsertionPlacementOperator(new ExchangeOperator(onlyLast: true, bestImprovement: false, minWindowSize: 0, maxWindowSize: 1), 
                null, true));
        public static CheapestInsertionPlacementOperator DefaultLastOnly => DefaultLastOnlyLazy.Value;
        
        private static readonly ThreadLocal<CheapestInsertionPlacementOperator> DefaultLazy = new ThreadLocal<CheapestInsertionPlacementOperator>(() => new CheapestInsertionPlacementOperator());
        public static CheapestInsertionPlacementOperator Default => DefaultLazy.Value;
    }
}