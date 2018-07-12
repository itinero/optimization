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
using System.Threading;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Shared.Seeds;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.CVRP_ND.SCI
{
    /// <summary>
    /// A strategy using a seeded cheapest insertion construction heuristic.
    /// </summary>
    internal class SeededCheapestInsertionStrategy : Strategy<CVRPNDProblem, CVRPNDCandidate>
    {
        private readonly SeededCheapestInsertionPlacementOperator _seededCheapestInsertionPlacementOperator;

        /// <summary>
        /// Creates a new strategy.
        /// </summary>
        /// <param name="seededCheapestInsertionPlacementOperator">The operator implementing the strategy.</param>
        public SeededCheapestInsertionStrategy(
            SeededCheapestInsertionPlacementOperator seededCheapestInsertionPlacementOperator = null)
        {
            _seededCheapestInsertionPlacementOperator = seededCheapestInsertionPlacementOperator ?? new SeededCheapestInsertionPlacementOperator();
        }
     
        /// <inheritdoc />
        public override string Name { get; } = "SCI";

        /// <inheritdoc />
        public override CVRPNDCandidate Search(CVRPNDProblem problem)
        {
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Problem = problem,
                Fitness = 0
            };
            
            // place visits until no more are left.
            var visits = candidate.GetUnplacedVisits();

            // apply the operator.
            _seededCheapestInsertionPlacementOperator.Apply(candidate, visits);
            
            return candidate;
        }
        
        private static readonly ThreadLocal<SeededCheapestInsertionStrategy> DefaultLazy = new ThreadLocal<SeededCheapestInsertionStrategy>(() => new SeededCheapestInsertionStrategy());
        public static SeededCheapestInsertionStrategy Default => DefaultLazy.Value;
    }
}