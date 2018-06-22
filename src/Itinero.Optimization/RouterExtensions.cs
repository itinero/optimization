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
        /// Optimizes the sequence along the given locations (the TSP).
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="profileName">The vehicle profile name.</param>
        /// <param name="locations">The locations to visit.</param>
        /// <param name="first">The location to start from, should point to an element in the locations index.</param>
        /// <param name="last">The location to stop at, should point to an element in the locations index if set.</param>
        /// <param name="errors">The visits in error and associated errors message if any.</param>
        /// <returns>A set of tours that visit the given visits using the vehicles in the pool.</returns>
        public static Result<Route> Optimize(this RouterBase router, string profileName, Coordinate[] locations,
            out IEnumerable<(Visit visit, string message)> errors, int first = 0, int? last = 0)
        {
            if (!router.Db.SupportProfile(profileName))
            {
                throw new ArgumentException("Profile not supported.", nameof(profileName)); 
            }
            var profile = router.Db.GetSupportedProfile(profileName);


            var vehiclePool = new Models.Vehicles.VehiclePool()
            {
                Vehicles = new[]
                {
                    new Vehicle()
                    {
                        Arrival = last,
                        Departure = first,
                        Metric = profile.Metric.ToModelMetric(),
                        Profile = profileName
                    }
                },
                Reusable = false
            };
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

            return router.Optimize(vehiclePool, visits, out errors).First();
        }

        /// <summary>
        /// Optimizes the tour(s) to visit the required visits using the vehicles in the pool.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="vehicles">The vehicle pool.</param>
        /// <param name="visits">The visits to make.</param>
        /// <param name="errors">The visits in error and associated errors message if any.</param>
        /// <returns>A set of tours that visit the given visits using the vehicles in the pool.</returns>
        public static IEnumerable<Result<Route>> Optimize(this RouterBase router, VehiclePool vehicles, Visit[] visits, 
            out IEnumerable<(Visit visit, string message)> errors)
        {
            var model = new Model()
            {
                VehiclePool = vehicles,
                Visits = visits
            };

            // do the mapping, maps the model to the road network.
            var mappings = ModelMapperRegistry.Map(router, model);
            errors = mappings.mapping.Errors?.Select(x => (visits[x.visit], x.message));
            
            // call the solvers.
            var solution = SolverRegistry.Solve(mappings.mappedModel, null);
            
            // convert the raw solution to actual routes.
            return mappings.mapping.BuildRoutes(solution);
        }
    }
}