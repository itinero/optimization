using System.Collections.Generic;
using Itinero.Algorithms.Matrices;
using Itinero.LocalGeo;
using Itinero.Optimization.Routing;
using Itinero.Profiles;

namespace Itinero.Optimization.Test.Functional
{
    /// <summary>
    /// Optimization convenience extension methods for the router class.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Calculates a no-depot CVRP.
        /// 
        /// To be removed after this is supported.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="profile">The profile to calculate for.</param>
        /// <param name="locations">The locations.</param>
        /// <param name="max">The maximum travel time per tour.</param>
        /// <returns></returns>
        public static Result<List<Route>> TryCalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations, float max)
        {
            var vrpRouter = new Itinero.Optimization.VRP.NoDepot.Capacitated.NoDepotCVRPRouter(new WeightMatrixAlgorithm(router, profile, locations),
                max);
            vrpRouter.Run();
            if (!vrpRouter.HasSucceeded)
            {
                return new Result<List<Route>>(vrpRouter.ErrorMessage);
            }
            return new Result<List<Route>>(vrpRouter.WeightMatrix.BuildRoutes(vrpRouter.RawSolution));
        }

        /// <summary>
        /// Calculates a no-depot CVRP.
        /// 
        /// To be removed after this is supported.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="profile">The profile to calculate for.</param>
        /// <param name="locations">The locations.</param>
        /// <param name="max">The maximum travel time per tour.</param>
        /// <returns></returns>
        public static List<Route> CalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations, float max)
        {
            return router.TryCalculateNoDepotCVRP(profile, locations, max).Value;
        }
    }
}