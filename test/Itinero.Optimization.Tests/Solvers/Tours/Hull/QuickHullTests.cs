using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Tours.Hull;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours.Hull
{
    public class QuickHullTests
    {
        public void QuickHullPartition_ShouldPartitionEmpty()
        {
            
        }

        public void QuickHullPartition_ShouldPartitionOne()
        {
            
        }

        [Fact]
        public void QuickHullPartition_ShouldPartition()
        {
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                },
                {
                    4.792957305908203,
                    51.26766141261736
                },
                {
                    4.795103073120117,
                    51.26234452724744
                },
                {
                    4.784202575683594,
                    51.258369887982454
                },
                {
                    4.780082702636719,
                    51.2625593628227
                },
                {
                    4.78729248046875,
                    51.263418695082386
                },
                {
                    4.79527473449707,
                    51.266265118440224
                },
                {
                    4.793901443481445,
                    51.26997057527367
                },
                {
                    4.778108596801758,
                    51.27120566115648
                },
                {
                    4.774675369262695,
                    51.266909567178125
                },
                {
                    4.782228469848633,
                    51.254556059682514
                },
                {
                    4.796304702758789,
                    51.257779033886585
                },
                {
                    4.796133041381836,
                    51.263687233118745
                },
                {
                    4.784717559814453,
                    51.26460025070665
                },
                {
                    4.777851104736328,
                    51.26680215968272
                },
                {
                    4.779396057128905,
                    51.270561272663755
                },
                {
                    4.8023128509521475,
                    51.273299861348995
                },
                {
                    4.799652099609375,
                    51.26374094053775
                },
                {
                    4.808921813964844,
                    51.26153888489712
                }
            };
            var locations = new List<(Coordinate location, int visit)>();
            for (var i = 0; i < rawLocations.GetLength(0); i++)
            {
                locations.Add((new Coordinate((float)rawLocations[i, 1], (float)rawLocations[i, 0]), 0));
            }

            var result = locations.Partition((1, locations.Count - 2));
            var distance1 = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                locations[result.farthest1].location).distance;
            var distance2 = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                locations[result.farthest2].location).distance;
            for (var i = 1; i < result.partition; i++)
            {
                var position = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                    locations[i].location);
                Assert.True(position.left, $"Location at {i} not to the left, {result} incorrect.");
                if (i != result.farthest1) Assert.True(position.distance <= distance1);
            }
            for (var i = result.partition; i < locations.Count - 2; i++)
            {
                var position = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                    locations[i].location);
                Assert.False(position.left, $"Location at {i} not to the right, {result} incorrect.");
                if (i != result.farthest2) Assert.True(position.distance <= distance2);
            }
        }

        [Fact]
        public void QuickHullPartition_ShouldPartitionWithFarthest()
        {
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                },
                {
                    4.792957305908203,
                    51.26766141261736
                },
                {
                    4.795103073120117,
                    51.26234452724744
                },
                {
                    4.784202575683594,
                    51.258369887982454
                },
                {
                    4.780082702636719,
                    51.2625593628227
                },
                {
                    4.78729248046875,
                    51.263418695082386
                },
                {
                    4.79527473449707,
                    51.266265118440224
                },
                {
                    4.793901443481445,
                    51.26997057527367
                },
                {
                    4.778108596801758,
                    51.27120566115648
                },
                {
                    4.774675369262695,
                    51.266909567178125
                },
                {
                    4.782228469848633,
                    51.254556059682514
                },
                {
                    4.796304702758789,
                    51.257779033886585
                },
                {
                    4.796133041381836,
                    51.263687233118745
                },
                {
                    4.784717559814453,
                    51.26460025070665
                },
                {
                    4.777851104736328,
                    51.26680215968272
                },
                {
                    4.779396057128905,
                    51.270561272663755
                },
                {
                    4.8023128509521475,
                    51.273299861348995
                },
                {
                    4.799652099609375,
                    51.26374094053775
                },
                {
                    4.808921813964844,
                    51.26153888489712
                }
            };
            var locations = new List<(Coordinate location, int visit)>();
            for (var i = 0; i < rawLocations.GetLength(0); i++)
            {
                locations.Add((new Coordinate((float)rawLocations[i, 1], (float)rawLocations[i, 0]), 0));
            }

            var result = locations.Partition((1, locations.Count - 2));

            var t = locations[result.farthest1];
            locations[result.farthest1] = locations[result.partition];
            locations[result.partition] = t;
            
            var recursiveResult = locations.Partition((1, result.partition - 1), 0, locations.Count - 1, result.partition);
            var partition1 = recursiveResult.partition1;
            Assert.Equal(1, partition1.start);
            Assert.Equal(3, partition1.length);
            var partition2 = recursiveResult.partition2;
            Assert.Equal(result.partition, partition2.start);
            Assert.Equal(0, partition2.length);
            Assert.Equal(result.partition, partition2.farthest);
        }

        [Fact]
        public void QuickHull_PositionToLineShouldReturnTrueWhenLeft()
        {
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                },
                {
                    4.792957305908203,
                    51.26766141261736
                },
                {
                    4.808921813964844,
                    51.26153888489712
                }
            };
            var locations = new List<(Coordinate location, int visit)>();
            for (var i = 0; i < rawLocations.GetLength(0); i++)
            {
                locations.Add((new Coordinate((float)rawLocations[i, 1], (float)rawLocations[i, 0]), 0));
            }
            var result = QuickHull.PositionToLine(locations[0].location, locations[2].location, locations[1].location);
            Assert.True(result.left);
        }

        [Fact]
        public void QuickHull_PositionToLineShouldReturnFalseWhenRight()
        {
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                },
                {
                    4.784631729125977,
                    51.25681216534918
                },
                {
                    4.808921813964844,
                    51.26153888489712
                }
            };
            var locations = new List<(Coordinate location, int visit)>();
            for (var i = 0; i < rawLocations.GetLength(0); i++)
            {
                locations.Add((new Coordinate((float)rawLocations[i, 1], (float)rawLocations[i, 0]), 0));
            }
            var result = QuickHull.PositionToLine(locations[0].location, locations[2].location, locations[1].location);
            Assert.False(result.left);
        }
    }
}