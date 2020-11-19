using System.Collections.Generic;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Contains extension methods for model mappings.
    /// </summary>
    public static class IModelMappingExtensions
    {
        /// <summary>
        /// Builds a single route for the given tour.
        /// </summary>
        /// <param name="modelMapping">The model mapping.</param>
        /// <param name="tourAndVehicle">The tour.</param>
        /// <returns>The route.</returns>
        public static Result<Route> BuildRoute(this IModelMapping modelMapping, (int vehicle, IEnumerable<int> tour) tourAndVehicle)
        {
            Route? route = null;
            foreach (var routeSegmentResult in modelMapping.BuildRoutesBetweenVisits(tourAndVehicle))
            {
                if (routeSegmentResult.IsError) return routeSegmentResult;

                if (route == null)
                {
                    route = routeSegmentResult.Value;
                }
                else
                {
                    route = route.Concatenate(routeSegmentResult.Value);
                }
            }
            
            if (route == null) return new Result<Route>("No route segments in route, route probably has only a single visit.");
            
            return new Result<Route>(route);
        }
    }
}