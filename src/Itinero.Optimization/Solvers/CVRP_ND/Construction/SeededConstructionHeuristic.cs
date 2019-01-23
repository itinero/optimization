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
    public class SeededConstructionHeuristic : Strategy<CVRPNDProblem, CVRPNDCandidate>
    {
        /// <inheritdoc/>
        public override string Name { get; } = "SEED_CON_HEUR";

        public override CVRPNDCandidate Search(CVRPNDProblem problem)
        {
            Func<int, int, float> weightFunc = (x, y) =>
            {
                var xL = problem.VisitLocation(x).Value;
                var yL = problem.VisitLocation(y).Value;

                return Coordinate.DistanceEstimateInMeter(xL, yL);
            };

            CVRPNDCandidate best = null;
            while (best == null)
            {
                var tours = 10;
                while (true) //tours > 5)
                {
                    var visits = new List<int>(problem.Visits);
//                var seeds = SeedHeuristics.GetSeeds(visits, tours, weightFunc, new NearestNeighbourArray(weightFunc,
//                    problem.Count, 20));
                    var seeds = SeedHeuristics.GetSeedsKMeans(visits, tours, (v) => problem.VisitLocation(v).Value);
                    foreach (var seed in seeds)
                    {
                        visits.Remove(seed);
                    }

                    var candidate = new CVRPNDCandidate()
                    {
                        Solution = new CVRPNDSolution(),
                        Problem = problem,
                        Fitness = 0
                    };
                    for (var i = 0; i < seeds.Length; i++)
                    {
                        candidate.AddNew(seeds[i]);
                    }

                    var candidateOk = true;
                    foreach (var visit in visits)
                    {
                        var priority = Priority(seeds, visit, weightFunc);
                        var t = priority.seedIndex;
                        var cheapest = candidate.Tour(priority.seedIndex).CalculateCheapest(weightFunc, visit,
                            null, (travelCost, v) => candidate.CanInsert(t, v, travelCost));
                        //var cheapest = candidate.Tour(priority.seedIndex).CalculateCheapest(weightFunc, visit);
                        if (cheapest.cost >= float.MaxValue)
                        {
                            // this is not feasible anymore.
                            candidateOk = false;
                            break;
                        }

                        candidate.InsertAfter(priority.seedIndex, cheapest.location.From, visit);
                    }

                    if (!candidateOk) break;

                    tours--;
                    best = candidate;
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
        
        private static readonly ThreadLocal<SeededConstructionHeuristic> DefaultLazy = new ThreadLocal<SeededConstructionHeuristic>(() => new SeededConstructionHeuristic());
        public static SeededConstructionHeuristic Default => DefaultLazy.Value;
    }
}