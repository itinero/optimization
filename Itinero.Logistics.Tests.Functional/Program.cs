using Itinero.Algorithms.Weights;
using Itinero.Data.Edges;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using Itinero.Logistics.Routing.Loops;
using Itinero.Logistics.Routing.Matrix.Contracted;
using Itinero.Logistics.Routing.STSP;
using Itinero.Osm.Vehicles;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;

namespace Itinero.Logistics.Tests.Functional
{
    class Program
    {
        static void Main(string[] args)
        {
            // enable logging.
            OsmSharp.Logging.Logger.LogAction = (origin, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", origin, level, message));
            };
            Itinero.Logging.Logger.LogAction = (origin, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", origin, level, message));
            };

            Vehicle.RegisterVehicles();

            // download test-data (if not there yet).
            Download.ToFile("http://files.itinero.tech/data/itinero/routerdbs/planet/europe/belgium.c.cf.routerdb", "belgium.c.cf.routerdb").Wait();

            var routerDb = RouterDb.Deserialize(File.OpenRead("belgium.c.cf.routerdb"));

            var router = new Router(routerDb);
            var profile = Vehicle.Car.Fastest();
            routerDb.AddContracted(profile, profile.AugmentedWeightHandler(router), true);

            var locations = new List<Coordinate>(new Coordinate[]
             {
                            new Coordinate(51.270453873703080f, 4.8008108139038080f),
                            new Coordinate(51.264197451065370f, 4.8017120361328125f),
                            new Coordinate(51.267446600889850f, 4.7830009460449220f),
                            new Coordinate(51.260733228426076f, 4.7796106338500980f),
                            new Coordinate(51.256489871317920f, 4.7884941101074220f),
                            new Coordinate(51.270964016530680f, 4.7894811630249020f)
             });

            var resolved = router.Resolve(profile, locations.ToArray());

            var matrix = new AdvancedManyToManyBidirectionalDykstra<Weight>(routerDb, profile, profile.AugmentedWeightHandlerCached(router.Db),
                resolved);
            matrix.Run();
            
            //Coordinate source;
            //Coordinate[] locations;
            //Problems.STSP.ProblemBuilder.BuildEmbedded("Itinero.Logistics.Tests.Functional.Problems.STSP.problem1.geojson", out source, out locations);

            //var route = router.CalculateSTSP(profile, source, locations, ProfileMetric.TimeInSeconds,
            //    4 * 3600);

            //var route = router.GenerateLoop(profile, new LocalGeo.Coordinate(51.26781748613334f, 4.801349937915802f), Profiles.ProfileMetric.TimeInSeconds, 
            //    4 * 3600);
            //var routeJson = route.ToGeoJson();

            //var features = new FeatureCollection();
            //foreach(var vertex in vertexSearch.Vertices)
            //{
            //    var coordinate = routerDb.Network.GetVertex(vertex);
            //    features.Add(new Feature(new Point(new Coordinate(coordinate.Longitude, coordinate.Latitude)), 
            //        new AttributesTable()));
            //}
            //var jsonWriter = new NetTopologySuite.IO.GeoJsonSerializer();
            //var stringWriter = new StringWriter();
            //jsonWriter.Serialize(stringWriter, features);
            //var json = stringWriter.ToString();
        }
    }
}
