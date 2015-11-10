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

using OsmSharp.Logistics.Routing;
using System;
using System.Linq;
using System.Collections.Generic;
using OsmSharp.Routing.Algorithms;

namespace OsmSharp.Logistics.Tests.Routing
{
    /// <summary>
    /// A mock of the weight matrix algorithm.
    /// </summary>
    class WeightMatrixAlgorithmMock : IWeightMatrixAlgorithm
    {
        private readonly List<Tuple<int, int>> _locationsAndRouterPoints;

        public WeightMatrixAlgorithmMock()
        {
            _locationsAndRouterPoints = new List<Tuple<int, int>>();
        }

        public WeightMatrixAlgorithmMock(IEnumerable<Tuple<int, int>> locationsAndRouterPoints)
        {
            _locationsAndRouterPoints = new List<Tuple<int, int>>(locationsAndRouterPoints);
        }

        public System.Collections.Generic.Dictionary<int, LocationError> Errors
        {
            get;
            set;
        }

        public int IndexOf(int locationIdx)
        {
            return _locationsAndRouterPoints.First((x) => x.Item1 == locationIdx).Item2;
        }

        public int LocationIndexOf(int routerPointIdx)
        {
            return _locationsAndRouterPoints.First((x) => x.Item2 == routerPointIdx).Item1;
        }

        public System.Collections.Generic.List<OsmSharp.Routing.RouterPoint> RouterPoints
        {
            get;
            set;
        }

        public float[][] Weights
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public bool HasRun
        {
            get;
            set;
        }

        public bool HasSucceeded
        {
            get;
            set;
        }

        public void Run()
        {
            this.HasRun = true;
        }
    }
}