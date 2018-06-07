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

using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.Random;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours
{
    /// <summary>
    /// Contains tests for the tour.
    /// </summary>
    public class TourTests
    {
        /// <summary>
        /// Tests cloning a tour.
        /// </summary>
        [Fact]
        public void Tour_ClonesShouldBeIdentical()
        {
            var tour = new Tour(new [] { 0, 1, 3, 2, 4 }, 0);
            var cloned = tour.Clone() as Tour;

            Assert.NotNull(cloned);
            Assert.Equal(tour.First, cloned.First);
            Assert.Equal(tour.Last, cloned.Last);
            Assert.Equal(tour.Count, cloned.Count);

            ExtraAssert.ItemsEqual(tour, cloned);

            tour = new Tour(new [] { 0, 1, 3, 2, 4 }, 4);
            cloned = tour.Clone() as Tour;

            Assert.NotNull(cloned);
            Assert.Equal(tour.First, cloned.First);
            Assert.Equal(tour.Last, cloned.Last);
            Assert.Equal(tour.Count, cloned.Count);

            ExtraAssert.ItemsEqual(tour, cloned);

            tour = new Tour(new [] { 0, 1, 3, 2, 4 }, null);
            cloned = tour.Clone() as Tour;

            Assert.NotNull(cloned);
            Assert.Equal(tour.First, cloned.First);
            Assert.Equal(tour.Last, cloned.Last);
            Assert.Equal(tour.Count, cloned.Count);

            ExtraAssert.ItemsEqual(tour, cloned);
        }
        
        /// <summary>
        /// Tests clearing a tour.
        /// </summary>
        [Fact]
        public void Tour_ClearShouldClearButNotFirst()
        {
            var tour = new Tour(new [] { 0, 1, 3, 2, 4 }, 0);

            tour.Clear();

            Assert.Equal(1, tour.Count);
            Assert.Equal(0, tour.First);
            Assert.Equal(0, tour.Last);
        }

        /// <summary>
        /// Tests get visit at in a tour.
        /// </summary>
        [Fact]
        public void Tour_GetVisitAtShouldReturnNthVisit()
        {
            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            var tour = new Tour(tourEnumerable, 0);

            Assert.Equal(tourEnumerable[0], tour.GetVisitAt(0));
            Assert.Equal(tourEnumerable[1], tour.GetVisitAt(1));
            Assert.Equal(tourEnumerable[2], tour.GetVisitAt(2));
            Assert.Equal(tourEnumerable[3], tour.GetVisitAt(3));
            Assert.Equal(tourEnumerable[4], tour.GetVisitAt(4));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.GetVisitAt(-1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.GetVisitAt(5);
            });
        }

        /// <summary>
        /// Tests get visit at in a tour.
        /// </summary>
        [Fact]
        public void Tour_GetVisitAtThrowsWhenOutOfRange()
        {
            var tour = new Tour(new [] { 0, 1, 3, 2, 4 }, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.GetVisitAt(-1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.GetVisitAt(5);
            });
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Fact]
        public void Tour_ClosedConstructorShouldCreateClosedTour()
        {
            var tour = new Tour(new [] { 0, 1, 3, 2, 4 }, 0);

            Assert.Equal(5, tour.Count);
            Assert.Equal(0, tour.First);
            Assert.Equal(0, tour.Last);

            var solutionList = new List<int>(tour);
            Assert.Equal(0, solutionList[0]);
            Assert.Equal(1, solutionList[1]);
            Assert.Equal(3, solutionList[2]);
            Assert.Equal(2, solutionList[3]);
            Assert.Equal(4, solutionList[4]); 
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Fact]
        public void Tour_FixedEndConstructorShouldCreateFixedEndTour()
        {
            var tour = new Tour(new int[] { 0, 1, 3, 2, 4 }, 4);

            Assert.Equal(5, tour.Count);
            Assert.Equal(0, tour.First);
            Assert.Equal(4, tour.Last);

            var solutionList = new List<int>(tour);
            Assert.Equal(0, solutionList[0]);
            Assert.Equal(1, solutionList[1]);
            Assert.Equal(3, solutionList[2]);
            Assert.Equal(2, solutionList[3]);
            Assert.Equal(4, solutionList[4]);
        }

        /// <summary>
        /// Tests enumerable constructors.
        /// </summary>
        [Fact]
        public void Tour_OpenConstructorShouldCreateOpenTour()
        {
            var tourEnumerable = new [] { 0, 1, 3, 2, 4 };
            var tour = new Tour(tourEnumerable, null);

            Assert.Equal(5, tour.Count);
            Assert.Equal(0, tour.First);
            Assert.Null(tour.Last);

            var solutionList = new List<int>(tour);
            Assert.Equal(0, solutionList[0]);
            Assert.Equal(1, solutionList[1]);
            Assert.Equal(3, solutionList[2]);
            Assert.Equal(2, solutionList[3]);
            Assert.Equal(4, solutionList[4]);
        }

        /// <summary>
        /// Tests count.
        /// </summary>
        [Fact]
        public void Tour_CountShouldReturnNumberOfVisits()
        {
            var tour = new Tour(new [] { 0 }, 0);
            Assert.Equal(1, tour.Count);
            tour.InsertAfter(0, 1);
            Assert.Equal(2, tour.Count);
            tour.InsertAfter(1, 2);
            Assert.Equal(3, tour.Count);
            tour.InsertAfter(2, 3);
            Assert.Equal(4, tour.Count);
            tour.InsertAfter(3, 4);

            tour = new Tour(new [] { 0 }, null);
            Assert.Equal(1, tour.Count);
            tour.InsertAfter(0, 1);
            Assert.Equal(2, tour.Count);
            tour.InsertAfter(1, 2);
            Assert.Equal(3, tour.Count);
            tour.InsertAfter(2, 3);
            Assert.Equal(4, tour.Count);
            tour.InsertAfter(3, 4);
        }

        /// <summary>
        /// Tests the first and last count.
        /// </summary>
        [Fact]
        public void Tour_FirstAndLastShouldReturnFirstAndLast()
        {
            var tour = new Tour(new int[] { 0 }, 0);
            Assert.Equal(0, tour.First);
            Assert.Equal(0, tour.Last);
            tour.InsertAfter(0, 1);
            Assert.Equal(0, tour.First);
            Assert.Equal(0, tour.Last);

            tour = new Tour(new int[] { 0 }, null);
            Assert.Equal(0, tour.First);
            Assert.Null(tour.Last);
            tour.InsertAfter(0, 1);
            Assert.Equal(0, tour.First);
            Assert.Null(tour.Last);

            var tourEnumerable = new int[] { 0, 1, 3, 2, 4 };
            tour = new Tour(tourEnumerable, 4);

            Assert.Equal(0, tour.First);
            Assert.Equal(4, tour.Last);

            tour = new Tour(tourEnumerable, 0);

            Assert.Equal(0, tour.First);
            Assert.Equal(0, tour.Last);

            tour = new Tour(tourEnumerable, null);

            Assert.Equal(0, tour.First);
            Assert.Null(tour.Last);
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Fact]
        public void Tour_ContainsShouldReturnTrueWhenVisitsExistInClosed()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var tour = new Tour(tourEnumerable, 0);

            Assert.False(tour.Contains(10));

            Assert.True(tour.Contains(0));
            Assert.True(tour.Contains(1));
            Assert.True(tour.Contains(2));
            Assert.True(tour.Contains(3));
            Assert.True(tour.Contains(4));

            Assert.False(tour.Contains(8, 0));
            Assert.False(tour.Contains(4, 2));
            Assert.False(tour.Contains(3, 2));
            Assert.True(tour.Contains(0, 1));
            Assert.True(tour.Contains(1, 2));
            Assert.True(tour.Contains(2, 3));
            Assert.True(tour.Contains(3, 4));
            Assert.True(tour.Contains(4, 0));
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Fact]
        public void Tour_ContainsShouldReturnTrueWhenVisitsExistInOpen()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var tour = new Tour(tourEnumerable, null);

            Assert.False(tour.Contains(10));

            Assert.True(tour.Contains(0));
            Assert.True(tour.Contains(1));
            Assert.True(tour.Contains(2));
            Assert.True(tour.Contains(3));
            Assert.True(tour.Contains(4));

            Assert.False(tour.Contains(8, 0));
            Assert.False(tour.Contains(4, 2));
            Assert.False(tour.Contains(3, 2));
            Assert.True(tour.Contains(0, 1));
            Assert.True(tour.Contains(0, 1));
            Assert.True(tour.Contains(1, 2));
            Assert.True(tour.Contains(2, 3));
            Assert.True(tour.Contains(3, 4));
            Assert.False(tour.Contains(4, 0));
        }

        /// <summary>
        /// Tests contains.
        /// </summary>
        [Fact]
        public void Tour_ContainsShouldReturnTrueWhenVisitsExistInFixedEnd()
        {
            var tourEnumerable = new int[] { 0, 1, 2, 3, 4 };
            var tour = new Tour(tourEnumerable, 4);

            Assert.False(tour.Contains(10));

            Assert.True(tour.Contains(0));
            Assert.True(tour.Contains(1));
            Assert.True(tour.Contains(2));
            Assert.True(tour.Contains(3));
            Assert.True(tour.Contains(4));

            Assert.False(tour.Contains(8, 0));
            Assert.False(tour.Contains(4, 2));
            Assert.False(tour.Contains(3, 2));
            Assert.True(tour.Contains(0, 1));
            Assert.True(tour.Contains(1, 2));
            Assert.True(tour.Contains(2, 3));
            Assert.True(tour.Contains(3, 4));
            Assert.False(tour.Contains(4, 0));
        }

        /// <summary>
        /// Test removing visits.
        /// </summary>
        [Fact]
        public void Tour_RemoveShouldRemoveInClosed()
        {
            // create a new empty tour.
            var count = 100;
            while (count > 0)
            {
                var visits = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var tour = new Tour(visits, 0);

                // test removing first.
                Assert.Throws<InvalidOperationException>(() => { tour.Remove(tour.First); });

                // remove visits.
                while (visits.Count > 1)
                {
                    var visitIdx = RandomGenerator.Default.Generate(
                        visits.Count);
                    var visit = visits[visitIdx];
                    if (visit == tour.First) continue;
                    visits.Remove(visit);

                    tour.Remove(visit);

                    Assert.Equal(visits.Count, tour.Count);
                    Assert.Equal(0, tour.First);
                    Assert.Equal(0, tour.Last);
                }
                count--;
            }
        }
        
        /// <summary>
        /// Test removing visits.
        /// </summary>
        [Fact]
        public void Tour_RemoveShouldRemoveInOpen()
        {
            // create a new empty tour.
            var count = 100;
            while (count > 0)
            {
                var visits = new List<int>(new [] { 0, 1, 2, 3, 4 });
                var tour = new Tour(visits, null);

                // test removing first.
                Assert.Throws<InvalidOperationException>(() => { tour.Remove(tour.First); });

                // remove visits.
                while (visits.Count > 2)
                {
                    var visitIdx = RandomGenerator.Default.Generate(
                        visits.Count);
                    var visit = visits[visitIdx];
                    if (visit == tour.First || visit == tour.Last) continue;
                    visits.Remove(visit);

                    tour.Remove(visit);

                    Assert.Equal(visits.Count, tour.Count);
                    Assert.Equal(0, tour.First);
                    Assert.Null(tour.Last);
                }
                count--;
            }
        }

        /// <summary>
        /// Test removing visits.
        /// </summary>
        [Fact]
        public void Tour_RemoveShouldRemoveInFixed()
        {
            // create a new empty tour.
            var count = 100;
            while (count > 0)
            {
                var visits = new List<int>(new int[] { 0, 1, 2, 3, 4 });
                var tour = new Tour(visits, 4);

                // test removing first.
                Assert.Throws<InvalidOperationException>(() => { tour.Remove(tour.First); });

                // test removing first.
                Assert.Throws<InvalidOperationException>(() => { tour.Remove(tour.Last.Value); });

                // remove visits.
                while (visits.Count > 2)
                {
                    var visitIdx = RandomGenerator.Default.Generate(
                        visits.Count);
                    var visit = visits[visitIdx];
                    if (visit == tour.First || visit == tour.Last) continue;
                    visits.Remove(visit);

                    tour.Remove(visit);

                    Assert.Equal(visits.Count, tour.Count);
                    Assert.Equal(0, tour.First);
                    Assert.Equal(4, tour.Last);
                }
                count--;
            }
        }

        /// <summary>
        /// Tests insert after throws when out of range.
        /// </summary>
        [Fact]
        public void Tour_InsertAfterThrowsWhenOutOfRange()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3 }, null);

            // test arguments.
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(0, -1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(-1, 0);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                tour.InsertAfter(1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                tour.InsertAfter(10, 11);
            });
        }

        /// <summary>    
        /// Tests insert after.
        /// </summary>
        [Fact]
        public void Tour_InsertAfterInsertsAfterOpen()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3 }, null);

            // insert visits.
            Assert.Equal(4, tour.Count);
            for (var visit = 4; visit < 100; visit++)
            {
                tour.InsertAfter(visit - 1, visit);

                Assert.Equal(visit + 1, tour.Count);
                Assert.Equal(0, tour.First);
                Assert.Null(tour.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Fact]
        public void Tour_InsertAfterInsertsAfterFixed()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 100 }, 0);

            // insert visits.
            Assert.Equal(5, tour.Count);
            for (var visit = 4; visit < 100; visit++)
            {
                tour.InsertAfter(visit - 1, visit);

                Assert.Equal(visit + 2, tour.Count);
                Assert.Equal(0, tour.First);
                Assert.Equal(0, tour.Last);
            }
        }

        /// <summary>
        /// Tests insert after.
        /// </summary>
        [Fact]
        public void Tour_InsertAfterInsertsAfterlosed()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3 }, 0);
            // insert visits.
            Assert.Equal(4, tour.Count);
            for (var visit = 4; visit < 100; visit++)
            {
                tour.InsertAfter(visit - 1, visit);

                Assert.Equal(visit + 1, tour.Count);
                Assert.Equal(0, tour.First);
                Assert.Equal(0, tour.Last);
            }
        }

        /// <summary>
        /// Tests insert after in a tour with one visit.
        /// </summary>
        [Fact]
        public void Tour_InsertAfterClosedOneVisitShouldStayClosed()
        {
            var tour = new Tour(new [] { 0 }, 0);

            // insert visit.
            tour.InsertAfter(0, 1);
            Assert.Equal(new [] { 0, 1 }, tour);
        }
        
        /// <summary>
        /// Tests enumeration of a tour.
        /// </summary>
        [Fact]
        public void Tour_EnumerateOpenShouldEnumerateUntilLast()
        {
            var list = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(list, null);
            var enumeratedRoute = new List<int>(tour);

            Assert.Equal(list.ToArray(), enumeratedRoute);

            var start = 0;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 1;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 2;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 3;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 4;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 5;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 6;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 7;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 8;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 9;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
        }

        /// <summary>
        /// Tests enumeration of a tour.
        /// </summary>
        [Fact]
        public void Tour_EnumerateClosedShouldEnumerateUntilLast()
        {
            var list = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(list, 0);
            var enumeratedRoute = new List<int>(tour);

            Assert.Equal(list, enumeratedRoute);

            var start = 0;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 1;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 2;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 3;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 4;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 5;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 6;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 7;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 8;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 9;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
        }

        /// <summary>
        /// Tests enumeration of a tour.
        /// </summary>
        [Fact]
        public void Tour_EnumerateFixedShouldEnumerateUntilLast()
        {
            var list = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(list, 9);
            var enumeratedRoute = new List<int>(tour);

            Assert.Equal(list, enumeratedRoute);

            var start = 0;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 1;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 2;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 3;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 4;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 5;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 6;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 7;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 8;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
            start = 9;
            enumeratedRoute = tour.GetEnumerator(start).ToList();
            Assert.Equal(list.GetRange(start, list.Count - start).ToArray(), enumeratedRoute.ToArray());
        }
        
        /// <summary>
        /// Tests all enumerations of a tour.
        /// </summary>
        [Fact]
        public void Tour_EnumerateOpenBetweenShouldEnumerateBetween()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, null);

            for (var from = 0; from < tour.Count; from++)
            {
                for (var to = 0; to < tour.Count; to++)
                {
                    using (var enumerator = tour.Between(from, to).GetEnumerator())
                    {
                        if (from <= to)
                        {
                            for (var visit = from; visit < to + 1; visit++)
                            {
                                Assert.True(enumerator.MoveNext());
                                Assert.Equal(visit, enumerator.Current);
                            }
                        }

                        Assert.False(enumerator.MoveNext());
                    }
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a tour.
        /// </summary>
        [Fact]
        public void Tour_EnumerateClosedBetweenShouldEnumerateBetween()
        {
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 0);

            for (var from = 0; from < tour.Count; from++)
            {
                for (var to = 0; to < tour.Count; to++)
                {
                    using (var enumerator = tour.Between(from, to).GetEnumerator())
                    {
                        if (from <= to)
                        {
                            for (var visit = from; visit < to + 1; visit++)
                            {
                                Assert.True(enumerator.MoveNext());
                                Assert.Equal(visit, enumerator.Current);
                            }
                        }
                        else
                        {
                            for (var visit = from; visit < tour.Count; visit++)
                            {
                                Assert.True(enumerator.MoveNext());
                                Assert.Equal(visit, enumerator.Current);
                            }

                            for (var visit = 0; visit < to + 1; visit++)
                            {
                                Assert.True(enumerator.MoveNext());
                                Assert.Equal(visit, enumerator.Current);
                            }
                        }

                        Assert.False(enumerator.MoveNext());
                    }
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a tour.
        /// </summary>
        [Fact]
        public void Tour_EnumerateFixedBetweenShouldEnumerateBetween()
        {
            var tour = new Tour(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 9);

            for (var from = 0; from < tour.Count; from++)
            {
                for (var to = 0; to < tour.Count; to++)
                {
                    using (var enumerator = tour.Between(from, to).GetEnumerator())
                    {
                        if (from <= to)
                        {
                            for (var visit = from; visit < to + 1; visit++)
                            {
                                Assert.True(enumerator.MoveNext());
                                Assert.Equal(visit, enumerator.Current);
                            }
                        }

                        Assert.False(enumerator.MoveNext());
                    }
                }
            }
        }

        /// <summary>
        /// Tests get neighbours.
        /// </summary>
        [Fact]
        public void Tour_GetNeigboursShouldGetNeighboursOnOpen()
        {
            const int count = 10;
            var visits = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(visits, null);

            var neighbour = Tour.NOT_SET;
            for (var visit = 0; visit < count - 1; visit++)
            {
                neighbour = tour.GetNeigbour(visit);
                Assert.Equal(visit + 1, neighbour);
            }
            neighbour = tour.GetNeigbour(count - 1);
            Assert.Equal(Tour.NOT_SET, neighbour);
        }

        /// <summary>
        /// Tests get neighbours.
        /// </summary>
        [Fact]
        public void Tour_GetNeigboursShouldGetNeighboursOnClosed()
        {
            const int count = 10;
            var visits = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(visits, 0);

            var neighbour = Tour.NOT_SET;
            for (var visit = 0; visit < count - 1; visit++)
            {
                neighbour = tour.GetNeigbour(visit);
                Assert.Equal(visit + 1, neighbour);
            }
            neighbour = tour.GetNeigbour(count - 1);
            Assert.Equal(0, neighbour);
        }

        /// <summary>
        /// Test shifting visits.
        /// </summary>
        [Fact]
        public void Tour_ShiftShouldFixedOnFixed()
        {
            var random = new DefaultRandomGenerator(116542346);

            // create a new empty tour.
            const int testCount = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(visits, 9);

            // remove visits.
            var testIdx = 0;
            while (testIdx < testCount)
            {
                var visitIdx = random.Generate(
                    visits.Count);
                var insertIdx = random.Generate(
                    visits.Count - 2);
                if (visitIdx <= insertIdx)
                {
                    insertIdx = insertIdx + 1;
                }

                var visit = visits[visitIdx];
                var insert = visits[insertIdx];
                if (visit == tour.First || visit == tour.Last) continue;
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

                Assert.Equal(visits.Count, tour.Count);
                Assert.Equal(0, tour.First);
                Assert.Equal(9, tour.Last);

                ExtraAssert.ItemsEqual(visits, tour);

                testIdx++;
            }
        }
        
        /// <summary>
        /// Test shifting visits.
        /// </summary>
        [Fact]
        public void Tour_ShiftShouldFixedOnClosed()
        {
            var random = new DefaultRandomGenerator(116542346);

            // create a new empty tour.
            const int testCount = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(visits, 0);

            // remove visits.
            var testIdx = 0;
            while (testIdx < testCount)
            {
                var visitIdx = random.Generate(visits.Count);
                var insertIdx = random.Generate(visits.Count - 2);
                if (visitIdx <= insertIdx)
                {
                    insertIdx = insertIdx + 1;
                }

                var visit = visits[visitIdx];
                var insert = visits[insertIdx];
                if (visit == tour.First) continue;
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

                Assert.Equal(visits.Count, tour.Count);
                Assert.Equal(0, tour.First);
                Assert.Equal(0, tour.Last);

                ExtraAssert.ItemsEqual(visits, tour);

                testIdx++;
            }
        }

        /// <summary>
        /// Test shifting visits.
        /// </summary>
        [Fact]
        public void Tour_ShiftShouldFixedOnOpen()
        {
            var random = new DefaultRandomGenerator(116542346);

            // create a new empty tour.
            const int testCount = 10;
            var visits = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(visits, null);

            // remove visits.
            int testIdx = 0;
            while (testIdx < testCount)
            {
                var visitIdx = random.Generate(visits.Count);
                var insertIdx = random.Generate(visits.Count - 2);
                if (visitIdx <= insertIdx)
                {
                    insertIdx = insertIdx + 1;
                }

                var visit = visits[visitIdx];
                var insert = visits[insertIdx];
                if (visit == tour.First || visit == tour.Last) continue;
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

                Assert.Equal(visits.Count, tour.Count);
                Assert.Equal(0, tour.First);
                Assert.Null(tour.Last);

                ExtraAssert.ItemsEqual(visits, tour);

                testIdx++;
            }
        }

        /// <summary>
        /// Tests replace.
        /// </summary>
        [Fact]
        public void Tour_ReplaceShouldReplaceVisits()
        {
            var visits = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var tour = new Tour(visits, 0);

            tour.Replace(0, 10);

            ExtraAssert.ItemsEqual(new [] { 10, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, tour);

            tour.Replace(5, 15);

            ExtraAssert.ItemsEqual(new [] { 10, 1, 2, 3, 4, 15, 6, 7, 8, 9 }, tour);

            tour.Replace(9, 19);

            ExtraAssert.ItemsEqual(new [] { 10, 1, 2, 3, 4, 15, 6, 7, 8, 19 }, tour);
        }

        /// <summary>
        /// Tests if the the presence of a fixed last visit is checked properly.
        /// </summary>
        [Fact]
        public void Tour_LastHasToMatchOrThrow()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Tour(new int[] { 0, 1 }, 2);
            });
        }
    }
}