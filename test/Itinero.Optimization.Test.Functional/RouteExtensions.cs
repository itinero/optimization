using System;
using System.Collections.Generic;
using System.IO;
using Itinero.Attributes;
using Itinero.Optimization.Models;

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
    }
}