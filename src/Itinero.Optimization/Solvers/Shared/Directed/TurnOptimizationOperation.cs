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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Itinero.Algorithms.PriorityQueues;
using Itinero.Optimization.Solvers.Tours;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
namespace Itinero.Optimization.Solvers.Shared.Directed
{
    /// <summary>
    /// Contains an algorithm to optimize all turns in a directed tour.
    /// </summary>
    internal static class TurnOptimizationOperation
    {
        internal static float OptimizeTurns(this Tour tour, Func<int, int, float> weightFunc,
            Func<TurnEnum, float> turnPenaltyFunc)
        {
            // TODO: this now uses a form of dykstra but we think there is a better algorithm.
            // TODO: this can be way more efficient, we haven't done any performance work on this.
            // TODO: check if we can reuse arrays.
            
            // build a small sequence of the original visits.
            var capacity = tour.Capacity + 3; // make sure all turns fit.
            var nextArray = new int[capacity / 4];
            var first = DirectedHelper.ExtractVisit(tour.First);
            var previous = -1;
            foreach (var directedVisit in tour)
            {
                var visit = DirectedHelper.ExtractVisit(directedVisit);
                if (previous != -1)
                {
                    nextArray[previous] = visit;
                }
                previous = visit;
            }
            var last = previous;
            
            var settled = new HashSet<int>();
            var queue = new BinaryHeap<int>();
            var paths = new (int previous, float cost)[capacity];
            for (var i = 0; i < paths.Length; i++)
            {
                paths[i] = (int.MaxValue, float.MaxValue);
            }
            
            // enqueue first visits and their paths.
            queue.Push(TurnEnum.ForwardForward.DirectedVisit(first), 0);
            queue.Push(TurnEnum.ForwardBackward.DirectedVisit(first), 0);
            var bestLast = -1;
            while (queue.Count > 0)
            {
                var currentWeight = queue.PeekWeight();
                var current = queue.Pop();
                if (settled.Contains(current))
                { // was already settled.
                    continue;
                }
                settled.Add(current);
                
                // TODO: pretty sure we can mark other directed visits for the same as settled here but not sure.

                var currentVisit = DirectedHelper.ExtractVisit(current);
                if (currentVisit == last && currentWeight > 0)
                {
                    bestLast = current;
                    break;
                }

                var next = nextArray[currentVisit];
                var departureWeightId = DirectedHelper.ExtractDepartureWeightId(current);
                for (var t = 0; t < 4; t++)
                {
                    var turn = (TurnEnum) t;
                    var nextDirected = turn.DirectedVisit(next);
                    if (settled.Contains(nextDirected))
                    {
                        continue;
                    }
                    var weight = turnPenaltyFunc(turn);
                    var arrivalWeightId = turn.Departure().WeightId(next);
                    var travelCost = weightFunc(departureWeightId, arrivalWeightId);
                    if (travelCost >= float.MaxValue)
                    { // this connection is impossible.
                        continue;
                    }
                    weight += travelCost;
                    weight += currentWeight;

                    Debug.Assert(nextDirected >= 0 && nextDirected < paths.Length);
                    if (paths[nextDirected].cost > weight)
                    {
                        paths[nextDirected] = (current, weight);
                        queue.Push(nextDirected, weight);
                    }
                }
            }

            if (bestLast == -1)
            { // tour is impossible.
                return float.MaxValue;
            }

            var best = bestLast;
            var newTour = new List<int>();
            Debug.Assert(best >= 0 && best < paths.Length);
            while (paths[best].previous > -1)
            {
                newTour.Add(best);
                best = paths[best].previous;
                if (DirectedHelper.ExtractVisit(best) == first)
                {
                    break;
                }
                Debug.Assert(best >= 0 && best < paths.Length);
            }
            newTour.Add(best);

            var tourDebug = tour.Clone();
            tour.Clear();
            tour.Replace(tour.First, newTour[newTour.Count - 1]);
            if (tour.Last.HasValue && 
                tour.First != tour.Last)
            {
                tour.Replace(tour.Last.Value, newTour[0]);
            }
            for (var i = newTour.Count - 2; i >= 0; i--)
            {
                tour.InsertAfter(newTour[i + 1], newTour[i]);
            }
            
            return paths[bestLast].cost;
        }
    }
}