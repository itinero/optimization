using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Shared.Seeds;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Hull;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Solvers.Shared.CheapestInsertion;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
namespace Itinero.Optimization.Solvers.CVRP_ND.Construction
{
    /// <summary>
    /// A seeded construction heuristic.
    /// </summary>
    public class SeededConstructionHeuristic : Strategy<CVRPNDProblem, CVRPNDCandidate>
    {
        /// <inheritdoc/>
        public override string Name { get; } = "SEED_CON_HEUR";

        /// <inheritdoc/>
        public override CVRPNDCandidate Search(CVRPNDProblem problem)
        {
            var visits = new List<int>(problem.Visits);
            var clusters = new (int visit, int seed)[visits.Count];
            CVRPNDCandidate best = null;

            while (best == null)
            {
                var toursMax = 10;
                var toursMin = 1;
                var tours = (toursMax + toursMin) / 2;
                while (true)
                {
                    // calculate clusters.
                    var seeds = SeedHeuristics.GetSeedsKMeans(visits, tours, (v) => problem.VisitLocation(v).Value, 
                        ref clusters);

                    // build tours out of the clusters.
                    var candidate = new CVRPNDCandidate()
                    {
                        Solution = new CVRPNDSolution(),
                        Problem = problem,
                        Fitness = 0
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
                            t = candidate.AddNew(seed);
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
                    if (newTours == tours)
                    {
                        break;
                    }
                    tours = newTours;
                }
            }

            return best;
        }
        
        private static readonly ThreadLocal<SeededConstructionHeuristic> DefaultLazy = new ThreadLocal<SeededConstructionHeuristic>(() => new SeededConstructionHeuristic());
        
        /// <summary>
        /// The default instance.
        /// </summary>
        public static SeededConstructionHeuristic Default => DefaultLazy.Value;
    }
}