using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Vehicles.Constraints;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Itinero.Optimization.Solvers.CVRP_ND;
using Itinero.Optimization.Solvers.CVRP_ND.Construction;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Shared.Seeds;
using Itinero.Optimization.Tests.Functional.Solvers.Shared.Seeds;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Itinero.Optimization.Tests.Functional.Solvers.CVRP_ND.Construction
{
    public static class SeededConstructionHeuristicTests
    {
        public static void TestLocations1_SeededConstructionHeuristic()
        {
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("Solvers.Shared.Seeds.data.locations1.geojson"));

            // build vehicle pool and capacity constraints.
            var vehicles = new VehiclePool()
            {
                Reusable = true,
                Vehicles = new[]
                {
                    new Vehicle()
                    {
                        Profile = vehicle.Fastest().FullName,
                        Metric = vehicle.Fastest().Metric.ToModelMetric(),
                        Departure = null,
                        Arrival = null,
                        CapacityConstraints = new CapacityConstraint[]
                        {
                            new CapacityConstraint()
                            {
                                Metric = Itinero.Optimization.Models.Metrics.Time,
                                Capacity = 3600 * 40
                            },
                            new CapacityConstraint()
                            {
                                Metric = Itinero.Optimization.Models.Metrics.Weight,
                                Capacity = 25000
                            }
                        }
                    }
                }
            };

            // build visits.
            var visits = new Visit[locations.Length];
            for (var v = 0; v < locations.Length; v++)
            {
                visits[v] = new Visit()
                {
                    Latitude = locations[v].Latitude,
                    Longitude = locations[v].Longitude,
                    VisitCosts = new VisitCost[]
                    {
                        new VisitCost()
                        {
                            Metric = Itinero.Optimization.Models.Metrics.Weight,
                            Value = 300
                        }
                    }
                };
            }

            var model = new Model()
            {
                VehiclePool = vehicles,
                Visits = visits
            };

            // do the mapping, maps the model to the road network.
            var mappings = ModelMapperRegistry.Default.Map(router, model);
            
            // convert to CVRPND problem.
            var problemResult = CVRPNDSolverHook.TryToCVRPND(mappings.mappedModel);
            if (problemResult.IsError)
            {
                throw new Exception($"Setup of test {nameof(SeededConstructionHeuristicTests)}.{nameof(TestLocations1_SeededConstructionHeuristic)} failed.");
            }

            var candidate = SeededConstructionHeuristicTest.Default.RunPerformance(problemResult.Value, 100);
        }

        public static void TestLocations1_Priority()
        {
            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("Solvers.Shared.Seeds.data.locations1.geojson"));

            Func<int, int, float> weightFunc = (x, y) => Coordinate.DistanceEstimateInMeter(locations[x], locations[y]);
            
            var nearestNeighbours = new NearestNeighbourArray(weightFunc,
                locations.Length, 20);
            var visits = new List<int>(Enumerable.Range(0, locations.Length));

            var seeds = SeedHeuristics.GetSeeds(visits, 5, weightFunc,
                nearestNeighbours);

            var features = new FeatureCollection();
            var visitsAndPriorities = new List<(float priority, int seed, int visit)>();
            foreach (var v in visits)
            {
                if (seeds.Contains(v)) continue;

                var (priority, closest, closestIndex) = SeededConstructionHeuristic.Priority(seeds, v, weightFunc);
                var attributes = new AttributesTable
                {
                    {"priority", priority},
                    {"visit", v},
                    {"seed", closest},
                    {"seed_idx", closestIndex}
                };
                features.Add(new Feature(new Point(new GeoAPI.Geometries.Coordinate(
                        locations[v].Longitude, locations[v].Latitude)),
                    attributes));
                
                visitsAndPriorities.Add((priority, closest, v));
            }

            File.WriteAllText("priorities.geojson", features.ToGeoJson());
            File.WriteAllText("priorities_seeds.geojson", seeds.ToGeoJson(locations));
            
            visitsAndPriorities.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
    }
}