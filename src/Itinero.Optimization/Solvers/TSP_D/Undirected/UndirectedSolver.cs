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
using Itinero.Optimization.Solvers.TSP;
using Itinero.Optimization.Solvers.TSP.EAX;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.TSP_D.Undirected
{
    /// <summary>
    /// A solver that uses an undirected TSP solver to solve the given directed TSP.
    /// </summary>
    public sealed class UndirectedSolver : Strategy<TSPDProblem, Candidate<TSPDProblem, Tour>>
    {
        private readonly Strategy<TSProblem, Candidate<TSProblem, Tour>> _undirectedStrategy = EAXSolver.Default;
        
        public override string Name => "UNDIR";

        public override Candidate<TSPDProblem, Tour> Search(TSPDProblem problem)
        {
            // call the strategy for the undirected scenario.
            var undirectedCandidate = _undirectedStrategy.Search(problem.UndirectedEquivalent);
            
            // convert to directed, while optimizing the turns.
            var directedResult = undirectedCandidate.Solution.ConvertToDirectedAndOptimizeTurns(problem.Weight, problem.TurnPenalty);

            return new Candidate<TSPDProblem, Tour>()
            {
                Fitness = directedResult.weight,
                Problem = problem,
                Solution = directedResult.tour
            };
        }
        
        private static readonly ThreadLocal<UndirectedSolver> DefaultLazy = new ThreadLocal<UndirectedSolver>(() => new UndirectedSolver());
        public static UndirectedSolver Default => DefaultLazy.Value;
    }
}