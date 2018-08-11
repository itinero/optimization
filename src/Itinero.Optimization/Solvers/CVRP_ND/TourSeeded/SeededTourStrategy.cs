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
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.CVRP_ND.TourSeeded
{
    /// <summary>
    /// A strategy using seeded tours as the basis to generate a new candidate.
    /// </summary>
    public class SeededTourStrategy : Strategy<CVRPNDProblem, CVRPNDCandidate>
    {
        public override string Name { get; } = "SEED_TOUR";
        
        public override CVRPNDCandidate Search(CVRPNDProblem problem)
        {
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Problem = problem,
                Fitness = 0
            };
            var pool = problem.SeededTourPool;
            
            var visits = new HashSet<int>(problem.Visits);
            
            // randomly select a tour.
            var selectedTours = new HashSet<int>();
            var firstTour = Strategies.Random.RandomGenerator.Default.Generate(pool.SeededTours.Count);
            selectedTours.Add(firstTour);
            CopyTour(candidate, pool.SeededTours[firstTour].Tour, visits);
            
            // select tours with lowest overlaps until done.
            while (visits.Count > 0)
            {
                (int tour, int overlap) best = (-1, int.MaxValue);
                for (var t = 0; t < pool.SeededTours.Count; t++)
                {
                    if (selectedTours.Contains(t)) continue;

                    var overlap = 0;
                    foreach (var selectedTour in selectedTours)
                    {
                        overlap += pool.Overlaps[t][selectedTour];
                    }

                    if (overlap < best.overlap)
                    {
                        best = (t, overlap);
                    }
                }

                if (best.tour != -1)
                {
                    CopyTour(candidate, pool.SeededTours[best.tour].Tour, visits);
                    selectedTours.Add(best.tour);
                }
            }

            return candidate;
        }

        private static void CopyTour(CVRPNDCandidate target, Tour tour, HashSet<int> visits)
        {
            // copy over tour.
            var bestTour = tour;
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
        }
    }
}