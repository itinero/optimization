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
    public class TourHull
    {
        private readonly List<(Coordinate location, int visit)> _locations =
            new List<(Coordinate location, int visit)>();

        private float? _surface;
        
        public void Add((Coordinate location, int visit) p)
        {
            _locations.Add(p);
        }

        public void Insert(int index, (Coordinate location, int visit) p)
        {
            _locations.Insert(index, p);
        }

        public void RemoveRange(int index, int count)
        {
            _locations.RemoveRange(index, count);
        }

        public int Count => _locations.Count;

        public float? Surface
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

        public (Coordinate location, int visit) this[int i] => _locations[i];

        private float CalculateSurface()
        {
            return 0;
        }
    }
}