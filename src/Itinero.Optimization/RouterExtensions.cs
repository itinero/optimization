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
        /// Returns an optimizer for the given router.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="configuration">The optimizer configuration.</param>
        /// <returns>An optimizer, configured with default settings.</returns>
        public static Optimizer Optimizer(this RouterBase router, OptimizerConfiguration configuration = null)
        {
            return new Optimizer(router, configuration);
        }
        
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
                Action<Result<Route>>? intermediateResultsCallback = null)
        {
            return router.Optimizer().Optimize(profileName, locations, out errors, first, last, max, turnPenalty,
                intermediateResultsCallback).GetRoutes().First();
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
                Action<IEnumerable<Result<Route>>>? intermediateResultsCallback = null)
        {
            return router.Optimizer().Optimize(vehicles, locations, out errors, intermediateResultsCallback).GetRoutes();
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
            out IEnumerable<(int visit, string message)> errors, Action<IEnumerable<Result<Route>>>? intermediateResultsCallback = null)
        {
            return router.Optimizer().Optimize(vehicles, visits, out errors, intermediateResultsCallback).GetRoutes();
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
            Action<IEnumerable<Result<Route>>>? intermediateResultsCallback = null)
        {
            return router.Optimizer().Optimize(model, out errors, intermediateResultsCallback).GetRoutes();
        }
    }
}