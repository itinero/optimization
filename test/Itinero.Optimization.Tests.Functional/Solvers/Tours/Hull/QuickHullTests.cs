using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.LocalGeo.IO;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Hull;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Tests.Functional.Solvers.Tours.Hull
{
    public static class QuickHullTests
    {
        public static void Test()
        {
            var p = 1000;
            while (p > 0)
            {
                var box = new Box(-0.01f, -0.01f, 0.01f, 0.01f);
                var locations = new List<Coordinate>();
                var allHull = new TourHull();
                var c = 5;
                while (locations.Count < c)
                {
                    locations.Add(box.RandomIn());
                    allHull.Add((locations[locations.Count - 1], locations.Count - 1));
                }

                var tour = new Tour(Enumerable.Range(0, c));

                var hull = tour.ConvexHull((v) => locations[v]);
                var hullGeoJson = hull.ToPolygon().ToGeoJson();
                var allHullGeoJson = allHull.ToPolygon().ToGeoJson();

                for (var i = 2; i < hull.Count; i++)
                {
                    var location1 = hull[i - 2].location;
                    var location2 = hull[i - 1].location;
                    var location3 = hull[i - 0].location;

                    var position = QuickHull.PositionToLine(location1, location3, location2);
                    if (position.right)
                    {
                        File.WriteAllText($"invalid-polygon-{p}.geojson", hullGeoJson);
                        throw new Exception("Invalid polygon generated.");
                    }
                }

                p--;
            }
        }
    
        private static Coordinate RandomIn(this Box box)
        {
            var rx = RandomGenerator.Default.Generate(1.0f);
            var ry = RandomGenerator.Default.Generate(1.0f);

            var latD = box.MaxLat - box.MinLat;
            var lonD = box.MaxLon - box.MinLon;
            
            return new Coordinate(box.MinLat + latD * rx, box.MinLon + lonD * ry);
        }
    }
}