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

using Itinero.Data.Edges;
using Itinero.Graphs;
using Itinero.Profiles;
using System;

namespace Itinero.Logistics.Routing.Loops
{
    /// <summary>
    /// Contains default implementations for the vertex scoring function.
    /// </summary>
    public static class VertexScoring
    {
        /// <summary>
        /// Generates a vertex scoring function for the given profile.
        /// </summary>
        public static Func<Graph, uint, float> GetDefaultProfileBasedScoring(this Profile p, RouterDb db)
        {
            return (g, v) =>
            {
                var enumerator = g.GetEdgeEnumerator();
                var cost = 0f;
                if (enumerator.MoveTo(v))
                {
                    while (enumerator.MoveNext())
                    {
                        float distance;
                        ushort edgeProfile;
                        EdgeDataSerializer.Deserialize(enumerator.Data0, out distance, out edgeProfile);
                        var profileTags = db.EdgeProfiles.Get(edgeProfile);
                        var factor = p.Factor(profileTags);
                        cost += (1 / factor.Value);
                    }
                }
                return cost;
            };
        }
    }
}