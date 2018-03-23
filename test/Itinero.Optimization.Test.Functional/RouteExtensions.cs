using System.Collections.Generic;
using System.IO;

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
    }
}