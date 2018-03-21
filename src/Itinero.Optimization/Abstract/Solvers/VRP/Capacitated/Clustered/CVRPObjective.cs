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
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Clustered.Solvers;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Clustered
{
    /// <summary>
    /// An objective of a CVRP.
    /// </summary>
    public class CVRPObjective : ObjectiveBase<CVRProblem, CVRPSolution, float>,
        IRelocateObjective<CVRProblem, CVRPSolution>, IExchangeObjective<CVRProblem, CVRPSolution>, IMultiExchangeObjective<CVRProblem>,
        IMultiRelocateObjective<CVRProblem>, ISeededCheapestInsertionObjective<CVRProblem, CVRPSolution>
        {
            private readonly Func<CVRProblem, IList<int>, int> _seedFunc;
            private readonly Func<CVRProblem, int, int, float> _localizationCostFunc;

            private readonly Delegates.OverlapsFunc<CVRProblem, ITour> _overlapsFunc;

            /// <summary>
            /// Creates a new objective.
            /// </summary>
            /// <param name="seedFunc">The seed function.</param>
            /// <param name="overlapsFunc">The overlaps function.</param>
            /// <param name="localizationFactor">The localization factor.</param>
            public CVRPObjective(Func<CVRProblem, IList<int>, int> seedFunc, Delegates.OverlapsFunc<CVRProblem, ITour> overlapsFunc, float localizationFactor = 0.5f)
            {
                _seedFunc = seedFunc;
                _overlapsFunc = overlapsFunc;

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
            public override float Add(CVRProblem problem, float fitness1, float fitness2)
            {
                return fitness1 + fitness2;
            }

            /// <summary>
            /// Calculates the weight of the given tour.
            /// </summary>
            public float Calculate(CVRProblem problem, CVRPSolution solution, int tourIdx)
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
            /// Calculate the cumulative weights.
            /// </summary>
            public float[] CalculateCumul(CVRProblem problem, CVRPSolution solution, int tourIdx)
            {
                var tour = solution.Tour(tourIdx);

                // intialize the result array.
                var cumul = new float[tour.Count + 1];

                int previous = -1; // the previous visit.
                float time = 0; // the current weight.
                int idx = 0; // the current index.
                foreach (int visit1 in tour)
                { // loop over all visits.
                    if (previous >= 0)
                    { // there is a previous visit.
                        // add one visit and the distance to the previous visit.
                        time = time + problem.Weights[previous][visit1];
                        cumul[idx] = time;
                    }
                    else
                    { // there is no previous visit, this is the first one.
                        cumul[idx] = 0;
                    }

                    time += problem.GetVisitCost(visit1);

                    idx++; // increase the index.
                    previous = visit1; // prepare for next loop.
                }
                // handle the edge last->first.
                time = time + problem.Weights[previous][tour.First];
                cumul[idx] = time;
                return cumul;
            }

            /// <summary>
            /// Updates all content properly.
            /// </summary>
            public void UpdateContent(CVRProblem problem, CVRPSolution solution)
            {
                for (var t = 0; t < solution.Count; t++)
                {
                    if (t >= solution.Contents.Count)
                    {
                        solution.Contents.Add(new Solvers.CapacityExtensions.Content());
                    }
                    solution.Contents[t].Weight = this.Calculate(problem, solution, t);
                    problem.Capacity.UpdateCosts(solution.Contents[t], solution.Tour(t));
                }
            }

            /// <summary>
            /// Calulates the total weight.
            /// </summary>
            public override float Calculate(CVRProblem problem, CVRPSolution solution)
            {
                var weight = 0f;

                for (var t = 0; t < solution.Count; t++)
                {
                    weight += this.Calculate(problem, solution, t);
                }

                return weight;
            }

            /// <summary>
            /// Compares the two given fitness values.
            /// </summary>
            /// <param name="problem"></param>
            /// <param name="fitness1"></param>
            /// <param name="fitness2"></param>
            /// <returns></returns>
            public override int CompareTo(CVRProblem problem, float fitness1, float fitness2)
            {
                return fitness1.CompareTo(fitness2);
            }

            /// <summary>
            /// Returns true if the given fitness value is equivalent to zero.
            /// </summary>
            public override bool IsZero(CVRProblem problem, float fitness)
            {
                return fitness == 0;
            }

            /// <summary>
            /// Subtracts the two given fitness values.
            /// </summary>
            public override float Subtract(CVRProblem problem, float fitness1, float fitness2)
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
            public bool TryMove(CVRProblem problem, CVRPSolution solution, int t1, int t2, Triple visit, out float delta)
            {
                var E = 0.1f;
                if (!problem.Capacity.CanAdd(solution.Contents[t2], visit.Along))
                { // capacity doesn't allow placing this visit.
                    delta = 0;
                    return false;
                }

                var tour1 = solution.Tour(t1);
                if (tour1.First == visit.Along)
                { // for now we cannot move the first visit.
                    // TODO: enable this objective to move the first visit.
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
                        tour2.ReplaceEdgeFrom(location.From, visit.Along);
                        tour2.ReplaceEdgeFrom(visit.Along, location.To);

                        // in this model of the solution we need to remove the visit.
                        tour1.Remove(visit.Along);

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
            public bool TrySwap(CVRProblem problem, CVRPSolution solution, int t1, int t2, Triple visit1, Triple visit2,
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
            /// <returns>An enumerable with sequences.</returns>
            public IEnumerable<Operators.Seq> SeqAndSmaller(CVRProblem problem, IEnumerable<int> tour, int minSize, int maxSize)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Reverses the given sequence.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="sequence">The sequence.</param>
            /// <returns>The reversed sequence.</returns>        
            public Operators.Seq Reverse(CVRProblem problem, Operators.Seq sequence)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Tries to swap the given sequences between the two given tours.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <param name="s1">The sequence from tour1.</param>
            /// <param name="s2">The sequence from tour2.</param>
            /// <param name="delta">The difference in visit.</param>
            /// <returns>True if the swap succeeded.</returns>   
            public bool TrySwap(CVRProblem problem, int t1, int t2, Operators.Seq s1, Operators.Seq s2, out float delta)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Tries to move the given sequence from t1 in between the given pair in t2.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <param name="seq">The sequence.</param>
            /// <param name="pair">The pair.</param>
            /// <param name="delta">The difference in visit.</param>
            /// <returns></returns>        
            public bool TryMove(CVRProblem problem, int t1, int t2, Operators.Seq seq, Pair pair, out float delta)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Creates a new and empty solution.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <returns>A new empty solution.</returns>
            public CVRPSolution NewSolution(CVRProblem problem)
            {
                return new CVRPSolution(problem.Weights.Length);
            }

            /// <summary>
            /// Gets a list of potential visits.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <returns>The list of visits to be visited, except potentially those uniquely used as seeds.</returns>
            public IList<int> PotentialVisits(CVRProblem problem)
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
            public int SeedNext(CVRProblem problem, CVRPSolution solution, IList<int> visits)
            {
                var seed = _seedFunc(problem, visits);
                visits.Remove(seed);
                var content = problem.Capacity.Empty();
                content.Weight += problem.GetVisitCost(seed);
                problem.Capacity.Add(content, seed);
                solution.Contents.Add(content);
                solution.Add(seed, seed);
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
            public bool TryPlaceAny(CVRProblem problem, CVRPSolution solution, int t, IList<int> visits)
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
            /// /// Tries to place any of the visits in the given visit list.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The tour.</param>
            /// <param name="visits">The visits to try.</param>
            /// <returns></returns>
            public bool TryPlaceAny(CVRProblem problem, CVRPSolution solution, IList<int> visits)
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
                costFunc = null;
                if (_localizationCostFunc != null)
                {
                    costFunc = (v) => _localizationCostFunc(problem, bestTour.First, v);
                    actualIncrease -= costFunc(bestPlacement.Value.Along);
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
            public ITSProblem BuildSubTourTSP(CVRProblem problem, ITour tour)
            {
                return new TSP.TSPSubProblem(tour, problem.Weights);
            }

            /// <summary>
            /// Returns true if the two tours could benifit from inter-improvement optimizations.
            /// </summary>
            /// <param name="problem">The problem.</param>
            /// <param name="solution">The solution.</param>
            /// <param name="t1">The first tour.</param>
            /// <param name="t2">The second tour.</param>
            /// <returns></returns>
            public bool HaveToTryInter(CVRProblem problem, CVRPSolution solution, int t1, int t2)
            {
                if (_overlapsFunc == null)
                { // if there's no function to limit the inter-optimizations then try them all.
                    return true;
                }
                return _overlapsFunc(problem, solution.Tour(t1), solution.Tour(t2));
            }
        }
}