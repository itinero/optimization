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

namespace Itinero.Routing.Optimization.STSP
{
    /// <summary>
    /// Contains extension methods to conveniently expose the STSP solvers.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Result<Route> TryCalculateSTSP(this RouterBase router, Profile profile, Coordinate[] locations, float max, int first = 0, int? last = null)
        {
            var tspRouter = new STSPRouter(router, profile, locations, max, first, last);
            tspRouter.Run();
            if (!tspRouter.HasSucceeded)
            {
                return new Result<Route>(tspRouter.ErrorMessage);
            }
            return new Result<Route>(tspRouter.BuildRoute());
        }

        /// <summary>
        /// Calculates STSP.
        /// </summary>
        public static Route CalculateSTSP(this RouterBase router, Profile profile, Coordinate[] locations, float max, int first = 0, int? last = null)
        {
            return router.TryCalculateSTSP(profile, locations, max, first, last).Value;
        }
    }
}