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
using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.EAX
{
    internal static class EAXOperation
    {
        /// <summary>
        /// Applies the EAX (edge assembly cross over) strategy to the two given tours to construct a new tour.
        /// </summary>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <param name="weightFunc">The weight function.</param>
        /// <param name="maxOffspring">The maximum offspring parameter.</param>
        /// <param name="strategy">The strategy.</param>
        /// <param name="forwardNearestNeigbours">The forward nearest neighbours.</param>
        /// <returns>All details about the new tour.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static (bool success, float weight, Tour newTour) DoEAXWith(this Tour tour1, Tour tour2, Func<int, int, float> weightFunc, 
            int maxOffspring = 30, EAXSelectionStrategyEnum strategy = EAXSelectionStrategyEnum.SingleRandom,
            NearestNeighbourArray forwardNearestNeigbours = null)
        {
            // TODO: PERFORMANCE Check if we can reduce allocations here.
            // REMARK: assumed is that whoever calls this know it only supports 'closed' problems
            //     and that they convert the tours if needed.
            
            if (tour1.First != tour1.Last) throw new ArgumentException($"{nameof(tour1)} is now closed.");
            if (tour2.First != tour2.Last) throw new ArgumentException($"{nameof(tour2)} is now closed.");
            if (tour1.First != tour2.First &&
                tour1.Last != tour2.Last) throw new ArgumentException($"{nameof(tour1)} and {nameof(tour2)} must have the same first and last visit.");
            
            var fitness = float.MaxValue;

            // first create E_a
            var capacity = Math.Max(tour1.Capacity, tour2.Capacity);
            var eA = new AsymmetricCycles(capacity);
            foreach (var edge in tour1.Pairs())
            {
                eA.AddEdge(edge.From, edge.To);
            }

            // create E_b
            var eB = new int[tour2.Capacity];
            foreach (var edge in tour2.Pairs())
            {
                eB[edge.To] = edge.From;
            }

            // create cycles.
            var cycles = new AsymmetricAlternatingCycles(capacity);
            for (var idx = 0; idx < eB.Length; idx++)
            {
                var a = eA[idx];
                if (a == Tour.NOT_SET) continue;
                var b = eB[a];
                if (idx != b && b != Tour.NOT_SET)
                {
                    cycles.AddEdge(idx, a, b);
                }
            }

            // the cycles that can be selected.
            var selectableCycles = new List<int>(cycles.Cycles.Keys);

            var generated = 0;
            Tour best = null;
            while (generated < maxOffspring && selectableCycles.Count > 0)
            {
                // select some random cycles.
                var cycleStarts = SelectCycles(selectableCycles, strategy);

                // copy if needed.
                AsymmetricCycles a = null;
                if (maxOffspring > 1)
                {
                    a = eA.Clone();
                }
                else
                {
                    a = eA;
                }

                // take e_a and remove all edges that are in the selected cycles and replace them by the eges
                var nextArrayA = a.NextArray;
                foreach (var start in cycleStarts)
                {
                    var current = start;
                    var currentNext = cycles.Next(current);
                    do
                    {
                        a.AddEdge(currentNext.Value, currentNext.Key);

                        current = currentNext.Value;
                        currentNext = cycles.Next(current);
                    } while (current != start);
                }

                // connect all subtoures.
                var cycleCount = a.Cycles.Count;
                while (cycleCount > 1)
                {
                    // get the smallest tour.
                    var currentTour = new KeyValuePair<int, int>(-1, int.MaxValue);
                    foreach (var tour in a.Cycles)
                    {
                        if (tour.Value < currentTour.Value)
                        {
                            currentTour = tour;
                        }
                    }

                    // first try nn approach.
                    var weight = double.MaxValue;
                    var selectedFrom1 = -1;
                    var selectedFrom2 = -1;
                    var selectedTo1 = -1;
                    var selectedTo2 = -1;

                    var ignoreList = new bool[a.Length];
                    var @from = currentTour.Key;
                    ignoreList[from] = true;
                    var to = nextArrayA[@from];
                    do
                    {
                        // step to the next ones.
                        from = to;
                        to = nextArrayA[from];

                        //ignore_list.Add(from);
                        ignoreList[from] = true;
                    } while (from != currentTour.Key);

                    if (forwardNearestNeigbours != null)
                    { // only try tours containing nn.

                        from = currentTour.Key;
                        to = nextArrayA[from];
                        var weightFromTo = weightFunc(from, to);
                        do
                        {
                            // TODO: this can be optimized by using one array and copying of the data from the cache or use an enumerator to prevent a new array in every loop.
                            // check the nearest neighbours of from
                            foreach (var nn in forwardNearestNeigbours[from])
                            {
                                var nnTo = nextArrayA[nn];

                                if (nnTo != Tour.NOT_SET &&
                                    !ignoreList[nn] &&
                                    !ignoreList[nnTo])
                                {
                                    //float mergeWeight =
                                    //    (weights[from][nnTo] + weights[nn][to]) -
                                    //    (weightFromTo + weights[nn][nnTo]);
//                                    var mergeWeight =
//                                        (problem.Weight(from, nnTo) + problem.Weight(nn, to)) +
//                                        (weightFromTo + problem.Weight(nn, nnTo));
                                    var mergeWeight =
                                        (weightFunc(from, nnTo) + weightFunc(nn, to)) +
                                        (weightFromTo + weightFunc(nn, nnTo));
                                    if (weight > mergeWeight)
                                    {
                                        weight = mergeWeight;
                                        selectedFrom1 = from;
                                        selectedFrom2 = nn;
                                        selectedTo1 = to;
                                        selectedTo2 = nnTo;
                                    }
                                }
                            }

                            // step to the next ones.
                            from = to;
                            to = nextArrayA[from];
                        } while (from != currentTour.Key);
                    }
                    if (selectedFrom2 < 0)
                    {
                        // check the nearest neighbours of from
                        foreach (var customer in tour1)
                        {
                            var customerTo = nextArrayA[customer];

                            if (!ignoreList[customer] &&
                                !ignoreList[customerTo])
                            {
                                //var mergeWeight =
                                //    (weights[from][customerTo] + weights[customer][to]) -
                                //    (weights[from][to] + weights[customer][customerTo]);
                                var mergeWeight =
                                    (weightFunc(from, customerTo) + weightFunc(customer, to)) -
                                    (weightFunc(from, to) + weightFunc(customer, customerTo));
                                if (weight > mergeWeight)
                                {
                                    weight = mergeWeight;
                                    selectedFrom1 = from;
                                    selectedFrom2 = customer;
                                    selectedTo1 = to;
                                    selectedTo2 = customerTo;
                                }
                            }
                        }
                    }

                    a.AddEdge(selectedFrom1, selectedTo2);
                    a.AddEdge(selectedFrom2, selectedTo1);

                    cycleCount--;
                }

                var newRoute = new Tour(new int[] { tour1.First }, tour1.Last);
                var previous = tour1.First;
                var next = nextArrayA[tour1.First];
                do
                {
                    newRoute.InsertAfter(previous, next);
                    previous = next;
                    next = nextArrayA[next];
                }
                while (next != Tour.NOT_SET &&
                    next != tour1.First);

                var newFitness = 0.0f;
                foreach (var edge in newRoute.Pairs())
                {
                    newFitness = newFitness + weightFunc(edge.From, edge.To);
                }

                if (newRoute.Count == tour1.Count) // WARNING: this is very very inefficient, this can be a *lot* better.
                {
                    if (best == null ||
                        fitness > newFitness)
                    {
                        best = newRoute;
                        fitness = newFitness;
                    }

                    generated++;
                }
            }

            if (best == null)
            {
                best = new Tour(new int[] { tour1.First }, tour1.Last);
                var previous = tour1.First;
                var next = eA[tour1.First];
                do
                {
                    best.InsertAfter(previous, next);
                    previous = next;
                    next = eA[next];
                }
                while (next != Tour.NOT_SET &&
                    next != tour1.First);

                fitness = 0.0f;
                foreach (var edge in best.Pairs())
                {
                    fitness = fitness + weightFunc(edge.From, edge.To);
                }
            }

            return (true, fitness, best);
        }
        
        
        private static List<int> SelectCycles(List<int> cycles, EAXSelectionStrategyEnum strategy)
        {
            var starts = new List<int>();
            if (strategy == EAXSelectionStrategyEnum.MultipleRandom)
            {
                foreach (var cycle in cycles)
                {
                    if (Strategies.Random.RandomGenerator.Default.Generate(1.0f) > 0.25)
                    {
                        starts.Add(cycle);
                    }
                }
                return starts;
            }
            else
            {
                if (cycles.Count <= 0) return starts;
                var idx = Strategies.Random.RandomGenerator.Default.Generate(cycles.Count);
                starts.Add(cycles[idx]);
                cycles.RemoveAt(idx);
            }
            return starts;
        }

    }
}