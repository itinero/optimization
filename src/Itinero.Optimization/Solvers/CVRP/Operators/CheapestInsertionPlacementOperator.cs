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
using System.Threading;
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Shared.Operators;

namespace Itinero.Optimization.Solvers.CVRP.Operators
{
    /// <summary>
    /// A placement operator that uses cheapest insertion to insert unplaced visit in the last tour until no more visits can be placed.
    /// </summary>
    internal class CheapestInsertionPlacementOperator : PlacementOperator<CVRPCandidate>
    {
        private readonly Func<int, int, float> _insertionCostHeuristic;
        
        public CheapestInsertionPlacementOperator(Func<int, int, float> insertionCostHeuristic = null)
        {
            _insertionCostHeuristic = insertionCostHeuristic;
        }

        public override string Name => "CI_PLACE";

        public override bool Apply(CVRPCandidate candidate)
        {
            var visits = candidate.GetUnplacedVisits();
            return this.Apply(candidate, visits);
        }

        public override bool Apply(CVRPCandidate candidate, ICollection<int> visits)
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

            return false;
        }
        
        private static readonly ThreadLocal<CheapestInsertionPlacementOperator> DefaultLazy = new ThreadLocal<CheapestInsertionPlacementOperator>(() => new CheapestInsertionPlacementOperator());
        public static CheapestInsertionPlacementOperator Default => DefaultLazy.Value;
    }
}