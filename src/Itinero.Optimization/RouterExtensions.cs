/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Models;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Solvers;

namespace Itinero.Optimization
{
    /// <summary>
    /// Contains extensions methods for the router.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Tries to find the best tour to visit all the given visits with the given vehicles and constraints.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="profileName">The vehicle profile name.</param>
        /// <param name="locations">The locations to visit.</param>
        /// <param name="first">The location to start from, should point to an element in the locations index.</param>
        /// <param name="last">The location to stop at, should point to an element in the locations index if set.</param>
        /// <param name="max">The maximum relative to the profile defined, means maximum travel time when using 'fastest', maximum distance when using 'shortest'.</param>
        /// <param name="turnPenalty">The turn penalty in the same metric as the profile.</param>
        /// <param name="errors">The locations in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>A tours that visit the given locations using the vehicle respecting the contraints.</returns>
        public static Result<Route> Optimize(this RouterBase router, string profileName, Coordinate[] locations,
            out IEnumerable<(int location, string message)> errors, int first = 0, int? last = 0, float max = float.MaxValue, float turnPenalty = 0,
                Action<Result<Route>> intermediateResultsCallback = null)
        {
            if (!router.Db.SupportProfile(profileName))
            {
                throw new ArgumentException("Profile not supported.", nameof(profileName)); 
            }
            var profile = router.Db.GetSupportedProfile(profileName);

            var vehiclePool = VehiclePool.FromProfile(profile, departure: first, arrival: last, reusable: false, max: max, turnPenalty: turnPenalty);

            Action<IEnumerable<Result<Route>>> internalCallback = null;
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
            
            return router.Optimize(vehiclePool, locations, out errors, internalCallback).First();
        }

        /// <summary>
        /// Tries to find the best tour(s) to visit all the given locations with the given vehicles.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="vehicles">The vehicle pool.</param>
        /// <param name="locations">The locations to visit.</param>
        /// <param name="errors">The locations in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>A set of tours that visit the given locations using the vehicles in the pool.</returns>
        public static IEnumerable<Result<Route>> Optimize(this RouterBase router, VehiclePool vehicles,
            Coordinate[] locations, out IEnumerable<(int location, string message)> errors, 
                Action<IEnumerable<Result<Route>>> intermediateResultsCallback = null)
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
            
            return router.Optimize(vehicles, visits, out errors, intermediateResultsCallback);
        }

        /// <summary>
        /// Tries to find the best tour(s) to visit all the given visits with the given vehicles.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="vehicles">The vehicle pool.</param>
        /// <param name="visits">The visits to make.</param>
        /// <param name="errors">The visits in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>A set of tours that visit the given visits using the vehicles in the pool.</returns>
        public static IEnumerable<Result<Route>> Optimize(this RouterBase router, VehiclePool vehicles, Visit[] visits, 
            out IEnumerable<(int visit, string message)> errors, Action<IEnumerable<Result<Route>>> intermediateResultsCallback = null)
        {
            var model = new Model()
            {
                VehiclePool = vehicles,
                Visits = visits
            };

            return router.Optimize(model, out errors, intermediateResultsCallback);
        }

        /// <summary>
        /// Tries to find the best tour(s) to visit all the given visits with the given vehicles.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="model">The model, the problem description, to optimize.</param>
        /// <param name="errors">The visits in error and associated errors message if any.</param>
        /// <param name="intermediateResultsCallback">A callback to report on any intermediate solutions.</param>
        /// <returns>A set of tours that visit the given visits using the vehicles in the pool.</returns>
        public static IEnumerable<Result<Route>> Optimize(this RouterBase router, Model model,
            out IEnumerable<(int visit, string message)> errors,
            Action<IEnumerable<Result<Route>>> intermediateResultsCallback = null)
        {
            // do the mapping, maps the model to the road network.
            var mappings = ModelMapperRegistry.Map(router, model);
            errors = mappings.mapping.Errors?.Select(x => (x.visit, x.message));
            
            // report on intermediates if requested.
            Action<IEnumerable<(int vehicle, IEnumerable<int>)>> internalCallback = null;
            if (intermediateResultsCallback != null)
            {
                internalCallback = (sol) => intermediateResultsCallback(mappings.mapping.BuildRoutes(sol));
            }
            
            // call the solvers.
            var solution = SolverRegistry.Solve(mappings.mappedModel, internalCallback);
            
            // convert the raw solution to actual routes.
            return mappings.mapping.BuildRoutes(solution);
        }
    }
}