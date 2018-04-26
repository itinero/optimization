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
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Depot.Capacitated
{
    /// <summary>
    /// The capacitated VRP.
    /// </summary>
    public class DepotCVRProblem : IRelocateProblem, IExchangeProblem
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
        /// Depot of the vehicle, thus the visit where it has to start its round and stop again
        /// </summary>
        /// <returns></returns>
        public int Depot
        {
            get;
            set;
        }

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
        /// Holds the nearest neigbours in travel cost.
        /// </summary>
        private NearestNeigbourArray _nNTravelCost = null;
        private int _depot;

        /// <summary>
        /// Gets the nearest neigbours in travel cost.
        /// </summary>
        /// <returns></returns>
        public NearestNeigbourArray NearestNeigboursTravelCost
        {
            get
            {
                if (_nNTravelCost == null)
                {
                    _nNTravelCost = new NearestNeigbourArray(this.Weights);
                }
                return _nNTravelCost;
            }
        }

        /// <summary>
        /// Solves this using a default solver.
        /// </summary>
        /// <returns></returns>
        public DepotCVRPSolution Solve()
        {
            return this.Solve((p, x, y) => true);
        }

        /// <summary>
        /// Solves this using a default solver.
        /// </summary>
        /// <returns></returns>
        public DepotCVRPSolution Solve(Delegates.OverlapsFunc<DepotCVRProblem, ITour> overlapsFunc)
        {
            // Declaration of some of the operators
            var crossMultiAllPairs10 = new MultiExchangeOperator<DepotCVRPObjective, DepotCVRProblem, DepotCVRPSolution>
                (1, 10, true, true, true, false);
            var crossMultiAllPairs20 = new MultiExchangeOperator<DepotCVRPObjective, DepotCVRProblem, DepotCVRPSolution>
                (1, 20, true, true, true, false);
            var crossMultiSomePairs5 = new MultiExchangeOperator<DepotCVRPObjective, DepotCVRProblem, DepotCVRPSolution>
                (1, 5, true, false, true, false);
            var crossMultiSomePairs10 = new MultiExchangeOperator<DepotCVRPObjective, DepotCVRProblem, DepotCVRPSolution>
                (1, 10, true, false, true, false);

            var crossMultiAllPairsUntil = new Algorithms.Solvers.IterativeOperator<float, DepotCVRProblem, DepotCVRPObjective, DepotCVRPSolution, float>
                (crossMultiAllPairs10, 20, stopAtFail : true);

            var relocMulti = new MultiRelocateOperator<DepotCVRPObjective, DepotCVRProblem, DepotCVRPSolution>
                (2, 5);
            var reloc = new RelocateOperator<DepotCVRPObjective, DepotCVRProblem, DepotCVRPSolution>
                (true, wrapAround : false /*no wrap around to preserve the depot*/ );

            var slci = new SeededCheapestInsertion<DepotCVRProblem, DepotCVRPObjective, DepotCVRPSolution>(
                    new TSP.Solvers.HillClimbing3OptSolver(),
                    new IInterTourImprovementOperator<float, DepotCVRProblem, DepotCVRPObjective, DepotCVRPSolution, float>[]
                    {
                        //   reloc,
                        //   relocMulti,
                        //   crossMultiSomePairs5
                    }, 0.03f, .25f
                );

            Func<DepotCVRProblem, IList<int>, int> seedFunc = (problem, visits) =>
            {
                var weights = problem.Weights;
                return Algorithms.Seeds.SeedHeuristics.GetSeedFarthest(
                    weights : problem.Weights, visitPool : visits, visit : problem.Depot);
                // return Algorithms.Seeds.SeedHeuristics.GetSeedRandom(visits);
                //return Algorithms.Seeds.SeedHeuristics.GetSeedWithCloseNeighbours(
                //    weights, this.NearestNeigboursTravelCost, visits, 20, .75f, .5f);
            };

            DepotCVRPObjective objective = new DepotCVRPObjective(seedFunc, localizationFactor : 0.9f);

            var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, DepotCVRProblem, DepotCVRPObjective, DepotCVRPSolution, float>
                (slci, 20, crossMultiSomePairs10);

            var iterate = new Algorithms.Solvers.IterativeSolver<float, DepotCVRProblem, DepotCVRPObjective, DepotCVRPSolution, float>(
                    constructionHeuristic, 1,
                    crossMultiAllPairs20,
                    crossMultiAllPairs10);

            return this.Solve(constructionHeuristic, objective);
        }

        /// <summary>
        /// Solvers this using the given solver.
        /// </summary>
        public DepotCVRPSolution Solve(
            Algorithms.Solvers.ISolver<float, DepotCVRProblem, DepotCVRPObjective, DepotCVRPSolution, float> solver,
            DepotCVRPObjective objective)
        {
            return solver.Solve(this, objective);
        }
    }
}