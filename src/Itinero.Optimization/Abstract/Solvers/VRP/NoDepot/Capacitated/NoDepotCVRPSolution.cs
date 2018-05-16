/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
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
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Tours.Sequences;
using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Represents a solution to a capacitated no-depot VRP.
    /// </summary>
    public class NoDepotCVRPSolution : ISolution, IRelocateSolution, IExchangeSolution, IMultiExchangeSolution, IMultiRelocateSolution,
        ISeededCheapestInsertionSolution, IGuidedVNSSolution
    {
        private readonly List<CapacityExtensions.Content> _contents;
        private readonly List<ITour> _tours; // does not use multitour, as some problems require the same depot to be visited multiple times

        public List<ITour> Tours => _tours;
        
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
        /// Calulates the total weight.
        /// </summary>
        public float CalculateTotalWeight()
        {
            var weight = 0f;

            for (var t = 0; t < Count; t++)
            {
                weight += Contents[t].Weight;
            }

            return weight * Count;
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
                // When creating a new tour, this tour is [first] -> [first]
                // The depot has to be visited in between, the "actual" tour the vehicle will do is thus: [first] -> depot -> [first]
                // We however don't save this, in order to lie to the rest of the heuristics (the depot should in this case **not** influence the solution)

                // We keep track of where the depot should be positioned in the tour and save the initial cost for this
                var w = problem.Weights;
                int depot = (int)problem.Depot;
                float cost = w[first][depot] + w[depot][last] - w[first][last];
                _depotPoint.Add(first);
                _depotCost.Add(cost);


                // but... The time is not the only cost of the depot. The depot might also have a visitcost (weight/time/...) associated with it
                // This has to be taken into account as well. 

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
        /// <param name="problem">The problem containing all information</param>
        /// <param name="tourIndex">The tour number for which the depot point should be calculated</param>
        /// <param name="cheapestCost">Will be filled with the cost of the depot round trip</param>
        /// <param name="removedPoints">If this is given, the tour will consider the shortcut removedPoints.First -> removedPoints.Last to be in the tour and ignore all the inner visits of the seq. This should not cut out the last element.</param>
        /// </summary>
        public int CalculateDepotPosition(NoDepotCVRProblem problem,
             int tourIndex, out float cheapestCost, Operators.Seq? removedPoints = null)
        {
            if (problem.Depot == null)
            {
                throw new NullReferenceException("CheapestDepotPosition can not work if there is no depot");
            }

            if (removedPoints != null)
            {
                var rp = (Operators.Seq)removedPoints;
                if (rp.Length < 3)
                {
                    throw new ArgumentException("When a sequence of removed points is given, it should contain at least three elements");
                }
                if (rp[0] == rp[rp.Length - 1])
                {
                    throw new ArgumentException("When a sequence of removed points is given, the first and last element should be different");

                }
            }

            var tour = Tour(tourIndex);
            int depot = (int)problem.Depot;
            var w = problem.Weights;

            int current = tour.First;
            int next = tour.GetNeigbour(current);

            cheapestCost = float.MaxValue;
            int cheapestPoint = tour.First;

            int? shortcutStart = null;
            int? shortcutEnd = null;
            if (removedPoints != null)
            {
                var rp = (Operators.Seq)removedPoints;
                shortcutStart = rp[0];
                shortcutEnd = rp[rp.Length - 1];

                if(current == shortcutStart){
                    next = (int) shortcutEnd;
                }
            }


            do
            {
                float currentCost =
                    w[current][depot]
                  + w[depot][next]
                  - w[current][next];

                if (cheapestCost > currentCost)
                {
                    cheapestPoint = current;
                    cheapestCost = currentCost;
                }

                current = next;
                if (current == tour.Last)
                {
                    next = tour.First;
                }
                else if (shortcutStart != null)
                {
                    next = current == (int)shortcutStart ? (int)shortcutEnd : tour.GetNeigbour(current);
                }
                else
                {
                    next = tour.GetNeigbour(current);
                }


            } while (current != tour.First);

            return cheapestPoint;
        }


        /// <summary>
        /// Calculates the depot position for the given tour and saves the cost in the solution
        /// <returns>The new cost</returns>
        /// </summary>
        public float UpdateDepotPosition(NoDepotCVRProblem problem, int tour)
        {
            var depotPoint = CalculateDepotPosition(problem, tour, out float depotCost);
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


        ///<summary>
        /// If no optional parameters are given, this method returns the already cached values
        ///
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
        /// If 'worstOnly' is enabled, then the mehtod is only interested in _the worst case_ to check that a constraint might be broken.
        /// It will thus only return a new cost if the cost might have increased.
        /// If a constraint has been broken due to a to high cost, we can still try to recover by looking for a new lower depot cost later on.
        /// 
        /// At last, if this method is invoked when the problem does not contain a depot, the value returned is 0
        ///
        ///<param name="removedVisits">The visits that are removed. Important: the first element and last element of the sequence are still retained in the tour! Only the elements in between are removed</param>
        ///<param name="placedVisit">A single, new visit which is placed, inserted after the 'after' parameter. Should not be used together with the placedVisits-sequence</param>
        ///<param name="placedVisits">Visits which are inserted in the tour. The are inserted after the 'after' parameter</param>
        /// <param name="after">The visit after which placedVisit(s) will be inserted</param>
        /// <param name="problem">The problem containing all the information</param>
        /// <param name="newDepotPoint">Will be filled with the new depot point</param>
        /// <param name="tour">The tour number</param>
        /// Simulates the given changes and calculates the best, new depot point. Gives the new depot cost as return and the Depot Point which it'll be afterwards
        ///<param name="worstOnly">If enabled, the depot will not be searched within the newly placed to speed up the calculation</param>
        ///</summary>
        public float SimulateDepotCost(NoDepotCVRProblem problem, out int newDepotPoint, int tour,
            int? placedVisit = null, Operators.Seq? placedVisits = null, int? after = null, Operators.Seq? removedVisits = null, bool worstOnly = false)
        {
            if (problem.Depot == null)
            {
                newDepotPoint = 0;
                return 0f;
            }
            var dp = DepotPoint(tour); // we need those constantly, so we make a convenient variable for it
            var dn = DepotNeighbour(tour);
            var dc = DepotCost(tour);
            newDepotPoint = dp;

            if (placedVisit != null && placedVisits != null)
            {
                throw new NotImplementedException("SimulateDepotCost: don't use a placedVisit (int) and placedVisits (sequence) at once. That's a bit complicated");
            }

            /* We have a tour, with a depot point, a sequence that is removed and a sequence that is added.
            * If the depot point or neighbour is in the sequence that is removed, we have to find a new depot point and evaluate everything of the old sequence
            * Then, we should also evaluate the newly added visits. We inspect them all to determine if a visit there is lower then the current visit.
            * At last, there is a pesky corner case if the depot point is the border of the newly added visits
             */


            var depotPointRemoved = false;
            if (removedVisits != null)
            {
                var removed = (Operators.Seq)removedVisits;
                if (removed.InnerContains(dp) || removed.InnerContains(dn))
                {
                    // we have to find a new depot 
                    depotPointRemoved = true;
                }
            }

            if (placedVisits != null)
            {
                var placed = (Operators.Seq)placedVisits;
                var aftr = (int)after;

                if (dp == aftr)
                {
                    // We should find a new depot as the route now continues with the inserted visits
                    depotPointRemoved = true;
                }
            }
            else if (placedVisit != null)
            {
                if (dp == (int)after)
                {
                    depotPointRemoved = true;
                }
            }

            // Here, we know if we have to search for a new depot point. If so, we search
            if (depotPointRemoved)
            {

                if (placedVisit != null)
                {
                    // the case that a single new point is placed. We add it, calculate the new position and remove it again
                    int aftr = (int)after;
                    int placed = (int)placedVisit;

                    var t = Tour(tour);
                    // simulate point addition
                    t.InsertAfter(aftr, placed);

                    newDepotPoint = CalculateDepotPosition(problem, tour, out dc, removedVisits);

                    t.ReplaceEdgeFrom(aftr, t.GetNeigbour(placed));

                }
                else
                {
                    newDepotPoint = CalculateDepotPosition(problem, tour, out dc, removedVisits);
                }

            }

            // Right now, we have a depot cost (new or old). The newly placed visits sequence might offer a better point
            if (!worstOnly && placedVisits != null)
            {
                var placed = (Operators.Seq)placedVisits;
                int aftr = (int)after;
                int depot = (int)problem.Depot;
                var w = problem.Weights;
                var lst = placed.Length - 1;
                var t = Tour(tour);
                // cost of depot round trip from 'after -> new visits'
                float depotCostBeforeNew = w[aftr][depot] + w[depot][placed[0]] - w[aftr][placed[0]];
                if (depotCostBeforeNew < dc)
                {
                    dc = depotCostBeforeNew;
                    newDepotPoint = aftr;
                }
                // cost of depot round trip from 'new visits' to 'neighbour(after)'
                float depotCostAfterNew = w[placed[lst]][depot] + w[depot][t.GetNeigbour(aftr)] - w[placed[lst]][t.GetNeigbour(aftr)];

                if (depotCostAfterNew < dc)
                {
                    dc = depotCostAfterNew;
                    newDepotPoint = placed[lst];
                }

                // lets start the actual search within the newly placed sequence
                for (int i = 0; i < placed.Length - 2; i++)
                {
                    float cost = w[placed[i]][depot] + w[depot][placed[i + 1]] - w[placed[i]][placed[i + 1]];
                    if (cost < dc)
                    {
                        dc = cost;
                        newDepotPoint = placed[i];
                    }
                }
            }
            return dc;
        }
    }
}