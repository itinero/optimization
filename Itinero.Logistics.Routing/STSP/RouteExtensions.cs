// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Algorithms.Weights;
using Itinero.LocalGeo;
using Itinero.Logistics.Routing.Weights;
using Itinero.Profiles;
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Routing.STSP
{
    /// <summary>
    /// Holds extension methods for the router class related to the STSP.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Tries to calculate a Selective-TSP solution for the given locations and maximum time/distance.
        /// </summary>
        public static Result<Route> TryCalculateSTSP(this Router router, Profile profile, Coordinate source, Coordinate[] locations, ProfileMetric maxMetric, float max)
        {
            var resolved = router.TryResolve(profile, locations);
            var resolvedValid = new List<RouterPoint>(resolved.AllValid());
            if (resolvedValid.Count != locations.Length)
            {
                return new Result<Route>("Not all given locations could be properly resolved.");
            }

            var sourceResolved = router.TryResolve(profile, source);
            if (sourceResolved.IsError)
            {
                return sourceResolved.ConvertError<Route>();
            }

            return router.TryCalculateSTSP(profile, sourceResolved.Value, resolvedValid, maxMetric, max);
        }

        /// <summary>
        /// Tries to calculate a Selective-TSP solution for the given locations and maximum time/distance.
        /// </summary>
        public static Result<Route> TryCalculateSTSP(this Router router, Profile profile, RouterPoint source, IEnumerable<RouterPoint> locations, ProfileMetric maxMetric, float max)
        {
            try
            {
                var stspRouter = new STSPRouter(router, profile, source, new HashSet<RouterPoint>(locations), (new Weight()).SetWithMetric(maxMetric, max));
                stspRouter.Run();

                if (!stspRouter.HasSucceeded)
                {
                    return new Result<Route>("Could not solve STSP: " + stspRouter.ErrorMessage);
                }
                return new Result<Route>(stspRouter.BuildRoute());
            }
            catch (Exception ex)
            {
                return new Result<Route>("Could not generate loop: " + ex.ToInvariantString());
            }
        }

        /// <summary>
        /// Calculates a Selective-TSP solution for the given locations and maximum time/distance.
        /// </summary>
        public static Route CalculateSTSP(this Router router, Profile profile, RouterPoint source, IEnumerable<RouterPoint> locations, ProfileMetric maxMetric, float max)
        {
            return router.TryCalculateSTSP(profile, source, locations, maxMetric, max).Value;
        }

        /// <summary>
        /// Calculates a Selective-TSP solution for the given locations and maximum time/distance.
        /// </summary>
        public static Route CalculateSTSP(this Router router, Profile profile, Coordinate source, Coordinate[] locations, ProfileMetric maxMetric, float max)
        {
            return router.TryCalculateSTSP(profile, source, locations, maxMetric, max).Value;
        }
    }
}