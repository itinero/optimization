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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Tours;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Itinero.Optimization.Test.Tours
{
    /// <summary>
    /// Tests for tour.
    /// </summary>
    [TestFixture]
    public class TourTest
    {
        /// <summary>
        /// Tests cloning a tour.
        /// </summary>
        [Test]
        public void TestClone()
        {
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 0);
            var cloned = tour.Clone() as Optimization.Tours.Tour;

            Assert.AreEqual(tour.First, cloned.First);
            Assert.AreEqual(tour.Last, cloned.Last);
            Assert.AreEqual(tour.Count, cloned.Count);

            ExtraAssert.ItemsAreEqual(tour, cloned);

            tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            tour = new Optimization.Tours.Tour(tourEnumerable, 4);
            cloned = tour.Clone() as Optimization.Tours.Tour;

            Assert.AreEqual(tour.First, cloned.First);
            Assert.AreEqual(tour.Last, cloned.Last);
            Assert.AreEqual(tour.Count, cloned.Count);

            ExtraAssert.ItemsAreEqual(tour, cloned);

            tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            tour = new Optimization.Tours.Tour(tourEnumerable, null);
            cloned = tour.Clone() as Optimization.Tours.Tour;

            Assert.AreEqual(tour.First, cloned.First);
            Assert.AreEqual(tour.Last, cloned.Last);
            Assert.AreEqual(tour.Count, cloned.Count);

            ExtraAssert.ItemsAreEqual(tour, cloned);
        }

        /// <summary>
        /// Tests clearing a tour.
        /// </summary>
        [Test]
        public void TestClear()
        {
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 0);

            tour.Clear();

            Assert.AreEqual(1, tour.Count);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(0, tour.Last);
        }

        /// <summary>
        /// Tests get visit at in a tour.
        /// </summary>
        [Test]
        public void TestGetVisitAt()
        {
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 0);

            Assert.AreEqual(tourEnumerable[0], tour.GetVisitAt(0));
            Assert.AreEqual(tourEnumerable[1], tour.GetVisitAt(1));
            Assert.AreEqual(tourEnumerable[2], tour.GetVisitAt(2));
            Assert.AreEqual(tourEnumerable[3], tour.GetVisitAt(3));
            Assert.AreEqual(tourEnumerable[4], tour.GetVisitAt(4));

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.GetVisitAt(-1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.GetVisitAt(5);
            });
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Test]
        public void TestEnumerableConstructorClosed()
        {
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 0);

            Assert.AreEqual(5, tour.Count);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(0, tour.Last);

            var solutionList = new List<int>(tour);
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
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 4);

            Assert.AreEqual(5, tour.Count);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(4, tour.Last);

            var solutionList = new List<int>(tour);
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
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, null);

            Assert.AreEqual(5, tour.Count);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(null, tour.Last);

            var solutionList = new List<int>(tour);
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
            var tour = new Optimization.Tours.Tour(new int[] { 0 }, 0);
            Assert.AreEqual(1, tour.Count);
            tour.InsertAfter(0, 1);
            Assert.AreEqual(2, tour.Count);
            tour.InsertAfter(1, 2);
            Assert.AreEqual(3, tour.Count);
            tour.InsertAfter(2, 3);
            Assert.AreEqual(4, tour.Count);
            tour.InsertAfter(3, 4);

            tour = new Optimization.Tours.Tour(new int[] { 0 }, null);
            Assert.AreEqual(1, tour.Count);
            tour.InsertAfter(0, 1);
            Assert.AreEqual(2, tour.Count);
            tour.InsertAfter(1, 2);
            Assert.AreEqual(3, tour.Count);
            tour.InsertAfter(2, 3);
            Assert.AreEqual(4, tour.Count);
            tour.InsertAfter(3, 4);
        }

        /// <summary>
        /// Tests the first and last count.
        /// </summary>
        [Test]
        public void TestFirstAndLast()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0 }, 0);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(0, tour.Last);
            tour.InsertAfter(0, 1);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(0, tour.Last);

            tour = new Optimization.Tours.Tour(new int[] { 0 }, null);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(null, tour.Last);
            tour.InsertAfter(0, 1);
            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(null, tour.Last);

            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            tour = new Optimization.Tours.Tour(tourEnumerable, 4);

            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(4, tour.Last);

            tour = new Optimization.Tours.Tour(tourEnumerable, 0);

            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(0, tour.Last);

            tour = new Optimization.Tours.Tour(tourEnumerable, null);

            Assert.AreEqual(0, tour.First);
            Assert.AreEqual(null, tour.Last);
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsClosed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 0);

            Assert.IsFalse(tour.Contains(10));

            Assert.IsTrue(tour.Contains(0));
            Assert.IsTrue(tour.Contains(1));
            Assert.IsTrue(tour.Contains(2));
            Assert.IsTrue(tour.Contains(3));
            Assert.IsTrue(tour.Contains(4));

            Assert.IsFalse(tour.Contains(8, 0));
            Assert.IsFalse(tour.Contains(4, 2));
            Assert.IsFalse(tour.Contains(3, 2));
            Assert.IsTrue(tour.Contains(0, 1));
            Assert.IsTrue(tour.Contains(1, 2));
            Assert.IsTrue(tour.Contains(2, 3));
            Assert.IsTrue(tour.Contains(3, 4));
            Assert.IsTrue(tour.Contains(4, 0));
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsOpen()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, null);

            Assert.IsFalse(tour.Contains(10));

            Assert.IsTrue(tour.Contains(0));
            Assert.IsTrue(tour.Contains(1));
            Assert.IsTrue(tour.Contains(2));
            Assert.IsTrue(tour.Contains(3));
            Assert.IsTrue(tour.Contains(4));

            Assert.IsFalse(tour.Contains(8, 0));
            Assert.IsFalse(tour.Contains(4, 2));
            Assert.IsFalse(tour.Contains(3, 2));
            Assert.IsTrue(tour.Contains(0, 1));
            Assert.IsTrue(tour.Contains(1, 2));
            Assert.IsTrue(tour.Contains(2, 3));
            Assert.IsTrue(tour.Contains(3, 4));
            Assert.IsFalse(tour.Contains(4, 0));
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Test]
        public void TestContainsFixed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var tour = new Optimization.Tours.Tour(tourEnumerable, 4);

            Assert.IsFalse(tour.Contains(10));

            Assert.IsTrue(tour.Contains(0));
            Assert.IsTrue(tour.Contains(1));
            Assert.IsTrue(tour.Contains(2));
            Assert.IsTrue(tour.Contains(3));
            Assert.IsTrue(tour.Contains(4));

            Assert.IsFalse(tour.Contains(8, 0));
            Assert.IsFalse(tour.Contains(4, 2));
            Assert.IsFalse(tour.Contains(3, 2));
            Assert.IsTrue(tour.Contains(0, 1));
            Assert.IsTrue(tour.Contains(1, 2));
            Assert.IsTrue(tour.Contains(2, 3));
            Assert.IsTrue(tour.Contains(3, 4));
            Assert.IsFalse(tour.Contains(4, 0));
        }

        /// <summary>
        /// Test removing visits.
        /// </summary>
        [Test]
        public void TestRemoveClosed()
        {
            // create a new empty tour.
            int count = 100;
            var visits = new List<int>();
            while (count > 0)
            {
                visits = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var tour = new Optimization.Tours.Tour(visits, 0);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { tour.Remove(tour.First); });

                // remove visits.
                while (visits.Count > 1)
                {
                    var visitIdx = RandomGeneratorExtensions.GetRandom().Generate(
                        visits.Count);
                    var visit = visits[visitIdx];
                    if (visit != tour.First)
                    {
                        visits.Remove(visit);

                        tour.Remove(visit);

                        Assert.AreEqual(visits.Count, tour.Count);
                        Assert.AreEqual(0, tour.First);
                        Assert.AreEqual(0, tour.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing visits.
        /// </summary>
        [Test]
        public void TestRemoveOpen()
        {
            // create a new empty tour.
            int count = 100;
            var visits = new List<int>();
            while (count > 0)
            {
                visits = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var tour = new Optimization.Tours.Tour(visits, null);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { tour.Remove(tour.First); });

                // remove visits.
                while (visits.Count > 2)
                {
                    var visitIdx = RandomGeneratorExtensions.GetRandom().Generate(
                        visits.Count);
                    var visit = visits[visitIdx];
                    if (visit != tour.First &&
                        visit != tour.Last)
                    {
                        visits.Remove(visit);

                        tour.Remove(visit);

                        Assert.AreEqual(visits.Count, tour.Count);
                        Assert.AreEqual(0, tour.First);
                        Assert.AreEqual(null, tour.Last);
                    }
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing visits.
        /// </summary>
        [Test]
        public void TestRemoveFixed()
        {
            // create a new empty tour.
            int count = 100;
            var visits = new List<int>();
            while (count > 0)
            {
                visits = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var tour = new Optimization.Tours.Tour(visits, 4);

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { tour.Remove(tour.First); });

                // test removing first.
                Assert.Catch<InvalidOperationException>(() => { tour.Remove(tour.Last.Value); });

                // remove visits.
                while (visits.Count > 2)
                {
                    var visitIdx = RandomGeneratorExtensions.GetRandom().Generate(
                        visits.Count);
                    var visit = visits[visitIdx];
                    if (visit != tour.First &&
                        visit != tour.Last)
                    {
                        visits.Remove(visit);

                        tour.Remove(visit);

                        Assert.AreEqual(visits.Count, tour.Count);
                        Assert.AreEqual(0, tour.First);
                        Assert.AreEqual(4, tour.Last);
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
            var tour = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3 }, null);

            // test arguments.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(0, -1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(-1, 0);
            });
            Assert.Catch<ArgumentException>(() =>
            {
                tour.InsertAfter(1, 1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(10, 11);
            });

            // insert visits.
            Assert.AreEqual(4, tour.Count);
            for (var visit = 4; visit < 100; visit++)
            {
                tour.InsertAfter(visit - 1, visit);

                Assert.AreEqual(visit + 1, tour.Count);
                Assert.AreEqual(0, tour.First);
                Assert.AreEqual(null, tour.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterFixed()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 100 }, 0);

            // test arguments.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(0, -1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(-1, 0);
            });
            Assert.Catch<ArgumentException>(() =>
            {
                tour.InsertAfter(1, 1);
            });

            // insert visits.
            Assert.AreEqual(5, tour.Count);
            for (var visit = 4; visit < 100; visit++)
            {
                tour.InsertAfter(visit - 1, visit);

                Assert.AreEqual(visit + 2, tour.Count);
                Assert.AreEqual(0, tour.First);
                Assert.AreEqual(0, tour.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Test]
        public void TestInsertAfterClosed()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3 }, 0);

            // test arguments.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(0, -1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(-1, 0);
            });
            Assert.Catch<ArgumentException>(() =>
            {
                tour.InsertAfter(1, 1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(10, 11);
            });

            // insert visits.
            Assert.AreEqual(4, tour.Count);
            for (var visit = 4; visit < 100; visit++)
            {
                tour.InsertAfter(visit - 1, visit);

                Assert.AreEqual(visit + 1, tour.Count);
                Assert.AreEqual(0, tour.First);
                Assert.AreEqual(0, tour.Last);
            }
        }

        /// <summary>
        /// Tests insert after in a tour with one visit.
        /// </summary>
        [Test]
        public void TestInsertAfterClosedOneVisit()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0 }, 0);

            // test arguments.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(0, -1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(-1, 0);
            });
            Assert.Catch<ArgumentException>(() =>
            {
                tour.InsertAfter(1, 1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(10, 11);
            });

            // insert visit.
            tour.InsertAfter(0, 1);
            Assert.AreEqual(new [] { 0, 1 }, tour);
        }

        /// <summary>
        /// Tests enumeration of a tour.
        /// </summary>
        [Test]
        public void TestEnumerateOpen()
        {
            var list = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(list, null);
            var enumeratedRoute = new List<int>(tour);

            Assert.AreEqual(list.ToArray(), enumeratedRoute);

            var start = 0;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 1;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 2;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 3;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 4;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 5;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 6;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 7;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 8;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 9;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
        }

        /// <summary>
        /// Tests enumeration of a tour.
        /// </summary>
        [Test]
        public void TestEnumerateClosed()
        {
            var list = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(list, 0);
            var enumeratedRoute = new List<int>(tour);

            Assert.AreEqual(list, enumeratedRoute);

            var start = 0;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 1;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 2;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 3;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 4;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 5;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 6;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 7;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 8;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 9;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
        }

        /// <summary>
        /// Tests enumeration of a tour.
        /// </summary>
        [Test]
        public void TestEnumerateFixed()
        {
            var list = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(list, 9);
            var enumeratedRoute = new List<int>(tour);

            Assert.AreEqual(list, enumeratedRoute);

            var start = 0;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 1;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 2;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 3;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 4;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 5;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 6;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 7;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 8;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 9;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.AreEqual(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
        }

        /// <summary>
        /// Tests all enumerations of a tour.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenOpen()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, null);

            for (var from = 0; from < tour.Count; from++)
            {
                for (var to = 0; to < tour.Count; to++)
                {
                    var enumerator = tour.Between(from, to).GetEnumerator();
                    if (from <= to)
                    {
                        for (var visit = from; visit < to + 1; visit++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(visit, enumerator.Current);
                        }
                    }
                    Assert.IsFalse(enumerator.MoveNext());
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a tour.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenClosed()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 0);

            for (var from = 0; from < tour.Count; from++)
            {
                for (var to = 0; to < tour.Count; to++)
                {
                    var enumerator = tour.Between(from, to).GetEnumerator();
                    if (from <= to)
                    {
                        for (var visit = from; visit < to + 1; visit++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(visit, enumerator.Current);
                        }
                    }
                    else
                    {
                        for (var visit = from; visit < tour.Count; visit++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(visit, enumerator.Current);
                        }
                        for (var visit = 0; visit < to + 1; visit++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(visit, enumerator.Current);
                        }
                    }
                    Assert.IsFalse(enumerator.MoveNext());
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a tour.
        /// </summary>
        [Test]
        public void TestEnumerateBetweenFixed()
        {
            var tour = new Optimization.Tours.Tour(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 9);

            for (var from = 0; from < tour.Count; from++)
            {
                for (var to = 0; to < tour.Count; to++)
                {
                    var enumerator = tour.Between(from, to).GetEnumerator();
                    if (from <= to)
                    {
                        for (var visit = from; visit < to + 1; visit++)
                        {
                            Assert.IsTrue(enumerator.MoveNext());
                            Assert.AreEqual(visit, enumerator.Current);
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
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(visits, null);

            int neighbour = Constants.NOT_SET;
            for (int visit = 0; visit < count - 1; visit++)
            {
                neighbour = tour.GetNeigbour(visit);
                Assert.AreEqual(visit + 1, neighbour);
            }
            neighbour = tour.GetNeigbour(count - 1);
            Assert.AreEqual(Constants.NOT_SET, neighbour);
        }

        /// <summary>
        /// Tests get neighbours.
        /// </summary>
        [Test]
        public void TestGetNeighboursClosed()
        {
            var count = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(visits, 0);

            var neighbour = Constants.NOT_SET;
            for (int visit = 0; visit < count - 1; visit++)
            {
                neighbour = tour.GetNeigbour(visit);
                Assert.AreEqual(visit + 1, neighbour);
            }
            neighbour = tour.GetNeigbour(count - 1);
            Assert.AreEqual(0, neighbour);
        }

        /// <summary>
        /// Test shifting visits.
        /// </summary>
        [Test]
        public void TestShiftFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(116542346);

            // create a new empty tour.
            var testCount = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(visits, 9);

            // remove visits.
            int testIdx = 0;
            while (testIdx < testCount)
            {
                var visitIdx = RandomGeneratorExtensions.GetRandom().Generate(
                    visits.Count);
                var insertIdx = RandomGeneratorExtensions.GetRandom().Generate(
                    visits.Count - 2);
                if (visitIdx <= insertIdx)
                {
                    insertIdx = insertIdx + 1;
                }

                var visit = visits[visitIdx];
                var insert = visits[insertIdx];
                if (visit != tour.First &&
                    visit != tour.Last)
                {
                    if (visitIdx < insertIdx)
                    {
                        visits.Insert(insertIdx + 1, visit);
                        visits.RemoveAt(visitIdx);
                    }
                    else
                    {
                        visits.RemoveAt(visitIdx);
                        visits.Insert(insertIdx + 1, visit);
                    }

                    tour.ShiftAfter(visit, insert);

                    Assert.AreEqual(visits.Count, tour.Count);
                    Assert.AreEqual(0, tour.First);
                    Assert.AreEqual(9, tour.Last);

                    ExtraAssert.ItemsAreEqual(visits, tour);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Test shifting visits.
        /// </summary>
        [Test]
        public void TestShiftClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(116542346);

            // create a new empty tour.
            var testCount = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(visits, 0);

            // remove visits.
            int testIdx = 0;
            while (testIdx < testCount)
            {
                var visitIdx = RandomGeneratorExtensions.GetRandom().Generate(
                    visits.Count);
                var insertIdx = RandomGeneratorExtensions.GetRandom().Generate(
                    visits.Count - 2);
                if (visitIdx <= insertIdx)
                {
                    insertIdx = insertIdx + 1;
                }

                var visit = visits[visitIdx];
                var insert = visits[insertIdx];
                if (visit != tour.First)
                {
                    if (visitIdx < insertIdx)
                    {
                        visits.Insert(insertIdx + 1, visit);
                        visits.RemoveAt(visitIdx);
                    }
                    else
                    {
                        visits.RemoveAt(visitIdx);
                        visits.Insert(insertIdx + 1, visit);
                    }

                    tour.ShiftAfter(visit, insert);

                    Assert.AreEqual(visits.Count, tour.Count);
                    Assert.AreEqual(0, tour.First);
                    Assert.AreEqual(0, tour.Last);

                    ExtraAssert.ItemsAreEqual(visits, tour);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Test shifting visits.
        /// </summary>
        [Test]
        public void TestShiftOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(116542346);

            // create a new empty tour.
            var testCount = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(visits, null);

            // remove visits.
            int testIdx = 0;
            while (testIdx < testCount)
            {
                var visitIdx = RandomGeneratorExtensions.GetRandom().Generate(
                    visits.Count);
                var insertIdx = RandomGeneratorExtensions.GetRandom().Generate(
                    visits.Count - 2);
                if (visitIdx <= insertIdx)
                {
                    insertIdx = insertIdx + 1;
                }

                var visit = visits[visitIdx];
                var insert = visits[insertIdx];
                if (visit != tour.First &&
                    visit != tour.Last)
                {
                    if (visitIdx < insertIdx)
                    {
                        visits.Insert(insertIdx + 1, visit);
                        visits.RemoveAt(visitIdx);
                    }
                    else
                    {
                        visits.RemoveAt(visitIdx);
                        visits.Insert(insertIdx + 1, visit);
                    }

                    tour.ShiftAfter(visit, insert);

                    Assert.AreEqual(visits.Count, tour.Count);
                    Assert.AreEqual(0, tour.First);
                    Assert.AreEqual(null, tour.Last);

                    ExtraAssert.ItemsAreEqual(visits, tour);

                    testIdx++;
                }
            }
        }

        /// <summary>
        /// Tests replace.
        /// </summary>
        [Test]
        public void TestReplaceClosed()
        {
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Optimization.Tours.Tour(visits, 0);

            tour.Replace(0, 10);

            ExtraAssert.ItemsAreEqual(new int[] { 10, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, tour);

            tour.Replace(5, 15);

            ExtraAssert.ItemsAreEqual(new int[] { 10, 1, 2, 3, 4, 15, 6, 7, 8, 9 }, tour);

            tour.Replace(9, 19);

            ExtraAssert.ItemsAreEqual(new int[] { 10, 1, 2, 3, 4, 15, 6, 7, 8, 19 }, tour);
        }

        /// <summary>
        /// Tests if the the presence of a fixed last visit is checked properly.
        /// </summary>
        [Test]
        public void TestLastCheck()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new Tour(new int[] { 0, 1 }, 2);
            });
        }
    }
}
