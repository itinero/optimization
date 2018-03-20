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
using Itinero.Optimization.Algorithms.Solvers.Objective;
using Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Clustered.Solvers;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using System.Collections.Generic;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Clustered
{
    /// <summary>
    /// An objective of a CVRP.
    /// </summary>
    public class CVRPObjective : ObjectiveBase<CVRProblem, CVRPSolution, float>, 
        IRelocateObjective<CVRProblem, CVRPSolution>, IExchangeObjective<CVRProblem>, IMultiExchangeObjective<CVRProblem>,
        IMultiRelocateObjective<CVRProblem>
    {
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

            // calculate the removal gain of the customer.
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
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="visit1">The visit from tour1.</param>
        /// <param name="visit2">The visit from tour2.</param>
        /// <param name="delta">The difference in visit.</param>
        /// <returns></returns>        
        public bool TrySwap(CVRProblem problem, int t1, int t2, Triple visit1, Triple visit2, out float delta)
        {
            throw new NotImplementedException();
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
    }
}