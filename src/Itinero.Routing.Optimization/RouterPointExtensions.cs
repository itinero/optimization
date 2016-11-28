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

using Itinero.Algorithms;
using Itinero.Algorithms.Weights;
using Itinero.Data.Edges;
using Itinero.Graphs.Geometric;
using Itinero.Profiles;

namespace Itinero.Routing.Optimization
{
    /// <summary>
    /// Contains extension methods related to router points.
    /// </summary>
    public static class RouterPointExtensions
    {
        /// <summary>
        /// Converts the router point to paths leading to the closest 2 vertices.
        /// </summary>
        internal static EdgePath<T>[] ToEdgePathsAdvanced<T>(this RouterPoint point, RouterDb routerDb, WeightHandler<T> weightHandler, bool asSource)
            where T : struct
        {
            var graph = routerDb.Network.GeometricGraph;
            var edge = graph.GetEdge(point.EdgeId);

            float distance;
            ushort profileId;
            EdgeDataSerializer.Deserialize(edge.Data[0], out distance, out profileId);
            Factor factor;
            var edgeWeight = weightHandler.Calculate(profileId, distance, out factor);

            var offset = point.Offset / (float)ushort.MaxValue;
            if (factor.Direction == 0)
            { // bidirectional.
                if (offset == 0)
                { // the first part is just the first vertex.
                    return new EdgePath<T>[] {
                    new EdgePath<T>(edge.From, weightHandler.Zero, -edge.IdDirected(), new EdgePath<T>(edge.To)),
                        new EdgePath<T>(edge.To, weightHandler.Calculate(profileId, distance * (1 - offset)), edge.IdDirected(), new EdgePath<T>(edge.From))
                    };
                }
                else if (offset == 1)
                { // the second path it just the second vertex.
                    return new EdgePath<T>[] {
                        new EdgePath<T>(edge.From, weightHandler.Calculate(profileId, distance * offset), -edge.IdDirected(), new EdgePath<T>(edge.To)),
                    new EdgePath<T>(edge.To, weightHandler.Zero, edge.IdDirected(), new EdgePath<T>(edge.From))
                    };
                }
                return new EdgePath<T>[] {
                    new EdgePath<T>(edge.From, weightHandler.Calculate(profileId, distance * offset), -edge.IdDirected(), new EdgePath<T>(edge.To)),
                    new EdgePath<T>(edge.To, weightHandler.Calculate(profileId, distance * (1 - offset)), edge.IdDirected(), new EdgePath<T>(edge.From))
                };
            }
            else if (factor.Direction == 1)
            { // edge is forward oneway.
                if (asSource)
                {
                    if (offset == 1)
                    { // just return the to-vertex.
                        return new EdgePath<T>[] {
                            new EdgePath<T>(edge.To, weightHandler.Zero, edge.IdDirected(), new EdgePath<T>(edge.From))
                        };
                    }
                    return new EdgePath<T>[] {
                        new EdgePath<T>(edge.To, weightHandler.Calculate(profileId, distance * (1 - offset)), edge.IdDirected(), new EdgePath<T>(edge.From))
                    };
                }
                if (offset == 0)
                { // just return the from vertex.
                    return new EdgePath<T>[] {
                        new EdgePath<T>(edge.From, weightHandler.Zero, -edge.IdDirected(), new EdgePath<T>(edge.To))
                    };
                }
                return new EdgePath<T>[] {
                        new EdgePath<T>(edge.From, weightHandler.Calculate(profileId, distance * offset), -edge.IdDirected(), new EdgePath<T>(edge.To))
                    };
            }
            else
            { // edge is backward oneway.
                if (!asSource)
                {
                    if (offset == 1)
                    { // just return the to-vertex.
                        return new EdgePath<T>[] {
                            new EdgePath<T>(edge.To, weightHandler.Zero, edge.IdDirected(), new EdgePath<T>(edge.From))
                        };
                    }
                    return new EdgePath<T>[] {
                        new EdgePath<T>(edge.To, weightHandler.Calculate(profileId, distance * (1 - offset)), edge.IdDirected(), new EdgePath<T>(edge.From))
                    };
                }
                if (offset == 0)
                { // just return the from-vertex.
                    return new EdgePath<T>[] {
                        new EdgePath<T>(edge.From, weightHandler.Zero, new EdgePath<T>(edge.To))
                    };
                }
                return new EdgePath<T>[] {
                        new EdgePath<T>(edge.From, weightHandler.Calculate(profileId, distance * offset), -edge.IdDirected(), new EdgePath<T>(edge.To))
                    };
            }
        }
    }
}