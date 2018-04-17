using System.Collections.Generic;
using System.Linq;
using Itinero.Algorithms.Matrices;
using Itinero.LocalGeo;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Costs;
using Itinero.Optimization.Models.Vehicles.Constraints;
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
        /// <param name="capacityConstraints">The capacity constraints.</param>
        /// <param name="costs">The costs.</param>
        /// <returns></returns>
        public static Result<List<Route>> TryCalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations,
            CapacityConstraint[] capacityConstraints, VisitCosts[] costs = null, int? depot = null)
        {
            // build model.
            var model = new Model()
            {
                Visits = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[]
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = depot,
                            Arrival = null,
                            CapacityConstraints = capacityConstraints
                        }
                    },
                    Reusable = true
                },
                VisitCosts = costs,
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<List<Route>>(routes.Value.ToList());
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
        public static List<Route> CalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations, float max, int? depot)
        {
            return router.TryCalculateNoDepotCVRP(profile, locations, new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = max
                }
            }).Value;
        }

        /// <returns></returns>
        public static List<Route> CalculateNoDepotCVRP(this Router router, Profile profile, Coordinate[] locations,
            CapacityConstraint[] capacityConstraints = null, VisitCosts[] costs = null)
        {
            return router.TryCalculateNoDepotCVRP(profile, locations, capacityConstraints, costs).Value;
        }

        // --------- Depot Shizzle ---------
        /// <summary>
        /// Calculates a no-depot CVRP.
        /// 
        /// To be removed after this is supported.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="profile">The profile to calculate for.</param>
        /// <param name="locations">The locations.</param>
        /// <param name="capacityConstraints">The capacity constraints.</param>
        /// <param name="costs">The costs.</param>
        /// <returns></returns>
        public static Result<List<Route>> TryCalculateDepotCVRP(this Router router, Profile profile, Coordinate[] locations, int depot,
            CapacityConstraint[] capacityConstraints, VisitCosts[] costs = null)
        {
            // build model.
            var model = new Model()
            {
                Visits = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[]
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = depot,
                            Arrival = depot,
                            CapacityConstraints = capacityConstraints
                        }
                    },
                    Reusable = true
                },
                VisitCosts = costs
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<List<Route>>(routes.Value.ToList());
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
        public static List<Route> CalculateDepotCVRP(this Router router, Profile profile, Coordinate[] locations, int depot, float max)
        {
            return router.TryCalculateDepotCVRP(profile, locations, depot, new CapacityConstraint[]
            {
                new CapacityConstraint()
                {
                    Name = Itinero.Optimization.Models.Metrics.Time,
                    Capacity = max
                }
            }).Value;
        }

        /// <returns></returns>
        public static List<Route> CalculateDepotCVRP(this Router router, Profile profile, Coordinate[] locations, int depot,
            CapacityConstraint[] capacityConstraints = null, VisitCosts[] costs = null)
        {
            return router.TryCalculateDepotCVRP(profile, locations, depot, capacityConstraints, costs).Value;
        }




    }
}