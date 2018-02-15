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
            for (var i = 0; i < routes.Count; i++)
            {
                if (routes[i] != null)
                {
                    File.WriteAllText(string.Format(fileName, i), routes[i].ToGeoJson());
                }
            }
        }       
    }
}