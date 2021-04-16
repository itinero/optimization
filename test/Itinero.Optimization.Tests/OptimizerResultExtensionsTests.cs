using System.Collections.Generic;
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

namespace Itinero.Optimization.Tests
{
    public class OptimizerResultExtensionsTests
    {
        [Fact]
        public void OptimizerResultExtensions_GetRoutes_ShouldReturnRoutes()
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

            var result = new OptimizerResult(model, mapping,
                new (int vehicle, IEnumerable<int> tour)[] {(0, new[] {0, 1})});
            var routes = result.GetRoutes().ToList();
            Assert.NotNull(routes);
            Assert.Single(routes); // one route.
            Assert.False(routes[0].IsError);
        }
        
        [Fact]
        public void OptimizerResultExtensions_GetRouteSegments_ShouldReturnSegmentsPerRoute()
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

            var result = new OptimizerResult(model, mapping,
                new (int vehicle, IEnumerable<int> tour)[] {(0, new[] {0, 1})});
            var routes = result.GetRouteSegments().ToList();
            Assert.NotNull(routes);
            Assert.Single(routes); // one route.

            var segments = routes[0].ToList();
            Assert.NotNull(segments);
            Assert.Equal(2, segments.Count); // a closed route.
            Assert.False(segments[0].IsError);
            Assert.False(segments[1].IsError);
        }
    }
}