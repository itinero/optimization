// Itinero.Optimization - Route optimization for .NET
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

using Itinero.Optimization.Tours;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Tours
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
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, null);
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
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 0);
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
            var route = new Optimization.Tours.Tour(new int[] { 0 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);

            route = new Optimization.Tours.Tour(new int[] { 0 }, null);
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
            var route = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4 }, 0);
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
            var route = new Optimization.Tours.Tour(new int[] { 0 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(0, pairs.Count);

            route = new Optimization.Tours.Tour(new int[] { 0 }, null);
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
            var route = new Optimization.Tours.Tour(new int[] { 0, 1 }, 0);
            var pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            var pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(2, pairs.Count);

            route = new Optimization.Tours.Tour(new int[] { 0, 1 }, null);
            pairEnumerable = new PairEnumerable<Tour>(route, route.IsClosed());
            pairs = new List<Pair>(pairEnumerable);

            Assert.AreEqual(1, pairs.Count);
        }
    }
}