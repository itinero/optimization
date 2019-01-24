using System;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Vehicles.Constraints;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Itinero.Optimization.Solvers.CVRP_ND;
using Itinero.Optimization.Tests.Functional.Staging;

namespace Itinero.Optimization.Tests.Functional.Solvers.CVRP_ND.Construction
{
    public static class SeededConstructionHeuristicTests
    {
        public static void TestLocations1_SeededConstructionHeuristic()
        {
            // build routerdb and save the result.
            var spijkenisse = RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);
            var locations = StagingHelpers.GetLocations(
                StagingHelpers.GetFeatureCollection("Solvers.Shared.Seeds.data.locations1.geojson"));

            // build vehicle pool and capacity constraints.
            var vehicles = new VehiclePool
            {
                Reusable = true,
                Vehicles = new[]
                {
                    new Vehicle
                    {
                        Profile = vehicle.Fastest().FullName,
                        Metric = vehicle.Fastest().Metric.ToModelMetric(),
                        Departure = null,
                        Arrival = null,
                        CapacityConstraints = new[]
                        {
                            new CapacityConstraint
                            {
                                Metric = Metrics.Time,
                                Capacity = 3600 * 40
                            },
                            new CapacityConstraint
                            {
                                Metric = Metrics.Weight,
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
                visits[v] = new Visit
                {
                    Latitude = locations[v].Latitude,
                    Longitude = locations[v].Longitude,
                    VisitCosts = new[]
                    {
                        new VisitCost
                        {
                            Metric = Metrics.Weight,
                            Value = 300
                        }
                    }
                };
            }

            var model = new Model
            {
                VehiclePool = vehicles,
                Visits = visits
            };

            // do the mapping, maps the model to the road network.
            var mappings = ModelMapperRegistry.Default.Map(router, model);
            
            // convert to CVRPND problem.
            var problemResult = mappings.mappedModel.TryToCVRPND();
            if (problemResult.IsError)
            {
                throw new Exception($"Setup of test {nameof(SeededConstructionHeuristicTests)}.{nameof(TestLocations1_SeededConstructionHeuristic)} failed.");
            }

            var candidate = SeededConstructionHeuristicTest.Default.RunPerformance(problemResult.Value, 100);
        }
    }
}