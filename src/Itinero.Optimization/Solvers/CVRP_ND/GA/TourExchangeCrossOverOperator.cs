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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Itinero.Optimization.Solvers.CVRP_ND.Operators;
using Itinero.Optimization.Solvers.CVRP_ND.SCI;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Solvers.CVRP_ND.GA
{
    /// <summary>
    /// A tour exchange cross over operator. Builds a new candidate based on tours from two other candidates.
    /// </summary>
    internal class TourExchangeCrossOverOperator : CrossOverOperator<CVRPNDCandidate>
    {
        private readonly Operator<CVRPNDCandidate> _postOperator;
        private readonly PlacementOperator<CVRPNDCandidate> _placeRemainingOperator;

        /// <summary>
        /// Creates a new cross over operator.
        /// </summary>
        /// <param name="postOperator">The operator to apply after cross over, if any.</param>
        /// <param name="placeRemainingOperator">The operator to place remaining visits if tours cannot be exchanged anymore.</param>
        public TourExchangeCrossOverOperator(Operator<CVRPNDCandidate> postOperator = null,
            PlacementOperator<CVRPNDCandidate> placeRemainingOperator = null)
        {
            _postOperator = postOperator ?? new ExchangeOperator(onlyLast: false, bestImprovement: false, tryAll: false, minWindowSize: 0, maxWindowSize: 1);
            _placeRemainingOperator = placeRemainingOperator ?? SeededCheapestInsertionPlacementOperator.Default;
        }
        
        public override string Name { get; } = "CROSS_EX_TOURS";
        
        public override CVRPNDCandidate Apply(CVRPNDCandidate candidate1, CVRPNDCandidate candidate2)
        {
            //Debug.Assert(candidate1.GetUnplacedVisits().Count == 0);
            //Debug.Assert(candidate2.GetUnplacedVisits().Count == 0);
            
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Problem = candidate1.Problem,
                Fitness = 0
            };
            var visits = new HashSet<int>(candidate.Problem.Visits);
            var remainingThreshold = (int) (0.03f * candidate.Problem.Count);
            
            // try to use as many tours as possible.
            var success = true;
            while (success &&
                   visits.Count > 0)
            {
                success = false;
                
                // try to select a tour from solution1 to use in the given solution.
                success |= this.SelectAndMoveTour(visits, candidate1, candidate);
                if (visits.Count < remainingThreshold)
                {
                    _placeRemainingOperator.Apply(candidate, visits);
                    break;
                }
                
                // try to select a tour from solution2 to use in the given solution.
                success |= this.SelectAndMoveTour(visits, candidate2, candidate);
                if (visits.Count < remainingThreshold)
                {
                    _placeRemainingOperator.Apply(candidate, visits);
                    break;
                }
            }

            // place remaining.
            if (visits.Count > 0)
            { 
                _placeRemainingOperator.Apply(candidate, visits);
                
                //Debug.Assert(candidate.GetUnplacedVisits().Count == 0);
            }

            // apply some post-exchange operator if defined.
            _postOperator?.Apply(candidate);
            
            //Debug.Assert(candidate1.GetUnplacedVisits().Count == 0);
            //Debug.Assert(candidate2.GetUnplacedVisits().Count == 0);

            return candidate;
        }

        private bool SelectAndMoveTour(HashSet<int> visits, CVRPNDCandidate source, CVRPNDCandidate target)
        {
            if (visits.Count == 0)
            { // no more visits, no more tours to copy over.
                return false;
            }
            
            // TODO: figure out if it makes sense to keep track of previously selected tours and exclude them.
            (int tour, int insertionCount, int overlap) selectedTour = (-1, -1, -1);
            if (target.Count == 0)
            { // target is empty, select random tour.
                selectedTour = (Strategies.Random.RandomGenerator.Default.Generate(source.Count), -1, 0);
            }
            else
            {
                // search for a tour that has the least overlapping visits.
                for (var t = 0; t < source.Count; t++)
                {
                    var tour = source.Tour(t);
                    var insertCount = 0;
                    var overlap = 0;
                    foreach (var v in tour)
                    {
                        if (visits.Contains(v))
                        {
                            insertCount++;
                        }
                        else
                        {
                            overlap++;
                        }
                    }

                    if (insertCount < tour.Count / 4)
                    {
                        continue;
                    }
                    if (insertCount > selectedTour.insertionCount)
                    {
                        selectedTour = (t, insertCount, overlap);
                    }
                }

                if (selectedTour.tour < 0)
                {
                    return false;
                }
                if (selectedTour.insertionCount == 0)
                {
                    return false;
                }
            }
            
            // copy over tour.
            var bestTour = source.Tour(selectedTour.tour);
            if (bestTour.Count < 2)
            {
                return false;
            }
            var targetT = -1;
            Tour targetTour = null;
            if (visits.Remove(bestTour.First))
            {
                targetT = target.AddNew(bestTour.First);
                targetTour = target.Tour(targetT);
            }
            var previous = bestTour.First;
            foreach (var pair in bestTour.Pairs())
            {
                if (targetT == -1)
                {
                    if (!visits.Remove(pair.From))
                    {
                        continue;
                    }
                    targetT = target.AddNew(pair.From);
                    targetTour = target.Tour(targetT);
                    previous = pair.From;
                }
                if (!visits.Remove(pair.To))
                {
                    continue;
                }
                if (targetTour == null)
                {
                    continue;
                }
                if (pair.To == targetTour.First)
                {
                    break;
                }
                target.InsertAfter(targetT, previous, pair.To);

                previous = pair.To;
            }
            return true;
        }
        
        private static readonly ThreadLocal<TourExchangeCrossOverOperator> DefaultLazy = new ThreadLocal<TourExchangeCrossOverOperator>(() => new TourExchangeCrossOverOperator());
        public static TourExchangeCrossOverOperator Default => DefaultLazy.Value;
    }
}