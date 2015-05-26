// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Logistics.Routes;
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Routes
{
    /// <summary>
    /// Tests for route.
    /// </summary>
    [TestFixture]
    public class RouteTest
    {
        /// <summary>
        /// Tests is closed.
        /// </summary>
        [Test]
        public void TestIsClosed()
        {
            var route = new Route(10, 0, true);
            Assert.AreEqual(true, route.IsClosed);

            route = new Route(10, 0, false);
            Assert.AreEqual(false, route.IsClosed);
        }

        /// <summary>
        /// Tests count.
        /// </summary>
        [Test]
        public void TestCount()
        {
            var route = new Route(4, 0, true);
            Assert.AreEqual(1, route.Count);
            route.InsertAfter(0, 1);
            Assert.AreEqual(2, route.Count);
            route.InsertAfter(1, 2);
            Assert.AreEqual(3, route.Count);
            route.InsertAfter(2, 3);
            Assert.AreEqual(4, route.Count);
            route.InsertAfter(3, 4);

            route = new Route(4, 0, false);
            Assert.AreEqual(1, route.Count);
            route.InsertAfter(0, 1);
            Assert.AreEqual(2, route.Count);
            route.InsertAfter(1, 2);
            Assert.AreEqual(3, route.Count);
            route.InsertAfter(2, 3);
            Assert.AreEqual(4, route.Count);
            route.InsertAfter(3, 4);
        }

        /// <summary>
        /// Tests the first and last count.
        /// </summary>
        [Test]
        public void TestFirstAndLast()
        {
            var route = new Route(4, 0, true);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);
            route.InsertAfter(0, 1);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);

            route = new Route(4, 0, false);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);
            route.InsertAfter(0, 1);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(1, route.Last);
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContains()
        {
            var route = new Route(10, 0, true);
            for (int customer = 2; customer < 10; customer = customer + 2)
            {
                route.InsertAfter(customer - 2, customer);
            }

            Assert.IsFalse(route.Contains(10));
            Assert.IsTrue(route.Contains(2));
            Assert.IsFalse(route.Contains(3));
            Assert.IsTrue(route.Contains(2, 4));
            Assert.IsFalse(route.Contains(4, 2));
            Assert.IsFalse(route.Contains(3, 2));
            Assert.IsTrue(route.Contains(8, 0));
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            // create a new empty route.
            const int count = 100;
            var route = new Route(10, 0, true);
            var customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                customers.Add(0);
                for (var customer = 1; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    customers.Add(customer);
                }

                // remove customers.
                while (customers.Count > 1)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var customer = customers[customerIdx];
                    if (customer != route.First)
                    {
                        customers.RemoveAt(customerIdx);

                        route.Remove(customer);

                        Assert.AreEqual(customers.Count, route.Count);
                        Assert.AreEqual(true, route.IsClosed);
                        Assert.AreEqual(route.Last, route.First);
                    }
                }
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfter()
        {
            var route = new Route(10, 0, true);

            // test arguments.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                route.InsertAfter(0, -1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                route.InsertAfter(-1, 0);
            });
            Assert.Catch<ArgumentException>(() =>
            {
                route.InsertAfter(1, 1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                route.InsertAfter(1, 2);
            });

            // insert customers.
            Assert.AreEqual(1, route.Count);
            Assert.AreEqual(true, route.IsClosed);
            for (int customer = 1; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 1, route.Count);
                Assert.AreEqual(true, route.IsClosed);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(0, route.Last);
            }

            // create open route.
            route = new Route(10, 0, false);

            // test update last.
            route.InsertAfter(0, 1);
            route.InsertAfter(1, 2);
            route.InsertAfter(2, 3);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(3, route.Last);
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        [Test]
        public void TestEnumerateBetween()
        {
            var count = 10;
            var route = new Route(count, 0, true);
            for (var customer = 1; customer < count; customer++)
            {
                route.InsertAfter(customer - 1, customer);
            }

            for (var from = 0; from < count; from++)
            {
                for (var to = 0; to < count; to++)
                {
                    var enumerator = route.Between(from, to).GetEnumerator();
                    for (var customer = from; customer - 1 != to; customer++)
                    {
                        if (customer == count)
                        {
                            customer = 0;
                        }

                        Assert.IsTrue(enumerator.MoveNext());
                        Assert.AreEqual(customer, enumerator.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Tests get neighbours.
        /// </summary>
        [Test]
        public void TestGetNeighbours()
        {
            var count = 10;
            var route = new Route(count, 0, true);
            if (route != null)
            { // this part needs testing.
                for (int customer = 1; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                }
            }

            int[] neighbours;
            for (int customer = 0; customer < count - 1; customer++)
            {
                neighbours = route.GetNeigbours(customer);
                Assert.IsTrue(neighbours[0] == customer + 1);
            }
            neighbours = route.GetNeigbours(count - 1);
            Assert.IsTrue(neighbours[0] == 0);
        }

        /// <summary>
        /// Test shifting customers.
        /// </summary>
        [Test]
        public void TestShift()
        {
            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var count = 10;
            var testCount = 10;
            var route = new Route(count, 0, true);
            var customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                customers.Add(0);
                for (int customer = 1; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    customers.Add(customer);
                }

                // remove customers.
                int testIdx = 0;
                while (testIdx < testCount)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var insertIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count - 2);
                    if (customerIdx <= insertIdx)
                    {
                        insertIdx = insertIdx + 1;
                    }

                    var customer = customers[customerIdx];
                    var insert = customers[insertIdx];
                    if (customer != route.First)
                    {
                        if (customerIdx < insertIdx)
                        {
                            customers.Insert(insertIdx + 1, customer);
                            customers.RemoveAt(customerIdx);
                        }
                        else
                        {
                            customers.RemoveAt(customerIdx);
                            customers.Insert(insertIdx + 1, customer);
                        }

                        route.ShiftAfter(customer, insert);

                        Assert.AreEqual(customers.Count, route.Count);
                        Assert.AreEqual(true, route.IsClosed);
                        Assert.AreEqual(route.Last, route.First);

                        ExtraAssert.ItemsAreEqual(customers, route);

                        testIdx++;
                    }
                }
            }
        }
    }
}