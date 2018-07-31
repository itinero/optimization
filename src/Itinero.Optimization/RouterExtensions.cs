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
using Itinero.Algorithms.Matrices;
using Itinero.LocalGeo;
using Itinero.Optimization.Sequences.Directed;
using Itinero.Optimization.Abstract.Solvers.STSP;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Profiles;
using Itinero.Algorithms.Search;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models;

namespace Itinero.Optimization
{
    /// <summary>
    /// Optimization convenience extension methods for the router class.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Calculates a directed sequence of the given sequence of locations.
        /// </summary>
        public static Route CalculateDirected(this RouterBase router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, Tour sequence)
        {
            return router.TryCalculateDirected(profile, locations, turnPenaltyInSeconds, sequence).Value;
        }

        /// <summary>
        /// Calculates a directed sequence of the given sequence of locations.
        /// </summary>
        public static Result<Route> TryCalculateDirected(this RouterBase router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, Tour sequence)
        {
            var directedRouter = new SequenceDirectedRouter(new DirectedWeightMatrixAlgorithm(router, profile, locations), turnPenaltyInSeconds, sequence);
            directedRouter.Run();
            if (!directedRouter.HasSucceeded)
            {
                return new Result<Route>(directedRouter.ErrorMessage);
            }
            return new Result<Route>(directedRouter.WeightMatrix.BuildRoute(directedRouter.Tour));
        }

        /// <summary>
        /// Calculates a directed sequence of the given sequence of locations.
        /// </summary>
        public static Route CalculateDirected(this RouterBase router, Profile profile, List<RouterPoint> locations, float turnPenaltyInSeconds, Tour sequence)
        {
            return router.TryCalculateDirected(profile, locations, turnPenaltyInSeconds, sequence).Value;
        }

        /// <summary>
        /// Calculates a directed sequence of the given sequence of locations.
        /// </summary>
        public static Result<Route> TryCalculateDirected(this RouterBase router, Profile profile, List<RouterPoint> locations, float turnPenaltyInSeconds, Tour sequence)
        {
            var directedRouter = new SequenceDirectedRouter(new DirectedWeightMatrixAlgorithm(router, profile, locations), turnPenaltyInSeconds, sequence);
            directedRouter.Run();
            if (!directedRouter.HasSucceeded)
            {
                return new Result<Route>(directedRouter.ErrorMessage);
            }
            return new Result<Route>(directedRouter.WeightMatrix.BuildRoute(directedRouter.Tour));
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSP(this RouterBase router, Profile profile, Coordinate[] locations, int first = 0, int? last = null)
        {
            return router.TryCalculateTSP(profile, locations, null, first, last);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSP(this RouterBase router, Profile profile, List<RouterPoint> locations, int first = 0, int? last = null)
        {
            return router.TryCalculateTSP(profile, null, locations, first, last);
        }

        static Result<Route> TryCalculateTSP(this RouterBase router, Profile profile, Coordinate[] visits, List<RouterPoint> locations, int first, int? last)
        {
            // build model.
            var model = new Model()
            {
                Visits = visits,
                Locations = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[]
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = first,
                            Arrival = last
                        }
                    },
                    Reusable = false
                }
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<Route>(routes.Value[0]);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Route CalculateTSP(this RouterBase router, Profile profile, Coordinate[] locations, int first = 0, int? last = null)
        {
            return router.TryCalculateTSP(profile, locations, first, last).Value;
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Route CalculateTSP(this RouterBase router, Profile profile, List<RouterPoint> locations, int first = 0, int? last = null)
        {
            return router.TryCalculateTSP(profile, locations, first, last).Value;
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPDirected(profile, locations, null, turnPenaltyInSeconds, first, last);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSPDirected(this Router router, Profile profile, List<RouterPoint> locations, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPDirected(profile, null, locations, turnPenaltyInSeconds, first, last);
        }

        static Result<Route> TryCalculateTSPDirected(this Router router, Profile profile, Coordinate[] visits, List<RouterPoint> locations, float turnPenaltyInSeconds, int first, int? last)
        {
            // build model.
            var model = new Model()
            {
                Visits = visits,
                Locations = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[] 
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = first,
                            Arrival = last,
                            TurnPentalty = turnPenaltyInSeconds
                        }
                    },
                    Reusable = false
                }
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<Route>(routes.Value[0]);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Route CalculateTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPDirected(profile, locations, turnPenaltyInSeconds, first, last).Value;
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSPTW(this RouterBase router, Profile profile, Coordinate[] locations, TimeWindow[] windows, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPTW(profile, locations, null, windows, first, last);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSPTW(this RouterBase router, Profile profile, List<RouterPoint> locations, TimeWindow[] windows, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPTW(profile, null, locations, windows, first, last);
        }

        static Result<Route> TryCalculateTSPTW(this RouterBase router, Profile profile, Coordinate[] visits, List<RouterPoint> locations, TimeWindow[] windows, int first, int? last)
        {
            // build model.
            var model = new Model()
            {
                Visits = visits,
                Locations = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[] 
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = first,
                            Arrival = last
                        }
                    },
                    Reusable = false
                },
                TimeWindows = windows
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<Route>(routes.Value[0]);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Route CalculateTSPTW(this RouterBase router, Profile profile, Coordinate[] locations, TimeWindow[] windows, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPTW(profile, locations, windows, first, last).Value;
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSPTWDirected(this Router router, Profile profile, Coordinate[] locations, TimeWindow[] windows, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPTWDirected(profile, locations, null, windows, turnPenaltyInSeconds, first, last);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSPTWDirected(this Router router, Profile profile, List<RouterPoint> locations, TimeWindow[] windows, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPTWDirected(profile, null, locations, windows, turnPenaltyInSeconds, first, last);
        }

        static Result<Route> TryCalculateTSPTWDirected(this Router router, Profile profile, Coordinate[] visits, List<RouterPoint> locations, TimeWindow[] windows, float turnPenaltyInSeconds, int first, int? last)
        {
            // build model.
            var model = new Model()
            {
                Visits = visits,
                Locations = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[] 
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = first,
                            Arrival = last,
                            TurnPentalty = turnPenaltyInSeconds
                        }
                    },
                    Reusable = false
                },
                TimeWindows = windows
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<Route>(routes.Value[0]);
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Route CalculateTSPTWDirected(this Router router, Profile profile, Coordinate[] locations, TimeWindow[] windows, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPTWDirected(profile, locations, windows, turnPenaltyInSeconds, first, last).Value;
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Result<Route> TryCalculateSTSP(this RouterBase router, Profile profile, Coordinate[] locations, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSP(profile, locations, null, max, first, last);
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Result<Route> TryCalculateSTSP(this RouterBase router, Profile profile, List<RouterPoint> locations, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSP(profile, null, locations, max, first, last);
        }

        static Result<Route> TryCalculateSTSP(this RouterBase router, Profile profile, Coordinate[] visits, List<RouterPoint> locations, float max, int first, int? last)
        {
            // build model.
            var model = new Model()
            {
                Visits = visits,
                Locations = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[] 
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = first,
                            Arrival = last,
                            CapacityConstraints = new Models.Vehicles.Constraints.CapacityConstraint[] 
                            {
                                new Models.Vehicles.Constraints.CapacityConstraint()
                                {
                                    Name = profile.Metric.ToModelMetric(),
                                    Capacity = max
                                }
                            }
                        }
                    },
                    Reusable = false
                }
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<Route>(routes.Value[0]);
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Route CalculateSTSP(this RouterBase router, Profile profile, Coordinate[] locations, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSP(profile, locations, max, first, last).Value;
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Route CalculateSTSP(this RouterBase router, Profile profile, List<RouterPoint> locations, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSP(profile, locations, max, first, last).Value;
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Result<Route> TryCalculateSTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSPDirected(profile, locations, null, turnPenaltyInSeconds, max, first, last);
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Result<Route> TryCalculateSTSPDirected(this Router router, Profile profile, List<RouterPoint> locations, float turnPenaltyInSeconds, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSPDirected(profile, null, locations, turnPenaltyInSeconds, max, first, last);
        }

        static Result<Route> TryCalculateSTSPDirected(this Router router, Profile profile, Coordinate[] visits, List<RouterPoint> locations, float turnPenaltyInSeconds, float max, int first, int? last)
        {
            // build model.
            var model = new Model()
            {
                Visits = visits,
                Locations = locations,
                VehiclePool = new Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Models.Vehicles.Vehicle[] 
                    {
                        new Models.Vehicles.Vehicle()
                        {
                            Profile = profile.FullName,
                            Departure = first,
                            Arrival = last,
                            CapacityConstraints = new Models.Vehicles.Constraints.CapacityConstraint[] 
                            {
                                new Models.Vehicles.Constraints.CapacityConstraint()
                                {
                                    Name = profile.Metric.ToModelMetric(),
                                    Capacity = max
                                }
                            },
                            TurnPentalty = turnPenaltyInSeconds
                        }
                    },
                    Reusable = false
                }
            };

            // solve model.
            var routes = router.Solve(model);

            return new Result<Route>(routes.Value[0]);
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Route CalculateSTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSPDirected(profile, locations, turnPenaltyInSeconds, max, first, last).Value;
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Route CalculateSTSPDirected(this Router router, Profile profile, List<RouterPoint> locations, float turnPenaltyInSeconds, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSPDirected(profile, locations, turnPenaltyInSeconds, max, first, last).Value;
        }

        /// <summary>
        /// Solvers the routing problem represented by the given model.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="model">The model to solve.</param>
        /// <param name="intermediateResult">Callback for intermediate results if any.</param>
        /// <returns></returns>
        public static Result<Route[]> Solve(this RouterBase router, Models.Model model, Action<Route[]> intermediateResult = null)
        {
            // map the model.
            var defaultModelMap = new MappedModel(model, router);

            // handle intermediate results if requested.
            Action<IList<ITour>> intermediateResultRaw = null;
            if (intermediateResult != null)
            {
                intermediateResultRaw = (intermediateTours) =>
                {
                    var intermediateRoutes = new Route[intermediateTours.Count];
                    for (var t = 0; t < intermediateTours.Count; t++)
                    {
                        intermediateRoutes[t] = defaultModelMap.BuildRoute(intermediateTours[t]);
                    }

                    intermediateResult(intermediateRoutes);
                };
            }

            // solve the abstract model.
            var tours = Abstract.Solvers.SolverRegistry.Solve(defaultModelMap, intermediateResultRaw);

            // use the map to convert to real-world routes.
            var routes = new Route[tours.Count];
            for (var t = 0; t < tours.Count; t++)
            {
                routes[t] = defaultModelMap.BuildRoute(tours[t]);
            }
            return new Result<Route[]>(routes);
        }
    }
}