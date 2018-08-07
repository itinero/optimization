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
using System.Globalization;
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Shared.HillClimbing3Opt;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.CVRP_ND.TourSeeded
{
    /// <summary>
    /// A pool of seeded tours.
    /// </summary>
    internal class SeededTourPool
    {
        private readonly int _count;
        private readonly CVRPNDProblem _problem;

        public SeededTourPool(CVRPNDProblem problem, int count = -1)
        {
            _problem = problem;
            _count = count;
            
            if (_count == -1)
            {
                _count = problem.Count;
            }
        }

        public List<SeededTour> SeededTours { get; } = new List<SeededTour>();

        public int[][] Overlaps { get; private set; } = null;

        public void Build()
        {
            var visits = new List<int>(_problem.Visits);
            if (_count != _problem.Count)
            {
                visits.Shuffle();
            }

            while (SeededTours.Count < _count)
            {
                var visit = visits[visits.Count - 1];
                visits.RemoveAt(visits.Count - 1);
                
                SeedTour(visit);
            }

            if (_count != _problem.Count)
            { // make sure all visits are in at least one tour.
                var visitsInPool = new HashSet<int>();
                foreach (var seededTour in this.SeededTours)
                {
                    visitsInPool.UnionWith(seededTour.Visits);
                }

                // count was reached but perhaps not all visits are in the pool.
                while (visitsInPool.Count < _problem.Count)
                {
                    // select visit not in pool.
                    var visitNotInPool = -1;
                    foreach (var visit in _problem.Visits)
                    {
                        if (!visitsInPool.Contains(visit))
                        {
                            visitNotInPool = visit;
                            break;
                        }
                    }

                    SeedTour(visitNotInPool);
                    visitsInPool.UnionWith(this.SeededTours[this.SeededTours.Count - 1].Visits);
                }
            }

            // TODO: cut this in half, it's a symetric matrix.
            Overlaps = new int[SeededTours.Count][];
            for (var x = 0; x < SeededTours.Count; x++)
            {
                Overlaps[x] = new int[SeededTours.Count];
                for (var y = 0; y <= x; y++)
                {
                    if (x == y)
                    {
                        Overlaps[x][y] = SeededTours[x].Visits.Count;
                    }
                    else
                    {
                        Overlaps[x][y] = OverlapCount(SeededTours[x].Visits, SeededTours[y].Visits);
                    }
                    Overlaps[y][x] = Overlaps[x][y];
                }
            }
        }

        private static int OverlapCount(HashSet<int> hashSet1, HashSet<int> hashSet2)
        {
            var count = 0;
            foreach (var visit in hashSet1)
            {
                if (hashSet2.Contains(visit))
                {
                    count++;
                }
            }

            return count;
        }

        private void SeedTour(int visit)
        {
            var visits = new HashSet<int>(_problem.Visits);
            visits.Remove(visit);
            var tour = new Tour(new [] { visit }, visit);
            var travelWeight = 0f;

            var threshold = 10;
            var thresholdWindow = 0;
            while (true)
            {
                var result = tour.CalculateCheapest(_problem.TravelWeight, visits);
                if (result.visit == -1) break;
                
                var visitTravelWeight = _problem.VisitWeight(result.visit);
                if (result.cost + travelWeight < _problem.MaxWeight)
                {
                    thresholdWindow++;
                    tour.InsertAfter(result.location.From, result.visit);
                    travelWeight += visitTravelWeight + result.cost;
                    visits.Remove(result.visit);

                    if (thresholdWindow == threshold)
                    {
                        var result3Opt = tour.Do3Opt(_problem.TravelWeight, _problem.MaxVisit,
                            _problem.NearestNeighbourCache.GetNNearestNeighbours(10));
                        if (result3Opt.improved)
                        {
                            travelWeight += result3Opt.delta;
                        }

                        thresholdWindow = 0;
                    }
                }
                else
                {
                    var result3Opt = tour.Do3Opt(_problem.TravelWeight, _problem.MaxVisit,
                        _problem.NearestNeighbourCache.GetNNearestNeighbours(10));
                    if (!result3Opt.improved)
                    {
                        break;
                    }
                    travelWeight += result3Opt.delta;
                }
            }

            SeededTours.Add(new SeededTour()
            {
                Tour = tour,
                TourData = (travelWeight, null),
                Visits = new HashSet<int>(tour)
            });
        }
    }
}