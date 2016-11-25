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

using Itinero.Algorithms.Weights;
using Itinero.Graphs;
using Itinero.LocalGeo;
using Itinero.Profiles;
using System;

namespace Itinero.Logistics.Routing.Loops
{
    /// <summary>
    /// A loop generator that generates attractive loops around a given location for a given profile with a maxium weight.
    /// </summary>
    public class LoopGenerator : Algorithm
    {
        private readonly Router _router;
        private readonly Coordinate _location;
        private readonly Profile _profile;
        private readonly Weight _max;

        /// <summary>
        /// Creates a new loop generator.
        /// </summary>
        public LoopGenerator(Router router, Coordinate location, Profile profile, Weight max)
        {
            _router = router;
            _location = location;
            _profile = profile;
            _max = max;

            _vertexScoring = _profile.GetDefaultProfileBasedScoring(router.Db);
        }

        private Func<Graph, uint, float> _vertexScoring = null;
        private Route _route;
        
        /// <summary>
        /// Gets or sets the vertex scoring function.
        /// </summary>
        public Func<Graph, uint, float> VertexScoring
        {
            get
            {
                return _vertexScoring;
            }
            set
            {
                if (this.HasRun)
                {
                    throw new InvalidOperationException("Cannot set vertex scoring function after the algorithm has run.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("Vertex scoring function cannot be null.");
                }
                _vertexScoring = value;
            }
        }

        /// <summary>
        /// Executes the actual run.
        /// </summary>
        protected override void DoRun()
        {
            // generate vertices.
            var distance = _max.Distance;
            if (_max.Distance == float.MaxValue)
            {
                distance = 5 * _max.Time;
            }


            // TODO: calculate this box based on max once we can define this as distance not time.
            var box = new Box(_location.OffsetWithDirection(distance, Navigation.Directions.DirectionEnum.NorthEast),
                 _location.OffsetWithDirection(distance, Navigation.Directions.DirectionEnum.SouthWest));
            var attractiveVertexSearch = new AttractiveVertexSearch(_router.Db.Network.GeometricGraph, box,
                this.VertexScoring);
            attractiveVertexSearch.Run();
            if (!attractiveVertexSearch.HasSucceeded)
            {
                this.ErrorMessage = "Attractive vertex search failed: " + attractiveVertexSearch.ErrorMessage;
                this.HasSucceeded = false;
                return;
            }

            // resolve source location.
            var source = _router.TryResolve(_profile, _location, 500);
            if (source.IsError)
            {
                this.ErrorMessage = "Source cannot be resolved: " + source.ErrorMessage;
                this.HasSucceeded = false;
                return;
            }

            // calculate a 'selective' TSP route.
            var stspRouter = new STSP.STSPRouter(_router, _profile, source.Value, attractiveVertexSearch.Vertices, _max);
            stspRouter.Run();
            if (!stspRouter.HasSucceeded)
            {
                this.ErrorMessage = "STSP calculation failed: " + stspRouter.ErrorMessage;
                this.HasSucceeded = false;
                return;
            }
            _route = stspRouter.BuildRoute();

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the resulting route.
        /// </summary>
        public Route Route
        {
            get
            {
                this.CheckHasRunAndHasSucceeded();
                return _route;
            }
        }
    }
}
