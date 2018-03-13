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
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// An objective of a no-depot CVRP.
    /// </summary>
    public class NoDepotCVRPObjective : ObjectiveBase<NoDepotCVRProblem, NoDepotCVRPSolution, float>
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
        /// Calculate the cumulative weights.
        /// </summary>
        public float[] CalculateCumul(NoDepotCVRProblem problem, NoDepotCVRPSolution solution, int tourIdx)
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
        public void UpdateContent(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
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
        public override float Calculate(NoDepotCVRProblem problem, NoDepotCVRPSolution solution)
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
    }
}
