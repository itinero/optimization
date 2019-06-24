using System;
using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Shared.Seeds;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.CVRP.Construction
{
    /// <summary>
    /// A strategy that uses K-means clustering to generate initial potential solutions.
    /// </summary>
    internal class KMeansConstructionStrategy : Strategy<CVRProblem, CVRPCandidate>
    {
        /// <inheritdoc/>
        public override string Name { get; } = "SEED_CON_HEUR";

        /// <inheritdoc/>
        public override CVRPCandidate Search(CVRProblem problem)
        {
            var visits = new List<int>(problem.Visits);
            visits.Remove(problem.Departure);
            if (problem.Arrival.HasValue) visits.Remove(problem.Departure);
            
            var clusters = new (int visit, int seed)[visits.Count];
            CVRPCandidate best = null;
            
            // make an optimistic estimate of max tours and tour count.
            // we increase this if fitting the visits fails.

            var initialToursMax = System.Math.Max(3, visits.Count / 25);
            var initialToursMin = System.Math.Max(1 ,visits.Count / 100);
            var initialTourCount = initialToursMax / 2;
            
            // make a better guess when there is a max stop constraint.
            // TODO: include this max stops constraint name in a constant.
            if (problem.CapacityConstraints != null)
            {
                foreach (var constraint in problem.CapacityConstraints)
                {
                    if (constraint.metric != "count") continue;
                    initialToursMax = System.Math.Max(3,
                        (int)System.Math.Ceiling(3.4f * System.Math.Ceiling(visits.Count / constraint.max)));
                    initialToursMin =  System.Math.Max(1 ,(int) System.Math.Ceiling(visits.Count / constraint.max));
                    initialTourCount = (initialToursMax + initialToursMin) / 2;
                }
            }

            var toursMax = initialToursMax;
            var toursMin = initialToursMin;
            var tours = initialTourCount;
            while (best == null)
            {
                while (true)
                {
                    // calculate clusters.
                    SeedHeuristics.GetSeedsKMeans(visits, tours, problem.VisitLocation, 
                        ref clusters, maxIterations: 2);

                    // build tours out of the clusters.
                    var candidate = new CVRPCandidate()
                    {
                        Solution = new CVRPSolution(),
                        Problem = problem
                    };

                    var candidateOk = true;
                    var t = -1;
                    var tSeed = -1;
                    for (var i = 0; i < clusters.Length; i++)
                    {
                        // start new tour if needed.
                        var (visit, seed) = clusters[i];
                        if (seed != tSeed)
                        { // start new tour.
                            t = candidate.AddNew();
                            candidate.InsertAfter(t, candidate.Tour(t).First, seed);
                            tSeed = seed;
                        }
                        
                        // skip the seed, it's already there.
                        if (visit == seed) continue;
                        
                        // add visit in cheapest position.
                        var cheapest = candidate.Tour(t).CalculateCheapest(problem.TravelWeight, visit,
                            null, (travelCost, v) => candidate.CanInsert(t, v, travelCost));
                        
                        if (cheapest.cost >= float.MaxValue)
                        {
                            // this is not feasible anymore.
                            candidateOk = false;
                            break;
                        }
                        candidate.InsertAfter(t, cheapest.location.From, visit);
                    }

                    // do binary search to get the best tour count.
                    if (!candidateOk)
                    { // tour count too low.
                        toursMin = tours;
                    }
                    else
                    { // tour count too high.
                        toursMax = tours;
                        if (best == null || best.Count > tours)
                        {
                            best = candidate;
                        }
                    }
                    var newTours = (toursMax + toursMin) / 2;
                    if (newTours == tours ||
                        newTours == toursMin)
                    {
                        break;
                    }
                    tours = newTours;
                }

                if (best == null)
                { // increase max, tours don't fit.
                    initialToursMax *= 2;
                    if (initialToursMax > visits.Count) initialToursMax = visits.Count;
                    toursMax = initialToursMax;

                    tours = (toursMin + toursMax) / 2;
                }
            }
            
            return best;
        }

        internal static (float priority, int seed, int seedIndex) Priority(int[] seeds, int visit, Func<int, int, float> weightFunc)
        {
            var closest = -1;
            var closestIndex = -1;
            var closestWeight = float.MaxValue;
            var secondClosestWeight = float.MaxValue;
            
            for (var s = 0; s < seeds.Length; s++)
            {
                var weight = weightFunc(seeds[s], visit);
                if (weight < closestWeight)
                {
                    secondClosestWeight = closestWeight;
                    closestIndex = s;
                    closest = seeds[s];
                    closestWeight = weight;
                    continue;
                }

                if (weight < secondClosestWeight)
                {
                    secondClosestWeight = weight;
                }
            }

            var priority = closestWeight / secondClosestWeight;

            return (priority, closest, closestIndex);
        }
        
        private static readonly ThreadLocal<KMeansConstructionStrategy> DefaultLazy = new ThreadLocal<KMeansConstructionStrategy>(() => new KMeansConstructionStrategy());
        public static KMeansConstructionStrategy Default => DefaultLazy.Value;
        
    }
}