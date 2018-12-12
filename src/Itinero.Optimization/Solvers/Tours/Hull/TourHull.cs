/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;
using Itinero.LocalGeo;

namespace Itinero.Optimization.Solvers.Tours.Hull
{
    /// <summary>
    /// A convex hull around a tour.
    /// </summary>
    public class TourHull
    {
        private readonly List<(Coordinate location, int visit)> _locations =
            new List<(Coordinate location, int visit)>();

        private float? _surface;
        private Box? _box;

        internal TourHull()
        {
            
        }

        private TourHull(IEnumerable<(Coordinate location, int visit)> locations)
        {
            _locations = new List<(Coordinate location, int visit)>(locations);
        }

        /// <summary>
        /// Updates this hull with a new visit/location.
        /// </summary>
        /// <param name="v">The visit/location.</param>
        /// <returns>True if the hull was updated.</returns>
        public bool UpdateWith((Coordinate location, int visit) v)
        {
            if (!this.UpdateHull(v)) return false;
            
            return true;
        }
        
        internal void Add((Coordinate location, int visit) p)
        {
            _locations.Add(p);
            _surface = null;
            _box = null;
        }
        
        internal void Insert(int index, (Coordinate location, int visit) p)
        {
            _locations.Insert(index, p);
            _surface = null;
            _box = null;
        }
        
        internal void RemoveRange(int index, int count)
        {
            _locations.RemoveRange(index, count);
            _surface = null;
            _box = null;
        }

        /// <summary>
        /// Returns the # of locations in this hull.
        /// </summary>
        public int Count => _locations.Count;

        /// <summary>
        /// Returns the surface of this hull.
        /// </summary>
        public float Surface
        {
            get
            {
                if (!_surface.HasValue)
                {
                    _surface = this.CalculateSurface();
                }

                return _surface.Value;
            }
        }

        /// <summary>
        /// The bounding box for this polygon.
        /// </summary>
        public Box Box
        {
            get
            {
                if (_box != null) return _box.Value;
                
                if (this.Count == 0) return new Box();
                if (this.Count == 1) return new Box(this[0].location, this[0].location);
                    
                var box = new Box(this[0].location, this[1].location);
                for (var v = 2; v < this.Count; v++)
                {
                    var vLocation = this[v].location;
                    box = box.ExpandWith(vLocation.Latitude, vLocation.Longitude);
                }

                _box = box;
                return _box.Value;
            }
        }

        /// <summary>
        /// Returns the location/visit at the given index.
        /// </summary>
        /// <param name="i"></param>
        public (Coordinate location, int visit) this[int i] => _locations[i];

        /// <summary>
        /// Calculates the surface of this hull.
        /// </summary>
        /// <returns></returns>
        private float CalculateSurface()
        {
            var l = this.Count;
            var area = 0.0;
            for (var i = 1; i < l+1; i++)
            {
                var p = this[i % l].location;
                var pi = this[(i + 1) % l].location;
                var pm = this[(i - 1)].location;
                area +=  p.Longitude * (pi.Latitude - (double)pm.Latitude);
            }

            return (float)System.Math.Abs(area / 2);
        }

        /// <summary>
        /// Clones this hull.
        /// </summary>
        /// <returns>A deep copy of this hull.</returns>
        public TourHull Clone()
        {
            return new TourHull(_locations);
        }
    }
}