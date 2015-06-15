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

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Routes.Cycles;
using OsmSharp.Logistics.Solvers.GA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Logistics.Solutions.TSP.GA.Operators
{
    /// <summary>
    /// An edge assembly crossover.
    /// </summary>
    public class EdgeAssemblyCrossover : ICrossOverOperator<ITSP, IRoute>
    {
        private int _maxOffspring;

        private EdgeAssemblyCrossoverSelectionStrategyEnum _strategy;

        private bool _nn;

        /// <summary>
        /// Creates a new edge assembly crossover.
        /// </summary>
        /// <param name="max_offspring"></param>
        /// <param name="strategy"></param>
        /// <param name="nn"></param>
        public EdgeAssemblyCrossover(int max_offspring,
            EdgeAssemblyCrossoverSelectionStrategyEnum strategy,
            bool nn)
        {
            _maxOffspring = max_offspring;
            _strategy = strategy;
            _nn = nn;
        }

        /// <summary>
        /// Returns the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                if (_strategy == EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom)
                {
                    if (_nn)
                    {
                        return string.Format("EAX_(SR{0}_NN)", _maxOffspring);
                    }
                    return string.Format("EAX_(SR{0})", _maxOffspring);
                }
                else
                {
                    if (_nn)
                    {
                        return string.Format("EAX_(MR{0}_NN)", _maxOffspring);
                    }
                    return string.Format("EAX_(MR{0})", _maxOffspring);
                }
            }
        }

        /// <summary>
        /// An enumeration of the crossover types.
        /// </summary>
        public enum EdgeAssemblyCrossoverSelectionStrategyEnum
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



        #region ICrossOverOperation<int,Problem> Members

        private List<int> SelectCycles(
            List<int> cycles)
        {
            List<int> starts = new List<int>();
            if (_strategy == EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom)
            {
                foreach (int cycle in cycles)
                {
                    if (OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0) > 0.25)
                    {
                        starts.Add(cycle);
                    }
                }
                return starts;
            }
            else
            {
                if (cycles.Count > 0)
                {
                    int idx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(cycles.Count);
                    starts.Add(cycles[idx]);
                    cycles.RemoveAt(idx);
                }
            }
            return starts;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution1"></param>
        /// <param name="solution2"></param>
        /// <param name="fitness"></param>
        /// <returns></returns>
        public IRoute Apply(ITSP problem, IRoute solution1, IRoute solution2, out double fitness)
        {
            fitness = double.MaxValue;
            var weights = problem.Weights;

            // first create E_a
            var e_a = new AsymmetricCycles(solution1.Count + 1);
            foreach(var edge in solution1.Pairs())
            {
                e_a.AddEdge(edge.From, edge.To);
            }

            // create E_b
            var e_b = new int[solution2.Count + 1];
            foreach(var edge in solution2.Pairs())
            {
                e_b[edge.To] = edge.From;
            }

            // create cycles.
            var cycles = new AsymmetricAlternatingCycles(solution2.Count + 1);
            for (var idx = 0; idx < e_b.Length; idx++)
            {
                int a = e_a[idx];
                int b = e_b[a];
                if (idx != b)
                {
                    cycles.AddEdge(idx, a, b);
                }
            }

            // the cycles that can be selected.
            var selectableCycles = new List<int>(cycles.Cycles.Keys);

            int generated = 0;
            Route best = null;
            while (generated < _maxOffspring
                && selectableCycles.Count > 0)
            {
                // select some random cycles.
                var cycleStarts = this.SelectCycles(selectableCycles);

                // copy if needed.
                AsymmetricCycles a = null;
                if (_maxOffspring > 1)
                {
                    a = e_a.Clone();
                }
                else
                {
                    a = e_a;
                }

                // take e_a and remove all edges that are in the selected cycles and replace them by the eges
                var nextArrayA = a.NextArray;
                foreach (int start in cycleStarts)
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
                    foreach (KeyValuePair<int, int> tour in a.Cycles)
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
                    int from;
                    int to;
                    from = currentTour.Key;
                    ignoreList[from] = true;
                    to = nextArrayA[from];
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
                        double weightFromTo = weights[from][to];
                        do
                        {
                            // check the nearest neighbours of from
                            foreach (var nn in problem.GetNNearestNeighbours(10, from))
                            {
                                int nnTo = nextArrayA[nn];

                                if (!ignoreList[nn] &&
                                    !ignoreList[nnTo])
                                {
                                    double mergeWeight =
                                        (weights[from][nnTo] + weights[nn][to]) -
                                        (weightFromTo + weights[nn][nnTo]);
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
                            int customerTo = nextArrayA[customer];

                            if (!ignoreList[customer] &&
                                !ignoreList[customerTo])
                            {
                                var mergeWeight =
                                    (weights[from][customerTo] + weights[customer][to]) -
                                    (weights[from][to] + weights[customer][customerTo]);
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

                var newRoute = new Route(a.Length, problem.First, problem.IsClosed);
                var previous = problem.First;
                var next = nextArrayA[problem.First];
                do
                {
                    newRoute.InsertAfter(previous, next);
                    previous = next;
                    next = nextArrayA[next];
                }
                while (next != 0);

                var newFitness = 0.0;
                foreach(var edge in newRoute.Pairs())
                {
                    newFitness = newFitness + weights[edge.From][edge.To];
                }

                if (best == null ||
                    fitness > newFitness)
                {
                    best = newRoute;
                    fitness = newFitness;
                }

                generated++;
            }

            if (best == null)
            {
                best = new Route(problem.Weights.Length, problem.First, problem.IsClosed);
                var previous = problem.First;
                var next = e_a[problem.First];
                do
                {
                    best.InsertAfter(previous, next);
                    previous = next;
                    next = e_a[next];
                }
                while (next != 0);

                fitness = 0.0;
                foreach (var edge in best.Pairs())
                {
                    fitness = fitness + weights[edge.From][edge.To];
                }
            }
            return best;
        }
    }
}