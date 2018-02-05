using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinero.Optimization.Tours;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Itinero.Optimization.Test.Staging
{
    /// <summary>
    /// Some helper methods to setup test-problems.
    /// </summary>
    public static class StagingHelpers
    {
        public static string EmbeddedResourceRoot = "Itinero.Optimization.Test.";

        /// <summary>
        /// Gets a feature collection for the given embedded resource.
        /// </summary>
        public static FeatureCollection GetFeatureCollection(this string embeddedResourcePath)
        {
            using (var stream = typeof(NotSoRandomGenerator).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath))
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                return json.ToFeatures();
            }
        }

        /// <summary>
        /// Builds a test problem and default solution (as-the-crow-flies distances) from the given lines.
        /// </summary>
        /// <param name="features">The linestring(s).</param>
        /// <param name="tours">The predefined tour(s).</param>
        /// <param name="locations">The locations.</param>
        /// <returns>The weights (as-the-crow-flies distances) between the points in the linestring(s).</returns>
        public static float[][] BuildMatrix(this FeatureCollection features, out List<ITour> tours, 
            out List<Itinero.LocalGeo.Coordinate> locations, out IAttributesTable attributes)
        {
            tours = new List<ITour>();
            locations = new List<Itinero.LocalGeo.Coordinate>();
            attributes = null;

            foreach (var feature in features.Features)
            {
                var multiLineString = feature.Geometry as MultiLineString;
                attributes = feature.Attributes;
                if (multiLineString != null)
                {
                    foreach (var geometry in multiLineString.Geometries)
                    {
                        var lineString = geometry as LineString;
                        var rawTour = new List<int>();
                        foreach (var coord in lineString.Coordinates)
                        {
                            var locationId = locations.AddAndCheckDuplicates(coord);
                            rawTour.Add(locationId);
                        }
                        var last = rawTour[rawTour.Count - 1];
                        if (rawTour[0] == last)
                        {
                            rawTour.RemoveAt(rawTour.Count - 1);
                        }
                        tours.Add(new Tour(rawTour, last));
                    }
                }
            }

            var weights = new float[locations.Count][];
            for (var x = 0; x < locations.Count; x++)
            {
                weights[x] = new float[locations.Count];
                for (var y = 0; y < locations.Count; y++)
                {
                    weights[x][y] = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(
                        locations[x],
                        locations[y]
                    );
                }
            }
            return weights;
        }

        /// <summary>
        /// Adds the given location but not if there is already another location nearby. Returns the new or existing ID.
        /// </summary>
        public static int AddAndCheckDuplicates(this List<Itinero.LocalGeo.Coordinate> locations, GeoAPI.Geometries.Coordinate location, int meters = 10)
        {
            return locations.AddAndCheckDuplicates(new Itinero.LocalGeo.Coordinate((float)location.Y, (float)location.X), meters);
        }

        /// <summary>
        /// Adds the given location but not if there is already another location nearby. Returns the new or existing ID.
        /// </summary>
        public static int AddAndCheckDuplicates(this List<Itinero.LocalGeo.Coordinate> locations, Itinero.LocalGeo.Coordinate location, int meters = 10)
        {
            for (var i = 0; i  < locations.Count; i++)
            {
                if (Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(locations[i], location) < meters)
                {
                    return i;
                }
            }
            locations.Add(location);
            return locations.Count - 1;
        }
    }
}