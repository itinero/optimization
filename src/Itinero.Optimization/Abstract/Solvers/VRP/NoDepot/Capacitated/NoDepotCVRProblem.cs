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
using Itinero.Optimization.General;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers;
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers.Operators;
using Itinero.Optimization.Algorithms.Solvers;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// The no-depot capacitated VRP.
    /// </summary>
    public class NoDepotCVRProblem
    {
        /// <summary>
        /// Gets or sets the vehicle capacity.
        /// </summary>
        public Capacity Capacity { get; set; }

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
        public Func<NoDepotCVRProblem, IList<int>, int> SelectSeedWithCloseNeighboursHeuristic
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
        public Func<NoDepotCVRProblem, IList<int>, int> SelectRandomSeedHeuristic
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
        public NoDepotCVRPSolution Solve()
        {
            return this.Solve((p, x, y) => true);
        }

        /// <summary>
        /// Solves this using a default solver.
        /// </summary>
        /// <returns></returns>
        public NoDepotCVRPSolution Solve(Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> overlapsFunc)
        {


            //var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
            //    new SeededLocalizedCheapestInsertionSolver(this.SelectSeedHeuristic, overlapsFunc), 1,
            //        );
            //var solver = new GuidedVNS(constructionHeuristic, overlapsFunc, 4 * 60);
            //var iterate = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
            //    solver, 10);

            var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
                new SeededLocalizedCheapestInsertionSolver(this.SelectRandomSeedHeuristic, overlapsFunc), 10);

            return this.Solve(constructionHeuristic);
        }

        /// <summary>
        /// Solvers this using the given solver.
        /// </summary>
        public NoDepotCVRPSolution Solve(Algorithms.Solvers.ISolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> solver)
        {
            return solver.Solve(this, new NoDepotCVRPObjective());
        }
    }
}