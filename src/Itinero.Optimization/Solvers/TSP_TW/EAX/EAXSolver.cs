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
using Itinero.Logging;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Solvers.TSP_TW.EAX
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    internal class EAXSolver : Strategy<TSPTWProblem, Candidate<TSPTWProblem, Tour>>
    {
        private readonly GAStrategy<TSPTWProblem, Candidate<TSPTWProblem, Tour>> _gaStrategy;
        
        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(Strategy<TSPTWProblem, Candidate<TSPTWProblem, Tour>> generator = null, 
            Operator<Candidate<TSPTWProblem, Tour>> mutation = null, GASettings settings = null, ISelector<Candidate<TSPTWProblem, Tour>> selector = null)
        {
            if (generator == null) generator = RandomSolver.Default;
            if (mutation == null) mutation = EmptyOperator<Candidate<TSPTWProblem, Tour>>.Default;
            if (settings == null) settings = GASettings.Default;
            var crossOver = EAXOperator.Default;
            
            _gaStrategy = new GAStrategy<TSPTWProblem, Candidate<TSPTWProblem, Tour>>(generator, crossOver, mutation, settings, selector);
            
            this.Name = $"EAX_{_gaStrategy.Name}";
        }

        public override string Name { get; }

        public override Candidate<TSPTWProblem, Tour> Search(TSPTWProblem problem)
        {
            if (problem.Count < 5)
            { // use brute-force solver when problem is very small.
                return BruteForceSolver.Default.Search(problem);
            }
            if (problem.Last == null)
            { // the problem is 'open', we need to convert the problem and then solution.
                Logger.Log($"{typeof(EAXSolver)}.{nameof(Search)}", TraceEventType.Warning,
                    "Performance warning: EAX solver cannot be applied to 'open' TSP's, converting problem to a closed equivalent.");

                var convertedProblem = problem.ClosedEquivalent;
                var convertedCandidate = _gaStrategy.Search(convertedProblem);
                
                // TODO: instead of recalculating the fitness value *should* be identical, confirm this with more testing.
                var tour = new Tour(convertedCandidate.Solution, null);

                return new Candidate<TSPTWProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Weight(problem.Weight)
                };
            }
            else if (problem.First != problem.Last)
            { // the problem is 'closed' but has a fixed end point.
                Logger.Log($"{typeof(EAXSolver)}.{nameof(Search)}", TraceEventType.Warning,
                    "Performance warning: EAX solver cannot be applied to'closed' TSP's with a fixed endpoint: converting problem to a closed equivalent.");

                var convertedProblem = problem.ClosedEquivalent;
                var convertedCandidate = _gaStrategy.Search(convertedProblem);
                
                // TODO: instead of recalculating the fitness adjust it to match the difference by adding last->problem.last.
                var tour = convertedCandidate.Solution;
                tour.InsertAfter(System.Linq.Enumerable.Last(tour), problem.Last.Value);
                tour = new Tour(tour, problem.Last.Value);
                return new Candidate<TSPTWProblem, Tour>()
                {
                    Problem = problem,
                    Solution = tour,
                    Fitness = tour.Weight(problem.Weight)
                };
            }
            else
            {
                return _gaStrategy.Search(problem);
            }
        }
        
        private static readonly ThreadLocal<EAXSolver> DefaultLazy = new ThreadLocal<EAXSolver>(() => new EAXSolver());
        public static EAXSolver Default => DefaultLazy.Value;
    }
}