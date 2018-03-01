using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms.Matrices;
using Itinero.LocalGeo;
using Itinero.Optimization.Models;
using Itinero.Optimization.Solutions.VRP.NoDepot.Capacitated;
using Itinero.Profiles;

namespace Itinero.Optimization.Test.Functional
{
    /// <summary>
    /// Optimization convenience extension methods for the router class.
    /// </summary>
    public static class RouterExtensions
    {
        // /// <summary>
        // /// Calculates a no-depot CVRP.
        // /// 
        // /// To be removed after this is supported.
        // /// </summary>
        // /// <param name="router">The router.</param>
        // /// <param name="profile">The profile to calculate for.</param>
        // /// <param name="locations">The locations.</param>
        // /// <param name="capacity">The capacity per tour.</param>
        // /// <returns></returns>
        // public static Result<List<Route>> TryCalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations, 
        //     Capacity capacity)
        // {
        //     // build model.
        //     var model = new Model()
        //     {
        //         Visits = locations,
        //         VehiclePool = new Models.Vehicles.VehiclePool()
        //         {
        //             Vehicles = new Models.Vehicles.Vehicle[] 
        //             {
        //                 new Models.Vehicles.Vehicle()
        //                 {
        //                     Profile = profile.FullName,
        //                     Departure = null,
        //                     Arrival = null
        //                 }
        //             },
        //             Reusable = true
        //         }
        //     };

        //     // solve model.
        //     var routes = router.Solve(model);

        //     return new Result<List<Route>>(routes.Value.ToList());
        // }

        // /// <summary>
        // /// Calculates a no-depot CVRP.
        // /// 
        // /// To be removed after this is supported.
        // /// </summary>
        // /// <param name="router">The router.</param>
        // /// <param name="profile">The profile to calculate for.</param>
        // /// <param name="locations">The locations.</param>
        // /// <param name="max">The maximum travel time per tour.</param>
        // /// <returns></returns>
        // public static List<Route> CalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations, float max)
        // {
        //     return router.TryCalculateNoDepotCVRP(profile, locations, new Capacity()
        //     {
        //         Max = max
        //     }).Value;
        // }

        // /// <summary>
        // /// Calculates a no-depot CVRP.
        // /// 
        // /// To be removed after this is supported.
        // /// </summary>
        // /// <param name="router">The router.</param>
        // /// <param name="profile">The profile to calculate for.</param>
        // /// <param name="locations">The locations.</param>
        // /// <param name="capacity">The capacity per tour.</param>
        // /// <returns></returns>
        // public static List<Route> CalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations, 
        //     Capacity capacity)
        // {
        //     return router.TryCalculateNoDepotCVRP(profile, locations, capacity).Value;
        // }
    }
}