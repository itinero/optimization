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
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.CVRP_ND.Operators
{
    /// <summary>
    /// A placement operator that uses cheapest insertion to insert unplaced visit in the last tour until no more visits can be placed.
    /// </summary>
    internal class CheapestInsertionPlacementOperator : PlacementOperator<CVRPNDCandidate>
    {
        private readonly Func<int, int, float> _insertionCostHeuristic;
        private readonly bool _lastOnly = false;
        
        public CheapestInsertionPlacementOperator(Func<int, int, float> insertionCostHeuristic = null,
            bool lastOnly = false)
        {
            _insertionCostHeuristic = insertionCostHeuristic;
            _lastOnly = lastOnly;
        }

        public override string Name => "CI_PLACE";

        public override bool Apply(CVRPNDCandidate candidate)
        {
            var visits = candidate.GetUnplacedVisits();
            return this.Apply(candidate, visits);
        }

        public override bool Apply(CVRPNDCandidate candidate, ICollection<int> visits)
        {
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
                    if (visits.Count == 0)
                    {
                        break;
                    }

                    cheapest = tour.CalculateCheapest(candidate.Problem.TravelWeight, visits,
                        insertionCostHeuristic, (travelCost, v) => candidate.CanInsert(t, v, travelCost));
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
                }
            }

            return false;
        }
        
        private static readonly ThreadLocal<CheapestInsertionPlacementOperator> DefaultLastOnlyLazy = new ThreadLocal<CheapestInsertionPlacementOperator>(() => new CheapestInsertionPlacementOperator(null, true));
        public static CheapestInsertionPlacementOperator DefaultLastOnly => DefaultLastOnlyLazy.Value;
        
        private static readonly ThreadLocal<CheapestInsertionPlacementOperator> DefaultLazy = new ThreadLocal<CheapestInsertionPlacementOperator>(() => new CheapestInsertionPlacementOperator());
        public static CheapestInsertionPlacementOperator Default => DefaultLazy.Value;
    }
}