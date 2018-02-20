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
using System.Text;
using Itinero.Optimization.General;
using Itinero.Optimization.Tours;
using Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// The no-depot capacitated VRP.
    /// </summary>
    public class NoDepotCVRProblem
    {
        /// <summary>
        /// The vehicle capacity.!--
        /// </summary>
        /// <returns></returns>
        public float Max { get; set; } 

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights { get; set; }

        /// <summary>
        /// Gets the seed heuristic.
        /// </summary>
        /// <returns></returns>
        public Func<NoDepotCVRProblem, IList<int>, int> SelectSeedHeuristic
        {
            get
            {
                return (problem, visits) => 
                {
                    var weights = problem.Weights;

                    //return Algorithms.Seeds.SeedHeuristics.GetSeedWithCloseNeighbours(
                    //     weights, visits);
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
            return this.Solve(new SeededLocalizedCheapestInsertionSolver(
                this.SelectSeedHeuristic, (p, x, y) => true));
        }

        /// <summary>
        /// Solves this using a default solver.
        /// </summary>
        /// <returns></returns>
        public NoDepotCVRPSolution Solve(Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> overlapsFunc)
        {
            // var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
            //     new SeededLocalizedCheapestInsertionSolver(this.SelectSeedHeuristic, overlapsFunc), 20);
            // var solver = new GuidedVNS(constructionHeuristic, overlapsFunc, 10 * 60);

            var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
                new SeededLocalizedCheapestInsertionSolver(this.SelectSeedHeuristic, overlapsFunc), 20);
            //var constructionHeuristic = new SeededLocalizedCheapestInsertionSolver(this.SelectSeedHeuristic, overlapsFunc);
            var solver = new GuidedVNS(constructionHeuristic, overlapsFunc, 4 * 60);

            return this.Solve(solver);
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