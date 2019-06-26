using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Optimization.Tests.Functional.Performance;

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
        
        public static Result<Route> RunWithIntermediates(this Func<Action<Result<Route>>, Result<Route>> func, string name)
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

        public static IEnumerable<Result<Route>> RunWithIntermediates(this Func<Action<IEnumerable<Result<Route>>>, IEnumerable<Result<Route>>> func, string name, int count = 1)
        {
            if (Program.DoIntermediates)
            {
                var allIntermediateRoutes = new List<Route>();
                var localFunc = new Func<IEnumerable<Result<Route>>>(() => func((intermediateResults) =>
                {
                    var routes = new List<Route>();
                    foreach (var result in intermediateResults)
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
                    allIntermediateRoutes.AddRange(routes);
                    allIntermediateRoutes.WriteGeoJsonOneFile(name + "-all.geojson");
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
                var results = localFunc.TestPerf(name, count).ToList();
                results.WriteStats();
                results.WriteGeoJson(name + "-{0}.geojson");
                return results;
            }
        }
    }
}