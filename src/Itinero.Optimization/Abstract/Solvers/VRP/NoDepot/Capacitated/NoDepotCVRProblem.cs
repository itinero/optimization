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
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS;
using Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.General;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// The capacitated VRP.
    /// 
    /// The goal of this VRP is to create routes which are as clusterd as possible, thus covering one neighbourhood.
    /// In completely ignores the depot in the solution (if there is one given)
    /// 
    /// If a depot is given, only the cost from the depot to the cluster is taken into account.
    /// In other words, visits along the path to the depot are not eligible for visting.
    /// 
    /// </summary>
    public class NoDepotCVRProblem : IRelocateProblem, IExchangeProblem
    {
        /// <summary>
        /// Gets or sets the vehicle capacity.
        /// </summary>
        public Capacity Capacity { get; set; }

        /// <summary>
        /// Gets the weights. This is the travelling time it takes to get from a point A to a point B. Indexed with [from][to]
        /// </summary>
        public float[][] Weights { get; set; }

        /// <summary>
        /// Gets or sets the visit costs. This is the handling time at a certain spot
        /// </summary>
        /// <returns></returns>
        public float[] VisitCosts { get; set; }

        /// <summary>
        /// Optional depot. If not null, the cost to travel from/to a cluster is taken into account.
        /// </summary>
        /// <returns></returns>
        public int? Depot { get; set; }

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
        /// Holds the nearest neigbours in travel cost. Only used for the caching.
        /// </summary>
        private NearestNeigbourArray _nNTravelCost = null;

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
        /// A seed heuristic.
        /// </summary>
        /// <returns></returns>
        public Func<NoDepotCVRProblem, IList<int>, int> SelectSeedWithCloseNeighboursHeuristic
        {
            get
            {
                return (problem, visits) => Algorithms.Seeds.SeedHeuristics.GetSeedWithCloseNeighbours(
                        problem.Weights, this.NearestNeigboursTravelCost, visits);
            }
        }

        /// <summary>
        /// A seed heuristic.
        /// </summary>
        /// <returns></returns>
        public Func<NoDepotCVRProblem, IList<int>, int> SelectRandomSeedHeuristic
        {
            get
            {
                return (problem, visits) =>
                    Algorithms.Seeds.SeedHeuristics.GetSeedRandom(visits);
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
            var crossMultiAllPairs = new MultiExchangeOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>
                (1, 10, true, true, true);
            var crossMultiAllPairsUntil = new Algorithms.Solvers.IterativeOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>
                (crossMultiAllPairs, 20, true);
            var multiReloc25 = new MultiRelocateOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>
               (2, 5);
            var reloc = new RelocateOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>(true);
            var multiExch15 = new MultiExchangeOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>
                (1, 5, true, false, true);


            var multiReloc23 = new MultiRelocateOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>(2, 3);
            var multiExch13 = new MultiExchangeOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>(1, 3, true, false, true);
            var multiExchOp110 = new MultiExchangeOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>(1, 10, true, true, true);
            var slci = new SeededCheapestInsertion<NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution>(
                new TSP.Solvers.HillClimbing3OptSolver(),
                new IInterTourImprovementOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>[]
                {   multiReloc23,
                    reloc,
                    multiExch13
                }, 0.03f, .25f
            );
            var slciIterate = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
               slci, 20, multiExchOp110);
            var multiExch120 = new MultiExchangeOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>(1, 20, true, true, true);

            var gvns = new GuidedVNS<NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, Penalties>(
                        slci, new TSP.Solvers.HillClimbing3OptSolver(),
                        new IInterTourImprovementOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>[]
                        {
                            multiReloc25,
                            reloc,
                            multiExch15
                        }
                        );


            var constructionHeuristic = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
                                 gvns, 20);
            var iterate = new Algorithms.Solvers.IterativeSolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>(
                    constructionHeuristic, 1,
                        new MultiExchangeOperator<NoDepotCVRPObjective, NoDepotCVRProblem, NoDepotCVRPSolution>(1, 20, true, true, true),
                        crossMultiAllPairs);



            return this.Solve(iterate, new NoDepotCVRPObjective((problem, visits) =>
                {
                    var weights = problem.Weights;

                    //return Algorithms.Seeds.SeedHeuristics.GetSeedRandom(visits);
                    return Algorithms.Seeds.SeedHeuristics.GetSeedWithCloseNeighbours(
                        weights, this.NearestNeigboursTravelCost, visits, 20, .75f, .5f);
                }, overlapsFunc, 1f, 0.01f, 480 * 2));
        }

        /// <summary>
        /// Solvers this using the given solver.
        /// </summary>
        public NoDepotCVRPSolution Solve(Algorithms.Solvers.ISolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> solver,
            NoDepotCVRPObjective objective)
        {
            return solver.Solve(this, objective);
        }


    }
}
