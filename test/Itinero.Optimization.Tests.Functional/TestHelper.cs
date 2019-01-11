using System;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Optimization.Tests.Functional
{
    public static class TestHelper
    {
        public static Result<Route> Run(this Func<Result<Route>> func, string name)
        {
            var result = func.TestPerf(name);
            result.WriteStats();
            result.WriteGeoJson(name + ".geojson");
            return result;
        }
        
        public static Result<Route> RunWithIntermedidates(this Func<Action<Result<Route>>, Result<Route>> func, string name)
        {
            if (Program.DoIntermediates)
            {
                var allIintermediateRoutes = new List<Route>();
                var localFunc = new Func<Result<Route>>(() => func((intermediateResult) =>
                {
                    var routes = new List<Route>();
                    if (intermediateResult.IsError)
                    {
                        return;
                    }
                    routes.Add(intermediateResult.Value);
                    routes.Sort();
                    routes.AddTimeStamp();
                    routes.AddRouteId();
                    allIintermediateRoutes.AddRange(routes);
                    allIintermediateRoutes.WriteGeoJsonOneFile(name + "-all.geojson");
                }));

                RouteExtensions.ResetTimeStamp();
                var result = localFunc.TestPerf(name);
                result.WriteStats();
                result.WriteGeoJson(name + ".geojson");
                return result;
            }
            else
            {
                var allIintermediateRoutes = new List<Route>();
                var localFunc = new Func<Result<Route>>(() => func(null));
                var result = localFunc.TestPerf(name);
                result.WriteStats();
                result.WriteGeoJson(name + ".geojson");
                return result;
            }
        }

        public static IEnumerable<Result<Route>> RunWithIntermedidates(this Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func, string name)
        {
            if (Program.DoIntermediates)
            {
                var allIintermediateRoutes = new List<Route>();
                var localFunc = new Func<IEnumerable<Result<Route>>>(() => func((intermedidateResults) =>
                {
                    var routes = new List<Route>();
                    foreach (var result in intermedidateResults)
                    {
                        if (result.IsError)
                        {
                            continue;
                        }

                        routes.Add(result.Value);
                    }
                    routes.Sort();
                    routes.AddTimeStamp();
                    routes.AddRouteId();
                    allIintermediateRoutes.AddRange(routes);
                    allIintermediateRoutes.WriteGeoJsonOneFile(name + "-all.geojson");
                }));

                RouteExtensions.ResetTimeStamp();
                var results = localFunc.TestPerf(name).ToList();
                results.WriteStats();
                results.WriteGeoJson(name + "-{0}.geojson");
                return results;
            }
            else
            {
                var allIintermediateRoutes = new List<Route>();
                var localFunc = new Func<IEnumerable<Result<Route>>>(() => func(null));
                var results = localFunc.TestPerf(name).ToList();
                results.WriteStats();
                results.WriteGeoJson(name + "-{0}.geojson");
                return results;
            }
        }
    }
}