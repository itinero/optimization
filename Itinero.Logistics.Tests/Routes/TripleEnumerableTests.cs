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

using NUnit.Framework;
using Itinero.Logistics.Routes;
using System.Collections.Generic;

namespace Itinero.Logistics.Tests.Routes
{
    /// <summary>
    /// Tests for the triple enumerable.
    /// </summary>
    [TestFixture]
    public class TripleEnumerableTests
    {
        /// <summary>
        /// Tests the triple enumerable on a route that isn't a round.
        /// </summary>
        [Test]
        public void Test1NoRound()
        {
            var route = new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4 }, null);
            var tripleEnumerable = new TripleEnumerable(route);
            var triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(3, triples.Count);
            Assert.Contains(new Triple(0, 1, 2), triples);
            Assert.Contains(new Triple(1, 2, 3), triples);
            Assert.Contains(new Triple(2, 3, 4), triples);
        }

        /// <summary>
        /// Tests the triple enumerable on a route that is a round.
        /// </summary>
        [Test]
        public void Test2Round()
        {
            var route = new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4 }, 0);
            var tripleEnumerable = new TripleEnumerable(route);
            var triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(5, triples.Count);
            Assert.Contains(new Triple(0, 1, 2), triples);
            Assert.Contains(new Triple(1, 2, 3), triples);
            Assert.Contains(new Triple(2, 3, 4), triples);
            Assert.Contains(new Triple(3, 4, 0), triples);
            Assert.Contains(new Triple(4, 0, 1), triples);
        }

        /// <summary>
        /// Tests the triple enumerable on a route that is empty.
        /// </summary>
        [Test]
        public void Test3Empty()
        {
            var route = new Logistics.Routes.Route(new int[] { 0 }, 0);
            var tripleEnumerable = new TripleEnumerable(route);
            var triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(0, triples.Count);

            route = new Logistics.Routes.Route(new int[] { 0 }, null);
            tripleEnumerable = new TripleEnumerable(route);
            triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(0, triples.Count);
        }

        /// <summary>
        /// Tests the triple enumerable's enumerator's reset.
        /// </summary>
        [Test]
        public void Test4Reset()
        {
            var route = new Logistics.Routes.Route(new int[] { 0, 1, 2, 3, 4 }, 0);
            var tripleEnumerable = new TripleEnumerable(route);
            var tripleEnumerator = tripleEnumerable.GetEnumerator();

            var triples = new List<Triple>();
            while (tripleEnumerator.MoveNext())
            {
                triples.Add(tripleEnumerator.Current);
            }

            Assert.AreEqual(5, triples.Count);
            Assert.Contains(new Triple(0, 1, 2), triples);
            Assert.Contains(new Triple(1, 2, 3), triples);
            Assert.Contains(new Triple(2, 3, 4), triples);
            Assert.Contains(new Triple(3, 4, 0), triples);
            Assert.Contains(new Triple(4, 0, 1), triples);

            tripleEnumerator.Reset();

            triples = new List<Triple>();
            while (tripleEnumerator.MoveNext())
            {
                triples.Add(tripleEnumerator.Current);
            }

            Assert.AreEqual(5, triples.Count);
            Assert.Contains(new Triple(0, 1, 2), triples);
            Assert.Contains(new Triple(1, 2, 3), triples);
            Assert.Contains(new Triple(2, 3, 4), triples);
            Assert.Contains(new Triple(3, 4, 0), triples);
            Assert.Contains(new Triple(4, 0, 1), triples);
        }

        /// <summary>
        /// Tests the triple enumerable on a route that contains only one customer.
        /// </summary>
        [Test]
        public void Test5OneCustomer()
        {
            var route = new Logistics.Routes.Route(new int[] { 0 }, 0);
            var tripleEnumerable = new TripleEnumerable(route);
            var triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(0, triples.Count);

            route = new Logistics.Routes.Route(new int[] { 0 }, null);
            tripleEnumerable = new TripleEnumerable(route);
            triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(0, triples.Count);
        }

        /// <summary>
        /// Tests the triple enumerable on a route that contains only two customers.
        /// </summary>
        [Test]
        public void Test5TwoCustomers()
        {
            var route = new Logistics.Routes.Route(new int[] { 0, 1 }, 0);
            var tripleEnumerable = new TripleEnumerable(route);
            var triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(0, triples.Count);

            route = new Logistics.Routes.Route(new int[] { 0, 1 }, null);
            tripleEnumerable = new TripleEnumerable(route);
            triples = new List<Triple>(tripleEnumerable);

            Assert.AreEqual(0, triples.Count);
        }
    }
}