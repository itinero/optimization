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
using System.Runtime.CompilerServices;
using Itinero.LocalGeo;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Functional")]
namespace Itinero.Optimization.Solvers.Tours.Hull
{
    /// <summary>
    /// The quick hull algorithm.
    /// </summary>
    public static class QuickHull
    {
        /// <summary>
        /// Calculates a convex hull.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="locationFunc">A function that returns the location per visit.</param>
        /// <returns>A hull with meta-data.</returns>
        public static TourHull ConvexHull(this IEnumerable<int> tour, Func<int, Coordinate> locationFunc)
        {
            var hull = new TourHull();

            // calculate most left and most right locations and build locations list.
            var locations = new List<(Coordinate location, int visit)>();
            float minLon = float.MaxValue, maxLon = float.MinValue;
            var left = -1;
            var right = -1;
            var count = 0;
            foreach (var visit in tour)
            {
                count++;
                
                var location = locationFunc(visit);
                locations.Add((location, visit));

                if (minLon > location.Longitude)
                {
                    minLon = location.Longitude;
                    left = locations.Count - 1;
                }

                if (maxLon < location.Longitude)
                {
                    maxLon = location.Longitude;
                    right = locations.Count - 1;
                }
            }

            if (count == 0) return hull;
            if (count == 1)
            {
                hull.Add(locations[0]);
                return hull;
            } 

            ConvexHull(hull, locations, left, right);

            return hull;
        }
        
        /// <summary>
        /// Calculates a convex hull.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="locationFunc">A function that returns the location per visit.</param>
        /// <returns>A hull with meta-data.</returns>
        public static TourHull ConvexHull(this Tour tour, Func<int, Coordinate> locationFunc)
        {
            var hull = new TourHull();
            if (tour.Count == 0) return hull;
            if (tour.Count == 1)
            {
                hull.Add((locationFunc(tour.First), tour.First));
                return hull;
            }

            // calculate most left and most right locations and build locations list.
            var locations = new List<(Coordinate location, int visit)>();
            float minLon = float.MaxValue, maxLon = float.MinValue;
            var left = -1;
            var right = -1;
            foreach (var visit in tour)
            {
                var location = locationFunc(visit);
                locations.Add((location, visit));

                if (minLon > location.Longitude)
                {
                    minLon = location.Longitude;
                    left = locations.Count - 1;
                }

                if (maxLon < location.Longitude)
                {
                    maxLon = location.Longitude;
                    right = locations.Count - 1;
                }
            }

            ConvexHull(hull, locations, left, right);

            return hull;
        }

        private static void ConvexHull(TourHull hull, List<(Coordinate location, int visit)> locations, int left, int right)
        {
            // move left to first and right to last.
            var t = locations[left];
            locations[left] = locations[0];
            locations[0] = t;
            if (right == 0) right = left;
            t = locations[right];
            locations[right] = locations[locations.Count - 1];
            locations[locations.Count - 1] = t;

            // divide in two partitions.
            var a = 0;
            var b = locations.Count - 1;
            var partitions = locations.Partition((1, locations.Count - 2));

            // create the hull.
            hull.Add(locations[a]); // add the left location.
            hull.AddForPartition(locations, (partitions.partition1.start, partitions.partition1.length), a, b,
                partitions.partition1.farthest); // do the top half.
            hull.Add(locations[b]); // add the right location.
            hull.AddForPartition(locations, (partitions.partition2.start, partitions.partition2.length), b, a,
                partitions.partition2.farthest); // do the bottom half.
        }

        /// <summary>
        /// Updates the hull with the given location.
        /// </summary>
        /// <param name="hull">The hull.</param>
        /// <param name="location">The location.</param>
        /// <returns>True if the hull was update, false otherwise.</returns>
        internal static bool UpdateHull(this TourHull hull, (Coordinate location, int visit) location)
        {
            if (hull.Count < 1)
            {
                hull.Add(location);
                return true;
            }

            if (hull.Count == 1)
            {
                if (hull[0].location.Longitude < location.location.Longitude)
                {
                    hull.Add(location);
                }
                else
                {
                    hull.Insert(0, location);
                }

                return true;
            }

            var lower = 0;
            var count = 0;

            var location1 = hull[0].location;
            var location2 = hull[1].location;
            var position = QuickHull.PositionToLine(location1, location2, location.location);
            var left = false;
            if (position.left)
            {
                left = true;
                
                // first segment on the left, investigate lower segments.
                count = 1;

                for (var i = 0; i < hull.Count; i++)
                {
                    location1 = hull[(hull.Count - i - 1) % hull.Count].location;
                    location2 = hull[(hull.Count - i - 0) % hull.Count].location;
                    position = QuickHull.PositionToLine(location1, location2, location.location);
                    if (!position.left) break;
                    lower--;
                    count++;
                }
            }

            // investigate higher segments.
            for (var i = 2; i < hull.Count + 1; i++)
            {
                location1 = hull[i - 1].location;
                location2 = hull[(i - 0) % hull.Count].location;
                position = QuickHull.PositionToLine(location1, location2, location.location);
                if (!position.left)
                {
                    if (left)
                    {
                        left = false;
                        break;
                    }
                }
                else
                {
                    if (!left) lower = i - 1;
                    count++;
                    left = true;
                }
            }

            // remove if anything to remove.
            if (count == 0)
            {
                return false;
            }

            // remove whatever needs removing.
            var insertPosition = lower + 1;
            if (count > 1)
            {
                if (lower < -1)
                {
                    var index = hull.Count + lower + 1; // don't remove first, start one after that.
                    hull.RemoveRange(index, -lower - 1);
                    count -= (-lower - 1); // remove the removed count from count.
                    lower = -1; // set as if the first to be removed is at index O.
                }

                hull.RemoveRange(lower + 1, count - 1);
            }

            // insert location.
            if (insertPosition >= 0)
            {
                hull.Insert(insertPosition, location);
            }
            else
            {
                hull.Add(location);
            }
            return true;
        }

        /// <summary>
        /// Returns true if this is actually a convex hull.
        /// </summary>
        /// <param name="hull">The hull.</param>
        /// <returns>True if this hull is convex.</returns>
        /// <exception cref="Exception"></exception>
        public static bool IsConvex(this TourHull hull)
        {
            for (var i = 1; i < hull.Count + 1; i++)
            {
                var location1 = hull[i - 1].location;
                var location2 = hull[(i - 0) % hull.Count].location;

                for (var j = 0; j < hull.Count; j++)
                {
                    if (j == (i - 0) % hull.Count || j == i - 1) continue;
                    
                    var position = QuickHull.PositionToLine(location1, location2, hull[j].location);
                    if (position.left)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if this hull contains the given location.
        /// </summary>
        /// <param name="hull">The hull.</param>
        /// <param name="location">The location.</param>
        /// <returns>True if the location is inside the hull.</returns>
        public static bool Contains(this TourHull hull, Coordinate location)
        {
            for (var i = 1; i < hull.Count + 1; i++)
            {
                var location1 = hull[i - 1].location;
                var location2 = hull[(i - 0) % hull.Count].location;
                
                var position = QuickHull.PositionToLine(location1, location2, location);
                if (position.left)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the two hulls have an intersection.
        /// </summary>
        /// <param name="hull">The hull.</param>
        /// <param name="other">The other hull.</param>
        /// <returns>True if there is an intersection.</returns>
        public static bool HasIntersection(this TourHull hull, TourHull other)
        { 
            for (var h = 0; h < hull.Count; h++)
            {
                var h0 = hull[h + 0];
                var h1 = hull[(h + 1) % hull.Count];
                var hBox = new Box(h0.location, h1.location);
                var hLine = new Line(h0.location, h1.location);
                for (var o = 0; o < other.Count; o++)
                {
                    var o0 = other[o + 0];
                    var o1 = other[(o + 1) % other.Count];

                    var h0pos = QuickHull.PositionToLine(o0.location, o1.location,
                        h0.location);
                    var o0pos = QuickHull.PositionToLine(h0.location, h1.location,
                        o0.location);
                    
                    var oBox = new Box(o0.location, o1.location);
                    
                    if (!oBox.Overlaps(hBox)) continue;
                    
                    var oLine = new Line(o0.location, o1.location);

                    var intersection = hLine.Intersect(oLine);
                    if (intersection == null) continue;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The intersection between the two hulls, if any.
        /// </summary>
        /// <param name="hull">The hull.</param>
        /// <param name="other">The other hull.</param>
        /// <returns>The convex polygon that represents the overlap between the two.</returns>
        public static TourHull Intersection(this TourHull hull, TourHull other)
        {
            float minLon = float.MaxValue, maxLon = float.MinValue;
            var left = -1;
            var right = -1;
            var intersections = new List<(Coordinate location, int visit)>();
            var hOutside = new HashSet<int>();
            var oOutside = new HashSet<int>();
            for (var h = 0; h < hull.Count; h++)
            {
                var h0 = hull[h + 0];
                var h1 = hull[(h + 1) % hull.Count];
                var hBox = new Box(h0.location, h1.location);
                var hLine = new Line(h0.location, h1.location);
                for (var o = 0; o < other.Count; o++)
                {
                    var o0 = other[o + 0];
                    var o1 = other[(o + 1) % other.Count];

                    var h0pos = QuickHull.PositionToLine(o0.location, o1.location,
                        h0.location);
                    if (h0pos.left)
                    {
                        hOutside.Add(h);
                    }
                    var o0pos = QuickHull.PositionToLine(h0.location, h1.location,
                        o0.location);
                    if (o0pos.left)
                    {
                        oOutside.Add(o);
                    }
                    
                    var oBox = new Box(o0.location, o1.location);
                    
                    if (!oBox.Overlaps(hBox)) continue;
                    
                    var oLine = new Line(o0.location, o1.location);

                    var intersection = hLine.Intersect(oLine);
                    if (intersection == null) continue;
                    
                    intersections.Add((intersection.Value, -1));
                    if (minLon > intersection.Value.Longitude)
                    {
                        minLon = intersection.Value.Longitude;
                        left = intersections.Count - 1;
                    }
                    if (maxLon < intersection.Value.Longitude)
                    {
                        maxLon = intersection.Value.Longitude;
                        right = intersections.Count - 1;
                    }
                }
            }

            for (var h = 0; h < hull.Count; h++)
            {
                if (hOutside.Contains(h)) continue;

                var intersection = hull[h].location;
                intersections.Add(hull[h]);
                if (minLon > intersection.Longitude)
                {
                    minLon = intersection.Longitude;
                    left = intersections.Count - 1;
                }
                if (maxLon < intersection.Longitude)
                {
                    maxLon = intersection.Longitude;
                    right = intersections.Count - 1;
                }
            }

            for (var o = 0; o < other.Count; o++)
            {
                if (oOutside.Contains(o)) continue;

                var intersection = other[o].location;
                intersections.Add(other[o]);
                if (minLon > intersection.Longitude)
                {
                    minLon = intersection.Longitude;
                    left = intersections.Count - 1;
                }
                if (maxLon < intersection.Longitude)
                {
                    maxLon = intersection.Longitude;
                    right = intersections.Count - 1;
                }
            }

            if (oOutside.Count == other.Count && hOutside.Count == hull.Count &&
                intersections.Count == 0) return null;

            var intersectionHull = new TourHull();
            
            ConvexHull(intersectionHull, intersections, left, right);

            return intersectionHull;
        }

        internal static ((int start, int length, int farthest) partition1, (int start, int length, int farthest) partition2) Partition(this List<(Coordinate location, int visit)> locations,
            (int start, int length) partition)
        {
            var a = locations[partition.start - 1].location;
            var b = locations[partition.start + partition.length].location;
            
            // move all the left-of-line locations to the beginning of the partition.
            var leftPointer = 0; // the first unsure position.
            var rightPointer = 0;
            var maxDistanceLeft = float.MinValue;
            var maxLeft = -1;
            var maxDistanceRight = float.MinValue;
            var maxRight = -1;
            while (leftPointer + rightPointer < partition.length)
            {
                (bool left, bool right, float distance)? status = (false, false, float.MinValue);
                
                // move left pointer until it encounters a right positioned location. 
                while (leftPointer < partition.length)
                {
                    status = PositionToLine(a, b, locations[partition.start + leftPointer].location);
                    if (!status.Value.left)
                    {
                        break;
                    }

                    if (status.Value.distance > maxDistanceLeft)
                    {
                        maxDistanceLeft = status.Value.distance;
                        maxLeft = partition.start + leftPointer;
                    }

                    leftPointer++;
                }
                var lastLeftStatus = status;

                // move right pointer until it encounters a left positioned location.
                status = null;
                while (rightPointer < partition.length)
                {
                    status = PositionToLine(a, b, locations[partition.start + partition.length - rightPointer - 1].location);
                    if (status.Value.left)
                    {
                        break;
                    }

                    if (status.Value.distance > maxDistanceRight)
                    {
                        maxDistanceRight = status.Value.distance;
                        maxRight = partition.start + partition.length - rightPointer - 1;
                    }

                    rightPointer++;
                }
                var lastRightStatus = status;

                if (leftPointer + rightPointer >= partition.length) break;

                // switch the location where the windows stopped.
                var t = locations[partition.start + leftPointer];
                locations[partition.start + leftPointer] =
                    locations[partition.start + partition.length - rightPointer - 1];
                locations[partition.start + partition.length - rightPointer - 1] = t;

                if (lastLeftStatus.Value.distance > maxDistanceRight)
                {
                    // the last left is actually right.
                    maxDistanceRight = lastLeftStatus.Value.distance;
                    maxRight = partition.start + partition.length - rightPointer - 1;
                }

                if (lastRightStatus.Value.distance > maxDistanceLeft)
                {
                    // the last right is actually left.
                    maxDistanceLeft = lastRightStatus.Value.distance;
                    maxLeft = partition.start + leftPointer;
                }

                leftPointer++;
                rightPointer++;
            }

            (int start, int length, int farthest) partition1 = (partition.start, leftPointer, maxLeft);
            if (partition1.length > 0)
            {
                var t = locations[partition1.start + partition1.length - 1];
                locations[partition1.start + partition1.length - 1] = locations[partition1.farthest];
                locations[partition1.farthest] = t;
                
                partition1 = (partition1.start, partition1.length - 1, partition1.start + partition1.length - 1);
            }
            
            (int start, int length, int farthest) partition2 = (partition.start + leftPointer, rightPointer, maxRight);
            if (partition2.length > 0)
            {
                var t = locations[partition2.start + partition2.length - 1];
                locations[partition2.start + partition2.length - 1] = locations[partition2.farthest];
                locations[partition2.farthest] = t;
                
                partition2 = (partition2.start, partition2.length - 1, partition2.start + partition2.length - 1);
            }

            return (partition1, partition2);
        }

        internal static void AddForPartition(this TourHull tourHull,
            List<(Coordinate location, int visit)> locations, (int start, int length) partition, int a, int b,
            int farthest)
        {
            if (farthest < 0) return;
            
            var aL = locations[a].location;
            var bL = locations[b].location;
            var c = locations[farthest].location;

            var aPointer = partition.start;
            var bPointer = partition.start + partition.length - 1;
            var aDistance = float.MinValue;
            var bDistance = float.MinValue;
            var aFarthest = -1;
            var bFarthest = -1;
            for (var i = partition.start; i <= bPointer; i++)
            {
                var location = locations[i].location;
                var position = QuickHull.PositionToLine(aL, c, location);
                if (position.left)
                {
                    // to the left of a->c is always include.
                    var t = locations[i];
                    locations[i] = locations[aPointer];
                    locations[aPointer] = t;

                    if (position.distance > aDistance)
                    {
                        aFarthest = aPointer;
                        aDistance = position.distance;
                    }

                    if (aPointer != i) i--;
                    aPointer++;
                    continue;
                }

                position = QuickHull.PositionToLine(c, bL, location);
                if (position.left)
                {
                    // to the left of c->b is always include.
                    var t = locations[i];
                    locations[i] = locations[bPointer];
                    locations[bPointer] = t;

                    if (position.distance > bDistance)
                    {
                        bFarthest = bPointer;
                        bDistance = position.distance;
                    }

                    i--;
                    bPointer--;
                    continue;
                }
            }

            // no need to remove anything, we just ignore these.
            // locations.RemoveRange(partition.start + aPointer, partition.length - aPointer - bPointer);

            (int start, int length, int farthest) partition1 = (partition.start, aPointer - partition.start, aFarthest);
            (int start, int length, int farthest) partition2 = (bPointer + 1, partition.start + partition.length - bPointer - 1,
                bFarthest);

            if (partition1.length > 0)
            {
                // if partition 1 is not empty partition it first.
                var t = locations[partition1.start + partition1.length - 1];
                locations[partition1.start + partition1.length - 1] = locations[partition1.farthest];
                locations[partition1.farthest] = t;

                partition1 = (partition1.start, partition1.length - 1, partition1.start + partition1.length - 1);

                // call recursively.
                tourHull.AddForPartition(locations, (partition1.start, partition1.length), a,
                    farthest, partition1.start + partition1.length);
            }
            tourHull.Add(locations[farthest]); // add farthest.

            if (partition2.length > 0)
            {
                // if partition 2 is not empty partition it now.
                var t = locations[partition2.start + partition2.length - 1];
                locations[partition2.start + partition2.length - 1] = locations[partition2.farthest];
                locations[partition2.farthest] = t;

                partition2 = (partition2.start, partition2.length - 1, partition2.start + partition2.length - 1);

                // call recursively.
                tourHull.AddForPartition(locations, (partition2.start, partition2.length),
                    farthest,
                    b, partition2.start + partition2.length);
            }
        }

        internal static (bool left, bool right, float distance) PositionToLine(Coordinate a, Coordinate b, Coordinate p)
        {
            double ax = a.Longitude;
            double ay = a.Latitude;

            double bx = b.Longitude;
            double by = b.Latitude;

            double x = p.Longitude;
            double y = p.Latitude;
            var d = Math.Abs((by - ay) * x - (bx - ax) * y + bx * ay - by * ax);
//            if (d < 1E-8)
//            {
//                return (false, false, 0);
//            }
            var position = (bx - ax) * (y - ay) - (by - ay) * (x - ax);
            return (position > 0, position < 0, (float)d);
        }
    }
}