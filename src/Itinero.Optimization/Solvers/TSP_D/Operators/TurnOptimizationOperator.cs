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
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.TSP_D.Operators
{
    /// <summary>
    /// An operator that optimizes turns.
    /// </summary>
    internal class TurnOptimizationOperator: Operator<Candidate<TSPDProblem, Tour>>
    {
        public override string Name { get; } = "TURN";

        public override bool Apply(Candidate<TSPDProblem, Tour> candidate)
        {
            var fitness = candidate.Solution.OptimizeTurns(candidate.Problem.Weight, candidate.Problem.TurnPenalty);
            if (fitness >= float.MaxValue)
            {
                return false;
            }
            var success = fitness < candidate.Fitness;
            candidate.Fitness = fitness;
            return success;
        }
        
        private static readonly ThreadLocal<TurnOptimizationOperator> DefaultLazy = new ThreadLocal<TurnOptimizationOperator>(() => new TurnOptimizationOperator());
        public static TurnOptimizationOperator Default => DefaultLazy.Value;
    }
}