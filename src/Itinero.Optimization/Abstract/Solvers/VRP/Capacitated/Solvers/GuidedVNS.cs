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

using System.Collections.Generic;
using System.Linq;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.General;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Exchange.Multi;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators.Relocate.Multi;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Capacitated.Solvers
{
    /// <summary>
    /// A Guided VNS search applying intra-tour improvements until no improvements can be found.
    /// </summary>
    public class GuidedVNS : SolverBase<float, CVRProblem, CVRPObjective, CVRPSolution, float>
    {
        private readonly float _lambda; // hold the lambda value, define a better name.
        private readonly SolverBase<float, CVRProblem, CVRPObjective, CVRPSolution, float> _constructionHeuristic;
        private readonly Delegates.OverlapsFunc<CVRProblem, ITour> _overlaps; // holds the overlap function.
        private readonly List<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>> _intraImprovements;
        private readonly List<IInterTourImprovementOperator<float, CVRProblem, CVRPObjective, CVRPSolution, float>> _interImprovements; // holds the inter-route improvements.

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="constructionHeuristic">The construction heurstic.</param>
        /// <param name="overlaps">The overlaps function.</param>
        /// <param name="lambda">The lambda parameter.</param>
        public GuidedVNS(SolverBase<float, CVRProblem, CVRPObjective, CVRPSolution, float> constructionHeuristic,
            Delegates.OverlapsFunc<CVRProblem, ITour> overlaps, float lambda)
        {
            _constructionHeuristic = constructionHeuristic;
            _overlaps = overlaps;
            _lambda = lambda;

            // register the default intra improvement operators.
            _intraImprovements = new List<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>>(1);
            _intraImprovements.Add(new TSP.Solvers.HillClimbing3OptSolver());

            // register the default inter improvement operators.
            _interImprovements = new List<IInterTourImprovementOperator<float, CVRProblem, CVRPObjective, CVRPSolution, float>>(4);
            _interImprovements.Add(new RelocateOperator<CVRPObjective, CVRProblem, CVRPSolution>(true));
            _interImprovements.Add(new ExchangeOperator<CVRPObjective, CVRProblem, CVRPSolution>());
            _interImprovements.Add(new MultiRelocateOperator<CVRPObjective, CVRProblem, CVRPSolution>(2 ,5));
            _interImprovements.Add(new MultiExchangeOperator<CVRPObjective, CVRProblem, CVRPSolution>(2, 10, true));
        }

        /// <summary>
        /// Gets the name of this solver.
        /// </summary>
        public override string Name => "G-VNS";

        /// <summary>
        /// Runs this solver.
        /// </summary>
        /// <param name="problem">The problem to be solved.</param>
        /// <param name="objective">The objective for a solution.</param>
        /// <param name="fitness">The resulting fitness value.</param>
        /// <returns></returns>
        public override CVRPSolution Solve(CVRProblem problem, CVRPObjective objective, out float fitness)
        {
            var worstFactor = 5;

            // construct initial solution.
            var solution = _constructionHeuristic.Solve(problem, objective);

            // loop over all tour pairs.
            bool improved = true;
            while (improved)
            {
                improved = false;

                for (var t1 = 0; t1 < solution.Count; t1++)
                {
                    for (var t2 = t1 + 1; t2 < solution.Count; t2++)
                    {
                        var tour1 = solution.Tour(t1);
                        var tour2 = solution.Tour(t2);
                        if (!_overlaps(problem, tour1, tour2))
                        { // don't overlap, move on.
                            continue;
                        }

                        var penalizations = new Dictionary<Pair, Penalty>();
                        var worstPenalty = -1f;
                        while (true)
                        { // keep penalizing->improving->penalizing until penalization exceeds bounds.
                            var tour1Before = solution.Contents[t1].Weight;
                            var tour2Before = solution.Contents[t2].Weight;

                            // select and execute next penalization.
                            var nextPenalty = PenalizeNext(problem, tour1, tour2, penalizations);
                            if (!nextPenalty.HasValue)
                            { // penalty was impossible, move on.
                                break;
                            }
                            if (worstPenalty < 0)
                            { // keep the first penalized weight.
                                worstPenalty = nextPenalty.Value.Original;
                            }

                            // apply penalizations again.
                            var localSolution = solution.Clone() as CVRPSolution;
                            foreach (var penalty in penalizations)
                            {
                                var e = penalty.Key;
                                var p = penalty.Value;

                                problem.Weights[e.From][e.To] = p.Original + _lambda * p.Count;
                            }

                            // apply inter-tour improvements.
                            if (this.ImproveInterRoute(problem, objective, localSolution, t1, t2))
                            {
                                tour1 = localSolution.Tour(t1);
                                this.ImproveIntraRoute(problem.Weights, tour1,
                                    localSolution.Contents[t1].Weight);
                                tour2 = localSolution.Tour(t2);
                                this.ImproveIntraRoute(problem.Weights, tour2,
                                    localSolution.Contents[t2].Weight);
                            }

                            // verifiy against original if there are improvements.
                            var totalPenalty = ResetPenalizations(problem, penalizations);

                            localSolution.Contents[t1].Weight = objective.Calculate(problem, solution, t1);
                            localSolution.Contents[t2].Weight = objective.Calculate(problem, solution, t2);

                            var tour1After = localSolution.Contents[t1].Weight;
                            var tour2After = localSolution.Contents[t2].Weight;

                            if (tour1Before + tour2Before > tour1After + tour2After)
                            { // new solution is better, yay!
                                Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Information,
                                    "Improvement found {0}-{1} : {2}->{3} : Penalty: {4}/{5}", t1, t2, tour1Before + tour2Before, tour1After + tour2After,
                                        totalPenalty, worstPenalty * worstFactor);

                                solution = localSolution;
                                improved = true;
                            }

                            if (totalPenalty > worstPenalty * worstFactor)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            fitness = objective.Calculate(problem, solution);
            return solution;
        }

        /// <summary>
        /// Apply some improvements within one tour.
        /// </summary>
        private float ImproveIntraRoute(float[][] weights, ITour tour, float currentWeight)
        {
            // TODO: this can probably be replaced by the iterative solver.
            var subTourProblem = new TSP.TSPSubProblem(tour, weights);

            var improvement = true;
            var newWeight = currentWeight;
            while (improvement)
            { // keep trying while there are still improvements.
                improvement = false;

                // loop over all improvement operations.
                foreach (var improvementOperator in _intraImprovements)
                { // try the current improvement operations.
                    float delta;
                    if (improvementOperator.Apply(subTourProblem, TSP.TSPObjective.Default, tour, out delta))
                    {
                        // Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Information,
                        //     "Intra-improvement found {0} {1}->{2}",
                        //         improvementOperator.Name, newWeight, newWeight - delta);

                        // update the weight.
                        newWeight = newWeight - delta;

                        improvement = true;
                        break;
                    }
                }
            }
            return newWeight;
        }

        /// <summary>
        /// Apply some improvements between the given routes and returns the resulting weight.
        /// </summary>
        private bool ImproveInterRoute(CVRProblem problem, CVRPObjective objective, CVRPSolution solution,
            int tour1Idx, int tour2Idx)
        {
            // get the routes.
            var route1 = solution.Tour(tour1Idx);
            var route2 = solution.Tour(tour2Idx);

            // loop over all improvement operations.
            bool globalImprovement = false;
            foreach (var improvementOperation in _interImprovements)
            { // try the current improvement operations.
                bool improvement = true;
                while (improvement)
                { // keep looping when there is improvement.
                    improvement = false;

                    var tour1Weight = solution.Contents[tour1Idx].Weight;
                    var tour2Weight = solution.Contents[tour2Idx].Weight;
                    var totalBefore = tour1Weight + tour2Weight;

                    float delta;
                    if (improvementOperation.Apply(problem, objective, solution, tour1Idx, tour2Idx, out delta))
                    { // there was an improvement.
                        improvement = true;
                        globalImprovement = true;

                        tour1Weight = solution.Contents[tour1Idx].Weight;
                        tour2Weight = solution.Contents[tour2Idx].Weight;
                        var totalAfter = tour1Weight + tour2Weight;

                        // Itinero.Logging.Logger.Log("G-VNS", Itinero.Logging.TraceEventType.Information,
                        //     "Inter-improvement found {0}<->{1}: {2} ({3}->{4})",
                        //         tour1Idx, tour2Idx, improvementOperation.Name, totalBefore, totalAfter);

                        //break;
                    }
                    else if (!improvementOperation.IsSymmetric &&
                        improvementOperation.Apply(problem, objective, solution, tour2Idx, tour1Idx, out delta))
                    { // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        globalImprovement = true;

                        // Itinero.Logging.Logger.Log("G-VNS", Itinero.Logging.TraceEventType.Information,
                        //     "Inter-improvement found {0}<->{1}: {2}",
                        //     tour1Idx, tour2Idx, improvementOperation.Name);

                        //break;
                    }
                }
            }
            return globalImprovement;
        }

        private float ResetPenalizations(CVRProblem problem, Dictionary<Pair, Penalty> penalizations)
        {
            var totalPenalty = 0f;
            foreach (var pair in penalizations)
            {
                var p = pair.Value;
                var e = pair.Key;

                totalPenalty += p.Count * _lambda;
                problem.Weights[e.From][e.To] = p.Original;
            }
            return totalPenalty;
        }

        private Penalty? PenalizeNext(CVRProblem problem, ITour tour1, ITour tour2, Dictionary<Pair, Penalty> penalizations)
        {
            // choose best next edge to penalize.
            var worstCost = 0f;
            Pair? worstPair = null;
            Penalty? worstPenalty = null;
            foreach (var e in tour1.Pairs().Concat(tour2.Pairs()))
            {
                Penalty p;
                var cost = 0f;
                if (!penalizations.TryGetValue(e, out p))
                {
                    cost = problem.Weights[e.From][e.To];

                    if (worstPair == null ||
                        worstCost < cost)
                    {
                        worstCost = cost;
                        worstPair = e;
                        worstPenalty = new Penalty()
                        {
                            Count = 1,
                            Original = cost
                        };
                    }
                }
                else
                {
                    cost = ((_lambda * p.Count) + p.Original) / (p.Count + 1);

                    if (worstPair == null ||
                        worstCost < cost)
                    {
                        worstCost = cost;
                        worstPair = e;
                        worstPenalty = new Penalty()
                        {
                            Count = (byte)(p.Count + 1),
                            Original = p.Original
                        };
                    }
                }
            }

            if (!worstPair.HasValue)
            { // no pairs found.
                return null;
            }
            penalizations[worstPair.Value] = worstPenalty.Value;
            return worstPenalty.Value;
        }

        private struct Penalty
        {
            /// <summary>
            /// The penalization count.
            /// </summary>
            /// <returns></returns>
            public byte Count { get; set; }

            /// <summary>
            /// The original weight.
            /// </summary>
            /// <returns></returns>
            public float Original { get; set; }

            /// <summary>
            /// Gets an empty penalty.
            /// </summary>
            /// <returns></returns>
            public static Penalty Empty = new Penalty()
            {
                Count = 0,
                Original = -1
            };
        }
    }
}