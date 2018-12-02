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
namespace Itinero.Optimization.Solvers.Tours.Hull
{
    /// <summary>
    /// The quick hull algorithm.
    /// </summary>
    internal static class QuickHull
    {
        /// <summary>
        /// Calculates a convex hull.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="locationFunc">A function that returns the location per visit.</param>
        /// <returns>A hull with meta-data.</returns>
        public static TourHull ConvexHull(this Tour tour, Func<int, Coordinate> locationFunc)
        {
            // calculate most left and most right locations and build locations list.
            var locations = new List<(Coordinate location, int visit)>();
            float minLat = float.MaxValue, minLon = float.MaxValue, maxLat = float.MinValue, maxLon = float.MinValue;
            var left = -1;
            var right = -1;
            foreach (var visit in tour)
            {
                var location = locationFunc(visit);
                locations.Add((location, visit));

                if (minLat > location.Latitude)
                {
                    minLat = location.Latitude;
                }

                if (minLon > location.Longitude)
                {
                    minLon = location.Longitude;
                    left = locations.Count - 1;
                }

                if (maxLat < location.Latitude)
                {
                    maxLat = location.Latitude;
                }

                if (maxLon < location.Longitude)
                {
                    maxLon = location.Longitude;
                    right = locations.Count - 1;
                }
            }
            
            // move left to first and right to last.
            var t = locations[left];
            locations[left] = locations[0];
            locations[0] = t;
            t = locations[right];
            locations[right] = locations[locations.Count - 1];
            locations[locations.Count - 1] = t;

            // divide in two partitions.
            var a = 0;
            var b = locations.Count - 1;
            var partitions = locations.Partition((1, locations.Count - 2));
            
            // create the hull.
            var hull = new TourHull();
            hull.Add(locations[a]); // add the left location.
            hull.AddForPartition(locations, (partitions.partition1.start, partitions.partition1.length), a, b, partitions.partition1.farthest); // do the top half.
            hull.Add(locations[b]); // add the right location.
            hull.AddForPartition(locations, (partitions.partition2.start, partitions.partition2.length), b, a, partitions.partition2.farthest); // do the bottom half.

            return hull;
        }

        /// <summary>
        /// Updates the hull with the given location.
        /// </summary>
        /// <param name="hull">The hull.</param>
        /// <param name="location">The location.</param>
        /// <returns>True if the hull was update, false otherwise.</returns>
        public static bool UpdateHull(this TourHull hull, (Coordinate location, int visit) location)
        {
            if (hull.Count < 1)
            {
                hull.Add(location);
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
                    hull.RemoveRange(hull.Count + lower - 1, -lower + 1);
                    count += lower;
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
                (bool left, float distance)? status = (false, float.MinValue);
                
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
            var aL = locations[a].location;
            var bL = locations[b].location;
            var c = locations[farthest].location;

            var aPointer = partition.start;
            var bPointer = partition.start + partition.length - 1;
            var aDistance = float.MinValue;
            var bDistance = float.MinValue;
            var aFarthest = -1;
            var bFarthest = -1;
            for (var i = partition.start; i < partition.start + partition.length; i++)
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

                    bPointer--;
                    continue;
                }
            }

            // no need to remove anything, we just ignore these.
            // locations.RemoveRange(partition.start + aPointer, partition.length - aPointer - bPointer);

            (int start, int length, int farthest) partition1 = (partition.start, aPointer - partition.start, aFarthest);
            (int start, int length, int farthest) partition2 = (partition.start + bPointer, partition.start + partition.length - bPointer - 1,
                bFarthest);

            if (partition1.length > 0)
            {
                // if partition 1 is not empty partition it first.
                var t = locations[partition1.start + partition1.length - 1];
                locations[partition1.start + partition1.length - 1] = locations[partition1.farthest];
                locations[partition1.farthest] = t;

                partition1 = (partition1.start, partition1.length - 1, partition1.start + partition1.length - 1);

                // call recursively.
                tourHull.AddForPartition(locations, (partition1.start, partition1.length), partition.start - 1,
                    partition.start + partition.length, partition1.start + partition1.length);
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
                    partition.start + partition.length,
                    partition.start - 1, partition2.start + partition2.length);
            }
        }

        internal static (bool left, float distance) PositionToLine(Coordinate a, Coordinate b, Coordinate p)
        {
            double ax = a.Longitude;
            double ay = a.Latitude;

            double bx = b.Longitude;
            double by = b.Latitude;

            double x = p.Longitude;
            double y = p.Latitude;
            var d = Math.Abs((by - ay) * x - (bx - ax) * y + bx * ay - by * ax);
            var position = (bx - ax) * (y - ay) - (by - ay) * (x - ax);
            return (position > 0, (float)d);
        }
    }
}