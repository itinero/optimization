using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using Itinero.Attributes;
using Itinero.LocalGeo;
using Itinero.Optimization.Models;
using NetTopologySuite.Triangulate;
using OsmSharp.IO.PBF;

namespace Itinero.Optimization.Test.Functional
{
    public static class RouteExtensions
    {
        /// <summary>
        /// Writes the route as geojson to the given file.
        /// </summary>
        public static void WriteGeoJson(this Route route, string fileName)
        {
            File.WriteAllText(fileName, route.ToGeoJson());
        }    

        /// <summary>
        /// Writes the routes as geojson to the given file names.
        /// </summary>
        public static void WriteGeoJson(this IList<Route> routes, string fileName)
        {
            var directory = new DirectoryInfo("result");
            if (!directory.Exists)
            {
                directory.Create();
            }

            for (var i = 0; i < routes.Count; i++)
            {
                if (routes[i] != null)
                {
                    File.WriteAllText(Path.Combine(directory.FullName, 
                        string.Format(fileName, i)), routes[i].ToGeoJson());
                }
            }
        }

        public static Box FromCoordinates(IEnumerable<Coordinate> coordinates)
        {
            Coordinate? coordinate1 = null;
            Coordinate? coordinate2 = null;
            Box? box = null;

            foreach (var coordinate in coordinates)
            {
                if (coordinate1 == null)
                {
                    coordinate1 = coordinate;
                }
                else if (coordinate2 == null)
                {
                    coordinate2 = coordinate;
                }
                else if (box == null)
                {
                    box = new Box(coordinate1.Value, coordinate2.Value);
                }
                else
                {
                    box = box.Value.ExpandWith(coordinate.Latitude, coordinate.Longitude);
                }
            }

            return box.Value;
        }

        public static void Sort(this List<Route> routes)
        {
            routes.Sort((r1, r2) =>
            {
                if (r1 == null && r2 == null)
                {
                    return 0;
                }

                if (r1 == null)
                {
                    return -1;
                }

                if (r2 == null)
                {
                    return 1;
                }
                
                var center1 = FromCoordinates(r1.Shape).Center;
                var center2 = FromCoordinates(r2.Shape).Center;

                if (center1.Latitude != center2.Latitude)
                {
                    return center1.Latitude.CompareTo(center2.Latitude);
                }

                return center1.Longitude.CompareTo(center2.Longitude);
            });
        }

        public static void Sort(this Route[] routes)
        {
            Array.Sort(routes, (r1, r2) =>
            {
                if (r1 == null && r2 == null)
                {
                    return 0;
                }

                if (r1 == null)
                {
                    return -1;
                }

                if (r2 == null)
                {
                    return 1;
                }
                
                var center1 = FromCoordinates(r1.Shape).Center;
                var center2 = FromCoordinates(r2.Shape).Center;

                if (center1.Latitude != center2.Latitude)
                {
                    return center1.Latitude.CompareTo(center2.Latitude);
                }

                return center1.Longitude.CompareTo(center2.Longitude);
            });
        }

        /// <summary>
        /// Writes all the routes to one geojson file.
        /// </summary>
        public static void WriteGeoJsonOneFile(this IEnumerable<Route> routes, string fileName)
        {
            var directory = new DirectoryInfo("result");
            if (!directory.Exists)
            {
                directory.Create();
            }

            fileName = Path.Combine(directory.FullName, fileName);
            using (var stream = File.Open(fileName, FileMode.Create))
            using (var streamWriter = new StreamWriter(stream))
            {
                routes.WriteGeoJson(streamWriter);
            }
        }

        /// <summary>
        /// Writes the routes as geojson to the given file names.
        /// </summary>
        public static void WriteJson(this IList<Route> routes, string fileName)
        {
            var directory = new DirectoryInfo("result");
            if (!directory.Exists)
            {
                directory.Create();
            }
            for (var i = 0; i < routes.Count; i++)
            {
                if (routes[i] != null)
                {
                    File.WriteAllText(Path.Combine(directory.FullName, 
                        string.Format(fileName, i)), routes[i].ToJson());
                }
            }
        }
        
        /// <summary>
        /// Writes the route as geojson.
        /// </summary>
        public static void WriteGeoJson(this IEnumerable<Route> routes, TextWriter writer, bool includeShapeMeta = true, bool includeStops = true, bool groupByShapeMeta = true,
            Action<IAttributeCollection> attributesCallback = null)
        {
            if (routes == null) { throw new ArgumentNullException(nameof(routes)); }
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }

            var jsonWriter = new Itinero.IO.Json.JsonWriter(writer);
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "FeatureCollection", true, false);
            jsonWriter.WritePropertyName("features", false);
            jsonWriter.WriteArrayOpen();

            foreach (var route in routes)
            {
                route?.WriteGeoJsonFeatures(jsonWriter, includeShapeMeta, includeStops, groupByShapeMeta, attributesCallback,
                    (k, v) => k == "anistart" || k == "aniend");
            }

            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();
        }

         /// <summary>
        /// Writes stats about the given routes.
        /// </summary>
        /// <param name="routes"></param>
        public static void WriteStats(this IList<Route> routes)
        {
            Console.WriteLine("# routes: {0}", routes.Count);

            var totalStops = 0;
            var totalDistance = 0f;
            var totalTime = 0f;

            for (var i = 0; i < routes.Count; i++)
            {
                Console.Write("route_{0}:", i);

                if (routes[i] == null)
                {
                    Console.WriteLine("null");
                    continue;
                }

                var extraTime = 0f;
                var extraWeight = 0f;
                if (routes[i].Stops != null)
                {
                    for (var s = 0; s < routes[i].Stops.Length - 1; s++)
                    {
                        var stop = routes[i].Stops[s];
                        if (stop.Attributes.TryGetSingle("cost_" + Metrics.Time, out float localExtraTime))
                        {
                            extraTime += localExtraTime;
                        }
                        if (stop.Attributes.TryGetSingle("cost_" + Metrics.Weight, out float localExtraWeight))
                        {
                            extraWeight += localExtraWeight;
                        }
                    }
                }
                Console.Write("{0}s | ", routes[i].TotalTime);
                Console.Write("{0}m | ", routes[i].TotalDistance);
                Console.Write("{0}stops with {1}s {2}kg", routes[i].Stops.Length - 1, extraTime, extraWeight);
                totalStops += routes[i].Stops.Length - 1;
                totalDistance += routes[i].TotalDistance;
                totalTime += routes[i].TotalTime;
                Console.WriteLine();
            }

            Console.Write("total:");
            Console.Write("{0}s | ", totalTime);
            Console.Write("{0}m | ", totalDistance);
            Console.Write("{0}stops", totalStops);
            Console.WriteLine();
        }

        public static void AddRouteId(this IEnumerable<Route> routes)
        {
            var routeId = 0;

            foreach (var route in routes)
            {
                route.AddExtraAttributes("route-id", routeId.ToString());
                routeId++;
            }
        }

        private static int previous = -1;

        public static void ResetTimeStamp()
        {
            previous = 1;
        }

        public static void AddTimeStamp(this IEnumerable<Route> routes)
        {
            if (previous == -1)
            {
                previous = 0;
            }
            var timestamp = previous.ToString(); //.ToString("HH:mm:ss");
            var now = previous + 1;
            var timestampEnd = now.ToString(); //.ToString("HH:mm:ss");
            
            foreach (var route in routes)
            {
                route.AddExtraAttributes("anistart", timestamp);
                route.AddExtraAttributes("aniend", timestampEnd);
            }

            previous += 2;
        }

        public static void AddTimeStamp(this Route route, string timestamp)
        {
            route.AddExtraAttributes("timestamp", timestamp);
        }

        public static void AddExtraAttributes(this Route route, string key, string value)
        {
            if (route == null)
            {
                return;
            }
            
            if (route.ShapeMeta != null)
            {
                foreach (var shapeMeta in route.ShapeMeta)
                {
                    shapeMeta.Attributes.AddOrReplace(key, value);
                }
            }

            if (route.Stops != null)
            {
                foreach (var stop in route.Stops)
                {
                    stop.Attributes.AddOrReplace(key, value);
                }
            }
        }
    }
}