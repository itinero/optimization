using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Shared.Seeds;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Itinero.Optimization.Tests.Functional.Solvers.Shared.Seeds
{
    public static class SeedHeuristicsTest
    {
        public static void TestLocations1()
        {
            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("Solvers.Shared.Seeds.data.locations1.geojson"));

            var nearestNeighbours = new NearestNeighbourArray(
                (x, y) => Coordinate.DistanceEstimateInMeter(locations[x], locations[y]),
                locations.Length, 20);
            var visits = new List<int>(Enumerable.Range(0, locations.Length));

            var seeds = SeedHeuristics.GetSeeds(visits, 5, (x, y) => Coordinate.DistanceEstimateInMeter(locations[x], locations[y]),
                nearestNeighbours);
            var geojson = seeds.ToGeoJson(locations);
            File.WriteAllText("seeds.geojson", geojson);
        }

        private static string ToGeoJson(this int[] seeds, Coordinate[] locations)
        {
            var featureCollection = new FeatureCollection();
            for (var i = 0; i < seeds.Length; i++)
            {
                var point = new Point(new GeoAPI.Geometries.Coordinate(
                    locations[seeds[i]].Longitude, locations[seeds[i]].Latitude));
                var attributes = new AttributesTable();
                attributes.Add("idx", i);
                attributes.Add("visit", seeds[i]);
                
                featureCollection.Features.Add(new Feature(point, attributes));
            }

            return featureCollection.ToGeoJson();
        }
    }
}