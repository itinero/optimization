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

using System.Threading;
using Itinero.Optimization.Solvers.Shared.Random1Shift;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.VNS;

namespace Itinero.Optimization.Solvers.TSP_TW.Operators
{
    /// <summary>
    /// An operator that runs the random 1 shift operation, remove a random visit and place it after another random visit.
    /// </summary>
    internal class Random1ShiftPerturber : Perturber<Candidate<TSPTWProblem, Tour>>
    {
        public override string Name => "RAN_1SHIFT";
        
        public override bool Apply(Candidate<TSPTWProblem, Tour> candidate, int level)
        {
            while (level >= 0)
            {
                candidate.Solution.DoRandom1Shift();
                level--;
            }

            var fitnessBefore = candidate.Fitness;
            candidate.Fitness = candidate.Solution.Fitness(candidate.Problem);

            return candidate.Fitness < fitnessBefore;
        }
        
        private static readonly ThreadLocal<Random1ShiftPerturber> DefaultLazy = new ThreadLocal<Random1ShiftPerturber>(() => new Random1ShiftPerturber());
        public static Random1ShiftPerturber Default => DefaultLazy.Value;
    }
}