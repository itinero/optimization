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
using System.Text;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represents a solution to a capacitated no-depot VRP.
    /// </summary>
    public class NoDepotCVRPSolution : ISolution, IRelocateSolution, IExchangeSolution, IMultiExchangeSolution, IMultiRelocateSolution,
        ISeededCheapestInsertionSolution
    {
        private readonly List<CapacityExtensions.Content> _contents;
        private readonly List<ITour> _tours; // does not use multitour, as some problems require the same depot to be visited multiple times


        /// The visit-ID _after_ which the depot is visited
        private readonly List<int> _depotPoint;

        /// The cost of visiting the depot, at this location
        private readonly List<float> _depotCost;

        /// <summary>
        /// Creates a new solution.
        /// </summary>
        public NoDepotCVRPSolution(int size)
        {
            _tours = new List<ITour>(size);
            _contents = new List<CapacityExtensions.Content>(size);
            _depotCost = new List<float>();
            _depotPoint = new List<int>();

        }

        /// <summary>
        /// Creates a new solution by deep-copying what's given.
        /// </summary>
        protected NoDepotCVRPSolution(NoDepotCVRPSolution toCopy)
            : this(toCopy._contents.Count)
        {
            // initialization of the lists is done by 'this' to call the other construction

            // make a deep-copy of the contents.
            List<CapacityExtensions.Content> contents = toCopy._contents;
            for (var c = 0; c < contents.Count; c++)
            {
                _contents.Add(new CapacityExtensions.Content()
                {
                    Weight = contents[c].Weight
                });
                if (contents[c].Quantities != null)
                {
                    _contents[c].Quantities = contents[c].Quantities.Clone() as float[];
                }
            }

            // make a deep-copy of the tours.
            foreach (var tour in toCopy._tours)
            {
                _tours.Add(tour.Clone() as Tour);
            }

            _depotPoint.AddRange(toCopy._depotPoint);
            _depotCost.AddRange(toCopy._depotCost);

        }


        /// <summary>
        /// Calculates the weight of the given tour.
        /// </summary>
        public float CalculateWeightOf(NoDepotCVRProblem problem, int tourIdx)
        {
            var weight = 0f;
            var tour = this.Tour(tourIdx);

            Pair? last = null;
            foreach (var pair in tour.Pairs())
            {
                weight += problem.GetVisitCost(pair.From);
                weight += problem.Weights[pair.From][pair.To];
            }
            if (last.HasValue && !tour.IsClosed())
            {
                weight += problem.GetVisitCost(last.Value.To);
            }

            return weight;
        }







        /// <summary>
        /// Gets or sets the contents of each tour.
        /// </summary>
        public List<CapacityExtensions.Content> Contents
        {
            get
            {
                return _contents;
            }
        }

        /// <summary>
        /// Gets the number of tours.
        /// </summary>
        public int Count
        {
            get
            {
                return _tours.Count;
            }
        }


        /// <summary>
        /// Gets the tour at the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ITour Tour(int i)
        {
            return _tours[i];
        }

        /// <summary>
        /// Replaces the given tour at the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="tour"></param>
        public void ReplaceTour(int i, ITour tour)
        {
            _tours[i] = tour;
        }

        /// <summary>
        /// Adds a new tour.
        /// </summary>
        /// <param name="problem">More details about the problem, used for the depot weights</param>
        /// <param name="first"></param>
        /// <returns></returns>
        public ITour Add(NoDepotCVRProblem problem, int first)
        {
            int last = first;
            var tour = new Tour(new int[] { first }, last);
            _tours.Add(tour);
            if (problem.Depot != null)
            {

                var w = problem.Weights;
                int depot = (int)problem.Depot;
                float cost = w[first][depot] + w[depot][last] - w[first][last];
                _depotPoint.Add(first);
                _depotCost.Add(cost);
            }
            else
            {
                _depotCost.Add(0);
            }
            return tour;
        }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new NoDepotCVRPSolution(this);
        }

        /// <summary>
        /// Overwrites what's in this solution by what's in the given solution.
        /// </summary>
        /// <param name="solution"></param>
        public void CopyFrom(ISolution solution)
        {
            throw new NotImplementedException();
        }

        internal float DepotCost(int tour)
        {
            return _depotCost[tour];
        }

        internal int DepotPoint(int tour)
        {
            return _depotPoint[tour];
        }

        internal int DepotNeighbour(int tour)
        {
            return Tour(tour).GetNeigbour(DepotPoint(tour));
        }

        /// <summary>
        /// Calculates where the depot would be cheapest for the given tour.
        /// </summary>
        public int CheapestDepotPosition(NoDepotCVRProblem problem,
             int tourIndex, out float cheapestCost)
        {
            if (problem.Depot == null)
            {
                throw new NullReferenceException("CheapestDepotPosition can not work if there is no depot");
            }
            ITour tour = Tour(tourIndex);
            int depot = (int)problem.Depot;
            var w = problem.Weights;

            int current = tour.First;
            int next = tour.GetNeigbour(current);

            cheapestCost = float.MaxValue;
            int cheapestPoint = tour.First;

            do
            {
                float currentCost =
                    w[current][depot]
                  + w[depot][next]
                  - w[tour.First][tour.GetNeigbour(tour.First)];

                if (cheapestCost > currentCost)
                {
                    cheapestPoint = current;
                    cheapestCost = currentCost;
                }

                current = next;
                next = current == tour.Last ? tour.First :
                         tour.GetNeigbour(current);


            } while (current != tour.First);

            return cheapestPoint;
        }


        /// <summary>
        /// Calculates the depot position for the given tour and saves the cost in the solution
        /// <returns>The new cost</returns>
        /// </summary>
        public float UpdateDepotPosition(NoDepotCVRProblem problem, int tour)
        {
            var depotPoint = CheapestDepotPosition(problem, tour, out float depotCost);
            UpdateDepotPosition(tour, depotPoint, depotCost);
            return depotCost;
        }
        /// <summary>
        /// Sets the new depot point and cost
        /// <returns>The new cost</returns>
        ///</summary>
        public void UpdateDepotPosition(int tour, int depotPoint, float cost)
        {
            _depotCost[tour] = cost;
            _depotPoint[tour] = depotPoint;
        }

        /// <summary>
        /// Simulates how much the depot round trip would cost
        /// - if a new visit is placed or
        /// - if a visit is removed
        /// 
        /// If a visit is placed, a few things can happen to the depot and its cost:
        /// - Nothing. The depot does not change
        /// - Improvement due to a new point being available after calculation
        /// - An increase of the cost. In the case of (A --> Depot --> B), and an insertion of (A --> Depot --> C -> B), if the trip 'Depot --> C' is longer then 'Depot --> B'
        /// 
        /// If a visit is removed, analogous stuff can happen to the depot position and cost:
        /// - Nothing, the Depot does not change (most of the cases)
        /// - A decrease of the depot cost is _never_ possible
        /// - A small increase in the depot cost if the removed point is a neighbour of the depot.
        ///     Note that the travel cost of 'A --> Depot --> removed point --> C' is always larger then 'A --> Depot --> C' due to triangle inequality
        ///     Also note that a new egde might have become the new cheapest edge, further lowering the increased cost
        /// 
        /// This method is only interested in _the worst case_ to check that a constraint might be broken.
        /// It will thus only return a delta if the cost might have increased.
        /// If a constraint has been broken due to a to high cost, we can still try to recover by looking for a new lower depot cost. This is not done here
        /// </summary>
        public float SimulateWorstDepotCost(NoDepotCVRProblem problem, out int newDepotPoint, out bool depotMoved, int tour,
            int? placedVisit, int? after, Triple? removed)
        {

            // Some default out values and sanity check. Do we really have a depot?
            depotMoved = false;
            newDepotPoint = 0;
            if (problem.Depot == null)
            {
                return 0;
            }

            newDepotPoint = DepotPoint(tour);
            var newDepotCost = DepotCost(tour);
            var dp = DepotPoint(tour);
            var dn = DepotNeighbour(tour);
            var t = Tour(tour);


            // we only care about a possible decrease of performance.
            // This happens when:
            depotMoved = (placedVisit == null || after == null) ? false :
                                dp == placedVisit; // case of new point insertion in the depot edge
            depotMoved = depotMoved ||
                            (removed == null ? false :
                                dp == ((Triple)removed).Along || dn == ((Triple)removed).Along); // and the case of a point removal increasing the cost

            if (depotMoved)
            {
                // simulate the depot movement, get the best position
                if (placedVisit != null && after != null)
                {
                    t.InsertAfter((int)after, (int)placedVisit);
                }
                if (removed != null)
                {
                    t.Remove(((Triple)removed).Along);
                }

                // Actually calculate the new depot position and cost
                newDepotPoint = CheapestDepotPosition(problem, tour, out newDepotCost);

                // roll back the changes
                if (removed != null)
                {
                    Triple r = (Triple)removed;
                    t.InsertAfter(r.From, r.Along);
                }
                if (placedVisit != null && after != null)
                {
                    t.Remove((int)placedVisit);
                }
            }



            return newDepotCost;
        }

        public float SimulateWorstDepotCost(NoDepotCVRProblem problem, out Operators.Seq  newDepotPoint, out bool depotMoved, int tour,
            int? placedVisit, int? after, Triple? removed)
        {

        }

        ///<summary>
        /// Simulates the given changes and how much the depot round trip would cost in that case
        ///</summary>
        public float SimulateBestDepotCost(NoDepotCVRProblem problem, out int newDepotPoint, int tour,
                int? placedVisit, int? after, Triple? removed)
        {
            // Some default out values and sanity check. Do we really have a depot?
            newDepotPoint = 0;
            if (problem.Depot == null)
            {
                return 0;
            }


            var t = Tour(tour);

            // simulate the depot movement, get the best position
            if (placedVisit != null && after != null)
            {
                t.InsertAfter((int)after, (int)placedVisit);
            }
            if (removed != null)
            {
                t.Remove(((Triple)removed).Along);
            }

            // Actually calculate the new depot position and cost
            newDepotPoint = CheapestDepotPosition(problem, tour, out float newDepotCost);

            // roll back the changes
            if (removed != null)
            {
                Triple r = (Triple)removed;
                t.InsertAfter(r.From, r.Along);
            }
            if (placedVisit != null && after != null)
            {
                t.Remove((int)placedVisit);
            }

            return newDepotCost;

        }
    }
}