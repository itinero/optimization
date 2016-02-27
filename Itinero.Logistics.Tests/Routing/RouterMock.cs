// Itinero - OpenStreetMap (OSM) SDK
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

using System.Collections.Generic;
using Itinero.Network;
using Itinero.Profiles;
using System;
using Itinero.Attributes;
using Itinero.LocalGeo;

namespace Itinero.Logistics.Tests.Routing
{
    class RouterMock : IRouter
    {
        private long _resolvedId = 0;
        private HashSet<int> _invalidSet = new HashSet<int>();
        private AttributeCollection _matchingTags;

        public RouterMock()
        {

        }

        public RouterMock(HashSet<int> invalidSet)
        {
            _invalidSet = invalidSet;
        }

        public RouterMock(AttributeCollection matchingTags)
        {
            _matchingTags = matchingTags;
        }

        public Result<Route[][]> TryCalculate(Itinero.Profiles.Profile profile, RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets)
        {
            throw new System.NotImplementedException();
        }

        public Result<Route> TryCalculate(Itinero.Profiles.Profile profile, RouterPoint source, RouterPoint target)
        {
            var route = new Route();
            route.Shape = new Coordinate[]
            {
                source.Location(),
                target.Location()
            };
            route.ShapeMeta = new Route.Meta[]
            {
                new Route.Meta()
                {
                    Shape = 0,
                    Profile = profile.Name
                },
                new Route.Meta()
                {
                    Shape = 1,
                    Profile = profile.Name
                }
            };
            return new Result<Route>(route);
        }

        public Result<float[][]> TryCalculateWeight(Itinero.Profiles.Profile profile, 
            RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets)
        {
            var weights = new float[sources.Length][];
            for (var s = 0; s < sources.Length; s++)
            {
                weights[s] = new float[targets.Length];
                for (var t = 0; t < sources.Length; t++)
                {
                    weights[s][t] = Coordinate.DistanceEstimateInMeter(
                        new Coordinate(sources[s].Latitude, sources[s].Longitude),
                        new Coordinate(targets[t].Latitude, targets[t].Longitude));
                }
            }

            foreach (var invalid in _invalidSet)
            {
                invalidSources.Add(invalid);
                invalidTargets.Add(invalid);
            }

            return new Result<float[][]>(weights);
        }

        public Result<float> TryCalculateWeight(Itinero.Profiles.Profile profile, RouterPoint source, RouterPoint target)
        {
            throw new System.NotImplementedException();
        }

        public Result<bool> TryCheckConnectivity(Itinero.Profiles.Profile profile, RouterPoint point, float radiusInMeters)
        {
            throw new System.NotImplementedException();
        }
        
        public bool SupportsAll(params Profile[] profiles)
        {
            throw new NotImplementedException();
        }

        public Result<RouterPoint> TryResolve(Profile[] profiles, float latitude, float longitude, Func<RoutingEdge, bool> isBetter, float searchDistanceInMeter = 50)
        {
            if (latitude < -90 || latitude > 90 ||
                longitude < -180 || longitude > 180)
            {
                return null;
            }
            _resolvedId++;
            return new Result<RouterPoint>(new RouterPoint(latitude, longitude, 0, 0));
        }
    }
}