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

using Itinero.Optimization.Abstract.Tours;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Abstract.Tours
{
    /// <summary>
    /// Tests for the pair enumerable.
    /// </summary>
    [TestFixture]
    public class PairEnumerableTests
    {
        /// <summary>
        /// Tests the pair enumerable on a route that isn't a round.
        /// </summary>
        [Test]
        public void Test1NotClosed()
        {
            var route = new Tour(new int[] { 0, 1, 2, 3, 4 }, null);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(4, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 0);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(4, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 1);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(3, pairs.Count);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 2);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(2, pairs.Count);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 3);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(1, pairs.Count);
            Assert.Contains(new Pair(3, 4), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 4);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that is a round.
        /// </summary>
        [Test]
        public void Test2Closed()
        {
            var route = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 0);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 1);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(4, pairs.Count);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 2);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(3, pairs.Count);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 3);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(2, pairs.Count);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed(), 4);
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(1, pairs.Count);
            Assert.Contains(new Pair(4, 0), pairs);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that is empty.
        /// </summary>
        [Test]
        public void Test3Empty()
        {
            var route = new Tour(new int[] { 0 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);

            route = new Tour(new int[] { 0 }, null);
            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);
        }

        /// <summary>
        /// Tests the pair enumerable's enumerator's reset.
        /// </summary>
        [Test]
        public void Test4Reset()
        {
            var route = new Tour(new int[] { 0, 1, 2, 3, 4 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairEnumerator = pairEnumerable.GetEnumerator();

            var pairs = new List<Pair>();
            while (pairEnumerator.MoveNext())
            {
                pairs.Add(pairEnumerator.Current);
            }

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);

            pairEnumerator.Reset();

            pairs = new List<Pair>();
            while (pairEnumerator.MoveNext())
            {
                pairs.Add(pairEnumerator.Current);
            }

            Assert.AreEqual(5, pairs.Count);
            Assert.Contains(new Pair(0, 1), pairs);
            Assert.Contains(new Pair(1, 2), pairs);
            Assert.Contains(new Pair(2, 3), pairs);
            Assert.Contains(new Pair(3, 4), pairs);
            Assert.Contains(new Pair(4, 0), pairs);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that contains only one customer.
        /// </summary>
        [Test]
        public void Test5OneCustomer()
        {
            var route = new Tour(new int[] { 0 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);

            route = new Tour(new int[] { 0 }, null);
            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);
        }

        /// <summary>
        /// Tests the pair enumerable on a route that contains only one customer.
        /// </summary>
        [Test]
        public void Test6TwoCustomers()
        {
            var route = new Tour(new int[] { 0, 1 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(2, pairs.Count);

            route = new Tour(new int[] { 0, 1 }, null);
            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(1, pairs.Count);
        }
    }
}