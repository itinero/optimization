// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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

using Itinero.LocalGeo;
using Itinero.Profiles;

namespace Itinero.Routing.Optimization.TSP
{
    /// <summary>
    /// Contains extension methods to conveniently expose the TSP solvers.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Result<Route> TryCalculateTSP(this RouterBase router, Profile profile, Coordinate[] locations, int first = 0, int? last = null)
        {
            var tspRouter = new TSPRouter(router, profile, locations, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.BuildRoute());
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
            var tspRouter = new Directed.TSPRouter(router, profile, router.Resolve(profile, locations), turnPenaltyInSeconds, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.BuildRoute());
        }

        /// <summary>
        /// Calculates TSP.
        /// </summary>
        public static Route CalculateTSPDirected(this Router router, Profile profile, Coordinate[] locations, float turnPenaltyInSeconds, int first = 0, int? last = null)
        {
            return router.TryCalculateTSPDirected(profile, locations, turnPenaltyInSeconds, first, last).Value;
        }
    }
}
