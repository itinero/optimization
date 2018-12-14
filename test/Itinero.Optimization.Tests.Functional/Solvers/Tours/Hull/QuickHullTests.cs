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
        public static void Run()
        {
            Test();
            TestUpdate();
            TestIntersections();
        }
        
        public static void Test()
        {
            var p = 10000;
            while (p > 0)
            {
                // generate random points.
                var box = new Box(-0.01f, -0.01f, 0.01f, 0.01f);
                var locations = new List<Coordinate>();
                var allHull = new TourHull();
                var c = RandomGenerator.Default.Generate(5) + 3;
                while (locations.Count < c)
                {
                    locations.Add(box.RandomIn());
                    allHull.Add((locations[locations.Count - 1], locations.Count - 1));
                }

                // calculate hull.
                var tour = new Tour(Enumerable.Range(0, c));
                var hull = tour.ConvexHull((v) => locations[v]);
                var hullGeoJson = hull.ToPolygon().ToGeoJson();
                var allHullGeoJson = allHull.ToPolygon().ToGeoJson();

                // check if the hull is convex.
                if (!hull.IsConvex())
                {
                    throw new Exception("hull is not convex!");
                }
                
                // check if all non-boundary visits are inside the hull.
                var hullVisits = new HashSet<int>();
                foreach (var visit in hull)
                {
                    hullVisits.Add(visit.visit);
                }
                foreach (var visit in tour)
                {
                    if (hullVisits.Contains(visit)) continue;

                    if (!hull.Contains(locations[visit]))
                    {
                        throw new Exception("one of the visits is not in the hull!?");
                    }
                }

                p--;
            }
        }
        
        public static void TestUpdate()
        {
            var box = new Box(-0.01f, -0.01f, 0.01f, 0.01f);
            var p = 1000;
            while (p > 0)
            {
                // generate a random box and generate a hull in there.
                var box1 = new Box(box.RandomIn(), box.RandomIn());
                var locations = new List<Coordinate>();
                var allHull = new TourHull();
                var c = RandomGenerator.Default.Generate(5) + 3;
                while (locations.Count < c)
                {
                    locations.Add(box1.RandomIn());
                    allHull.Add((locations[locations.Count - 1], locations.Count - 1));
                }
                var tour = new Tour(Enumerable.Range(0, c));
                var hull = tour.ConvexHull((v) => locations[v]);
                var originalHullClone = hull.Clone();
                var hullGeoJson = hull.ToPolygon().ToGeoJson();
                var allHullGeoJson = allHull.ToPolygon().ToGeoJson();

                // check if hull is convex.
                if (!hull.IsConvex())
                {
                    throw new Exception("Invalid polygon generated.");
                }

                // update with other locations.
                var u = 15;
                while (u > 0)
                {
                    var expandWith = box.RandomIn();
                    var hullClone = hull.Clone();
                    if (hull.UpdateWith((expandWith, locations.Count)))
                    {
                        System.Diagnostics.Debug.WriteLine(string.Empty);
                    }

                    // check if hull is convex.
                    if (!hull.IsConvex())
                    {
                        throw new Exception("Invalid polygon generated.");
                    }

                    u--;
                }

                p--;
            }
        }
        
        public static void TestIntersections()
        {
            var p = 1000;
            while (p > 0)
            {
                var box = new Box(-0.01f, -0.01f, 0.01f, 0.01f);
                var locations = new List<Coordinate>();
                var allHull = new TourHull();
                var c = 10;
                while (locations.Count < c)
                {
                    locations.Add(box.RandomIn());
                    allHull.Add((locations[locations.Count - 1], locations.Count - 1));
                }

                var tour1 = new Tour(Enumerable.Range(0, c / 2));
                var hull1 = tour1.ConvexHull((v) => locations[v]);
                var tour2 = new Tour(Enumerable.Range(c / 2, c / 2));
                var hull2 = tour2.ConvexHull((v) => locations[v]);

                var intersection = hull1.Intersection(hull2);
                if (intersection != null)
                {
                    if (!intersection.IsConvex())
                    {
                        //File.WriteAllText($"invalid-intersection-polygon-{p}.geojson", intersection.ToPolygon().ToGeoJson());
                        throw new Exception("Invalid intersection polygon generated.");
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