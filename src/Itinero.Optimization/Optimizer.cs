using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;

namespace Itinero.Optimization
{
    /// <summary>
    /// An optimizer, configured for certain optimization scenarios.
    /// </summary>
    public class Optimizer
    {
        private readonly RouterBase _router;
        private readonly OptimizerConfiguration _configuration;

        /// <summary>
        /// Creates a new optimizer.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="configuration">The configuration.</param>
        public Optimizer(RouterBase router, OptimizerConfiguration configuration = null)
        {
            _router = router;
            _configuration = configuration ?? OptimizerConfiguration.Default;
        }
        
        /// <summary>
        /// Tries to find the best tour to visit all the given visits with the given vehicles and constraints.
        /// </summary>
        /// <param name="profileName">The vehicle profile name.</param>
        /// <param name="locations">The locations to visit.</param>
        /// <param name="first">The location to start from, should point to an element in the locations index.</param>
        /// <param name="last">The location to stop at, should point to an element in the locations index if set.</param>
        /// <param name="max">The maximum relative to the profile defined, means maximum travel time when using 'fastest', maximum distance when using 'shortest'.</param>
        /// <param name="turnPenalty">The turn penalty in the same metric as the profile.</param>
        /// <param name="errors">The locations in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>The result of the optimization.</returns>
        public OptimizerResult Optimize(string profileName, Coordinate[] locations,
            out IEnumerable<(int location, string message)> errors, int first = 0, int? last = 0, float max = float.MaxValue, float turnPenalty = 0,
                Action<Result<Route>>? intermediateResultsCallback = null)
        {
            if (!_router.Db.SupportProfile(profileName))
            {
                throw new ArgumentException("Profile not supported.", nameof(profileName)); 
            }
            var profile = _router.Db.GetSupportedProfile(profileName);

            var vehiclePool = VehiclePool.FromProfile(profile, departure: first, arrival: last, reusable: false, max: max, turnPenalty: turnPenalty);

            Action<IEnumerable<Result<Route>>>? internalCallback = null;
            if (intermediateResultsCallback != null)
            {
                internalCallback = (rs) =>
                {
                    var r = rs.FirstOrDefault();
                    if (r != null)
                    {
                        intermediateResultsCallback(r);
                    }
                };
            }
            
            return this.Optimize(vehiclePool, locations, out errors, internalCallback);
        }

        /// <summary>
        /// Tries to find the best tour(s) to visit all the given locations with the given vehicles.
        /// </summary>
        /// <param name="vehicles">The vehicle pool.</param>
        /// <param name="locations">The locations to visit.</param>
        /// <param name="errors">The locations in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>The result of the optimization.</returns>
        public OptimizerResult Optimize(VehiclePool vehicles,
            Coordinate[] locations, out IEnumerable<(int location, string message)> errors, 
                Action<IEnumerable<Result<Route>>>? intermediateResultsCallback = null)
        {
            var visits = new Visit[locations.Length];
            for (var i = 0; i < visits.Length; i++)
            {
                var location = locations[i];
                visits[i] = new Visit()
                {
                    Longitude = location.Longitude,
                    Latitude = location.Latitude
                };
            }
            
            return this.Optimize(vehicles, visits, out errors, intermediateResultsCallback);
        }

        /// <summary>
        /// Tries to find the best tour(s) to visit all the given visits with the given vehicles.
        /// </summary>
        /// <param name="vehicles">The vehicle pool.</param>
        /// <param name="visits">The visits to make.</param>
        /// <param name="errors">The visits in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>The result of the optimization.</returns>
        public OptimizerResult Optimize(VehiclePool vehicles, Visit[] visits, 
            out IEnumerable<(int visit, string message)> errors, Action<IEnumerable<Result<Route>>>? intermediateResultsCallback = null)
        {
            var model = new Model()
            {
                VehiclePool = vehicles,
                Visits = visits
            };

            return this.Optimize(model, out errors, intermediateResultsCallback);
        }

        /// <summary>
        /// Tries to find the best tour(s) to visit all the given visits with the given vehicles.
        /// </summary>
        /// <param name="model">The model, the problem description, to optimize.</param>
        /// <param name="errors">The visits in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>The result of the optimization.</returns>
        public OptimizerResult Optimize(Model model,
            out IEnumerable<(int visit, string message)> errors,
            Action<IEnumerable<Result<Route>>>? intermediateResultsCallback = null)
        {
            try
            {
                // do the mapping, maps the model to the road network.
                var mappings = _configuration.ModelMapperRegistry.Map(_router, model, _configuration.RoutingSettings);
                errors = mappings.mapping.Errors?.Select(x => (x.visit, x.message)) ??
                         Enumerable.Empty<(int visit, string message)>();

                // report on intermediates if requested.
                Action<IEnumerable<(int vehicle, IEnumerable<int>)>>? internalCallback = null;
                if (intermediateResultsCallback != null)
                {
                    internalCallback = (sol) => intermediateResultsCallback(
                        sol.Select(x => mappings.mapping.BuildRoute(x)));
                }

                try
                {
                    // call the solvers.
                    var solution = _configuration.SolverRegistry.Solve(mappings.mappedModel, internalCallback);

                    // convert the raw solution to actual routes.
                    return new OptimizerResult(mappings.mappedModel, mappings.mapping, solution);
                }
                catch (Exception e)
                {
                    return new OptimizerResult(mappings.mappedModel, mappings.mapping, 
                        $"Unhandled exception while solving mapped model: {e.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}