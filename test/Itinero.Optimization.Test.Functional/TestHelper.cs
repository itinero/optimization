using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Functional
{
    public static class TestHelper
    {
        public static void RunWithIntermedidates(this Func<Action<Route[]>, List<Route>> func, string name)
        {
            var allIintermediateRoutes = new List<Route>();
            var localFunc = new Func<List<Route>>(() => func((intermediateRoutes) =>
            {
//                intermediateRoutes.Sort();
//                intermediateRoutes.AddTimeStamp();
//                intermediateRoutes.AddRouteId();
//                allIintermediateRoutes.AddRange(intermediateRoutes);
//                allIintermediateRoutes.WriteGeoJsonOneFile(name + "-all.geojson");
            }));
            RouteExtensions.ResetTimeStamp();
            var routes = localFunc.TestPerf(name);
            
            routes.WriteStats();
            routes.WriteGeoJson(name + "-{0}.geojson");
            routes.WriteJson(name + "-{0}.json");
        }
    }
}