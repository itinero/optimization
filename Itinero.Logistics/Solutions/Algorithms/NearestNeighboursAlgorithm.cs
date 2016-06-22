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

using Itinero.Collections;
using Itinero.Logistics.Weights;
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.Algorithms
{
    /// <summary>
    /// Contains n-nearest neighbour algorithm.
    /// </summary>
    public class NearestNeighboursAlgorithm<T> : Algorithm
        where T : struct
    {
        private readonly T[][] _weights;
        private readonly int _n;
        private readonly int _customer;
        private readonly WeightHandler<T> _weightHandler;

        /// <summary>
        /// Creates an instance of the nearest neighbour algorithm.
        /// </summary>
        public NearestNeighboursAlgorithm(WeightHandler<T> weightHandler, T[][] weights, int n, int customer)
        {
            _weightHandler = weightHandler;
            _weights = weights;
            _n = n;
            _customer = customer;
        }

        private INearestNeighbours<T> _nn;

        /// <summary>
        /// Executes the actual run of the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            if(_n > 0)
            { // do the n-nearest neighbours, forward.
                _nn = NearestNeighboursAlgorithm<T>.Forward(_weightHandler, _weights, _n, _customer);
            }
            else
            { // not supported.
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Calculates then n-nearest neighbours in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static INearestNeighbours<T> Forward(WeightHandler<T> weightHandler, T[][] weights, int n, int customer)
        {
            var neighbours = new SortedDictionary<T, List<int>>(weightHandler);
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[customer][current];
                    List<int> customers = null;
                    if (!neighbours.TryGetValue(weight, out customers))
                    {
                        customers = new List<int>();
                        neighbours.Add(weight, customers);
                    }
                    customers.Add(current);
                }
            }

            var result = new NearestNeighbours<T>(n);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    if (result.Count < n)
                    {
                        if (weightHandler.IsSmallerThan(result.Max, pair.Key))
                        {
                            result.Max = pair.Key;
                        }
                        result.Add(current);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates then n-nearest neighbours in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static INearestNeighbours<T> Backward(WeightHandler<T> weightHandler, T[][] weights, int n, int customer)
        {
            var neighbours = new SortedDictionary<T, List<int>>(weightHandler);
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[current][customer];
                    List<int> customers = null;
                    if (!neighbours.TryGetValue(weight, out customers))
                    {
                        customers = new List<int>();
                        neighbours.Add(weight, customers);
                    }
                    customers.Add(current);
                }
            }

            var result = new NearestNeighbours<T>(n);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    if (result.Count < n)
                    {
                        if (weightHandler.IsSmallerThan(result.Max, pair.Key))
                        {
                            result.Max = pair.Key;
                        }
                        result.Add(current);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a forward-direction only.
        /// </summary>
        /// <returns></returns>
        public static ISortedNearestNeighbours<T> Forward(WeightHandler<T> weightHandler, T[][] weights, T max, int customer)
        {
            var neighbours = new SortedDictionary<T, List<int>>(weightHandler);
            var maxFound = weightHandler.Zero;
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[customer][current];
                    if (weightHandler.IsSmallerThanOrEqual(weight, max))
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if(weightHandler.IsSmallerThan(maxFound, weight))
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours<T>(maxFound);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    result.Add(current);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculates then nearest neighbours using a weight smaller than or equal to a maximum in a backward-direction only.
        /// </summary>
        /// <returns></returns>
        public static ISortedNearestNeighbours<T> Backward(WeightHandler<T> weightHandler, T[][] weights, T max, int customer)
        {
            var neighbours = new SortedDictionary<T, List<int>>(weightHandler);
            var maxFound = weightHandler.Zero;
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[current][customer];
                    if (weightHandler.IsSmallerThanOrEqual(weight, max))
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if (weightHandler.IsSmallerThan(maxFound, weight))
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours<T>(maxFound);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    result.Add(current);
                }
            }
            return result;
        }
    }
}