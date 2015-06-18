// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.Routers;
using OsmSharp.Routing.Vehicles;

namespace OsmSharp.Logistics.Tests.Routing
{
    public class RouterMock : ITypedRouter
    {
        private long _resolvedId = 0;

        public Route Calculate(Vehicle vehicle, RouterPoint source, RouterPoint target, 
            float max = 3.40282e+038f, bool geometryOnly = false)
        {
            var route = new Route();
            route.Segments = new RouteSegment[2];
            route.Segments[0] = new RouteSegment()
            {
                Latitude = (float)source.Location.Latitude,
                Longitude = (float)source.Location.Longitude,
                Type = RouteSegmentType.Start,
                Vehicle = vehicle.UniqueName
            };
            route.Segments[1] = new RouteSegment()
            {
                Latitude = (float)target.Location.Latitude,
                Longitude = (float)target.Location.Longitude,
                Type = RouteSegmentType.Stop,
                Vehicle = vehicle.UniqueName
            };
            route.Vehicle = vehicle.UniqueName;
            return route;
        }

        public Route[][] CalculateManyToMany(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets, float max = 3.40282e+038f, bool geometryOnly = false)
        {
            throw new System.NotImplementedException();
        }

        public double[][] CalculateManyToManyWeight(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets)
        {
            var weights = new double[sources.Length][];
            for(var s = 0; s < sources.Length; s++)
            {
                weights[s] = new double[targets.Length];
                for (var t = 0; t < sources.Length; t++)
                {
                    weights[s][t] = sources[s].Location.DistanceReal(targets[t].Location).Value;
                }
            }
            return weights;
        }

        public Route[] CalculateOneToMany(Vehicle vehicle, RouterPoint source, RouterPoint[] targets, float max = 3.40282e+038f, bool geometryOnly = false)
        {
            throw new System.NotImplementedException();
        }

        public double[] CalculateOneToManyWeight(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.HashSet<Math.Geo.GeoCoordinate> CalculateRange(Vehicle vehicle, RouterPoint orgine, float weight)
        {
            throw new System.NotImplementedException();
        }

        public Route CalculateToClosest(Vehicle vehicle, RouterPoint source, RouterPoint[] targets, float max = 3.40282e+038f, bool geometryOnly = false)
        {
            throw new System.NotImplementedException();
        }

        public double CalculateWeight(Vehicle vehicle, RouterPoint source, RouterPoint target)
        {
            throw new System.NotImplementedException();
        }

        public bool[] CheckConnectivity(Vehicle vehicle, RouterPoint[] point, float weight)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckConnectivity(Vehicle vehicle, RouterPoint point, float weight)
        {
            throw new System.NotImplementedException();
        }

        public bool IsCalculateRangeSupported
        {
            get { throw new System.NotImplementedException(); }
        }

        public RouterPoint[] Resolve(Vehicle vehicle, float delta, Math.Geo.GeoCoordinate[] coordinates, IEdgeMatcher matcher, Collections.Tags.TagsCollectionBase[] matchingTags)
        {
            throw new System.NotImplementedException();
        }

        public RouterPoint[] Resolve(Vehicle vehicle, Math.Geo.GeoCoordinate[] coordinates, IEdgeMatcher matcher, Collections.Tags.TagsCollectionBase[] matchingTags)
        {
            throw new System.NotImplementedException();
        }

        public RouterPoint[] Resolve(Vehicle vehicle, float delta, Math.Geo.GeoCoordinate[] coordinate)
        {
            throw new System.NotImplementedException();
        }

        public RouterPoint[] Resolve(Vehicle vehicle, GeoCoordinate[] coordinates)
        {
            var result = new RouterPoint[coordinates.Length];
            for (var idx = 0; idx < coordinates.Length; idx++)
            {
                result[idx] = this.Resolve(vehicle, 0, coordinates[idx]);
            }
            return result;
        }

        public RouterPoint Resolve(Vehicle vehicle, float delta, Math.Geo.GeoCoordinate coordinate, IEdgeMatcher matcher, Collections.Tags.TagsCollectionBase matchingTags)
        {
            throw new System.NotImplementedException();
        }

        public RouterPoint Resolve(Vehicle vehicle, Math.Geo.GeoCoordinate coordinate, IEdgeMatcher matcher, Collections.Tags.TagsCollectionBase matchingTags)
        {
            throw new System.NotImplementedException();
        }

        public RouterPoint Resolve(Vehicle vehicle, float delta, Math.Geo.GeoCoordinate coordinate)
        {
            _resolvedId++;
            return new RouterPoint(_resolvedId, vehicle, coordinate);
        }

        public RouterPoint Resolve(Vehicle vehicle, Math.Geo.GeoCoordinate coordinate, bool verticesOnly)
        {
            throw new System.NotImplementedException();
        }

        public Math.Geo.GeoCoordinate Search(Vehicle vehicle, float delta, Math.Geo.GeoCoordinate coordinate)
        {
            throw new System.NotImplementedException();
        }

        public Math.Geo.GeoCoordinate Search(Vehicle vehicle, Math.Geo.GeoCoordinate coordinate)
        {
            throw new System.NotImplementedException();
        }

        public bool SupportsVehicle(Vehicle vehicle)
        {
            throw new System.NotImplementedException();
        }
    }
}