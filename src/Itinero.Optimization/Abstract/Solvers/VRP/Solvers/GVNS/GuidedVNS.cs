using System.Collections.Generic;
using Itinero.Logging;
using Itinero.Optimization.Abstract.Solvers.TSP;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Solvers.GVNS
{
    /// <summary>
    /// A Guided variable neigbourhood search penalizing the worst connections and applying intra-tour improvements until no improvements can be found.
    /// </summary>
    public class
        GuidedVNS<TProblem, TObjective, TSolution, TPenalty> : SolverBase<float, TProblem, TObjective, TSolution, float>
        where TSolution : class, IGuidedVNSSolution
        where TObjective : ObjectiveBase<TProblem, TSolution, float>, IGuidedVNSObjective<TProblem, TSolution, TPenalty>
        where TPenalty : class, new()
    {
        private readonly SolverBase<float, TProblem, TObjective, TSolution, float> _constructionHeuristic;

        private readonly IOperator<float, ITSProblem, TSPObjective, ITour, float>
            _intraImprovements; // holds the intra-route improvements;

        private readonly List<IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>>
            _interImprovements; // holds the inter-route improvements.

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="solver">The solver to apply before starting the guided VNS.</param>
        /// <param name="intraImprovement">The intra-improvement operator.</param>
        /// <param name="interImprovements">The inter-improvement operator.</param>
        public GuidedVNS(SolverBase<float, TProblem, TObjective, TSolution, float> solver,
            IOperator<float, ITSProblem, TSPObjective, ITour, float> intraImprovement,
            IEnumerable<IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>> interImprovements)
        {
            _constructionHeuristic = solver;
            _interImprovements =
                new List<IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>>(
                    interImprovements);
            _intraImprovements = intraImprovement;
        }

        /// <summary>
        /// Gets the name of this solver.
        /// </summary>
        /// <returns></returns>
        public override string Name => "GVNS";

        /// <summary>
        /// Solvers the given problem using the given objective.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="fitness">The fitness.</param>
        /// <returns></returns>
        public override TSolution Solve(TProblem problem, TObjective objective, out float fitness)
        {
            // construct initial solution.
            var solution = _constructionHeuristic.Solve(problem, objective);
            fitness = objective.Calculate(problem, solution);

            // loop over all tour pairs.
            bool improved = true;
            while (improved)
            {
                improved = false;

                for (var t1 = 0; t1 < solution.Count; t1++)
                {
                    for (var t2 = t1 + 1; t2 < solution.Count; t2++)
                    {
                        if (!objective.HaveToTryInter(problem, solution, t1, t2))
                        {
                            // don't overlap, move on.
                            continue;
                        }

                        var penalty = new TPenalty();
                        while (true)
                        {
                            // keep penalizing->improving->penalizing until penalization exceeds bounds.
                            // select and execute next penalization.
                            if (!objective.ApplyPenalty(problem, solution, t1, t2, penalty))
                            {
                                // penalty was impossible, move on. It has no use anymore to apply this or any bigger penalty
                                // so we stop the loop
                                break;
                            }

                            // try to optimize solution against penalized problem.
                            var localSolution = solution.Clone() as TSolution;


                            // update the solution weights according to the given penalties.
                            objective.UpdateSolution(problem, localSolution);

                            // apply inter-tour improvements.
                            var success = false;
                            if (this.ApplyInter(problem, objective, localSolution, t1, t2))
                            {
                                this.ApplyIntra(problem, objective, localSolution, t1);
                                this.ApplyIntra(problem, objective, localSolution, t2);

                                success = true;
                            }

                            // verifiy against original if there are improvements.
                            objective.ResetPenalty(problem, penalty);

                            if (!success)
                            {
                                // some constraints are broken. We skip this solution and continue searching
                                continue;
                            }

                            // figure out if this also improves the original.

                            // update the solution weights according to the original solution.
                            objective.UpdateSolution(problem, localSolution);

                            var after = objective.Calculate(problem, localSolution);
                            if (after >= fitness)
                            {
                                // No improvement. We skip
                                continue;
                            }

                            for (int i = 0; i < localSolution.Count; i++)
                            {
                                if (localSolution.Tour(i).Count == 6)
                                {
                                    break; // used to place a breakpoint
                                }
                            }

                            System.Diagnostics.Debug.WriteLine("Improvement found {0}-{1} : {2}->{3} : Penalty: {4}",
                                t1, t2, fitness, after,
                                penalty.ToString());
                            Logger.Log(this.Name, TraceEventType.Verbose,
                                "Improvement found {0}-{1} : {2}->{3} : Penalty: {4}", t1, t2, fitness, after,
                                penalty.ToString());

                            solution = localSolution;
                            fitness = after;
                            improved = true;
                        }
                    }
                }
            }

            fitness = objective.Calculate(problem, solution);
            return solution;
        }

        /// <summary>
        /// Applies the intra-improvements the the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t">The tour.</param>
        private bool ApplyIntra(TProblem problem, TObjective objective, TSolution solution, int t)
        {
            var tour = solution.Tour(t);
            var tsp = objective.BuildSubTourTSP(problem, tour);

            var convertedTour = tsp.Map(tour);

            if (_intraImprovements.ApplyUntil(tsp, TSPObjective.Default, convertedTour, out float localDelta))
            {
                foreach (var pair in convertedTour.Pairs())
                {
                    tour.ReplaceEdgeFrom(tsp.Original(pair.From), tsp.Original(pair.To));
                }

                // Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Verbose,
                //     "Intra-improvement found {0}: {1}",
                //     t, _intraImprovements.Name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies inter-improvements between the given tour and all other existing tours.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t">The tour.</param>
        private bool ApplyInter(TProblem problem, TObjective objective, TSolution solution, int t)
        {
            var improvement = false;
            for (int other = 0; other < solution.Count; other++)
            {
                // apply the intra-route heurstic between the new and all existing routes.
                if (other != t && objective.HaveToTryInter(problem, solution, t, other))
                {
                    // only check routes that overlap.
                    if (this.ApplyInter(problem, objective, solution, t, other))
                    {
                        // TODO: there used to be intra-improvements here, is this needed? can this be more efficiënt.
                        improvement = true;
                    }
                }
            }

            return improvement;
        }

        /// <summary>
        /// Applies inter-improvements between the two given tours.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        private bool ApplyInter(TProblem problem, TObjective objective, TSolution solution, int t1, int t2)
        {
            // get the routes.
            var route1 = solution.Tour(t1);
            var route2 = solution.Tour(t2);

            // TODO: break here at first improvement before moving to the next operator.
            // TODO: do best-improvement here or not? or is this up to the operators added?

            // loop over all improvement operations.
            var globalImprovement = false;
            foreach (var improvementOperation in _interImprovements)
            {
                // try the current improvement operations.
                var improvement = true;
                while (improvement)
                {
                    // keep looping when there is improvement.
                    improvement = false;

                    if (improvementOperation.Apply(problem, objective, solution, t1, t2, out float delta))
                    {
                        // there was an improvement.
                        improvement = true;
                        globalImprovement = true;

                        // Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Verbose,
                        //     "Inter-improvement found {0}<->{1}: {2}",
                        //     t1, t2, improvementOperation.Name);
                    }
                    else // could not improve... try to improve symmetrically
                    if (!improvementOperation.IsSymmetric &&
                        improvementOperation.Apply(problem, objective, solution, t1, t2, out delta))
                    {
                        // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        globalImprovement = true;

                        // Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Verbose,
                        //     "Inter-improvement found {0}<->{1}: {2}",
                        //     t1, t2, improvementOperation.Name);
                    }
                }
            }

            return globalImprovement;
        }
    }
}