// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Solutions.TSP;
using Itinero.Logistics.Weights;
using System.Collections.Generic;
using System.Linq;

namespace Itinero.Logistics.Solutions.TSP.Clustering
{
    /// <summary>
    /// Represents an asymmetric clustering algorithm.
    /// </summary>
    public class AsymmetricClustering<T> : Algorithm
        where T : struct
    {
        private readonly float _e = .1f; // epsilon in seconds (diff beneath this value is considered equal).
        private readonly float _threshold = 10; // a threshold in seconds.
        private readonly T[][] _weights;
        private readonly WeightHandler<T> _weightHandler;

        /// <summary>
        /// Creates a new instance of this algorithm.
        /// </summary>
        public AsymmetricClustering(WeightHandler<T> weightHandler, T[][] weights)
        {
            _weightHandler = weightHandler;
            _weights = weights;
        }

        /// <summary>
        /// Creates a new instance of this algorithm.
        /// </summary>
        public AsymmetricClustering(WeightHandler<T> weightHandler, T[][] weights, float threshold)
        {
            _weightHandler = weightHandler;
            _threshold = threshold;
            _weights = weights;
        }

        /// <summary>
        /// Creates a new instance of this algorithm.
        /// </summary>
        public AsymmetricClustering(WeightHandler<T> weightHandler, T[][] weights, float threshold, float e)
        {
            _weightHandler = weightHandler;
            _threshold = threshold;
            _e = e;
            _weights = weights;
        }

        private List<List<int>> _clusters;
        private T[][] _clusteredWeights;

        /// <summary>
        /// Get the resulting clusters.
        /// </summary>
        public List<List<int>> Clusters
        {
            get
            {
                return _clusters;
            }
        }

        /// <summary>
        /// Clustered weights.
        /// </summary>
        public T[][] Weights
        {
            get
            {
                return _clusteredWeights;
            }
        }

        /// <summary>
        /// Executes the actual run of the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            // build initial cluster and clone weights.
            _clusters = new List<List<int>>();
            var weights = new T[_weights.Length][];
            for (var i = 0; i < _weights.Length; i++)
            {
                _clusters.Add(new List<int>(new int[] { i }));
                weights[i] = _weights[i].Clone() as T[];
            }

            // merge customers until no longer possible.
            bool found = true;
            while(found)
            {
                found = false;

                // find the two closest neighbours within the threshold, and the reverse weight is within tolerance.
                int c1 = Constants.NOT_SET, c2 = Constants.NOT_SET;
                var forwardWeight = _threshold;
                for (var x = 0; x < _clusters.Count; x++)
                { 
                    for (var y = 0; y < x; y++)
                    {
                        if(x == y)
                        {
                            continue;
                        }
                        var localForwardWeight = weights[x][y];
                        if(_weightHandler.GetTime(localForwardWeight) > forwardWeight)
                        {
                            continue;
                        }
                        var localBackwardWeight = weights[y][x];
                        if(System.Math.Abs(_weightHandler.GetTime(localBackwardWeight) - _weightHandler.GetTime(localForwardWeight)) < _e)
                        { // ok, small neighbours and reverse direction is almost identical.
                            c1 = y;
                            c2 = x;

                            forwardWeight = _weightHandler.GetTime(localForwardWeight);
                        }
                    }
                }

                if(c1 != Constants.NOT_SET)
                { // a value was found.
                    var size = _clusters.Count;
                    // put average at c1-position.
                    for (int x = 0; x < size; x++)
                    {
                        weights[x][c1] = _weightHandler.Divide(_weightHandler.Add(weights[x][c1], weights[x][c2]), 2);
                        weights[c1][x] = _weightHandler.Divide(_weightHandler.Add(weights[c1][x], weights[c2][x]), 2);
                    }
                    weights[c1][c1] = _weightHandler.Zero;

                    // remove c2.
                    for (int x = 0; x < size - 1; x++)
                    {
                        if (x >= c2)
                        {
                            weights[x] = weights[x + 1];
                        }
                        for (int y = c2; y < size - 1; y++)
                        {
                            weights[x][y] = weights[x][y + 1];
                        }
                    }

                    // update clusters.
                    _clusters[c1].AddRange(_clusters[c2]);
                    for(var i = c2; i < _clusters.Count - 1; i++)
                    {
                        _clusters[i] = _clusters[i + 1];
                    }
                    _clusters.RemoveAt(_clusters.Count - 1);
                    found = true;
                }
            }

            // resize things to fit only cluster-values.
            _clusteredWeights = new T[_clusters.Count][];
            for(var x = 0; x < _clusters.Count; x++)
            {
                _clusteredWeights[x] = new T[_clusters.Count];
                for(var y = 0; y < _clusters.Count; y++)
                {
                    _clusteredWeights[x][y] = weights[x][y];
                }
            }
        }
    }
}