using System.Linq;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Mapping.Default;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Vehicles.Constraints;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Itinero.Optimization.Tests.Mocks;
using Xunit;

namespace Itinero.Optimization.Tests.Models.Mapping.Default
{
    public class DefaultModelMappingTests
    {
        [Fact]
        public void DefaultModelMapping_BuildRoutesBetweenVisits_ShouldBuildSegments()
        {
            var model = new MappedModel()
            {
                Visits = new[]
                {
                    new Visit()
                    {
                        Latitude = 1.01f,
                        Longitude = 1.02f,
                        TimeWindow = new TimeWindow(),
                        VisitCosts = new[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 100
                            }
                        }
                    },
                    new Visit()
                    {
                        Latitude = 2.01f,
                        Longitude = 2.02f,
                        TimeWindow = new TimeWindow(),
                        VisitCosts = new[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 200
                            }
                        }
                    }
                },
                VehiclePool = new VehiclePool()
                {
                    Vehicles = new[]
                    {
                        new Vehicle()
                        {
                            Arrival = null,
                            Departure = null,
                            Profile = "car",
                            CapacityConstraints = new CapacityConstraint[]
                            {
                                new CapacityConstraint()
                                {
                                    Capacity = 100,
                                    Metric = Metrics.Weight
                                }
                            },
                            Metric = Metrics.Time,
                            TurnPentalty = 0
                        }
                    }
                },
                TravelCosts = new[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = new[]
                        {
                            new[] {0f, 1f},
                            new[] {2f, 3f}
                        }
                    }
                }
            };
            var weights = new WeightMatrixAlgorithmMock(model);
            
            var mapping = new DefaultModelMapping(model, weights);

            var routes = mapping.BuildRoutesBetweenVisits((0, new [] { 0, 1 })).ToList();
            Assert.NotNull(routes);
            Assert.Equal(2, routes.Count); // a closed route.
            Assert.False(routes[0].IsError);
            Assert.False(routes[1].IsError);
            Assert.Equal(model.Visits[0].Latitude, routes[0].Value.Shape[0].Latitude);
            Assert.Equal(model.Visits[0].Longitude, routes[0].Value.Shape[0].Longitude);
            Assert.Equal(model.Visits[1].Latitude, routes[0].Value.Shape[1].Latitude);
            Assert.Equal(model.Visits[1].Longitude, routes[0].Value.Shape[1].Longitude);
            
            Assert.Equal(model.Visits[0].Latitude, routes[1].Value.Shape[1].Latitude);
            Assert.Equal(model.Visits[0].Longitude, routes[1].Value.Shape[1].Longitude);
            Assert.Equal(model.Visits[1].Latitude, routes[1].Value.Shape[0].Latitude);
            Assert.Equal(model.Visits[1].Longitude, routes[1].Value.Shape[0].Longitude);
        }
    }
}