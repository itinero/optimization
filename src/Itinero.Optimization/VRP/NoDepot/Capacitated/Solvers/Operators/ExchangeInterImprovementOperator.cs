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
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers.Operators
{
    /// <summary>
    /// An exchange inter improvement operator.
    /// </summary>
    /// <remarks>
    /// This follows a 'stop on first'-improvement strategy and this operator will only modify the solution when it improves things. 
    /// 
    /// The algorithm works as follows:
    /// 
    /// - Select 2 random tours.
    /// - Loop over all visits in tour1.
    ///   - Loop over all visits in tour2.
    ///     - Check if a swap between tours improves things.
    /// 
    /// The search stops from the moment any improvement is found.
    /// </remarks>
    public class ExchangeInterImprovementOperator : IInterTourImprovementOperator
    {
        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name => "EX";

        /// <summary>
        /// Returns true if it doesn't matter if tour indexes are switched.
        /// </summary>
        public bool IsSymmetric => true;

        /// <summary>
        /// Returns true if this operator supports the given objective.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports(NoDepotCVRPObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply (NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, out float delta) 
        {
            // check if solution has at least two tours.
            if (solution.Count < 2)
            {
                delta = 0;
                return false;
            }

            // choose two random routes.
            var random = RandomGeneratorExtensions.GetRandom ();
            var tourIdx1 = random.Generate (solution.Count);
            var tourIdx2 = random.Generate (solution.Count - 1);
            if (tourIdx2 >= tourIdx1) {
                tourIdx2++;
            }

            return Apply(problem, objective, solution, tourIdx1, tourIdx2, out delta);
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, 
            int tourIdx1, int tourIdx2, out float delta)
        {            
            var tour1 = solution.Tour(tourIdx1);
            var tour2 = solution.Tour(tourIdx2);

            var tour1Weight = objective.Calculate(problem, solution, tourIdx1);
            var tour2Weight = objective.Calculate(problem, solution, tourIdx2);
            var totalBefore =  tour1Weight + tour2Weight;

            // this heuristic removes a visit1 from tour1 and a visit2 from tour2 and inserts the visits again
            // but swaps them; visit1 in tour2 and visit2 in tour1.
            int previousVisit1 = -1;
            foreach (int visit1 in tour1)
            { // loop over all customers in route1.
                if (previousVisit1 >= 0)
                { // the previous customer is set.
                    int nextVisit1 = tour1.GetNeigbour(visit1);
                    int previousVisit2 = -1;

                    foreach (int visit2 in tour2)
                    { // loop over all customers in route2. 
                        int nextVisit2 = tour2.GetNeigbour(visit2);
                        if (previousVisit2 >= 0)
                        { // the previous customer is set.
                            var weight1 = problem.Weights[previousVisit1][visit1] +
                                problem.Weights[visit1][nextVisit1];
                            var weight2 = problem.Weights[previousVisit2][visit2] +
                                problem.Weights[visit2][nextVisit2];

                            var weight1After = (float)problem.Weights[previousVisit1][visit2] +
                                (float)problem.Weights[visit2][nextVisit1];
                            var weight2After = (float)problem.Weights[previousVisit2][visit1] +
                                (float)problem.Weights[visit1][nextVisit2];
                            var difference = (weight1After + weight2After) - (weight1 + weight2);

                            if (difference < -0.01)
                            { // the old weights are bigger!
                                // check if the new routes are bigger than max.
                                if (tour1Weight + (weight1After - weight1) <= problem.Capacity.Max &&
                                    tour2Weight + (weight2After - weight2) <= problem.Capacity.Max)
                                { // the exchange can happen, both routes stay within bound!

                                    // check constraints if any.
                                    if (!problem.Capacity.ExchangeIsPossible(solution.Contents[tourIdx1], 
                                            visit1, visit2))
                                    {
                                        continue;
                                    }
                                    if (!problem.Capacity.ExchangeIsPossible(solution.Contents[tourIdx2],
                                            visit2, visit1))
                                    {
                                        continue;
                                    }

                                    // exchange customer.
                                    tour1.ReplaceEdgeFrom(previousVisit1, visit2);
                                    tour1.ReplaceEdgeFrom(visit2, nextVisit1);
                                    tour2.ReplaceEdgeFrom(previousVisit2, visit1);
                                    tour2.ReplaceEdgeFrom(visit1, nextVisit2);

                                    // update content.
                                    problem.Capacity.UpdateExchange(solution.Contents[tourIdx1], visit1, visit2);
                                    problem.Capacity.UpdateExchange(solution.Contents[tourIdx2], visit2, visit1);

                                    // automatically removed in release mode.
                                    tour1.Verify(problem.Weights.Length);
                                    tour2.Verify(problem.Weights.Length);

                                    delta = -difference;
                                    return true;
                                }
                            }
                        }

                        previousVisit2 = visit2; // set the previous customer.
                    }
                }
                previousVisit1 = visit1; // set the previous customer.
            }
            delta = 0;
            return false;
        }
    }
}