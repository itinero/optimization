using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Mapping.Rewriters.VisitPosition;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Xunit;

namespace Itinero.Optimization.Tests.Models.Mapping.Rewriters.VisitPosition
{
    public class VisitPositionRewriterTests
    {
        [Fact]
        public void VisitPositionRewriter_N2_ShouldReturnNewCosts()
        {
            var visitPositionRewriter = new VisitPositionRewriter(new VisitPositionRewriterSettings()
            {
                AngleFunc = p => 90
            });

            var mockModelMapping = new MockModelMapping();
            var mappedModel = new MappedModel()
            {
                Visits = new []
                {
                    new Visit(),
                    new Visit()
                },
                TravelCosts = new []
                {
                    new TravelCostMatrix()
                    {
                        Costs = new []
                        {
                            new []{ 0f, 10, 20, 20 },
                            new []{ 10f, 0, 20, 20 },
                            new []{ 20f, 20, 0, 10 },
                            new []{ 20f, 20, 10, 0 }
                        },
                        Directed = true
                    }
                }
            };

            var rewrittenModel = visitPositionRewriter.Rewrite(mappedModel, mockModelMapping);
            Assert.NotNull(rewrittenModel);
            Assert.NotNull(rewrittenModel.TravelCosts);
            Assert.Single(rewrittenModel.TravelCosts);
            var travelCosts = rewrittenModel.TravelCosts[0];
            Assert.NotNull(travelCosts);
            var cost = travelCosts.Costs;
            Assert.NotNull(cost);
            Assert.Equal(4, cost.Length);
            Assert.Equal(4, cost[0].Length);
            Assert.Equal(4, cost[1].Length);
            Assert.Equal(4, cost[2].Length);
            Assert.Equal(4, cost[3].Length);
        }

        [Fact]
        public void VisitPositionRewriter_N2_Bidirectional_AllLeft_ShouldRemoveLeftWeights()
        {
            var visitPositionRewriter = new VisitPositionRewriter(new VisitPositionRewriterSettings()
            {
                AngleFunc = p => 90
            });

            var mockModelMapping = new MockModelMapping();
            var mappedModel = new MappedModel()
            {
                Visits = new[]
                {
                    new Visit(),
                    new Visit()
                },
                TravelCosts = new[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = new[]
                        {
                            new[] {0f, 10, 20, 20},
                            new[] {10f, 0, 20, 20},
                            new[] {20f, 20, 0, 10},
                            new[] {20f, 20, 10, 0}
                        },
                        Directed = true
                    }
                }
            };

            var rewrittenModel = visitPositionRewriter.Rewrite(mappedModel, mockModelMapping);
            var costs = rewrittenModel.TravelCosts[0].Costs;

            for (var from = 0; from < 4; from++)
            for (var to = 0; to < 4; to++)
            {
                var fromForward = (from % 2) == 1;
                var toForward = (to % 2) == 1;

                if (fromForward && toForward)
                {
                    // there should be a non-infinite weight.
                    Assert.True(costs[from][to] < float.MaxValue, $"Costs at [{from}][{to}] don't match.");
                }
                else
                {
                    // one of the two is forward, and left, so not allowed.
                    Assert.True(costs[from][to] >= float.MaxValue, $"Costs at [{from}][{to}] don't match.");
                }
            }
        }

        [Fact]
        public void VisitPositionRewriter_N2_OneWayForward_AllLeft_ShouldNotRemoveLeftWeights()
        {
            var visitPositionRewriter = new VisitPositionRewriter(new VisitPositionRewriterSettings()
            {
                AngleFunc = p => 90
            });

            var mockModelMapping = new MockModelMapping();
            var mappedModel = new MappedModel()
            {
                Visits = new[]
                {
                    new Visit(),
                    new Visit()
                },
                TravelCosts = new[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = new[]
                        {
                            new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
                            new[] {float.MaxValue,  0, float.MaxValue, 20},
                            new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
                            new[] {float.MaxValue, 20, float.MaxValue,  0}
                        },
                        Directed = true
                    }
                }
            };

            var rewrittenModel = visitPositionRewriter.Rewrite(mappedModel, mockModelMapping);
            var costs = rewrittenModel.TravelCosts[0].Costs;

            for (var from = 0; from < 4; from++)
            for (var to = 0; to < 4; to++)
            {
                var fromForward = (from % 2) == 1;
                var toForward = (to % 2) == 1;

                if (fromForward && toForward)
                {
                    // there should be a non-infinite weight.
                    // it should not have been overwritten.
                    Assert.True(costs[from][to] < float.MaxValue);
                }
                else
                {
                    // one of the two is backward and that's impossible.
                    Assert.True(costs[from][to] >= float.MaxValue);
                }
            }
        }

        [Fact]
        public void VisitPositionRewriter_N3_MixedDirections_MixedAngles_ShouldRemoveLeftWeights()
        {
            var visitPositionRewriter = new VisitPositionRewriter(new VisitPositionRewriterSettings()
            {
                AngleFunc = p =>
                {
                    if (p.EdgeId == 0)
                    {
                        return 90;
                    }
                    else
                    {
                        return 270;
                    }
                }
            });

            var mockModelMapping = new MockModelMapping();
            var mappedModel = new MappedModel()
            {
                Visits = new[]
                {
                    new Visit(),
                    new Visit(),
                    new Visit()
                },
                TravelCosts = new[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = new[]
                        {
                            new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
                            new[] {float.MaxValue,              0, float.MaxValue,            288,            254,            286},
                            new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
                            new[] {float.MaxValue,            352, float.MaxValue,              0,            128,             60},
                            new[] {float.MaxValue,            312, float.MaxValue,             49,              0,             48},
                            new[] {float.MaxValue,            380, float.MaxValue,            117,             89,              0},
                        },
                        Directed = true
                    }
                }
            };

            var rewrittenModel = visitPositionRewriter.Rewrite(mappedModel, mockModelMapping);
            var costs = rewrittenModel.TravelCosts[0].Costs;

            var expectedCosts =new[]
            {
                new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
                new[] {float.MaxValue,              0, float.MaxValue,            288,            254, float.MaxValue},
                new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
                new[] {float.MaxValue,            352, float.MaxValue,              0,            128, float.MaxValue},
                new[] {float.MaxValue,            312, float.MaxValue,             49,              0, float.MaxValue},
                new[] {float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue},
            };
            
            for (var from = 0; from < 6; from++)
            for (var to = 0; to < 6; to++)
            {
                Assert.Equal(expectedCosts[from][to], costs[from][to]);
            }
        }
    }
}