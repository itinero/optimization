using System.Collections.Generic;
using System.IO;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Itinero.Optimization.Test.Functional.Staging
{    
    /// <summary>
    /// Some helper methods to setup test-problems.
    /// </summary>
    public static class StagingHelpers
    {
        public static string EmbeddedResourceRoot = "Itinero.Optimization.Test.Functional.";

        /// <summary>
        /// Gets a feature collection for the given embedded resource.
        /// </summary>
        public static FeatureCollection GetFeatureCollection(this string embeddedResourcePath)
        {
            using (var stream = typeof(PerformanceInfoConsumer).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                return json.ToFeatures();
            }
        }
        
        /// <summary>
        /// Extracts an array of Itinero coordinates from the given feature collection.
        /// </summary>
        public static Itinero.LocalGeo.Coordinate[] GetLocations(this FeatureCollection features)
        {
            var locations = new List<Itinero.LocalGeo.Coordinate>();

            foreach (var feature in features.Features)
            {
                if (feature.Geometry is Point)
                {
                    locations.Add(new Itinero.LocalGeo.Coordinate((float)feature.Geometry.Coordinate.Y,
                        (float)feature.Geometry.Coordinate.X));
                }
            }
            return locations.ToArray();
        }
    }
}