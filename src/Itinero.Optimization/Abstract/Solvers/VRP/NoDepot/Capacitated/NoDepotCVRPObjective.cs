﻿/*
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
using System.Linq;
using Itinero.Algorithms.Contracted.EdgeBased.Witness;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Tours.Sequences;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// An objective of a CVRP, without a depot.
    /// 
    /// If a depot is given, this depot is only used to calculate the extra cost between the cluster and the depot (and to calculate the cheapest point in the cluster to visit the depot)
    /// 
    /// </summary>
    public class NoDepotCVRPObjective : ObjectiveBase<NoDepotCVRProblem, NoDepotCVRPSolution, float>,
        IRelocateObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        IExchangeObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        IMultiExchangeObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        IMultiRelocateObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        ISeededCheapestInsertionObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        IGuidedVNSObjective<NoDepotCVRProblem, NoDepotCVRPSolution, Penalties>
    {
        private readonly Func<NoDepotCVRProblem, IList<int>, int> _seedFunc;
        private readonly Func<NoDepotCVRProblem, int, int, float> _localizationCostFunc;
        private readonly float _gvnsPenalty; // hold the lambda value, define a better name.

        private readonly Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> _overlapsFunc;
        private readonly float _slackPercentage;

        /// <summary>
        /// Creates a new objective.
        /// </summary>
        /// <param name="seedFunc">The seed function.</param>
        /// <param name="overlapsFunc">The overlaps function.</param>
        /// <param name="localizationFactor">The localization factor.</param>
        /// <param name="slackPercentage">The slack percentage.</param>        
        /// <param name="gvnsPenalty">The penalty value.</param>
        public NoDepotCVRPObjective(Func<NoDepotCVRProblem, IList<int>, int> seedFunc,
            Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> overlapsFunc,
            float localizationFactor = 0.5f, float slackPercentage = .05f, float gvnsPenalty = 8 * 60)
        {
            _seedFunc = seedFunc;
            _overlapsFunc = overlapsFunc;
            _slackPercentage = slackPercentage;
            _gvnsPenalty = gvnsPenalty;
            _localizationCostFunc = null;
            if (localizationFactor != 0)
            {
                // create a function to add the localized effect (relative to the seed) to the CI algorithm
                // if lambda is set.
                _localizationCostFunc = (p, s, v) => (p.Weights[s][v] +
                                                      p.Weights[v][s]) * localizationFactor;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override string Name => "DEFAULT";

        /// <summary>
        /// Returns true if this is non-continuous.
        /// 
        /// If a problem is continuous, an update in fitness can happen by taking the local differences and having a delta.
        /// This is not possible here, fitness should always be recalculated by taking everything into account.
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsNonContinuous => false;

        /// <summary>
        /// Returns the fitness value equivalent to zero.
        /// </summary>
        public override float Zero => 0;

        /// <summary>
        /// Returns the fitness value equivalent to infinite.
        /// </summary>
        public override float Infinite => float.MaxValue;

        /// <summary>
        /// Adds the two given values.
        /// </summary>
        public override float Add(NoDepotCVRProblem problem, float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }


        /// <summary>
        /// Compares the two given fitness values.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="fitness1"></param>
        /// <param name="fitness2"></param>
        /// <returns></returns>
        public override int CompareTo(NoDepotCVRProblem problem, float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is equivalent to zero.
        /// </summary>
        public override bool IsZero(NoDepotCVRProblem problem, float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Subtracts the two given fitness values.
        /// </summary>
        public override float Subtract(NoDepotCVRProblem problem, float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }

        /// <summary>
        /// Updates the solution after the problem has changed.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        public void UpdateSolution(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
        {
            this.UpdateContent(problem, solution);
        }

        /// <summary>
        /// Updates all content properly.
        /// </summary>
        public void UpdateContent(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
        {
            for (var t = 0; t < solution.Count; t++)
            {
                if (t >= solution.Contents.Count)
                {
                    solution.Contents.Add(new CapacityExtensions.Content());
                }

                solution.Contents[t].Weight = solution.CalculateWeightOf(problem, t);
                problem.Capacity.UpdateCosts(solution.Contents[t], solution.Tour(t));
            }
        }


        /// <summary>
        /// Tries to move the given visit (the middle of the triple) from t1 into t2.
        /// CENTRAL_DEPOT_PROOF
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="visit">The visit.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns>True if the move has succeeded.</returns>
        public bool TryMove(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, 
            int t1, int t2, 
            Triple visit,
            out float delta)
        {
            
            
            // Epsilon. An increase should be at least E before being considered
            var E = 0.1f;

            delta = 0;
            if (!problem.Capacity.CanAdd(solution.Contents[t2], visit.Along))
            {
                // capacity doesn't allow placing this visit.
                return false;
            }

            // calculate the gain of removing visit.Along from t1.
            var removalGain = problem.Weights[visit.From][visit.Along] + problem.Weights[visit.Along][visit.To] -
                              problem.Weights[visit.From][
                                  visit.To]; // we do NOT take depotDelta into account here; we do not optimize for this as well
            if (removalGain <= E)
            {
                // to little to gain from this removal. We don't do it
                return false;
            }


            // calculate cheapest placement.
            var tour2 = solution.Tour(t2);
            var visitCost = problem.GetVisitCost(visit.Along);

            var result = tour2.CalculateCheapest(problem.Weights, visit.Along, out Pair location);
            if (result >= removalGain - E ||
                solution.Contents[t2].Weight + result + visitCost >= problem.Capacity.Max)
            {
                // there is NO a gain in relocating this visit.
                return false;
            }

            /* 
            Keeping track of the constraint 'total cost + driving to the depot < maxAllowed':
            
            - Tour 1 loses a point, and will always have a lower cost (especially when the depot has to be, per triangle inequality).
              Thus we don't have to worry about the extra depot cost going over capacity and we only have to calculate a new cheapest depot if a depot neighbour has moved.

            - Tour 2 will have higher costs (due to the extra point). As long as 'oldCost + newVisitCost + oldDepotCost' is lower then the maximum, there is no problem as well.
              If the costs raise above the maximum, we might try to place the depot again and hope that 'oldCost + newVisitCost + newDepotCost' falls below the maximum.
              The newDepotCost can only be lower at this point, as an extra and potentially cheaper point is available.
            
            In other words, we can postpone calculating a new depot until we really have to; although we have to check if the constraint is not broken for tour2.
            * */

            float newDepotCost = solution.SimulateDepotCost(problem, out int newDepotPoint, t2,
                placedVisit: visit.Along, after: location.From, worstOnly: true);
            if (!problem.Capacity.Subtract(newDepotCost).CanAdd(solution.Contents[t2], visit.Along))
            {
                // HOUSTON: we have a problem
                // The capacity constraint is broken

                // Simulate a new best position

                // Try to recover by getting a better depot placement 
                var newCost = solution.SimulateDepotCost(problem, out int newDepot, t2,
                    placedVisit: visit.Along, after: location.From);
                if (!problem.Capacity.Subtract(newCost).CanAdd(solution.Contents[t2], visit.Along))
                {
                    // still to much to handle. Revert!
                    return false;
                }

                // K, chill, the new depot position works. Lets tell the solution it has to update this, as we commit anyway
                solution.UpdateDepotPosition(t2, newDepot, newCost);
            }

            var tour1 = solution.Tour(t1);
            if (tour1.First == visit.Along)
            {
                // move first.
                var newTour1 = new Tour(tour1.From(visit.To), visit.To);
                solution.ReplaceTour(t1, newTour1);
                tour1 = newTour1;
            }

            if (problem.Depot != null)
            {
                if (visit.Along == solution.DepotPoint(t1) || visit.Along == solution.DepotNeighbour(t1))
                {
                    // t1 should update the depot position
                    solution.UpdateDepotPosition(problem, t1);
                }
            }

            // cut out visit.Along from t1
            tour1.ReplaceEdgeFrom(visit.From, visit.To);
            // add the visit to t2
            tour2.InsertAfter(location.From, visit.Along);

            // update weights and capacities.
            problem.Capacity.Remove(solution.Contents[t1], visit.Along);
            problem.Capacity.Add(solution.Contents[t2], visit.Along);
            solution.Contents[t1].Weight = solution.CalculateWeightOf(problem, t1);
            solution.Contents[t2].Weight = solution.CalculateWeightOf(problem, t2);

            // Upate the depot position
            if (problem.Depot != null)
            {
                solution.UpdateDepotPosition(problem, t1);
                solution.UpdateDepotPosition(problem, t2);
            }

            delta = removalGain - result;
            return true;
        }

        /// <summary>
        /// Tries to swap the given visits between the two given tours.
        /// CENTRAL_DEPOT_PROOF
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="visit1">The visit from tour1.</param>
        /// <param name="visit2">The visit from tour2.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns></returns>
        public bool TrySwap(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2, Triple visit1,
            Triple visit2,
            out float delta)
        {
            var E = 0.01f;

            var tour1 = solution.Tour(t1);
            var tour2 = solution.Tour(t2);

            if (tour1.First == visit1.Along ||
                tour2.First == visit2.Along)
            {
                // for now we cannot move the first visit.
                // TODO: enable this objective to move the first visit.
                delta = 0;
                return false;
            }

            // The current weight of visit1.along within t1
            var weight1 = problem.Weights[visit1.From][visit1.Along] +
                          problem.Weights[visit1.Along][visit1.To];
            var weight2 = problem.Weights[visit2.From][visit2.Along] +
                          problem.Weights[visit2.Along][visit2.To];

            // how much weight visit2.along will weigh within t1
            var weight1Swapped = problem.Weights[visit1.From][visit2.Along] +
                                 problem.Weights[visit2.Along][visit1.To];
            var weight2Swapped = problem.Weights[visit2.From][visit1.Along] +
                                 problem.Weights[visit1.Along][visit2.To];

            // the weight difference
            var difference = (weight1 + weight2) - (weight1Swapped + weight2Swapped);
            if (difference <= E)
            {
                // new weights are not better.
                delta = 0;
                return false;
            }

            var visit1Cost = problem.GetVisitCost(visit1.Along);
            var visit2Cost = problem.GetVisitCost(visit2.Along);

            var tour1WeightSwapped = solution.Contents[t1].Weight - weight1 +
                                     weight1Swapped - visit1Cost + visit2Cost;

            // We have weight difference and know that the new weights are a better solution
            // Time to check that we don't violate constraints
            // For this, we also need the depot cost. 

            // Lets get the simulated worst depot costs
            var tour1depotWeight = solution.SimulateDepotCost(problem, out int newDepotPoint1, t1,
                placedVisit: visit2.Along, after: visit1.From, removedVisits: visit1.AsSequence, worstOnly: true);

            if (tour1WeightSwapped + tour1depotWeight > problem.Capacity.Max)
            {
                // constraint violated.
                // try to recover by having the best depot position
                tour1depotWeight = solution.SimulateDepotCost(problem, out newDepotPoint1, t1,
                    placedVisit: visit2.Along, after: visit1.From, removedVisits: visit1.AsSequence);
                if (tour1WeightSwapped + tour1depotWeight > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }


            var tour2depotWeight = solution.SimulateDepotCost(problem, out int newDepotPoint2, t2,
                placedVisit: visit1.Along, after: visit2.From, removedVisits: visit2.AsSequence, worstOnly: true);
            var tour2WeightSwapped = solution.Contents[t2].Weight - weight2 +
                                     weight2Swapped - visit2Cost + visit1Cost;
            if (tour2WeightSwapped + tour2depotWeight > problem.Capacity.Max)
            {
                // constraint violated. try to recover with a better depot position
                tour2depotWeight = solution.SimulateDepotCost(problem, out newDepotPoint2, t2,
                    placedVisit: visit1.Along, after: visit2.From, removedVisits: visit2.AsSequence, worstOnly: true);
                if (tour2WeightSwapped + tour2depotWeight > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }


            // check constraints if any.
            if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1],
                visit1.Along, visit2.Along, tour1depotWeight))
            {
                // constraint violated. Try to recover...
                tour1depotWeight = solution.SimulateDepotCost(problem, out newDepotPoint1, t1,
                    visit2.Along, after: visit1.From, removedVisits: visit1.AsSequence);
                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1],
                    visit1.Along, visit2.Along, tour1depotWeight))
                {
                    delta = 0;
                    return false;
                }
            }

            if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2],
                visit2.Along, visit1.Along, tour2depotWeight))
            {
                // constraint violated. Try to recover...
                tour2depotWeight = solution.SimulateDepotCost(problem, out newDepotPoint2,
                    t2, visit1.Along, after: visit2.From, removedVisits: visit2.AsSequence, worstOnly: true);
                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2],
                    visit2.Along, visit1.Along, tour2depotWeight))
                {
                    delta = 0;
                    return false;
                }
            }

            // exchange visit.
            tour1.ReplaceEdgeFrom(visit1.From, visit2.Along);
            tour1.ReplaceEdgeFrom(visit2.Along, visit1.To);
            tour2.ReplaceEdgeFrom(visit2.From, visit1.Along);
            tour2.ReplaceEdgeFrom(visit1.Along, visit2.To);

            solution.UpdateDepotPosition(t1, newDepotPoint1, tour1depotWeight);
            solution.UpdateDepotPosition(t2, newDepotPoint2, tour2depotWeight);
            // update content.
            problem.Capacity.UpdateExchange(solution.Contents[t1], visit1.Along, visit2.Along);
            problem.Capacity.UpdateExchange(solution.Contents[t2], visit2.Along, visit1.Along);
            solution.Contents[t1].Weight = tour1WeightSwapped;
            solution.Contents[t2].Weight = tour2WeightSwapped;

            // automatically removed in release mode.
            tour1.Verify(problem.Weights.Length);
            tour2.Verify(problem.Weights.Length);

            delta = difference;
            
            return true;
        }

        /// <summary>
        /// Simulates a swap of the given sequences between the two given tours.
        /// DEPOT_PROOF
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="s1">The sequence from tour1.</param>
        /// <param name="s2">The sequence from tour2.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns>True if the swap is possible.</returns>   
        public bool SimulateSwap(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2,
            Operators.Seq s1, Operators.Seq s2, out float delta)
        {
            var E = 0.01f;

            var pair1 = new Pair(s1[0], s1[s1.Length - 1]);
            var pair1Between = new Pair(s1[1], s1[s1.Length - 2]);
            var pair2 = new Pair(s2[0], s2[s2.Length - 1]);
            var pair2Between = new Pair(s2[1], s2[s2.Length - 2]);

            var tour1Current = s1.TotalOriginal;
            var tour1Future = problem.Weights[pair1.From][pair2Between.From] +
                              problem.Weights[pair2Between.To][pair1.To] + s2.Between;

            var tour2Current = s2.TotalOriginal;
            var tour2Future = problem.Weights[pair2.From][pair1Between.From] +
                              problem.Weights[pair1Between.To][pair2.To] + s1.Between;

            var difference = tour1Current - tour1Future +
                             tour2Current - tour2Future;

            if (difference <= E)
            {
                // new weights are not better.
                delta = 0;
                return false;
            }

            var tour1FutureComplete = solution.Contents[t1].Weight - tour1Current + tour1Future;
            var tour1depotCost = solution.SimulateDepotCost
            (problem, out var newDepotPoint1, t1, placedVisits: s2, after: s1[0], removedVisits: s1,
                worstOnly: true);
            if (tour1FutureComplete + tour1depotCost > problem.Capacity.Max)
            {
                // constraint violated. Try to recover
                tour1depotCost = solution.SimulateDepotCost
                (problem, out newDepotPoint1, t1, placedVisits: s2, after: s1[0], removedVisits: s1,
                    worstOnly: false);
                if (tour1FutureComplete + tour1depotCost > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }

            var tour2FutureComplete = solution.Contents[t2].Weight - tour2Current + tour2Future;
            var tour2depotCost = solution.SimulateDepotCost
            (problem, out int newDepotPoint2, t2, placedVisits: s1, after: s2[0], removedVisits: s2,
                worstOnly: true);
            if (tour2FutureComplete + tour2depotCost > problem.Capacity.Max)
            {
                // constraint violated. Try to recover
                tour2depotCost = solution.SimulateDepotCost
                (problem, out newDepotPoint2, t2, placedVisits: s1, after: s2[0], removedVisits: s2,
                    worstOnly: false);
                if (tour2FutureComplete + tour2depotCost > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }

            if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1], s1, s2))
            {
                // constraint violated.
                delta = 0;
                return false;
            }

            if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2], s2, s1))
            {
                // constraint violated.
                delta = 0;
                return false;
            }

            // do the swap.
            delta = difference;
            return true;
        }

        /// <summary>
        /// Tries to swap the given sequences between the two given tours. If possible, the swap is performed
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="s1">The sequence from tour1.</param>
        /// <param name="s2">The sequence from tour2.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns>True if the swap succeeded.</returns>   
        public bool TrySwap(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2,
            Operators.Seq s1, Operators.Seq s2, out float delta)
        {
            var E = 0.01f;

            var pair1 = new Pair(s1[0], s1[s1.Length - 1]);
            var pair1Between = new Pair(s1[1], s1[s1.Length - 2]);
            var pair2 = new Pair(s2[0], s2[s2.Length - 1]);
            var pair2Between = new Pair(s2[1], s2[s2.Length - 2]);

            var tour1Current = s1.TotalOriginal;
            var tour1Future = problem.Weights[pair1.From][pair2Between.From] +
                              problem.Weights[pair2Between.To][pair1.To] + s2.Between;

            var tour2Current = s2.TotalOriginal;
            var tour2Future = problem.Weights[pair2.From][pair1Between.From] +
                              problem.Weights[pair1Between.To][pair2.To] + s1.Between;

            var difference = tour1Current - tour1Future +
                             tour2Current - tour2Future;

            if (difference <= E)
            {
                // new weights are not better.
                delta = 0;
                return false;
            }

            var tour1FutureComplete = solution.Contents[t1].Weight - tour1Current + tour1Future;
            var tour1depotCost = solution.SimulateDepotCost
            (problem, out int newDepotPoint1, t1, placedVisits: s2, after: s1[0], removedVisits: s1,
                worstOnly: true);
            if (tour1FutureComplete + tour1depotCost > problem.Capacity.Max)
            {
                // constraint violated. Try to recover
                tour1depotCost = solution.SimulateDepotCost
                (problem, out newDepotPoint1, t1, placedVisits: s2, after: s1[0], removedVisits: s1,
                    worstOnly: false);
                if (tour1FutureComplete + tour1depotCost > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }

            var tour2FutureComplete = solution.Contents[t2].Weight - tour2Current + tour2Future;
            var tour2depotCost = solution.SimulateDepotCost
            (problem, out int newDepotPoint2, t2, placedVisits: s1, after: s2[0], removedVisits: s2,
                worstOnly: true);
            if (tour2FutureComplete + tour2depotCost > problem.Capacity.Max)
            {
                // constraint violated. Try to recover
                tour2depotCost = solution.SimulateDepotCost
                (problem, out newDepotPoint2, t2, placedVisits: s1, after: s2[0], removedVisits: s2,
                    worstOnly: false);
                if (tour2FutureComplete + tour2depotCost > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }

            if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1], s1, s2))
            {
                // constraint violated.
                delta = 0;
                return false;
            }

            if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2], s2, s1))
            {
                // constraint violated.
                delta = 0;
                return false;
            }

            var tour1 = solution.Tour(t1);
            if (s1.Wraps)
            {
                // move first.
                var newTour1 = new Tour(tour1.From(s1[s1.Length - 1]), s1[s1.Length - 1]);
                solution.ReplaceTour(t1, newTour1);
                tour1 = newTour1;
            }

            var tour2 = solution.Tour(t2);
            if (s2.Wraps)
            {
                // move first.
                var newTour2 = new Tour(tour2.From(s2[s2.Length - 1]), s2[s2.Length - 1]);
                solution.ReplaceTour(t2, newTour2);
                tour2 = newTour2;
            }

            // do the swap.
            // s2 -> tour1
            var previous = pair1.From;
            for (var v = 1; v < s2.Length - 1; v++)
            {
                var visit = s2[v];
                tour1.ReplaceEdgeFrom(previous, visit);
                previous = visit;
            }

            tour1.ReplaceEdgeFrom(previous, pair1.To);

            // s1 -> tour2
            previous = pair2.From;
            for (var v = 1; v < s1.Length - 1; v++)
            {
                var visit = s1[v];
                tour2.ReplaceEdgeFrom(previous, visit);
                previous = visit;
            }

            tour2.ReplaceEdgeFrom(previous, pair2.To);
            delta = difference;

            problem.Capacity.UpdateExchange(solution.Contents[t1], s1, s2);
            problem.Capacity.UpdateExchange(solution.Contents[t2], s2, s1);

            solution.Contents[t1].Weight = tour1FutureComplete;
            solution.Contents[t2].Weight = tour2FutureComplete;

            if (problem.Depot != null)
            {
                solution.UpdateDepotPosition(t1, newDepotPoint1, tour1depotCost);
                solution.UpdateDepotPosition(t2, newDepotPoint2, tour2depotCost);
            }

            return true;
        }

        /// <summary>
        /// Tries to move the given inner contents of the sequence from t1 in between the given pair in t2.
        /// Seq[0] and seq[last] will still remain in tour1
        /// DEPOT_PROOF
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="seq">The sequence.</param>
        /// <param name="pair">The pair where the sequence will end in between</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns></returns>        
        public bool TryMove(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2,
            Operators.Seq seq, Pair pair, out float delta)
        {
            var E = 0.01f; // Needed minimal improvement to continue

            var pair1 = new Pair(seq[0], seq[seq.Length - 1]);
            var pair2 = pair;
            var sStart = seq[1];
            var sEnd = seq[seq.Length - 2];

            var tour1Current = problem.Weights[pair1.From][sStart] +
                               problem.Weights[sEnd][pair1.To];
            var tour1Future = problem.Weights[pair1.From][pair1.To];
            var tour2Current = problem.Weights[pair2.From][pair2.To];
            var tour2Future = problem.Weights[pair2.From][sStart] +
                              problem.Weights[sEnd][pair2.To];
            var difference = 0f;
            difference += tour1Current; // add what's in tour1 currently.
            difference += tour2Current; // add what's in tour2 currently.
            difference -= tour1Future; // subtract what would be new in tour1.
            difference -= tour2Future; // subtract what would be new in tour2.
            // Note: while the depot might move, the net cost of tour1 will always be less. No need to calculate a new depot cost, the depot round trip cost will not be violated
            if (difference <= E)
            {
                // new weights are not better.
                delta = 0;
                return false;
            }

            var tour2WeightMoved = solution.Contents[t2].Weight; // current weight of t2
            tour2WeightMoved -= tour2Current; // subtract the edge that is broken (pair.from -> pair.to)
            tour2WeightMoved += seq.Between; // add the cost of the inserted sequence
            tour2WeightMoved +=
                tour2Future; // the added cost of the new edges: 'pair.from -> seq[0]' and 'seq[last] -> pair.to'

            // tour2 will be a lot heavier, and even the depot itself might have significant new cost. Lets simulate it
            var tour2depotCost = solution.SimulateDepotCost(problem, out int newDepotPoint2, t2, placedVisits: seq,
                after: pair.From, worstOnly: true);

            if (tour2WeightMoved + tour2depotCost > problem.Capacity.Max)
            {
                // constraint violated. Try to recover

                tour2depotCost = solution.SimulateDepotCost(problem, out newDepotPoint2, t2, placedVisits: seq,
                    after: pair.From, worstOnly: false);
                if (tour2WeightMoved + tour2depotCost > problem.Capacity.Max)
                {
                    delta = 0;
                    return false;
                }
            }

            if (!problem.Capacity.CanAdd(solution.Contents[t2], seq))
            {
                // constraint violated.
                delta = 0;
                return false;
            }

            var tour1 = solution.Tour(t1);
            if (seq.Wraps)
            {
                // move first.
                var newTour1 = new Tour(tour1.From(seq[seq.Length - 1]), seq[seq.Length - 1]);
                solution.ReplaceTour(t1, newTour1);
                tour1 = newTour1;
            }

            // move the sequence.
            var tour2 = solution.Tour(t2);
            tour1.ReplaceEdgeFrom(pair1.From, pair1.To); // cut out the sequence
            var previous = pair.From;
            for (var i = 1; i < seq.Length - 1; i++)
            {
                // add the sequence to the other tour
                var visit = seq[i];
                tour2.ReplaceEdgeFrom(previous, visit);
                previous = visit;
            }

            tour2.ReplaceEdgeFrom(previous, pair.To);

            // update weights.
            var tour1WeightMoved = solution.Contents[t1].Weight;
            tour1WeightMoved -= tour1Current;
            tour1WeightMoved -= seq.Between;
            tour1WeightMoved += tour1Future;
            delta = (solution.Contents[t1].Weight - tour1WeightMoved) +
                    (solution.Contents[t2].Weight - tour2WeightMoved);
            solution.Contents[t1].Weight = tour1WeightMoved;
            solution.Contents[t2].Weight = tour2WeightMoved;

            if (problem.Depot != null)
            {
                solution.UpdateDepotPosition(t2, newDepotPoint2, tour2depotCost);
            }

        

            return true;
        }

        /// <summary>
        /// Enumerates all sequence of the given sizes.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="maxSize">The maximum size.</param>
        /// <param name="wrap">Wrap around first if true.</param>
        /// <returns>An enumerable with sequences.</returns>
        public IEnumerable<Operators.Seq> SeqAndSmaller(NoDepotCVRProblem problem, IEnumerable<int> tour,
            int minSize, int maxSize, bool wrap)
        {
            var visits = tour.ToArray();
            if (maxSize > visits.Length)
            {
                maxSize = visits.Length;
            }

            if (minSize > visits.Length)
            {
                // make sure not to enumerate anything.
                maxSize = -1;
            }

            var tourSequence = new Sequence(visits);
            for (var length = maxSize; length >= minSize; length--)
            {
                float start = 0;
                float end = 0;
                float? between = null;
                float betweenReversed = 0;
                var visitCost = 0f;
                foreach (var s in tourSequence.SubSequences(length, length, wrap))
                {
                    if (between == null)
                    {
                        // first sequence of this length.
                        var betweenSeq = s.SubSequence(1, s.Length - 2);
                        between = problem.Weights.Seq(betweenSeq);
                        start = problem.Weights[s[0]][s[1]];
                        end = problem.Weights[s[s.Length - 2]][s[s.Length - 1]];
                        betweenReversed = problem.Weights.SeqReversed(betweenSeq);
                        if (problem.VisitCosts != null)
                        {
                            visitCost = problem.VisitCosts.Seq(betweenSeq);
                        }
                    }
                    else
                    {
                        // move weights along.
                        var newStart = problem.Weights[s[0]][s[1]];
                        var newEnd = problem.Weights[s[s.Length - 2]][s[s.Length - 1]];
                        between -= newStart;
                        between += end;
                        betweenReversed -= problem.Weights[s[1]][s[0]];
                        betweenReversed += problem.Weights[s[s.Length - 2]][s[s.Length - 3]];
                        start = newStart;
                        end = newEnd;
                        if (problem.VisitCosts != null)
                        {
                            visitCost -= problem.VisitCosts[s[0]];
                            visitCost += problem.VisitCosts[s[s.Length - 2]];
                        }
                    }

                    if (between + start + end < problem.Weights[s[0]][s[s.Length - 1]] * 2f)
                    {
                        continue;
                    }

                    yield return new Operators.Seq(s)
                    {
                        BetweenTravelCost = between.Value,
                        BetweenTravelCostReversed = betweenReversed,
                        BetweenVisitCost = visitCost,
                        TotalOriginal = between.Value + end + start + visitCost
                    };
                }
            }
        }

        /// <summary>
        /// Reverses the given sequence.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="sequence">The sequence.</param>
        /// <returns>The reversed sequence.</returns>        
        public Operators.Seq Reverse(NoDepotCVRProblem problem, Operators.Seq sequence)
        {
            // mark as reversed and recalculate travel cost.
            sequence.Reversed = true;

            return sequence;
        }

        /// <summary>
        /// Creates a new and empty solution.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A new empty solution.</returns>
        public NoDepotCVRPSolution NewSolution(NoDepotCVRProblem problem)
        {
            return new NoDepotCVRPSolution(problem.Weights.Length);
        }

        /// <summary>
        /// Gets a list of potential visits.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>The list of visits to be visited, except potentially those uniquely used as seeds.</returns>
        public IList<int> PotentialVisits(NoDepotCVRProblem problem)
        {
            var visits = new List<int>(System.Linq.Enumerable.Range(0, problem.Weights.Length));
            if (problem.Depot != null)
            {
                visits.Remove((int) problem.Depot);
            }

            return visits;
        }

        /// <summary>
        /// Seeds the next tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="visits">The visit list.</param>
        /// <returns>The index of the new tour.</returns>
        public int SeedNext(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, IList<int> visits)
        {
            var seed = _seedFunc(problem, visits);
            visits.Remove(seed);
            var content = problem.Capacity.Empty();
            content.Weight += problem.GetVisitCost(seed);
            problem.Capacity.Add(content, seed);
            solution.Contents.Add(content);
            solution.Add(problem, seed); // Seed is both first and last
            return solution.Count - 1;
        }


        /// <summary>
        /// Tries to place any of the visits in the given visit list in the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t">The tour.</param>
        /// <param name="visits">The visit list.</param>
        /// <returns></returns>
        public bool TryPlaceAny(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t, IList<int> visits)
        {
            // We ignore the Depot round trip in this method

            var tour = solution.Tour(t);

            Func<int, float> costFunc = null;
            if (_localizationCostFunc != null)
            {
                costFunc = (v) => _localizationCostFunc(problem, tour.First, v);
            }

            Pair location;
            int visit;
            var increase = tour.CalculateCheapestAny(problem.Weights, visits,
                out location, out visit, costFunc);

            // calculate the actual increase if an extra cost was added.
            if (costFunc != null)
            {
                // use the seed cost; the cost to the seed visit.
                increase -= costFunc(visit);
            }

            // get the visit cost.
            var visitCost = problem.GetVisitCost(visit);

            // calculate the new weight.
            var potentialWeight = solution.Contents[t].Weight + increase + visitCost;
            // cram as many visits into one route as possible.
            if (!problem.Capacity.UpdateAndCheckCosts(solution.Contents[t], potentialWeight, visit))
            {
                // best placement is not impossible.
                return false;
            }

            if (!problem.Capacity.ConstraintsAreHonored(solution.Contents, solution.Tours))
            {
                return false;
            }

            // insert the visit, it is possible to add it. 
            visits.Remove(visit);
            tour.InsertAfter(location.From, visit);

            // update the cost of the route.
            solution.Contents[t].Weight = potentialWeight;

            return true;
        }

        /// <summary>
        /// Tries to place any of the visits in the given visit list.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The tour.</param>
        /// <param name="visits">The visits to try.</param>
        /// <returns></returns>
        public bool TryPlaceAny(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, IList<int> visits)
        {
            Func<int, float> costFunc = null;

            // Note: this function does not have to take the depot round trip into account

            // loop over all tours and find visit and the place to insert with the lowest cost.
            var bestIncrease = float.MaxValue;
            Triple? bestPlacement = null;
            int bestT = -1;
            for (int t = 0; t < solution.Count; t++)
            {
                var tour = solution.Tour(t);
                var seed = tour.First;

                costFunc = null;
                if (_localizationCostFunc != null)
                {
                    costFunc = (v) => _localizationCostFunc(problem, seed, v);
                }

                // run cheapest insertion algorithm.
                var increase = tour.CalculateCheapestAny(problem.Weights, visits,
                    out Pair location, out int visit,
                    costFunc);
                if (increase < bestIncrease)
                {
                    bestIncrease = increase;
                    bestPlacement = new Triple(location.From, visit, location.To);
                    bestT = t;
                }
            }

            if (bestPlacement == null)
            {
                // no best placement found.
                return false;
            }

            // calculate the actual increase if an extra cost was added.
            var bestTour = solution.Tour(bestT);
            var actualIncrease = bestIncrease;
            if (_localizationCostFunc != null)
            {
                actualIncrease -= _localizationCostFunc(problem, bestTour.First, bestPlacement.Value.Along);
            }

            // add the visit cost.
            actualIncrease += problem.GetVisitCost(bestPlacement.Value.Along);

            // try to do the actual insert
            var tourTime = solution.Contents[bestT].Weight; //objective.Calculate(problem, solution, bestTourIdx);
            if (!problem.Capacity.UpdateAndCheckCosts(solution.Contents[bestT], tourTime + actualIncrease,
                bestPlacement.Value.Along))
            {
                // insertion not possible.
                return false;
            }

            // insertion is possible, execute it.
            bestTour.InsertAfter(bestPlacement.Value.From, bestPlacement.Value.Along);
            visits.Remove(bestPlacement.Value.Along);

            return true;
        }

        /// <summary>
        /// Builds a TSP for the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="tour">The tour.</param>
        /// <returns>A TSP that can be solved by a TSP solver to optimize the given tour.</returns>
        public TSP.TSPSubProblem BuildSubTourTSP(NoDepotCVRProblem problem, ITour tour)
        {
            var newProblem = new TSP.TSPSubProblem(tour, problem.Weights);
            return newProblem;
        }

        /// <summary>
        /// Returns true if the two tours could benifit from inter-improvement optimizations.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns></returns>
        public bool HaveToTryInter(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2)
        {
            if (_overlapsFunc == null)
            {
                // if there's no function to limit the inter-optimizations then try them all.
                return true;
            }

            return _overlapsFunc(problem, solution.Tour(t1), solution.Tour(t2));
        }

        /// <summary>
        /// Creates a new instance of the given problem that has tighter constraints.
        /// </summary>
        /// <param name="problem">The original problem.</param>
        /// <returns>A problem that has a tighter constraints.</returns>
        public NoDepotCVRProblem ProblemWithSlack(NoDepotCVRProblem problem)
        {
            // create a new problem definition, taking into account the slack percentage.
            return new NoDepotCVRProblem(problem.Capacity.Scale(1 - _slackPercentage), problem.Weights,
                problem.VisitCosts, problem.Depot);
        }


        /// <summary>
        /// Applies a penalty to the given problem.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="penalty">The penalty.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <returns>True if a penalty was applied, false otherwise.</returns>
        public bool ApplyPenalty(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2,
            Penalties penalty)
        {
            var tour1 = solution.Tour(t1);
            var tour2 = solution.Tour(t2);

            // choose best next edge to penalize.
            var worstCost = 0f;
            Pair? worstPair = null;
            Penalty? worstPenalty = null;
            foreach (var e in tour1.Pairs().Concat(tour2.Pairs()))
            {
                var cost = 0f;
                if (!penalty.TryGetValue(e, out Penalty p))
                {
                    cost = problem.Weights[e.From][e.To];

                    if (worstPair == null ||
                        worstCost < cost)
                    {
                        worstCost = cost;
                        worstPair = e;
                        worstPenalty = new Penalty()
                        {
                            Count = 1,
                            Original = cost
                        };
                    }
                }
                else
                {
                    cost = ((_gvnsPenalty * p.Count) + p.Original) / (p.Count + 1);

                    if (worstPair == null ||
                        worstCost < cost)
                    {
                        worstCost = cost;
                        worstPair = e;
                        worstPenalty = new Penalty()
                        {
                            Count = (byte) (p.Count + 1),
                            Original = p.Original
                        };
                    }
                }
            }

            if (!worstPair.HasValue)
            {
                // no pairs found.
                return false;
            }

            if (penalty.Worst == 0)
            {
                // keep the worst cost to determine when we stop.
                penalty.Worst = worstCost;
            }

            penalty[worstPair.Value] = worstPenalty.Value;
            penalty.Total += 1;

            // check if we've penalized enough.
            if (penalty.Total * _gvnsPenalty > penalty.Worst * 5)
            {
                // enough penalties.
                return false;
            }

            // apply the penalties.
            foreach (var pair in penalty)
            {
                var e = pair.Key;
                var p = pair.Value;

                problem.Weights[e.From][e.To] = p.Original + _gvnsPenalty * p.Count;
            }

            return true;
        }

        /// <summary>
        /// Resets the penalty applied to the given problem.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="penalty">The penalty.</param>        
        public void ResetPenalty(NoDepotCVRProblem problem, Penalties penalty)
        {
            foreach (var pair in penalty)
            {
                var p = pair.Value;
                var e = pair.Key;

                problem.Weights[e.From][e.To] = p.Original;
            }
        }

        /// <summary>
        /// Calculates the total weight of the current solution
        /// </summary>
        public override float Calculate(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
        {
            return solution.CalculateTotalWeight();
        }
    }
}