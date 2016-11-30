using System.IO;
using Itinero.Routing.Optimization.TSP;
using Itinero.LocalGeo;
using System.Collections.Generic;
using Itinero.Routing.Optimization.Directed;
using System;
using Itinero.Optimization.TSP.Directed;
using Itinero.Algorithms.Weights;
using Itinero.Algorithms;
using NetTopologySuite.Features;
using Itinero.Profiles;

namespace Itinero.Optimization.Test.Functional
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // enable logging.
            Itinero.Logging.Logger.LogAction = (origin, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", origin, level, message));
            };
            
            var routerDb = RouterDb.Deserialize(File.OpenRead(@"C:\work\data\routing\belgium.c.cf-e.routerdb"));
            var car = routerDb.GetSupportedVehicle("car");
            var router = new Router(routerDb);

            var locations = new List<Coordinate>(new Coordinate[]
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(51.270964016530680f, 4.7894811630249020f),
                new Coordinate(51.26216325894976f, 4.779932498931885f),
                new Coordinate(51.26579184564325f, 4.777781367301941f),
                new Coordinate(51.26855085674035f, 4.779181480407715f),
                new Coordinate(51.26906437701784f, 4.7879791259765625f),
                new Coordinate(51.259820134021695f, 4.7985148429870605f),
                new Coordinate(51.257040455587656f, 4.780147075653076f),
                new Coordinate(51.256248149311716f, 4.788386821746826f),
                new Coordinate(51.270054481615624f, 4.799646735191345f)
            });

            var route = router.CalculateTSP(car.Fastest(), locations.ToArray(), 0, 0);

            var routeWithTurnPenalty000 = router.CalculateTSPDirected(car.Fastest(), locations.ToArray(), 0, 0, 0);
            var routeWithTurnPenalty020 = router.CalculateTSPDirected(car.Fastest(), locations.ToArray(), 20, 0, 0);
            var routeWithTurnPenalty040 = router.CalculateTSPDirected(car.Fastest(), locations.ToArray(), 40, 0, 0);
            var routeWithTurnPenalty060 = router.CalculateTSPDirected(car.Fastest(), locations.ToArray(), 60, 0, 0);
            var routeWithTurnPenalty100 = router.CalculateTSPDirected(car.Fastest(), locations.ToArray(), 100, 0, 0);

            //var features = routerDb.GetFeaturesIn(new Coordinate(51.25691959619085f, 4.781885147094727f),
            //    new Coordinate(51.274937566783876f, 4.8088788986206055f), true, true);
            //var featuresJson = ToJson(features);

            //// test directed calculations.
            //var profile = car.Fastest();
            //var weightHandler = profile.DefaultWeightHandler(router);
            //for (var i = 0; i < locations.Count - 1; i++)
            //{
            //    var routerpoint1 = router.Resolve(profile, locations[i]);
            //    var routerpoint2 = router.Resolve(profile, locations[i + 1]);

            //    //var rawRoute = router.TryCalculateRaw(profile, weightHandler, routerpoint1.EdgeId + 1, routerpoint2.EdgeId + 1, null).Value;
            //    //rawRoute.StripSource();
            //    //rawRoute.StripTarget();
            //    //var route = router.BuildRoute(profile, weightHandler, routerpoint1, routerpoint2, rawRoute);

            //    //rawRoute = router.TryCalculateRaw(profile, weightHandler, routerpoint1.EdgeId + 1, -(routerpoint2.EdgeId + 1), null).Value;
            //    //rawRoute.StripSource();
            //    //rawRoute.StripTarget();
            //    //route = router.BuildRoute(profile, weightHandler, routerpoint1, routerpoint2, rawRoute);

            //    //rawRoute = router.TryCalculateRaw(profile, weightHandler, -(routerpoint1.EdgeId + 1), routerpoint2.EdgeId + 1, null).Value;
            //    //rawRoute.StripSource();
            //    //rawRoute.StripTarget();
            //    //route = router.BuildRoute(profile, weightHandler, routerpoint1, routerpoint2, rawRoute);

            //    var rawRoute = router.TryCalculateRaw(profile, weightHandler, -(routerpoint1.EdgeId + 1), -(routerpoint2.EdgeId + 1), null).Value;
            //    rawRoute.StripSource();
            //    rawRoute.StripTarget();
            //    var route = router.BuildRoute(profile, weightHandler, routerpoint1, routerpoint2, rawRoute);
            //}
        }

        private static string ToJson(FeatureCollection featureCollection)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringWriter();
            jsonSerializer.Serialize(jsonStream, featureCollection);
            var json = jsonStream.ToInvariantString();
            return json;
        }
    }
}