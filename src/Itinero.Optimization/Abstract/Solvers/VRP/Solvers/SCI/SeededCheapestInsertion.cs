using System.Collections.Generic;
using Itinero.Optimization.Abstract.Solvers.VRP.Operators;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Abstract.Solvers.VRP.Solvers.SCI
{
    /// <summary>
    /// A construction heuristic using a tour-by-tour strategy of seeding a new tour and filling it with visits.
    /// </summary>
    public class SeededCheapestInsertion<TProblem, TObjective, TSolution> : SolverBase<float, TProblem, TObjective, TSolution, float>
        where TSolution : ISeededCheapestInsertionSolution
    where TObjective : ObjectiveBase<TProblem, TSolution, float>, ISeededCheapestInsertionObjective<TProblem, TSolution>
    {
        private readonly float _interImprovementsThreshold; // the threshold for when the apply inter-route improvements.
        private readonly IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float> _intraImprovements; // holds the intra-route improvements;
        // TODO: consider replacing this by one operator implementation for better outside control over how this is executed.
        private readonly List<IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>> _interImprovements; // holds the inter-route improvements.
        private readonly float _remainingThreshold; // the threshold percentage.

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="remainingThreshold">The percentage of visits to just place in other tours at the end.</param>
        /// <param name="interImprovementsThreshold">The precentage of visits place without applying inter-route improvements.</param>
        /// <param name="intraImprovement">The intra-improvement operator.</param>
        /// <param name="interImprovements">The inter-improvement operator.</param>
        public SeededCheapestInsertion(IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float> intraImprovement,
            IEnumerable<IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>> interImprovements, float remainingThreshold = 0.03f, float interImprovementsThreshold = 0.25f)
        {
            _remainingThreshold = remainingThreshold;
            _interImprovementsThreshold = interImprovementsThreshold;

            _intraImprovements = intraImprovement;
            _interImprovements = new List<IInterTourImprovementOperator<float, TProblem, TObjective, TSolution, float>>(interImprovements);
        }

        // TODO: expand the name to include the improvement operators.

        /// <summary>
        /// Gets the name of this solver.
        /// </summary>
        /// <returns></returns>
        public override string Name => "SC";

        /// <summary>
        /// Solves the given problem using the given objective.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="fitness">The resulting fitness.</param>
        /// <returns></returns>
        public override TSolution Solve(TProblem problem, TObjective objective, out float fitness)
        {
            // create the solution.
            var solution = objective.NewSolution(problem);

            // get the list of potential visits.
            var visits = objective.PotentialVisits(problem);
            var totalVisits = visits.Count;

            // calculate absolute thresholds.
            var interImprovementThreshold = (int) (visits.Count * _interImprovementsThreshold);
            var remainingThreshold = (int) (visits.Count * _remainingThreshold);

            while (visits.Count > 0)
            { // keep placing visits until there are no more left.

                if (visits.Count < remainingThreshold)
                { // try and distribute the remaining visits if there are only a few left.
                    if (this.TryPlaceRemaining(problem, objective, solution, visits))
                    { // all remaining where placed.
                        break;
                    }
                }

                // start a new tour.
                var t = objective.SeedNext(problem, solution, visits);
                var tour = solution.Tour(t);

                while (visits.Count > 0)
                { // fill up this tour until there are no more visits.

                    // try and place the next visit.
                    if (objective.TryPlaceAny(problem, solution, t, visits))
                    { // placement succeeded.
                        if (((totalVisits - visits.Count) % interImprovementThreshold) == 0)
                        { // apply improvement local search if threshold reached.
                            // apply the intra-route improvements.
                            this.ApplyIntra(problem, objective, tour);

                            // apply the inter-route improvements.
                            this.ApplyInter(problem, objective, solution, t);
                        }
                    }
                    else
                    { // ok we are done!
                        // apply the intra-route improvements.
                        this.ApplyIntra(problem, objective, tour);

                        // apply the inter-route improvements.
                        this.ApplyInter(problem, objective, solution, t);

                        // move to the next tour.
                        // TODO: what if we suddenly have more space left? perhaps check if the two above succeed and try once more.
                        break;
                    }
                }
            }

            fitness = objective.Calculate(problem, solution);
            return solution;
        }

        /// <summary>
        /// Tries to place the remaining visits in the current tours.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="visits">The visits.</param>
        private bool TryPlaceRemaining(TProblem problem, TObjective objective, TSolution solution, IList<int> visits)
        {
            while (objective.TryPlaceAny(problem, solution, visits)) { }
            return visits.Count == 0;
        }

        /// <summary>
        /// Applies the intra-improvements the the given tour.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="tour">The tour.</param>
        private void ApplyIntra(TProblem problem, TObjective objective, ITour tour)
        {
            var tsp = objective.BuildSubTourTSP(problem, tour);

            _intraImprovements.ApplyUntil(tsp, TSP.TSPObjective.Default, tour, out float localDelta);
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
            { // apply the intra-route heurstic between the new and all existing routes.
                if (other != t && objective.HaveToTryInter(problem, solution, t, other))
                { // only check routes that overlap.
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
            { // try the current improvement operations.
                var improvement = true;
                while (improvement)
                { // keep looping when there is improvement.
                    improvement = false;

                    if (improvementOperation.Apply(problem, objective, solution, t1, t2, out float delta))
                    { // there was an improvement.
                        improvement = true;
                        globalImprovement = true;

                        Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2}",
                            t1, t2, improvementOperation.Name);
                    }
                    else if (!improvementOperation.IsSymmetric &&
                        improvementOperation.Apply(problem, objective, solution, t1, t2, out delta))
                    { // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        globalImprovement = true;

                        Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2}",
                            t1, t2, improvementOperation.Name);
                    }
                }
            }
            return globalImprovement;
        }
    }
}