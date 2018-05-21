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
using System.Linq;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GA;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Tours.Sequences;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// An objective of a CVRP.
    /// </summary>
    public class NoDepotCVRPObjective : ObjectiveBase<NoDepotCVRProblem, NoDepotCVRPSolution, float>,
        IRelocateObjective<NoDepotCVRProblem, NoDepotCVRPSolution>, IExchangeObjective<NoDepotCVRProblem, NoDepotCVRPSolution>, IMultiExchangeObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        IMultiRelocateObjective<NoDepotCVRProblem, NoDepotCVRPSolution>, ISeededCheapestInsertionObjective<NoDepotCVRProblem, NoDepotCVRPSolution>,
        IGuidedVNSObjective<NoDepotCVRProblem, NoDepotCVRPSolution, Penalties>, IGAObjective<NoDepotCVRProblem, NoDepotCVRPSolution>
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
            /// <param name="gvnsPenalty">The penalty value.</param>
            /// <param name="slackPercentage">The slack percentage.</param>
            public NoDepotCVRPObjective(Func<NoDepotCVRProblem, IList<int>, int> seedFunc, Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> overlapsFunc,
                float localizationFactor = 0.5f, float slackPercentage = .05f, float gvnsPenalty = 8 * 60)
            {
                _seedFunc = seedFunc;
                _overlapsFunc = overlapsFunc;
                _slackPercentage = slackPercentage;
                _gvnsPenalty = gvnsPenalty;

                _localizationCostFunc = null;
                if (localizationFactor != 0)
                { // create a function to add the localized effect (relative to the seed) to the CI algorithm
                    // if lambda is set.
                    _localizationCostFunc = (p, s, v) =>(p.Weights[s][v] +
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
            /// Calculates the weight of the given tour.
            /// </summary>
            public float Calculate(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int tourIdx)
            {
                var weight = 0f;
                var tour = solution.Tour(tourIdx);

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
                    solution.Contents[t].Weight = this.Calculate(problem, solution, t);
                    problem.Capacity.UpdateCosts(solution.Contents[t], solution.Tour(t));
                }
            }

            /// <summary>
            /// Calulates the total weight.
            /// </summary>
            public override float Calculate(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
            {
                var weight = 0f;

                for (var t = 0; t < solution.Count; t++)
                {
                    weight += solution.Contents[t].Weight;
                }

                return weight * solution.Count;
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
            /// Tries to move the given visit (the middle of the triple) from t1 -> t2.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <param name="visit">The visit.</param>
            /// <param name="delta">The difference in visit.</param>
            /// <returns>True if the move has succeeded.</returns>
            public bool TryMove(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2, Triple visit, out float delta)
            {
                var E = 0.1f;
                if (!problem.Capacity.CanAdd(solution.Contents[t2], visit.Along))
                { // capacity doesn't allow placing this visit.
                    delta = 0;
                    return false;
                }

                // calculate the removal gain of the visit.
                var removalGain = problem.Weights[visit.From][visit.Along] + problem.Weights[visit.Along][visit.To] -
                    problem.Weights[visit.From][visit.To];
                if (removalGain > E)
                { // calculate cheapest placement.

                    var tour2 = solution.Tour(t2);
                    var visitCost = problem.GetVisitCost(visit.Along);

                    Pair location;
                    var result = tour2.CalculateCheapest(problem.Weights, visit.Along, out location);
                    if (result < removalGain - E &&
                        solution.Contents[t2].Weight + result + visitCost < problem.Capacity.Max)
                    { // there is a gain in relocating this visit.
                        var tour1 = solution.Tour(t1);
                        if (tour1.First == visit.Along)
                        { // move first.
                            var newTour1 = new Tour(tour1.From(visit.To), visit.To);
                            solution.ReplaceTour(t1, newTour1);
                            tour1 = newTour1;
                        }

                        tour2.ReplaceEdgeFrom(location.From, visit.Along);
                        tour2.ReplaceEdgeFrom(visit.Along, location.To);

                        // in this model of the solution we need to remove the visit.
                        tour1.ReplaceEdgeFrom(visit.From, visit.To);

                        // update weights and capacities.
                        problem.Capacity.Remove(solution.Contents[t1], visit.Along);
                        problem.Capacity.Add(solution.Contents[t2], visit.Along);
                        solution.Contents[t1].Weight = this.Calculate(problem, solution, t1);
                        solution.Contents[t2].Weight = this.Calculate(problem, solution, t2);

                        delta = removalGain - result;
                        return true;
                    }
                }
                delta = 0;
                return false;
            }

            /// <summary>
            /// Tries to swap the given visits between the two given tours.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <param name="visit1">The visit from tour1.</param>
            /// <param name="visit2">The visit from tour2.</param>
            /// <param name="delta">The difference in visit.</param>
            /// <returns></returns>
            public bool TrySwap(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2, Triple visit1, Triple visit2,
                out float delta)
            {
                var E = 0.01f;

                var tour1 = solution.Tour(t1);
                var tour2 = solution.Tour(t2);

                if (tour1.First == visit1.Along ||
                    tour2.First == visit2.Along)
                { // for now we cannot move the first visit.
                    // TODO: enable this objective to move the first visit.
                    delta = 0;
                    return false;
                }

                var weight1 = problem.Weights[visit1.From][visit1.Along] +
                    problem.Weights[visit1.Along][visit1.To];
                var weight2 = problem.Weights[visit2.From][visit2.Along] +
                    problem.Weights[visit2.Along][visit2.To];

                var weight1Swapped = problem.Weights[visit1.From][visit2.Along] +
                    problem.Weights[visit2.Along][visit1.To];
                var weight2Swapped = problem.Weights[visit2.From][visit1.Along] +
                    problem.Weights[visit1.Along][visit2.To];

                var difference = (weight1 + weight2) - (weight1Swapped + weight2Swapped);
                if (difference <= E)
                { // new weights are not better.
                    delta = 0;
                    return false;
                }
                var visit1Cost = problem.GetVisitCost(visit1.Along);
                var visit2Cost = problem.GetVisitCost(visit2.Along);

                var tour1WeightSwapped = solution.Contents[t1].Weight - weight1 +
                    weight1Swapped - visit1Cost + visit2Cost;
                if (tour1WeightSwapped > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }
                var tour2WeightSwapped = solution.Contents[t2].Weight - weight2 +
                    weight2Swapped - visit2Cost + visit1Cost;
                if (tour2WeightSwapped > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                // check constraints if any.
                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1],
                        visit1.Along, visit2.Along))
                { // constraint violated.
                    delta = 0;
                    return false;
                }
                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2],
                        visit2.Along, visit1.Along))
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                // exchange visit.
                tour1.ReplaceEdgeFrom(visit1.From, visit2.Along);
                tour1.ReplaceEdgeFrom(visit2.Along, visit1.To);
                tour2.ReplaceEdgeFrom(visit2.From, visit1.Along);
                tour2.ReplaceEdgeFrom(visit1.Along, visit2.To);

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
                { // make sure not to enumerate anything.
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
                        { // first sequence of this length.
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
                        { // move weights along.
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
            /// Simulates a swap of the given sequences between the two given tours.
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
                { // new weights are not better.
                    delta = 0;
                    return false;
                }

                var tour1FutureComplete = solution.Contents[t1].Weight - tour1Current + tour1Future;
                if (tour1FutureComplete > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                var tour2FutureComplete = solution.Contents[t2].Weight - tour2Current + tour2Future;
                if (tour2FutureComplete > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1], s1, s2))
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2], s2, s1))
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                // don't do the swap.
                delta = difference;
                return true;
            }

            /// <summary>
            /// Tries to swap the given sequences between the two given tours.
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
                { // new weights are not better.
                    delta = 0;
                    return false;
                }

                var tour1FutureComplete = solution.Contents[t1].Weight - tour1Current + tour1Future;
                if (tour1FutureComplete > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                var tour2FutureComplete = solution.Contents[t2].Weight - tour2Current + tour2Future;
                if (tour2FutureComplete > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t1], s1, s2))
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                if (!problem.Capacity.ExchangeIsPossible(solution.Contents[t2], s2, s1))
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                var tour1 = solution.Tour(t1);
                if (s1.Wraps)
                { // move first.
                    var newTour1 = new Tour(tour1.From(s1[s1.Length - 1]), s1[s1.Length - 1]);
                    solution.ReplaceTour(t1, newTour1);
                    tour1 = newTour1;
                }

                var tour2 = solution.Tour(t2);
                if (s2.Wraps)
                { // move first.
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
                return true;
            }

            /// <summary>
            /// Tries to move the given sequence from t1 in between the given pair in t2.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <param name="seq">The sequence.</param>
            /// <param name="pair">The pair.</param>
            /// <param name="delta">The difference in visit.</param>
            /// <returns></returns>        
            public bool TryMove(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2, Operators.Seq seq, Pair pair, out float delta)
            {
                var E = 0.01f;

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
                if (difference <= E)
                { // new weights are not better.
                    delta = 0;
                    return false;
                }

                var tour2WeightMoved = solution.Contents[t2].Weight;
                tour2WeightMoved -= tour2Current;
                tour2WeightMoved += seq.Between;
                tour2WeightMoved += tour2Future;

                if (tour2WeightMoved > problem.Capacity.Max)
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                if (!problem.Capacity.CanAdd(solution.Contents[t2], seq))
                { // constraint violated.
                    delta = 0;
                    return false;
                }

                var tour1 = solution.Tour(t1);
                if (seq.Wraps)
                { // move first.
                    var newTour1 = new Tour(tour1.From(seq[seq.Length - 1]), seq[seq.Length - 1]);
                    solution.ReplaceTour(t1, newTour1);
                    tour1 = newTour1;
                }

                // move the sequence.
                var tour2 = solution.Tour(t2);
                tour1.ReplaceEdgeFrom(pair1.From, pair1.To);
                var previous = pair.From;
                for (var v = 1; v < seq.Length - 1; v++)
                {
                    var visit = seq[v];
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

                problem.Capacity.Remove(solution.Contents[t1], seq);
                problem.Capacity.Add(solution.Contents[t2], seq);

                return true;
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
                return new List<int>(System.Linq.Enumerable.Range(0, problem.Weights.Length));
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
                return this.NewTour(problem, solution, seed);
            }

            /// <summary>
            /// Creates a new tour with the given visit as first visit.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="visit">The first visit.</param>
            /// <returns>The index of the new tour.</returns>
            public int NewTour(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int visit)
            {
                var content = problem.Capacity.Empty();
                content.Weight += problem.GetVisitCost(visit);
                problem.Capacity.Add(content, visit);
                solution.Contents.Add(content);
                solution.Add(visit, visit);
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
                { // use the seed cost; the cost to the seed visit.
                    increase -= costFunc(visit);
                }

                // get the visit cost.
                var visitCost = problem.GetVisitCost(visit);

                // calculate the new weight.
                var potentialWeight = solution.Contents[t].Weight + increase + visitCost;
                // cram as many visits into one route as possible.
                if (!problem.Capacity.UpdateAndCheckCosts(solution.Contents[t], potentialWeight, visit))
                { // best placement is not impossible.
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

                    // run CI algorithm.
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
                { // no best placement found.
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
                { // insertion not possible.
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
                { // if there's no function to limit the inter-optimizations then try them all.
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
                return new NoDepotCVRProblem()
                {
                    Capacity = problem.Capacity.Scale(1 - _slackPercentage),
                        Weights = problem.Weights,
                        VisitCosts = problem.VisitCosts
                };
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
            public bool ApplyPenalty(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int t1, int t2, Penalties penalty)
            {
                var tour1 = solution.Tour(t1);
                var tour2 = solution.Tour(t2);

                // choose best next edge to penalize.
                var worstCost = 0f;
                Pair? worstPair = null;
                Penalty? worstPenalty = null;
                foreach (var e in tour1.Pairs().Concat(tour2.Pairs()))
                {
                    Penalty p;
                    var cost = 0f;
                    if (!penalty.TryGetValue(e, out p))
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
                { // no pairs found.
                    return false;
                }
                if (penalty.Worst == 0)
                { // keep the worst cost to determine when we stop.
                    penalty.Worst = worstCost;
                }
                penalty[worstPair.Value] = worstPenalty.Value;
                penalty.Total += 1;

                // check if we've penalized enough.
                if (penalty.Total * _gvnsPenalty > penalty.Worst * 5)
                { // enough penalties.
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
            /// Selects a good candidate tour to be place in the target tour from the source tour.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="source">The source.</param>
            /// <param name="target">The target.</param>
            /// <returns>True if a good tour was found.</returns>
            public bool SelectTour(NoDepotCVRProblem problem, NoDepotCVRPSolution source, NoDepotCVRPSolution target)
            {   
                // if target is empty select random tour.
                if (target.Count == 0)
                {
                    var t = Itinero.Optimization.Algorithms.Random.RandomGeneratorExtensions.GetRandom()
                        .Generate(source.Count);
                    
                    // copy over tour and it's content.
                    var sourceTour = source.Tour(t);
                    var targetT = this.NewTour(problem, target, sourceTour.First);
                    var targetTour = target.Tour(targetT);
                    foreach (var pair in sourceTour.Pairs())
                    {
                        targetTour.ReplaceEdgeFrom(pair.From, pair.To);
                    }
                    target.Contents[targetT].Weight = source.Contents[t].Weight;
                    target.Contents[targetT].Quantities = source.Contents[t].Quantities.Clone() as float[];

                    return true;
                }
                else
                {
                    // build list of remaining visits.
                    var visits = new HashSet<int>(this.PotentialVisits(problem));
                    for (var t = 0; t < target.Count; t++)
                    {
                        var tour = target.Tour(t);
                        foreach (var v in tour)
                        {
                            visits.Remove(v);
                        }
                    }

                    if (visits.Count == 0)
                    {
                        return false;
                    }
                
                    // search for a tour that has the least overlapping visits.
                    // TODO: this may give smaller (and worse) tours an advantage. Perhaps change this to a metric with the most visits inserted or something similar.
                    var bestT = -1;
                    var bestOverlap = int.MaxValue;
                    for (var t = 0; t < source.Count; t++)
                    {
                        var tour = source.Tour(t);
                        var tourOverlap = 0;
                        var hasUnplaced = false;
                        foreach (var v in tour)
                        {
                            if (!visits.Contains(v))
                            {
                                tourOverlap++;
                            }
                            else
                            {
                                hasUnplaced = true;
                            }
                        }

                        if (hasUnplaced &&
                            tourOverlap < bestOverlap)
                        {
                            bestOverlap = tourOverlap;
                            bestT = t;
                        }
                    }

                    if (bestT < 0)
                    {
                        return false;
                    }
                
                    // copy over tour.
                    var bestTour = source.Tour(bestT);
                    ITour targetTour = null;
                    CapacityExtensions.Content targetContent = null;
                    var previous = bestTour.First;
                    foreach (var pair in bestTour.Pairs())
                    {
                        if (targetTour == null)
                        {
                            if (!visits.Contains(pair.From))
                            {
                                continue;
                            }
                            var targetT = this.NewTour(problem, target, pair.From);
                            targetTour = target.Tour(targetT);
                            targetContent = target.Contents[targetT];
                        }

                        if (!visits.Contains(pair.To))
                        { // TODO: do we need to close the tour here or not?
                            continue;
                        }
                        targetTour.ReplaceEdgeFrom(previous, pair.To);
                        problem.Capacity.Add(targetContent, pair.To);
                        
                        previous = pair.To;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Place all the remaining unplaced visits in the given solution.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The solution.</param>
            public void PlaceRemaining(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
            {
                // build list of remaining visits.
                var visits = this.PotentialVisits(problem);
                for (var t = 0; t < solution.Count; t++)
                {
                    var tour = solution.Tour(t);
                    foreach (var v in tour)
                    {
                        visits.Remove(v);
                    }
                }
                
                // keep placing until empty.
                while (visits.Count > 0)
                {
                    if (this.TryPlaceAny(problem, solution, visits))
                    {
                        continue;
                    }
                    
                    // oeps, this is very bad a new tour here.
                    this.SeedNext(problem, solution, visits);
                }
            }
        }
}