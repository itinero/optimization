using GeoAPI.Geometries;
using Itinero.Data.Edges;
using Itinero.IO.Osm;
using Itinero.Logistics.Routing.Loops;
using Itinero.Osm.Vehicles;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
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

            var routerDb = new RouterDb();
            routerDb.LoadOsmData(File.OpenRead(@"D:\work\data\OSM\kempen-big.osm.pbf"), false, Vehicle.Bicycle);
            var profile = Vehicle.Bicycle.Fastest();
            //routerDb.AddContracted(profile);
            var router = new Router(routerDb);
            
            var route = router.GenerateLoop(profile, new LocalGeo.Coordinate(51.26781748613334f, 4.801349937915802f), 4 * 3600);
            var routeJson = route.ToGeoJson();

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
