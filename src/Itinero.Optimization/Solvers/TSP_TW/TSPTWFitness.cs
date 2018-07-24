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
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.TSP_TW
{
    internal static class TSPTWFitness
    {
        /// <summary>
        /// Calculates a fitness value for the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="problem">The problem.</param>
        /// <param name="waitingTimePenaltyFactor">A penalty applied to time window violations as a factor. A very high number compared to travel times.</param>
        /// <param name="timeWindowViolationPenaltyFactor">A penalty applied to waiting times as a factor.</param>
        /// <returns>A fitness value that reflects violations of timewindows by huge penalties.</returns>
        public static float Fitness(this Tour tour, TSPTWProblem problem, float waitingTimePenaltyFactor = 1,
            float timeWindowViolationPenaltyFactor = 1000000)
        {
            var violations = 0.0f;
            var waitingtime = 0.0f; // waits are not really a violation but a waist.
            var time = 0.0f;
            var previous = Tour.NOT_SET;
            using (var enumerator = tour.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (previous != Tour.NOT_SET)
                    {
                        // keep track of time.
                        time += problem.Weight(previous, current);
                    }

                    var window = problem.Windows[current];
                    if (!window.IsEmpty)
                    {
                        if (window.Max < time)
                        {
                            // ok, unfeasible.
                            violations += time - window.Max;
                        }

                        if (window.Min > time)
                        {
                            // wait here!
                            waitingtime += (window.Min - time);
                            time = window.Min;
                        }
                    }

                    previous = current;
                }
            }

            if (tour.First == tour.Last &&
                previous != Tour.NOT_SET)
            {
                time += problem.Weight(previous, tour.First);
            }

            if (violations > float.Epsilon &&
                violations < 1)
            { // make the violations at least 1 if there are any, to make sure the penalty
              // stays unacceptable even for tiny violations.
                violations = 1;
            }

            return (violations * timeWindowViolationPenaltyFactor) + time + (waitingtime * waitingTimePenaltyFactor);
        }
    }
}