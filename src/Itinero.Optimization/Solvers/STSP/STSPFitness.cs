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

using System.Linq;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.STSP
{
    internal static class STSPFitness
    {
        /// <summary>
        /// Calculates a fitness value for the given STSP tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="problem">The problem.</param>
        /// <returns>A fitness value that reflects how good the STSP solution is.</returns>
        public static float Fitness(this Tour tour, STSProblem problem)
        {
            // description of this fitness value:
            // - more placed visit is better.
            // - once placed visits are the same:
            //    - minimize travel weight.
            
            // we use (#possiblevisits * max - #placedvisit) + travelweight.
            
            var max = problem.Max;
            var possibleVisitCount = problem.Count;
            var maxPossible = max * possibleVisitCount;

            var travelWeight = tour.Weight(problem.Weight, out var visitCount);
            if (travelWeight > max)
            { // this is very very very bad.
                return float.MaxValue;
            }
            return (possibleVisitCount * max) - (visitCount * max) + travelWeight;
        }
    }
}