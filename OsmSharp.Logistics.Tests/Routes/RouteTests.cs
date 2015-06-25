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
        /// Tests enumerable constructors.
        /// </summary>
        [Test]
        public void TestEnumerableConstructorClosedNotFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(true, route.IsClosed);
            Assert.AreEqual(false, route.IsLastFixed);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(Constants.NOT_SET, route.Last);

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
        public void TestEnumerableConstructorClosedFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, true, true);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(true, route.IsClosed);
            Assert.AreEqual(true, route.IsLastFixed);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(Constants.NOT_SET, route.Last);

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
        public void TestEnumerableConstructorOpenNotFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, false, false);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(false, route.IsClosed);
            Assert.AreEqual(false, route.IsLastFixed);
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
        public void TestEnumerableConstructorOpenFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var route = new Route(routeEnumerable, false, true);

            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(false, route.IsClosed);
            Assert.AreEqual(true, route.IsLastFixed);
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
        /// Tests is last fixed.
        /// </summary>
        [Test]
        public void TestIsLastFixed()
        {
            var route = new Route(10, 0, 9, true);
            Assert.AreEqual(true, route.IsLastFixed);

            route = new Route(10, 0, 9, false);
            Assert.AreEqual(true, route.IsLastFixed);
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
            Assert.AreEqual(Constants.NOT_SET, route.Last);
            route.InsertAfter(0, 1);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(Constants.NOT_SET, route.Last);

            route = new Route(4, 0, false);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(0, route.Last);
            route.InsertAfter(0, 1);
            Assert.AreEqual(0, route.First);
            Assert.AreEqual(1, route.Last);

            var routeEnumerable = new int[] { 0, 1, 3, 2, 4 };
            route = new Route(routeEnumerable, false, false);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(4, route.Last);

            route = new Route(routeEnumerable, true, false);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(Constants.NOT_SET, route.Last);

            route = new Route(routeEnumerable, false, true);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(4, route.Last);

            route = new Route(routeEnumerable, true, true);

            Assert.AreEqual(0, route.First);
            Assert.AreEqual(Constants.NOT_SET, route.Last);
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsNotClosedNotFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, false, false);

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
        public void TestContainsNotClosedFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, false, true);

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
        public void TestContainsClosedNotFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, true, false);

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
        public void TestContainsClosedFixed()
        {
            var routeEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var route = new Route(routeEnumerable, true, true);

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
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveNotClosedNotFixed()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, false, false);

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
                        Assert.AreEqual(false, route.IsClosed);
                        Assert.AreEqual(false, route.IsLastFixed);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(customers[customers.Count - 1], route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveNotClosedFixed()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, false, true);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.First); });

                // test removing last.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.Last); });

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
                        Assert.AreEqual(false, route.IsClosed);
                        Assert.AreEqual(true, route.IsLastFixed);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(4, route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveClosedNotFixed()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, true, false);

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
                        Assert.AreEqual(true, route.IsClosed);
                        Assert.AreEqual(false, route.IsLastFixed);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(Constants.NOT_SET, route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        [Test]
        public void TestRemoveClosedFixed()
        {
            // create a new empty route.
            int count = 100;
            var customers = new List<int>();
            while (count > 0)
            {
                customers = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var route = new Route(customers, true, true);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(route.First); });

                // test removing last.
                Assert.Catch<InvalidOperationException>(() => { route.Remove(4); });

                // remove customers.
                while (customers.Count > 2)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var customer = customers[customerIdx];
                    if (customer != route.First &&
                        customer != 4)
                    {
                        customers.Remove(customer);

                        route.Remove(customer);

                        Assert.AreEqual(customers.Count, route.Count);
                        Assert.AreEqual(true, route.IsClosed);
                        Assert.AreEqual(true, route.IsLastFixed);
                        Assert.AreEqual(0, route.First);
                        Assert.AreEqual(Constants.NOT_SET, route.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterNotClosedNotFixed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3 }, false, false);

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
            Assert.AreEqual(false, route.IsClosed);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 1, route.Count);
                Assert.AreEqual(false, route.IsClosed);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(customer, route.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterNotClosedFixed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 100 }, false, true);

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
            //Assert.Catch<ArgumentOutOfRangeException>(() =>
            //{
            //    route.InsertAfter(10, 11);
            //});
            Assert.Catch<ArgumentException>(() =>
            {
                route.InsertAfter(100, 11);
            });

            // insert customers.
            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(false, route.IsClosed);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 2, route.Count);
                Assert.AreEqual(false, route.IsClosed);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(100, route.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterClosedNotFixed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3 }, true, false);

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
            Assert.AreEqual(true, route.IsClosed);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 1, route.Count);
                Assert.AreEqual(true, route.IsClosed);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(Constants.NOT_SET, route.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterClosedFixed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 100 }, true, true);

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
            //Assert.Catch<ArgumentOutOfRangeException>(() =>
            //{
            //    route.InsertAfter(10, 11);
            //});
            Assert.Catch<ArgumentException>(() =>
            {
                route.InsertAfter(100, 11);
            });

            // insert customers.
            Assert.AreEqual(5, route.Count);
            Assert.AreEqual(true, route.IsClosed);
            for (var customer = 4; customer < 100; customer++)
            {
                route.InsertAfter(customer - 1, customer);

                Assert.AreEqual(customer + 2, route.Count);
                Assert.AreEqual(true, route.IsClosed);
                Assert.AreEqual(0, route.First);
                Assert.AreEqual(Constants.NOT_SET, route.Last);
            }
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenNotClosed()
        {
            var route = new Route(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, false);

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
            var route = new Route(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

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
        /// Tests get neighbours.
        /// </summary>
        [Test]
        public void TestGetNeighboursNotClosed()
        {
            var count = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, false, false);

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
            var route = new Route(customers, true, false);

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
        public void TestShiftNotClosedNotFixed()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, false);

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
                    Assert.AreEqual(false, route.IsClosed);
                    Assert.AreEqual(0, route.First);

                    ExtraAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Test shifting customers.
        /// </summary>
        [Test]
        public void TestShiftNotClosedFixed()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, false, true);

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
                    Assert.AreEqual(false, route.IsClosed);
                    Assert.AreEqual(true, route.IsLastFixed);
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
        public void TestShiftClosedNotFixed()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, true);

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
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(Constants.NOT_SET, route.Last);

                    ExtraAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Test shifting customers.
        /// </summary>
        [Test]
        public void TestShiftClosedFixed()
        {
            StaticRandomGenerator.Set(116542346);

            // create a new empty route.
            var testCount = 10;
            var customers = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var route = new Route(customers, true, true);

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
                    Assert.AreEqual(true, route.IsClosed);
                    Assert.AreEqual(true, route.IsLastFixed);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(Constants.NOT_SET, route.Last);

                    ExtraAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }
    }
}