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

            // divide in two partitions.
            var partition = locations.Partition((1, locations.Count - 2));
            
            // remove inside triangle bits.
            

//            // move left to first and right to last.
//            var t = locations[left];
//            locations[left] = locations[0];
//            locations[0] = t;
//            t = locations[right];
//            locations[right] = locations[locations.Count - 1];
//            locations[locations.Count - 1] = t;
//
//            // partition into two along the left->right line.
//            var partition = locations.Partition((1, locations.Count - 2));
//            var partitionLeftSize = partition - 1;
//            var partitionRightSize = locations.Count - 1 - partition;
//            
//            // create the hull.
//            var hull = new TourHull();
//            hull.Add(locations[0]); // add the left location.
//            BuildHull(hull, locations, );
            return null;
        }
//
//        internal static (int start, int length) RemoveInner(this List<(Coordinate location, int visit)> locations,
//            (int start, int length) partition, Coordinate a, Coordinate b, Coordinate c)
//        {
//            var length = partition.length;
//            for (var i = partition.start; i < partition.start + length; i++)
//            {
//                var p = locations[i].location;
//                if (LeftOfLine(a, b, p) && LeftOfLine(b, c, p) && LeftOfLine(c, a, p)) continue;
//                
//                // left of all the lines: element of the triangle -> we remove it
//                locations.RemoveAt(i);
//                i--; // a location was removed at i.
//                length--;
//            }
//        }
//
//        internal static void BuildHull(this TourHull hull, List<(Coordinate location, int visit)> locations,
//            (int start, int length) partition, Coordinate a, Coordinate b)
//        {
//            if (partition.length == 0) return;
//            if (partition.length == 1)
//            {
//                hull.Add(locations[partition.start]);
//                return;
//            }
//            
//            // partition recursively.
//            var c = locations[max].location;
//            var middle = locations.Partition(partition, a, c);
//        }

        internal static (int partition, int farthest1, int farthest2) Partition(this List<(Coordinate location, int visit)> locations,
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

            return (partition.start + leftPointer, maxLeft, maxRight);
        }

        internal static ((int start, int length, int farthest) partition1, (int start, int length, int farthest) partition2) Partition(this List<(Coordinate location, int visit)> locations,
            (int start, int length) partition, int a, int b, int farthest)
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
                { // to the left of a->c is always include.
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
                { // to the left of c->b is always include.
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
            
            var partition1 = (partition.start, aPointer - partition.start, partition.start + aFarthest);
            var partition2 = (partition.start + bPointer, partition.length - bPointer , partition.start + partition.length - bFarthest - 1);
            return (partition1, partition2);
        }
        
        internal static (bool left, float distance) PositionToLine(Coordinate a, Coordinate b, Coordinate p)
        {
            var ax = a.Longitude;
            var ay = a.Latitude;

            var bx = b.Longitude;
            var by = b.Latitude;

            var x = p.Longitude;
            var y = p.Latitude;
            var d = Math.Abs((by - ay) * x - (bx - ax) * y + bx * ay - by * ax);
            var position = (bx - ax) * (y - ay) - (by - ay) * (x - ax);
            return (position > 0, d);
        }
    }
}