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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Tours.Operations;

namespace Itinero.Optimization.TSP.Solvers.Operators
{
    /// <summary>
    /// A local 1-Shift search for the TSP.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere else, also called reinsertion heuristic.</remarks>
    public class Local1Shift : IOperator<float, ITSProblem, TSPObjective, ITour, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "LCL_1SHFT"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TSPObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSProblem problem, TSPObjective objective, ITour tour, out float delta)
        {
            var originalRoute = tour;
            var originalProblem = problem;
            var originalFitness = 0.0f;
            if (!problem.Last.HasValue)
            { // the problem is 'open', convert to a closed equivalent.
                originalFitness = objective.Calculate(problem, tour);
                problem = problem.ToClosed();
                tour = new Tour(tour, problem.Last);
            }

            delta = 0;
            var success = false;

            var bestDelta = 0.0f;
            do
            {
                bestDelta = 0.0f;

                // search the entire route for a customer that can be moved to improve it.
                var bestTriple = new Triple();
                var bestPair = new Pair();
                foreach (var triple in tour.Triples())
                { // the middle customer of each triple is a candidate for re-insertion.
                    foreach (var pair in tour.Pairs())
                    { // each pair is a candidate to recieve the candidate customers.
                        if (pair.From != triple.Along &&
                            pair.To != triple.Along)
                        { // this candidate may fit here.
                            var localDelta = ShiftAfter.If(problem, tour, triple.Along, pair.From, triple.From, triple.To, pair.To);
                            if (localDelta < bestDelta)
                            { // this means a (better) improvement.
                                bestDelta = localDelta;
                                bestTriple = triple;
                                bestPair = pair;
                            }
                        }
                    }
                }

                if (bestDelta < 0)
                { // if an improvement was found, then apply it.
                    // make the changes.
                    tour.ShiftAfter(bestTriple.Along, bestPair.From);
                    // store the delta.
                    delta = delta + bestDelta;
                    success = true;
                }
            } while (bestDelta < 0);

            if (!originalProblem.Last.HasValue)
            { // the original problem was open, convert the route again.
                originalRoute.Clear();
                foreach (var pair in tour.Pairs())
                {
                    if (pair.To != problem.First)
                    {
                        originalRoute.InsertAfter(pair.From, pair.To);
                    }
                }

                var newFitness = objective.Calculate(problem, originalRoute);
                delta = originalFitness - newFitness;
            }
            return success;
        }
    }
}