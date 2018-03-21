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
using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Clustered.Solvers;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Clustered
{
    /// <summary>
    /// The capacitated VRP.
    /// </summary>
    public class CVRProblem : IRelocateProblem, IExchangeProblem
    {
        /// <summary>
        /// Gets or sets the vehicle capacity.
        /// </summary>
        public Capacity Capacity { get; set; }

        /// <summary>
        /// Gets or sets the depot.
        /// </summary>
        public int Depot { get; set; }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights { get; set; }

        /// <summary>
        /// Gets or sets the visit costs.
        /// </summary>
        /// <returns></returns>
        public float[] VisitCosts { get; set; }

        /// <summary>
        /// Gets the cost for the given visit (if any).
        /// </summary>
        /// <param name="visit">The visit.</param>
        /// <returns></returns>
        public float GetVisitCost(int visit)
        {
            if (this.VisitCosts == null)
            {
                return 0;
            }
            return this.VisitCosts[visit];
        }

        /// <summary>
        /// Gets the seed heuristic.
        /// </summary>
        /// <returns></returns>
        public Func<CVRProblem, IList<int>, int> SelectSeedWithCloseNeighboursHeuristic
        {
            get
            {
                return (problem, visits) =>
                {
                    var weights = problem.Weights;

                    return Algorithms.Seeds.SeedHeuristics.GetSeedWithCloseNeighbours(
                        weights, visits);
                };
            }
        }

        /// <summary>
        /// Gets the seed heuristic.
        /// </summary>
        /// <returns></returns>
        public Func<CVRProblem, IList<int>, int> SelectRandomSeedHeuristic
        {
            get
            {
                return (problem, visits) =>
                {
                    return Algorithms.Seeds.SeedHeuristics.GetSeedRandom(visits);
                };
            }
        }

        /// <summary>
        /// Solves this using a default solver.
        /// </summary>
        /// <returns></returns>
        public CVRPSolution Solve()
        {
            return this.Solve((p, x, y) => true);
        }

        /// <summary>
        /// Solves this using a default solver.
        /// </summary>
        /// <returns></returns>
        public CVRPSolution Solve(Delegates.OverlapsFunc<CVRProblem, ITour> overlapsFunc)
        {
            var crossMultiAllPairs = new MultiExchangeOperator<CVRPObjective, CVRProblem, CVRPSolution>(2, 10, true, true);
            var crossMultiAllPairsUntil = new Algorithms.Solvers.IterativeOperator<float, CVRProblem, CVRPObjective, CVRPSolution, float>(
                    crossMultiAllPairs, 1, true);

            //var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
            //    new SeededLocalizedCheapestInsertionSolver(this.SelectSeedHeuristic, overlapsFunc), 1,
            //        );
            //var solver = new GuidedVNS(constructionHeuristic, overlapsFunc, 4 * 60);
            //var iterate = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
            //    solver, 10);
            
            var slci = new SeededCheapestInsertion<CVRProblem, CVRPObjective, CVRPSolution>(
                new TSP.Solvers.HillClimbing3OptSolver(),
                new IInterTourImprovementOperator<float, CVRProblem, CVRPObjective, CVRPSolution, float>[]
                {
                    //new MultiExchangeOperator<CVRPObjective, CVRProblem, CVRPSolution>(2, 10, false),
                    new ExchangeOperator<CVRPObjective, CVRProblem, CVRPSolution>(),
                    new MultiRelocateOperator<CVRPObjective, CVRProblem, CVRPSolution>(2, 5)
                    //new RelocateOperator<CVRPObjective, CVRProblem, CVRPSolution>(true),
                }
            );

            //var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, CVRProblem, CVRPObjective, CVRPSolution, float>(
            //         slci, 1);
            // var iterate = new Algorithms.Solvers.IterativeSolver<float, CVRProblem, CVRPObjective, CVRPSolution, float>(
            //         constructionHeuristic, 1, crossMultiAllPairsUntil);
            // var solver = new GuidedVNS(iterate, overlapsFunc, 8 * 60);

            return this.Solve(slci, new CVRPObjective(this.SelectRandomSeedHeuristic, overlapsFunc));
        }

        /// <summary>
        /// Solvers this using the given solver.
        /// </summary>
        public CVRPSolution Solve(Algorithms.Solvers.ISolver<float, CVRProblem, CVRPObjective, CVRPSolution, float> solver,
            CVRPObjective objective)
        {
            return solver.Solve(this, objective);
        }
    }
}