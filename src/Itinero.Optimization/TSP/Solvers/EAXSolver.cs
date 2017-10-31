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
using Itinero.Optimization.Algorithms.Solvers.GA;
using Itinero.Optimization.Logging;
using Itinero.Optimization.Tours;
using Itinero.Optimization.TSP.Solvers.Operators;

namespace Itinero.Optimization.TSP.Solvers
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    public class EAXSolver : GASolver<float, ITSProblem, TSPObjective, Tour, float>
    {
        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(GASettings settings)
            : base(new TSPObjective(), new HillClimbing3OptSolver(), new EAXOperator(30, EAXOperator.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom, true),
            new TournamentSelectionOperator<ITSProblem, Tour, TSPObjective, float>(10, 0.5), new EmptyOperator<float, ITSProblem, TSPObjective, Tour, float>(), settings)
        {

        }

        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(GASettings settings, IOperator<float, ITSProblem, TSPObjective, Tour, float> mutation)
            : base(new TSPObjective(), new HillClimbing3OptSolver(), new EAXOperator(30, EAXOperator.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom, true),
            new TournamentSelectionOperator<ITSProblem, Tour, TSPObjective, float>(10, 0.5), mutation, settings)
        {

        }

        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(GASettings settings, ISolver<float, ITSProblem, TSPObjective, Tour, float> generator, 
            IOperator<float, ITSProblem, TSPObjective, Tour, float> mutation)
            : base(new TSPObjective(), generator, new EAXOperator(30, EAXOperator.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom, true),
            new TournamentSelectionOperator<ITSProblem, Tour, TSPObjective, float>(10, 0.5), mutation, settings)
        {

        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override Tour Solve(ITSProblem problem, TSPObjective objective, out float fitness)
        {
            if (problem.Count < 5)
            { // use brute-force solver when problem is very small.
                var bruteForceSolver = new BruteForceSolver();
                return bruteForceSolver.Solve(problem, objective, out fitness);
            }

            if (problem.Last == null)
            { // the problem is 'open', we need to convert the problem and then solution.
                Logging.Logger.Log("EAXSolver.Solve", Logging.TraceEventType.Warning,
                    string.Format("EAX cannot solver 'open' TSP's: converting problem to a closed equivalent."));

                var convertedProblem = problem.ToClosed();
                var solution = base.Solve(convertedProblem, objective, out fitness);
                var route = new Tour(solution, null);
                fitness = 0;
                foreach (var pair in route.Pairs())
                {
                    fitness = fitness + problem.Weight(pair.From, pair.To);
                }
                return route;
            }
            else if (problem.First != problem.Last)
            { // the problem is 'closed' but has a fixed end point.
                Logger.Log("EAXSolver.Solve", Logging.TraceEventType.Warning,
                    string.Format("EAX cannot solver 'closed' TSP's with a fixed endpoint: converting problem to a closed equivalent."));

                var convertedProblem = problem.ToClosed();
                var convertedRoute = base.Solve(convertedProblem, objective, out fitness);
                convertedRoute.InsertAfter(System.Linq.Enumerable.Last(convertedRoute), problem.Last.Value);
                var route = new Tour(convertedRoute, problem.Last.Value);
                fitness = 0;
                foreach (var pair in route.Pairs())
                {
                    fitness = fitness + problem.Weight(pair.From, pair.To);
                }
                return route;
            }
            return base.Solve(problem, objective, out fitness);
        }
    }
}