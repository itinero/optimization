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
using OsmSharp.Math.Random;
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
        /// Tests cloning a route.
        /// </summary>
        [Test]
        public void TestClone()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, 0);
            var cloned = route.Clone() as Route;

            Assert.AreEqual(route.First, cloned.First);
            Assert.AreEqual(route.Last, cloned.Last);
            Assert.AreEqual(route.Count, cloned.Count);

            ExtraAssert.ItemsAreEqual(route, cloned);

            routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            route = new Route(routeEnumerable, 4);
            cloned = route.Clone() as Route;

            Assert.AreEqual(route.First, cloned.First);
            Assert.AreEqual(route.Last, cloned.Last);
            Assert.AreEqual(route.Count, cloned.Count);

            ExtraAssert.ItemsAreEqual(route, cloned);

            routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            route = new Route(routeEnumerable, null);
            cloned = route.Clone() as Route;

            Assert.AreEqual(route.First, cloned.First);
            Assert.AreEqual(route.Last, cloned.Last);
            Assert.AreEqual(route.Count, cloned.Count);

            ExtraAssert.ItemsAreEqual(route, cloned);
        }

        /// <summary>
        /// Tests clearing a route.
        /// </summary>
        [Test]
        public void TestClear()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, 0);

            route.Clear();

            Assert.AreEqual(1, route.Count);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);
        }
        
        /// <summary>
        /// Tests get customer at in a route.
        /// </summary>
        [Test]
        public void TestGetCustomerAt()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, 0);

            Assert.AreEqual(routeEnumerable[0], route.GetCustomerAt(0));
            Assert.AreEqual(routeEnumerable[1], route.GetCustomerAt(1));
            Assert.AreEqual(routeEnumerable[2], route.GetCustomerAt(2));
            Assert.AreEqual(routeEnumerable[3], route.GetCustomerAt(3));
            Assert.AreEqual(routeEnumerable[4], route.GetCustomerAt(4));

            Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    route.GetCustomerAt(-1);
                });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    route.GetCustomerAt(5);
                });
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Test]
        public void TestEnumerableConstructorClosed()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, 0);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);

            var solutionList = new List<int>(route);
            Assert.AreEqual(0, solutionList[0]);
            Assert.AreEqual(1, solutionList[1]);
            Assert.AreEqual(3, solutionList[2]);
            Assert.AreEqual(2, solutionList[3]);
            Assert.AreEqual(4, solutionList[4]);
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Test]
        public void TestEnumerableConstructorFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, 4);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(4, route.Last);

            var solutionList = new List<int>(route);
            Assert.AreEqual(0, solutionList[0]);
            Assert.AreEqual(1, solutionList[1]);
            Assert.AreEqual(3, solutionList[2]);
            Assert.AreEqual(2, solutionList[3]);
            Assert.AreEqual(4, solutionList[4]);
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Test]
        public void TestEnumerableConstructorOpen()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, null);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(null, route.Last);

            var solutionList = new List<int>(route);
            Assert.AreEqual(0, solutionList[0]);
            Assert.AreEqual(1, solutionList[1]);
            Assert.AreEqual(3, solutionList[2]);
            Assert.AreEqual(2, solutionList[3]);
            Assert.AreEqual(4, solutionList[4]);
        }

        /// <summary>
        /// Tests count.
        /// </summary>
        [Test]
        public void TestCount()
        {
            var route = new Route(new int[] { 0 }, 0);
            Assert.AreEqual(1, route.Count);
            route.InsertAfter(0, 1);
            Assert.AreEqual(2, route.Count);
            route.InsertAfter(1, 2);
            Assert.AreEqual(3, route.Count);
            route.InsertAfter(2, 3);
            Assert.AreEqual(4, route.Count);
            route.InsertAfter(3, 4);

            route = new Route(new int[] { 0 }, null);
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
            var route = new Route(new int[] { 0 }, 0);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);
            route.InsertAfter(0, 1);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);

            route = new Route(new int[] { 0 }, null);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(null, route.Last);
            route.InsertAfter(0, 1);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(null, route.Last);

            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            route = new Route(routeEnumerable, 4);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(4, route.Last);

            route = new Route(routeEnumerable, 0);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);

            route = new Route(routeEnumerable, null);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(null, route.Last);
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsClosed()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, 0);

            Assert.IsFalse(route.Contains(10));

            Assert.IsTrue(route.Contains(0));
            Assert.IsTrue(route.Contains(1));
            Assert.IsTrue(route.Contains(2));
            Assert.IsTrue(route.Contains(3));
            Assert.IsTrue(route.Contains(4));

            Assert.IsFalse(route.Contains(8, 0));
            Assert.IsFalse(route.Contains(4, 2));
            Assert.IsFalse(route.Contains(3, 2));
            Assert.IsTrue(route.Contains(0, 1));
            Assert.IsTrue(route.Contains(1, 2));
            Assert.IsTrue(route.Contains(2, 3));
            Assert.IsTrue(route.Contains(3, 4));
            Assert.IsTrue(route.Contains(4, 0));
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsOpen()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, null);

            Assert.IsFalse(route.Contains(10));

            Assert.IsTrue(route.Contains(0));
            Assert.IsTrue(route.Contains(1));
            Assert.IsTrue(route.Contains(2));
            Assert.IsTrue(route.Contains(3));
            Assert.IsTrue(route.Contains(4));

            Assert.IsFalse(route.Contains(8, 0));
            Assert.IsFalse(route.Contains(4, 2));
            Assert.IsFalse(route.Contains(3, 2));
            Assert.IsTrue(route.Contains(0, 1));
            Assert.IsTrue(route.Contains(1, 2));
            Assert.IsTrue(route.Contains(2, 3));
            Assert.IsTrue(route.Contains(3, 4));
            Assert.IsFalse(route.Contains(4, 0));
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, 4);

            Assert.IsFalse(route.Contains(10));

            Assert.IsTrue(route.Contains(0));
            Assert.IsTrue(route.Contains(1));
            Assert.IsTrue(route.Contains(2));
            Assert.IsTrue(route.Contains(3));
            Assert.IsTrue(route.Contains(4));

            Assert.IsFalse(route.Contains(8, 0));
            Assert.IsFalse(route.Contains(4, 2));
            Assert.IsFalse(route.Contains(3, 2));
            Assert.IsTrue(route.Contains(0, 1));
            Assert.IsTrue(route.Contains(1, 2));
            Assert.IsTrue(route.Contains(2, 3));
            Assert.IsTrue(route.Contains(3, 4));
            Assert.IsFalse(route.Contains(4, 0));
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveClosed()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, 0);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.First); });

                // remove customers.
                while (customers.Count > 1)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var customer = customers[customerIdx];
                    if (customer != route.First)
                    {
                        customers.Remove(customer);

                        route.Remove(customer);

                        Assert.AreEqual(customers.Count, route.Count);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(0, route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveOpen()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, null);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.First); });

                // remove customers.
                while (customers.Count > 2)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var customer = customers[customerIdx];
                    if (customer != route.First &&
                        customer != route.Last)
                    {
                        customers.Remove(customer);

                        route.Remove(customer);

                        Assert.AreEqual(customers.Count, route.Count);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(null, route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveFixed()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, 4);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.First); });

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.Last.Value); });

                // remove customers.
                while (customers.Count > 2)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var customer = customers[customerIdx];
                    if (customer != route.First &&
                        customer != route.Last)
                    {
                        customers.Remove(customer);

                        route.Remove(customer);

                        Assert.AreEqual(customers.Count, route.Count);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(4, route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterOpen()
        {
            var route = new Route(new int[] { 0, 1, 2, 3 }, null);

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
                route.InsertAfter(10, 11);
            });

            // insert customers.
            Assert.AreEqual(4, route.Count);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 1, route.Count);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(null, route.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterFixed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 100 }, 0);

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

            // insert customers.
            Assert.AreEqual(5, route.Count);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 2, route.Count);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(0, route.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterClose()
        {
            var route = new Route(new int[] { 0, 1, 2, 3 }, 0);

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
                route.InsertAfter(10, 11);
            });

            // insert customers.
            Assert.AreEqual(4, route.Count);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 1, route.Count);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(0, route.Last);
            }
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenOpen()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, null);

            for (var from = 0; from < route.Count; from++)
            {
                for (var to = 0; to < route.Count; to++)
                {
                    var enumerator = route.Between(from, to).GetEnumerator();
                    if (from <= to)
                    {
                        for (var customer = from; customer < to + 1; customer++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(customer, enumerator.Current);
                        }
                    }
                    Assert.IsFalse(enumerator.MoveNext());
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenClosed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 0);

            for (var from = 0; from < route.Count; from++)
            {
                for (var to = 0; to < route.Count; to++)
                {
                    var enumerator = route.Between(from, to).GetEnumerator();
                    if (from <= to)
                    {
                        for (var customer = from; customer < to + 1; customer++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(customer, enumerator.Current);
                        }
                    }
                    else
                    {
                        for (var customer = from; customer < route.Count; customer++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(customer, enumerator.Current);
                        }
                        for (var customer = 0; customer < to + 1; customer++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(customer, enumerator.Current);
                        }
                    }
                    Assert.IsFalse(enumerator.MoveNext());
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenFixed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 9);

            for (var from = 0; from < route.Count; from++)
            {
                for (var to = 0; to < route.Count; to++)
                {
                    var enumerator = route.Between(from, to).GetEnumerator();
                    if (from <= to)
                    {
                        for (var customer = from; customer < to + 1; customer++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(customer, enumerator.Current);
                        }
                    }
                    Assert.IsFalse(enumerator.MoveNext());
                }
            }
        }

        /// <summary>
        /// Tests get neighbours.
        /// </summary>
        [Test]
        public void TestGetNeighboursOpen()
        {
            var count = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, null);

            int[] neighbours;
            for (int customer = 0; customer < count - 1; customer++)
            {
                neighbours = route.GetNeigbours(customer);
                Assert.IsTrue(neighbours[0] == customer + 1);
            }
            neighbours = route.GetNeigbours(count - 1);
            Assert.IsTrue(neighbours.Length == 0);
        }

        /// <summary>
        /// Tests get neighbours.
        /// </summary>
        [Test]
        public void TestGetNeighboursClosed()
        {
            var count = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, 0);

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
        public void TestShiftFixed()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, 9);

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
                if (customer != route.First &&
                    customer != route.Last)
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
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(9, route.Last);

                    ExtraAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Test shifting customers.
        /// </summary>
        [Test]
        public void TestShiftClosed()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, 0);

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
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);

                    ExtraAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Test shifting customers.
        /// </summary>
        [Test]
        public void TestShiftOpen()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, null);

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
                if (customer != route.First &&
                    customer != route.Last)
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
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(null, route.Last);

                    ExtraAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }
    }
}