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
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Routing
{
    /// <summary>
    /// An algorithm to calculate a weight-matrix for a set of locations.
    /// </summary>
    public class WeightMatrixAlgorithm : Algorithm
    {
        private readonly ITypedRouter _router;
        private readonly Vehicle _vehicle;
        private readonly GeoCoordinate[] _locations;

        /// <summary>
        /// Creates a new weight-matrix algorithm.
        /// </summary>
        public WeightMatrixAlgorithm(ITypedRouter router, Vehicle vehicle, GeoCoordinate[] locations)
        {
            _router = router;
            _vehicle = vehicle;
            _locations = locations;
        }

        private Dictionary<int, string> _errors; // all errors per original location idx.
        private List<int> _resolvedPointsIndices; // the original location per resolved point index.
        private List<RouterPoint> _resolvedPoints; // only the valid resolved points.
        private double[][] _weights; // the weights between all valid resolved points.

        /// <summary>
        /// Executes the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            _errors = new Dictionary<int,string>(_locations.Length);
            _resolvedPoints = new List<RouterPoint>(_locations.Length);
            _resolvedPointsIndices = new List<int>(_locations.Length);

            // resolve all locations.
            var resolvedPoints = _router.Resolve(_vehicle, _locations);

            // remove all points that could not be resolved.
            for(var i = 0; i < resolvedPoints.Length; i++)
            {
                if(resolvedPoints[i] == null)
                { // could not be resolved!
                    _errors[i] = "Location could not be linked to the road network.";
                }
                else
                { // resolve is ok.
                    _resolvedPointsIndices.Add(i);
                    _resolvedPoints.Add(resolvedPoints[i]);
                }
            }

            // calculate matrix.
            var nonNullResolvedArray = _resolvedPoints.ToArray();
            var nonNullInvalids = new HashSet<int>();
            _weights = _router.CalculateManyToManyWeight(_vehicle, nonNullResolvedArray, nonNullResolvedArray, nonNullInvalids);

            // take into account the non-null invalids now.
            if(nonNullInvalids.Count> 0)
            { // shrink lists and add errors.
                foreach(var invalid in nonNullInvalids)
                {
                    _errors[_resolvedPointsIndices[invalid]] = "Location could not routed to or from.";
                }

                _resolvedPoints = _resolvedPoints.ShrinkAndCopyList(nonNullInvalids);
                _resolvedPointsIndices = _resolvedPointsIndices.ShrinkAndCopyList(nonNullInvalids);
                _weights = _weights.SchrinkAndCopyMatrix(nonNullInvalids);
            }
            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the valid router points.
        /// </summary>
        public List<RouterPoint> RouterPoints
        {
            get
            {
                this.CheckHasRunAndHasSucceeded();

                return _resolvedPoints;
            }
        }

        /// <summary>
        /// Gets the weights between all valid router points.
        /// </summary>
        public double[][] Weights
        {
            get
            {
                this.CheckHasRunAndHasSucceeded();

                return _weights;
            }
        }

        /// <summary>
        /// Returns the index of the location in the resolved points list.
        /// </summary>
        /// <param name="locationIdx"></param>
        /// <returns></returns>
        public int IndexOf(int locationIdx)
        {
            this.CheckHasRunAndHasSucceeded();

            return _resolvedPointsIndices.IndexOf(locationIdx);
        }

        /// <summary>
        /// Returns the index of the router point in the original locations array.
        /// </summary>
        /// <param name="routerPointIdx"></param>
        /// <returns></returns>
        public int LocationIndexOf(int routerPointIdx)
        {
            this.CheckHasRunAndHasSucceeded();

            return _resolvedPointsIndices[routerPointIdx];
        }

        /// <summary>
        /// Returns the errors indexed per location idx.
        /// </summary>
        public Dictionary<int, string> Errors
        {
            get
            {
                this.CheckHasRunAndHasSucceeded();

                return _errors;
            }
        }
    }
}