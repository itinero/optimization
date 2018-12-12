using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.LocalGeo.IO;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Hull;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours.Hull
{
    public class QuickHullTests
    {
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
        public void QuickHull_PositionToLineShouldNotReturnLeftWhenOnLine()
        {
            var rawLocations = new[,]
            {
                {
                    4.793214797973633,
                    51.268238714159686
                },
                {
                    4.795253276824951,
                    51.265392413036416
                },
                {
                    4.7972917556762695,
                    51.26259964438126
                }
            };
            var locations = new List<(Coordinate location, int visit)>();
            for (var i = 0; i < rawLocations.GetLength(0); i++)
            {
                locations.Add((new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]), 0));
            }

            var result = QuickHull.PositionToLine(locations[0].location, locations[2].location, locations[1].location);
            Assert.False(result.left);
        }
        
        [Fact]
        public void QuickHull_PositionToLineShouldReturnTrueWhenRight()
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
            var result = QuickHull.PositionToLine(locations[2].location, locations[0].location, locations[1].location);
            Assert.True(result.right);
        }

        [Fact]
        public void QuickHull_PositionToLineShouldNotReturnRightWhenOnLine()
        {
            var rawLocations = new[,]
            {
                {
                    4.793214797973633,
                    51.268238714159686
                },
                {
                    4.795253276824951,
                    51.265392413036416
                },
                {
                    4.7972917556762695,
                    51.26259964438126
                }
            };
            var locations = new List<(Coordinate location, int visit)>();
            for (var i = 0; i < rawLocations.GetLength(0); i++)
            {
                locations.Add((new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]), 0));
            }

            var result = QuickHull.PositionToLine(locations[2].location, locations[0].location, locations[1].location);
            Assert.False(result.right);
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
                locations[result.partition1.farthest].location).distance;
            var distance2 = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                locations[result.partition2.farthest].location).distance;
            for (var i = result.partition1.start; i < result.partition1.start + result.partition1.length; i++)
            {
                var position = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                    locations[i].location);
                Assert.True(position.left, $"Location at {i} not to the left, {result} incorrect.");
                Assert.True(position.distance <= distance1);
            }
            for (var i = result.partition2.start; i < result.partition2.start + result.partition2.length; i++)
            {
                var position = QuickHull.PositionToLine(locations[0].location, locations[locations.Count - 1].location,
                    locations[i].location);
                Assert.False(position.left, $"Location at {i} not to the right, {result} incorrect.");
                Assert.True(position.distance <= distance2);
            }
        }
        
        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHullWithOneLocation()
        {
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(1, result.Count);
            Assert.Equal(0, result[0].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHullWithTwoLocations()
        { // based on quickhull-test1 data.
            var rawLocations = new [,]
            {
                {
                    4.769268035888672,
                    51.26460025070665
                },
                {
                    4.792957305908203,
                    51.26766141261736
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(2, result.Count);
            Assert.Equal(0, result[0].visit);
            Assert.Equal(1, result[1].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHull1()
        { // based on quickhull-test1 data.
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
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(6, result.Count);
            Assert.Equal(0, result[0].visit);
            Assert.Equal(8, result[1].visit);
            Assert.Equal(16, result[2].visit);
            Assert.Equal(18, result[3].visit);
            Assert.Equal(11, result[4].visit);
            Assert.Equal(10, result[5].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHull2()
        { // based on quickhull-test2 data.
            var rawLocations = new[,]
            {
                {
                    3.7105894088745117,
                    51.06440502028951
                },
                {
                    3.718442916870117,
                    51.05976594881228
                },
                {
                    3.7393856048583984,
                    51.05836334731389
                },
                {
                    3.7316608428955074,
                    51.066697413052474
                },
                {
                    3.7187862396240234,
                    51.06815369771887
                },
                {
                    3.723635673522949,
                    51.04603481029628
                },
                {
                    3.7147521972656254,
                    51.05830940025386
                },
                {
                    3.6995172500610347,
                    51.064809568438314
                },
                {
                    3.7061691284179688,
                    51.02563291294353
                },
                {
                    3.7624740600585933,
                    51.0107306152741
                },
                {
                    3.77105712890625,
                    51.062975588514966
                },
                {
                    3.7078857421874996,
                    51.08325320780629
                },
                {
                    3.6642837524414062,
                    51.06427017012091
                },
                {
                    3.7168121337890625,
                    51.042904943831246
                },
                {
                    3.7432479858398438,
                    51.04743778523849
                },
                {
                    3.7054824829101567,
                    51.07052680418648
                },
                {
                    3.69140625,
                    51.044415940251206
                },
                {
                    3.7034225463867183,
                    51.03858753963084
                },
                {
                    3.7446212768554688,
                    51.0191542415962
                },
                {
                    3.6127853393554683,
                    51.003817779005814
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            var geoJson = result.ToPolygon().ToGeoJson();
            Assert.Equal(5, result.Count);
            Assert.Equal(19, result[0].visit);
            Assert.Equal(12, result[1].visit);
            Assert.Equal(11, result[2].visit);
            Assert.Equal(10, result[3].visit);
            Assert.Equal(9, result[4].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHull3()
        {
            // based on quickhull-test5 data.
            var rawLocations = new[,]
            {
                {
                    4.344124,
                    51.83203
                },
                {
                    4.341613,
                    51.83187
                },
                {
                    4.341636,
                    51.83261
                },
                {
                    4.340333,
                    51.83325
                },
                {
                    4.338998,
                    51.8319
                },
                {
                    4.33851,
                    51.82996
                },
                {
                    4.338156,
                    51.83088
                },
                {
                    4.338167,
                    51.83089
                },
                {
                    4.336222,
                    51.83364
                },
                {
                    4.337722,
                    51.83294
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(4, result.Count);
            Assert.Equal(8, result[0].visit);
            Assert.Equal(3, result[1].visit);
            Assert.Equal(0, result[2].visit);
            Assert.Equal(5, result[3].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHull4()
        {
            // based on quickhull-test6 data.
            var rawLocations = new[,]
            {
                {
                    4.341386,
                    51.8541
                },
                {
                    4.342323,
                    51.85313
                },
                {
                    4.342957,
                    51.85265
                },
                {
                    4.33011,
                    51.85273
                },
                {
                    4.316804,
                    51.85365
                },
                {
                    4.315493,
                    51.85392
                },
                {
                    4.315526,
                    51.85396
                },
                {
                    4.315224,
                    51.85316
                },
                {
                    4.314861,
                    51.85225
                },
                {
                    4.309917,
                    51.85264
                },
                {
                    4.311972,
                    51.85228
                },
                {
                    4.310591,
                    51.85429
                },
                {
                    4.313155,
                    51.85436
                },
                {
                    4.312361,
                    51.85533
                },
                {
                    4.313437,
                    51.85707
                },
                {
                    4.311151,
                    51.85653
                },
                {
                    4.310231,
                    51.85749
                },
                {
                    4.312275,
                    51.85863
                },
                {
                    4.308912,
                    51.85757
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(7, result.Count);
            Assert.Equal(18, result[0].visit);
            Assert.Equal(17, result[1].visit);
            Assert.Equal(0, result[2].visit);
            Assert.Equal(2, result[3].visit);
            Assert.Equal(8, result[4].visit);
            Assert.Equal(10, result[5].visit);
            Assert.Equal(9, result[6].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHull5()
        {
            // based on quickhull-test7 data.
            var rawLocations = new[,]
            {
                {
                    4.342957,
                    51.85265
                },
                {
                    4.343845,
                    51.85194
                },
                {
                    4.342323,
                    51.85313
                },
                {
                    4.341386,
                    51.8541
                },
                {
                    4.342041,
                    51.85342
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result[0].visit);
            Assert.Equal(1, result[1].visit);
            Assert.Equal(2, result[2].visit);
        }

        [Fact]
        public void QuickHullPartition_ShouldBuildConvexHull6()
        {
            // based on quickhull-test8 data.
            var rawLocations = new[,]
            {
                {
                    0.002415606,
                    -0.006519126
                },
                {
                    0.005151248,
                    0.008702632
                },
                {
                    0.004671521,
                    -0.004189965
                },
                {
                    -0.00953385,
                    -0.006119041
                },
                {
                    0.003975872,
                    0.0004160181
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));

            var result = tour.ConvexHull((i) => new Coordinate((float) rawLocations[i, 1], (float) rawLocations[i, 0]));
            Assert.Equal(4, result.Count);
            Assert.Equal(3, result[0].visit);
            Assert.Equal(1, result[1].visit);
            Assert.Equal(2, result[2].visit);
            Assert.Equal(0, result[3].visit);
        }

        [Fact]
        public void QuickHull_ShouldUpdateConvexHullWhenOutsideRemove1()
        {
            // based on quickhull-test3 data.
            var rawLocations = new[,]
            {
                {
                    3.6127853393554683,
                    51.00403382073359
                },
                {
                    3.663597106933594,
                    51.06427017012091
                },
                {
                    3.707714080810547,
                    51.0834688793963
                },
                {
                    3.7705421447753906,
                    51.06319135463012
                },
                {
                    3.7623023986816406,
                    51.010838620166446
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }

            var outsideLocation = (new Coordinate(51.083684549980795f, 3.63321304321289f), rawLocations.GetLength(0));
            Assert.True(hull.UpdateHull(outsideLocation));
            var geoJson = hull.ToPolygon().ToGeoJson();
            Assert.Equal(5, hull.Count);
            Assert.Equal(0, hull[0].visit);
            Assert.Equal(5, hull[1].visit);
            Assert.Equal(2, hull[2].visit);
            Assert.Equal(3, hull[3].visit);
            Assert.Equal(4, hull[4].visit);
        }

        [Fact]
        public void QuickHull_ShouldUpdateConvexHullWhenOutsideRemove2()
        {
            // based on quickhull-test3 data.
            var rawLocations = new[,]
            {
                {
                    3.6127853393554683,
                    51.00403382073359
                },
                {
                    3.663597106933594,
                    51.06427017012091
                },
                {
                    3.707714080810547,
                    51.0834688793963
                },
                {
                    3.7705421447753906,
                    51.06319135463012
                },
                {
                    3.7623023986816406,
                    51.010838620166446
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }

            var outsideLocation = (new Coordinate(51.11214424141388f, 3.668060302734375f), rawLocations.GetLength(0));
            Assert.True(hull.UpdateHull(outsideLocation));
            var geoJson = hull.ToPolygon().ToGeoJson();
            Assert.Equal(4, hull.Count);
            Assert.Equal(0, hull[0].visit);
            Assert.Equal(5, hull[1].visit);
            Assert.Equal(3, hull[2].visit);
            Assert.Equal(4, hull[3].visit);
        }

        [Fact]
        public void QuickHull_ShouldUpdateConvexHullWhenRemovingFirstLocation()
        {
            // based on quickhull-test3 data.
            var rawLocations = new[,]
            {
                {
                    3.6127853393554683,
                    51.00403382073359
                },
                {
                    3.663597106933594,
                    51.06427017012091
                },
                {
                    3.707714080810547,
                    51.0834688793963
                },
                {
                    3.7705421447753906,
                    51.06319135463012
                },
                {
                    3.7623023986816406,
                    51.010838620166446
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }
            
            var outsideLocation = (new Coordinate(50.994094859909154f, 3.573989868164062f), rawLocations.GetLength(0));
            Assert.True(hull.UpdateHull(outsideLocation));
            var geoJson = hull.ToPolygon().ToGeoJson();
            Assert.Equal(5, hull.Count);
            Assert.Equal(5, hull[0].visit);
            Assert.Equal(1, hull[1].visit);
            Assert.Equal(2, hull[2].visit);
            Assert.Equal(3, hull[3].visit);
            Assert.Equal(4, hull[4].visit);
        }

        [Fact]
        public void QuickHull_ShouldUpdateConvexHullWhenRemovingLastLocations()
        {
            // based on quickhull-test3 data.
            var rawLocations = new[,]
            {
                {
                    3.6127853393554683,
                    51.00403382073359
                },
                {
                    3.663597106933594,
                    51.06427017012091
                },
                {
                    3.707714080810547,
                    51.0834688793963
                },
                {
                    3.7705421447753906,
                    51.06319135463012
                },
                {
                    3.7623023986816406,
                    51.010838620166446
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }
            
            var outsideLocation = (new Coordinate(50.99279831675386f, 3.7865066528320312f), rawLocations.GetLength(0));
            Assert.True(hull.UpdateHull(outsideLocation));
            var geoJson = hull.ToPolygon().ToGeoJson();
            Assert.Equal(5, hull.Count);
            Assert.Equal(0, hull[0].visit);
            Assert.Equal(1, hull[1].visit);
            Assert.Equal(2, hull[2].visit);
            Assert.Equal(3, hull[3].visit);
            Assert.Equal(5, hull[4].visit);
        }

        [Fact]
        public void QuickHull_ShouldUpdateConvexHull1()
        {
            // this is based on a failing case in quickhull-test9.
            var rawLocations = new[,]
            {
                {
                    -0.007477262,
                    -0.007843573
                },
                {
                    -0.00407523,
                    0.007215831
                },
                {
                    0.007546691,
                    -0.0008975575
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }

            var outsideLocation = (new Coordinate(-0.007505637f, -0.009466955f), rawLocations.GetLength(0));
            Assert.True(hull.UpdateHull(outsideLocation));
            Assert.Equal(4, hull.Count);
            Assert.Equal(0, hull[0].visit);
            Assert.Equal(3, hull[1].visit);
            Assert.Equal(1, hull[2].visit);
            Assert.Equal(2, hull[3].visit);
        }

        [Fact]
        public void QuickHull_ShouldUpdateConvexHull2()
        {
            // this is based on a failing case in quickhull-test10.
            var rawLocations = new[,]
            {
                {
                    -0.004583338,
                    -0.004557872
                },
                {
                    -0.004224535,
                    0.009189833
                },
                {
                    0.007053101,
                    0.003090885
                },
                {
                    0.006746819,
                    -0.005273492
                },
                {
                    0.003436277,
                    -0.005789657
                }
            };
            
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }

            var outsideLocation = (new Coordinate(-0.008669036f, -0.007516229f), rawLocations.GetLength(0));
            Assert.True(hull.UpdateHull(outsideLocation));
            Assert.Equal(4, hull.Count);
            Assert.Equal(1, hull[0].visit);
            Assert.Equal(2, hull[1].visit);
            Assert.Equal(3, hull[2].visit);
            Assert.Equal(5, hull[3].visit);
        }

        [Fact]
        public void QuickHull_BoxShouldBeTheExactBoundingBox()
        {
            // based on quickhull-test3 data.
            var rawLocations = new[,]
            {
                {
                    3.6127853393554683,
                    51.00403382073359
                },
                {
                    3.663597106933594,
                    51.06427017012091
                },
                {
                    3.707714080810547,
                    51.0834688793963
                },
                {
                    3.7705421447753906,
                    51.06319135463012
                },
                {
                    3.7623023986816406,
                    51.010838620166446
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }

            var box = hull.Box;
            Assert.Equal(51.004033820733590f, box.MinLat);
            Assert.Equal(51.083468879396300f, box.MaxLat);
            Assert.Equal(3.6127853393554683f, box.MinLon);
            Assert.Equal(3.7705421447753906f, box.MaxLon);
        }

        [Fact]
        public void QuickHull_ShouldCalculateSurface()
        {
            // based on quickhull-test4 data.
            var rawLocations = new[,]
            {
                {
                    4.0848541259765625,
                    51.13110763758015
                },
                {
                    4.2194366455078125,
                    51.13110763758015
                },
                {
                    4.2194366455078125,
                    51.204732392637645
                },
                {
                    4.0848541259765625,
                    51.204732392637645
                }
            };
            var tour = new Tour(Enumerable.Range(0, rawLocations.GetLength(0)));
            var hull = new TourHull();
            foreach (var visit in tour)
            {
                hull.Add((new Coordinate((float) rawLocations[visit, 1], (float) rawLocations[visit, 0]), visit));
            }

            var surface = hull.Surface;
            var boxSurface = (hull.Box.MaxLat - hull.Box.MinLat) * (hull.Box.MaxLon - hull.Box.MinLon);
            Assert.Equal(boxSurface, surface);
        }
    }
}