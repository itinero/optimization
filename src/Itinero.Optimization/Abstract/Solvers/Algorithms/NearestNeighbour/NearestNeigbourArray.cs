using System;
using System.Collections.Generic;

namespace Itinero.Optimization.Algorithms.NearestNeighbour
{
    /// <summary>
    /// A nearest neighbour array.
    /// </summary>
    public class NearestNeigbourArray
    {
        /// <summary>
        /// Contains the nearest neigbour per visit.
        /// </summary>
        private readonly int[] _nn;
        /// <summary>
        /// The number of neigbours to keep for each visit.
        /// </summary>
        private readonly int _n;

        /// <summary>
        /// /// Creates a new nearest neigbour array using bidirectional weights for all visits.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        public NearestNeigbourArray(float[][] weights) : this(weights, weights.Length - 1)
        {

        }

        /// <summary>
        /// Creates a new nearest neigbour array using bidirectional weights keeping <paramref name="n"/> per visit.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        /// <param name="n">The number of nearest neigbours to keep.</param>
        public NearestNeigbourArray(float[][] weights, int n) : this((v1, v2) => weights[v1][v2] + weights[v2][v1], weights.Length, n)
        {

        }

        /// <summary>
        /// Creates a new nearest neigbour array.
        /// </summary>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="count">The # of visits.</param>
        /// <param name="n">The number of neigbours to keep per visit.</param>
        public NearestNeigbourArray(Func<int, int, float> weightFunc, int count, int n)
        {
            _n = n;
            _nn = new int[n * count];

            for (var v = 0; v < count; v++)
            {
                var neighbours = new Collections.SortedDictionary<float, List<int>>();
                for (var current = 0; current < count; current++)
                {
                    if (current != v)
                    {
                        var weight = weightFunc(v, current);
                        List<int> visits = null;
                        if (!neighbours.TryGetValue(weight, out visits))
                        {
                            visits = new List<int>();
                            neighbours.Add(weight, visits);
                        }
                        visits.Add(current);
                    }
                }

                var neigbourCount = 0;
                foreach (var pair in neighbours)
                {
                    foreach (var current in pair.Value)
                    {
                        if (neigbourCount >= n)
                        {
                            break;
                        }

                        _nn[v * n + neigbourCount] = current;
                        neigbourCount++;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the # of nearest neigbours per visit.
        /// </summary>
        /// <returns></returns>
        public int N
        {
            get
            {
                return _n;
            }
        }

        /// <summary>
        /// Gets the nearest neigbours for the given visit.
        /// </summary>
        /// <param name="v">The visit.</param>
        public int[] this[int v]
        {
            get
            {
                var nn = new int[_n];
                _nn.CopyTo(nn, 0, v * _n, _n);
                return nn;
            }
        }

        /// <summary>
        /// Copies the nearest neigbours of the given visit to the given array.
        /// </summary>
        /// <param name="v">The visit.</param>
        /// <param name="nn">The array to copy to.</param>
        /// <param name="index">The index to start copying at.</param>
        public void CopyTo(int v, int[] nn, int index = 0)
        {
            _nn.CopyTo(nn, index, v * _n, _n);
        }

        /// <summary>
        /// Copies the nearest neigbours of the given visit to the given array.
        /// </summary>
        /// <param name="v">The visit.</param>
        /// <param name="nn">The array to copy to.</param>
        /// <param name="index">The index to start copying at.</param>
        /// <param name="count">The # of nn to copy.</param>
        public void CopyTo(int v, int[] nn, int index, int count)
        {
            _nn.CopyTo(nn, index, v * _n, count);
        }
    }
}