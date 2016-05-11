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

using Itinero.Algorithms.Search.Hilbert;
using Itinero.Graphs;
using Itinero.Graphs.Geometric;
using Itinero.LocalGeo;
using Itinero.Refactoring;
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Routing.Loops
{
    /// <summary>
    /// Searches for the most attractive vertices and drops the ones that are too close together.
    /// </summary>
    public class AttractiveVertexSearch : Algorithm
    {
        private readonly GeometricGraph _graph;
        private readonly Func<Graph, uint, float> _score;
        private readonly Box _box;

        /// <summary>
        /// Creates a new loop generator.
        /// </summary>
        public AttractiveVertexSearch(GeometricGraph graph, Box box, Func<Graph, uint, float> score)
        {
            _graph = graph;
            _score = score;
            _box = box;
        }

        private HashSet<uint> _vertices;

        /// <summary>
        /// Executes the actual run of the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            // get all the closest best-scoring vertices.
            var sorted = new SortedSet<ScoredVertex>();
            var vertices = _graph.Search(_box.MinLat, _box.MinLon, _box.MaxLat, _box.MaxLon);
            foreach (var vertex in vertices)
            {
                var score = _score(_graph.Graph, vertex);
                if (score < float.MaxValue)
                {
                    sorted.Add(new ScoredVertex()
                    {
                        Score = score,
                        Vertex = vertex
                    });
                }
            }

            // get only the highest scoring vertices.
            var bestCount = System.Math.Min(250, sorted.Count);
            var best = new List<ScoredVertex>();
            foreach (var scoredVertex in sorted)
            {
                if (best.Count == bestCount)
                {
                    break;
                }
                best.Add(scoredVertex);
            }

            // remove vertices too close to eachother.
            var success = true;
            var maxCellRadius = System.Math.Min(_box.MaxLat - _box.MinLat, _box.MaxLon - _box.MinLon) / 10;
            var maxCellCount = 1;
            while (success)
            {
                success = false;
                var toRemove = new HashSet<ScoredVertex>();
                foreach (var vertex in best)
                {
                    // check if this vertex has the lowest score of it's 'cell' and if it's cell is crowded.
                    var vertexLocation = _graph.GetVertex(vertex.Vertex);
                    var vertexBox = new Box(vertexLocation.Latitude - maxCellRadius, vertexLocation.Longitude - maxCellRadius,
                        vertexLocation.Latitude + maxCellRadius, vertexLocation.Longitude + maxCellRadius);
                    var isLowest = true;
                    var closeCount = 0;
                    foreach (var other in best)
                    {
                        if (other.Vertex != vertex.Vertex &&
                            !toRemove.Contains(other))
                        {
                            var otherLocation = _graph.GetVertex(other.Vertex);
                            if (vertexBox.Overlaps(otherLocation.Latitude, otherLocation.Longitude))
                            {
                                if (other.Score < vertex.Score)
                                {
                                    isLowest = false;
                                    break;
                                }
                                closeCount++;
                            }
                        }
                    }
                    
                    if (isLowest && closeCount >= maxCellCount)
                    { // vertex cell is crowded and it has the lowest score, remove it.
                        success = true;
                        toRemove.Add(vertex);
                    }
                }

                foreach(var vertex in toRemove)
                {
                    best.Remove(vertex);
                }
            }

            _vertices = new HashSet<uint>();
            this.HasRun = true;
            this.HasSucceeded = true;
            foreach(var vertex in best)
            {
                _vertices.Add(vertex.Vertex);
            }
        }
        
        /// <summary>
        /// Gets the 'best' vertices in their area.
        /// </summary>
        public HashSet<uint> Vertices
        {
            get
            {
                return _vertices;
            }
        }

        private struct ScoredVertex : IComparable<ScoredVertex>
        {
            public uint Vertex { get; set; }

            public float Score { get; set; }

            public int CompareTo(ScoredVertex other)
            {
                return -this.Score.CompareTo(other.Score);
            }
        }
    }
}