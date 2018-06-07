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
using System.Threading;
using Itinero.Logging;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.GA;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.TSP.EAX
{
    /// <summary>
    /// An edge assembly crossover.
    /// </summary>
    internal sealed class EAXOperator : CrossOverOperator<Candidate<TSProblem, Tour>>
    {
        private readonly int _maxOffspring;
        private readonly EAXSelectionStrategyEnum _strategy;
        private readonly bool _nn;
        private readonly RandomGenerator _random;

        /// <summary>
        /// Creates a new EAX crossover.
        /// </summary>
        public EAXOperator()
            : this(30, EAXOperator.EAXSelectionStrategyEnum.SingleRandom, true)
        {

        }

        /// <summary>
        /// Creates a new EAX crossover.
        /// </summary>
        public EAXOperator(int maxOffspring, EAXSelectionStrategyEnum strategy, bool nn)
        {
            _maxOffspring = maxOffspring;
            _strategy = strategy;
            _nn = nn;

            _random = RandomGenerator.Default;
        }

        /// <summary>
        /// Returns the name of this operator.
        /// </summary>
        public override string Name
        {
            get
            {
                switch (_strategy)
                {
                    case EAXSelectionStrategyEnum.SingleRandom when _nn:
                        return $"EAX_(SR{_maxOffspring}_NN)";
                    case EAXSelectionStrategyEnum.SingleRandom:
                        return $"EAX_(SR{_maxOffspring})";
                    case EAXSelectionStrategyEnum.MultipleRandom:
                        if (_nn)
                        {
                            return $"EAX_(MR{_maxOffspring}_NN)";
                        }
                        return $"EAX_(MR{_maxOffspring})";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// An enumeration of the crossover types.
        /// </summary>
        public enum EAXSelectionStrategyEnum
        {
            /// <summary>
            /// SingleRandom.
            /// </summary>
            SingleRandom, // EAX-1AB
            /// <summary>
            /// MultipleRandom.
            /// </summary>
            MultipleRandom
        }

        private List<int> SelectCycles(
            List<int> cycles)
        {
            var starts = new List<int>();
            if (_strategy == EAXSelectionStrategyEnum.MultipleRandom)
            {
                foreach (int cycle in cycles)
                {
                    if (_random.Generate(1.0f) > 0.25)
                    {
                        starts.Add(cycle);
                    }
                }
                return starts;
            }
            else
            {
                if (cycles.Count <= 0) return starts;
                var idx = _random.Generate(cycles.Count);
                starts.Add(cycles[idx]);
                cycles.RemoveAt(idx);
            }
            return starts;
        }

        /// <summary>
        /// Applies this operator using the given solutions and produces a new solution.
        /// </summary>
        /// <returns></returns>
        public override Candidate<TSProblem, Tour> Apply(Candidate<TSProblem, Tour> candidate1, Candidate<TSProblem, Tour> candidate2)
        {
            var problem = candidate1.Problem;
            var solution1 = candidate1.Solution;
            var solution2 = candidate2.Solution;
            
            if (solution1.Last != problem.Last) { throw new ArgumentException("Tour and problem have to have the same last customer."); }
            if (solution2.Last != problem.Last) { throw new ArgumentException("Tour and problem have to have the same last customer."); }

            var originalProblem = problem;
            var originalSolution1 = solution1;
            var originalSolution2 = solution2;
            if (!problem.Last.HasValue)
            { // convert to closed problem.
                Logger.Log($"{typeof(EAXOperator)}.{nameof(Apply)}", TraceEventType.Warning,
                    "Performance warning: EAX operator cannot be applied to 'open' TSP's, converting problem and tours to a closed equivalent.");

                problem =  problem.ToClosed();
                solution1 = new Tour(solution1, 0);
                solution2 = new Tour(solution2, 0);
            }
            else if (problem.First != problem.Last)
            { // last is set but is not the same as first.
                Logger.Log($"{typeof(EAXOperator)}.{nameof(Apply)}", TraceEventType.Warning,
                    "Performance warning: EAX operator cannot be applied to 'closed' TSP's with a fixed endpoint, converting problem and tours to a closed equivalent.");

                problem = problem.ToClosed();
                solution1 = new Tour(solution1, 0);
                solution2 = new Tour(solution2, 0);
                solution1.Remove(originalProblem.Last.Value);
                solution2.Remove(originalProblem.Last.Value);
            }
            
            var fitness = float.MaxValue;

            // first create E_a
            var eA = new AsymmetricCycles(solution1.Count);
            foreach (var edge in solution1.Pairs())
            {
                eA.AddEdge(edge.From, edge.To);
            }

            // create E_b
            var eB = new int[solution2.Count];
            foreach (var edge in solution2.Pairs())
            {
                eB[edge.To] = edge.From;
            }

            // create cycles.
            var cycles = new AsymmetricAlternatingCycles(solution2.Count);
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
            while (generated < _maxOffspring
                && selectableCycles.Count > 0)
            {
                // select some random cycles.
                var cycleStarts = this.SelectCycles(selectableCycles);

                // copy if needed.
                AsymmetricCycles a = null;
                if (_maxOffspring > 1)
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

                    if (_nn)
                    { // only try tours containing nn.

                        from = currentTour.Key;
                        to = nextArrayA[from];
                        var weightFromTo = problem.Weight(from, to); // weights[from][to];
                        do
                        {
                            // TODO: this can be optimized by using one array and copying of the data from the cache or use an enumerator to prevent a new array in every loop.
                            // check the nearest neighbours of from
                            foreach (var nn in problem.NearestNeighbourCache.GetNNearestNeighboursForward(10)[from])
                            {
                                var nnTo = nextArrayA[nn];

                                if (nnTo != Tour.NOT_SET &&
                                    !ignoreList[nn] &&
                                    !ignoreList[nnTo])
                                {
                                    //float mergeWeight =
                                    //    (weights[from][nnTo] + weights[nn][to]) -
                                    //    (weightFromTo + weights[nn][nnTo]);
                                    var mergeWeight =
                                        (problem.Weight(from, nnTo) + problem.Weight(nn, to)) +
                                        (weightFromTo + problem.Weight(nn, nnTo));
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
                        foreach (var customer in solution1)
                        {
                            var customerTo = nextArrayA[customer];

                            if (!ignoreList[customer] &&
                                !ignoreList[customerTo])
                            {
                                //var mergeWeight =
                                //    (weights[from][customerTo] + weights[customer][to]) -
                                //    (weights[from][to] + weights[customer][customerTo]);
                                var mergeWeight =
                                    (problem.Weight(from, customerTo) + problem.Weight(customer, to)) -
                                    (problem.Weight(from, to) + problem.Weight(customer, customerTo));
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

                var newRoute = new Tour(new int[] { problem.First }, problem.Last);
                var previous = problem.First;
                var next = nextArrayA[problem.First];
                do
                {
                    newRoute.InsertAfter(previous, next);
                    previous = next;
                    next = nextArrayA[next];
                }
                while (next != Tour.NOT_SET &&
                    next != problem.First);

                var newFitness = 0.0f;
                foreach (var edge in newRoute.Pairs())
                {
                    newFitness = newFitness + problem.Weight(edge.From, edge.To);
                }

                if (newRoute.Count == solution1.Count)
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
                best = new Tour(new int[] { problem.First }, problem.Last);
                var previous = problem.First;
                var next = eA[problem.First];
                do
                {
                    best.InsertAfter(previous, next);
                    previous = next;
                    next = eA[next];
                }
                while (next != Tour.NOT_SET &&
                    next != problem.First);

                fitness = 0.0f;
                foreach (var edge in best.Pairs())
                {
                    fitness = fitness + problem.Weight(edge.From, edge.To);
                }
            }

            if (!originalProblem.Last.HasValue)
            { // original problem as an 'open' problem, convert to an 'open' route.
                best = new Tour(best, null);
                fitness = originalProblem.Weights(best);
            }
            else if (originalProblem.First != originalProblem.Last)
            { // original problem was a problem with a fixed last point different from the first point.
                best.InsertAfter(System.Linq.Enumerable.Last(best), originalProblem.Last.Value);
                best = new Tour(best, problem.Last.Value);
                fitness = originalProblem.Weights(best);
            }

            return new Candidate<TSProblem, Tour>()
            {
                Solution = best,
                Fitness = fitness,
                Problem = originalProblem
            };
        }
        
        private static readonly ThreadLocal<EAXOperator> DefaultLazy = new ThreadLocal<EAXOperator>(() => new EAXOperator());
        public static EAXOperator Default => DefaultLazy.Value;
    }
}