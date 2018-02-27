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

using System.Collections.Generic;
using Itinero.Algorithms.Matrices;
using Itinero.LocalGeo;
using Itinero.Optimization.Routing;
using Itinero.Optimization.Sequence.Directed;
using Itinero.Optimization.Solutions.STSP;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Solutions.TSP;
using Itinero.Profiles;

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
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSP(this RouterBase router, Profile profile, Coordinate[] locations, int first = 0, int? last = null)
        {
            var tspRouter = new TSPRouter(new WeightMatrixAlgorithm(router, profile, locations), first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.WeightMatrix.BuildRoute(tspRouter.Tour));
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
        public static Result<Route> TryCalculateTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            var tspRouter = new Solutions.TSP.Directed.TSPRouter(new DirectedWeightMatrixAlgorithm(router, profile, locations), turnPenaltyInSeconds, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.WeightMatrix.BuildRoute(tspRouter.Tour));
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
            var tspRouter = new Solutions.TSP.TimeWindows.TSPTWRouter(new WeightMatrixAlgorithm(router, profile, locations), windows, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.WeightMatrix.BuildRoute(tspRouter.Tour));
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
            var tspRouter = new Solutions.TSP.TimeWindows.Directed.TSPTWRouter(new DirectedWeightMatrixAlgorithm(router, profile, locations), windows, turnPenaltyInSeconds, first, last, null);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.WeightMatrix.BuildRoute(tspRouter.Tour));
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
            var tspRouter = new STSPRouter(new WeightMatrixAlgorithm(router, profile, locations), max, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.WeightMatrix.BuildRoute(tspRouter.Tour));
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
        public static Result<Route> TryCalculateSTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, float max, int first = 0, int? last = null)
        {
            var tspRouter = new Itinero.Optimization.Solutions.STSP.Directed.STSPRouter(new DirectedWeightMatrixAlgorithm(router, profile, locations), turnPenaltyInSeconds, max, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.WeightMatrix.BuildRoute(tspRouter.Tour));
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Route CalculateSTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSPDirected(profile, locations, turnPenaltyInSeconds, max, first, last).Value;
        }
    }
}