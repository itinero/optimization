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

using Itinero.Collections;
using System;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.Algorithms
{
    /// <summary>
    /// Contains n-nearest neighbour algorithm.
    /// </summary>
    public class NearestNeighboursAlgorithm : Algorithm
    {
        private readonly float[][] _weights;
        private readonly int _n;
        private readonly int _customer;

        /// <summary>
        /// Creates an instance of the nearest neighbour algorithm.
        /// </summary>
        public NearestNeighboursAlgorithm(float[][] weights, int n, int customer)
        {
            _weights = weights;
            _n = n;
            _customer = customer;
        }

        private INearestNeighbours _nn;

        /// <summary>
        /// Executes the actual run of the algorithm.
        /// </summary>
        protected override void DoRun()
        {
            if(_n > 0)
            { // do the n-nearest neighbours, forward.
                _nn = NearestNeighboursAlgorithm.Forward(_weights, _n, _customer);
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
        public static INearestNeighbours Forward(float[][] weights, int n, int customer)
        {
            var neighbours = new SortedDictionary<double, List<int>>();
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

            var result = new NearestNeighbours(n);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    if (result.Count < n)
                    {
                        if (result.Max < pair.Key)
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
        public static INearestNeighbours Backward(float[][] weights, int n, int customer)
        {
            var neighbours = new SortedDictionary<double, List<int>>();
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

            var result = new NearestNeighbours(n);
            foreach (var pair in neighbours)
            {
                foreach (var current in pair.Value)
                {
                    if (result.Count < n)
                    {
                        if (result.Max < pair.Key)
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
        public static ISortedNearestNeighbours Forward(float[][] weights, double max, int customer)
        {
            var neighbours = new SortedDictionary<double, List<int>>();
            var maxFound = 0.0;
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[customer][current];
                    if (weight <= max)
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if(maxFound < weight)
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours(maxFound);
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
        public static ISortedNearestNeighbours Backward(float[][] weights, double max, int customer)
        {
            var neighbours = new SortedDictionary<double, List<int>>();
            var maxFound = 0.0;
            for (var current = 0; current < weights.Length; current++)
            {
                if (current != customer)
                {
                    var weight = weights[current][customer];
                    if (weight <= max)
                    {
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(current);

                        if (maxFound < weight)
                        {
                            maxFound = weight;
                        }
                    }
                }
            }

            var result = new SortedNearestNeighbours(maxFound);
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