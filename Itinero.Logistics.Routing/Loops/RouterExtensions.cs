// Itinero.Logistics - Route optimization for .NET
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
using System;

namespace Itinero.Logistics.Routing.Loops
{
    /// <summary>
    /// Contains extension methods for the router and the loop generator.
    /// </summary>
    public static class RouterExtensions
    {
        /// <summary>
        /// Tries to generates a loop with a maximum weight around the given location.
        /// </summary>
        public static Result<Route> TryGenerateLoop(this Router router, Profile profile, Coordinate location, float max)
        {
            try
            {
                var loopGenerator = new LoopGenerator(router, location, profile, max);
                loopGenerator.Run();
                if (!loopGenerator.HasSucceeded)
                {
                    return new Result<Route>("Could not generate loop: " + loopGenerator.ErrorMessage);
                }
                return new Result<Route>(loopGenerator.Route);
            }
            catch (Exception ex)
            {
                return new Result<Route>("Could not generate loop: " + ex.ToInvariantString());
            }
        }

        /// <summary>
        /// Generates a loop with a maximum weight around the given location.
        /// </summary>
        public static Route GenerateLoop(this Router router, Profile profile, Coordinate location, float max)
        {
            return router.TryGenerateLoop(profile, location, max).Value;
        }
    }
}