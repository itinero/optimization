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
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.General;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers
{
    /// <summary>
    /// A seeded localized cheapest insertion solver.
    /// </summary>
    public class SeededLocalizedCheapestInsertionSolver : SolverBase<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>
    {
        private readonly int _k; // the amount of visits to place before applying local improvements.
        private readonly float _slackPercentage;  // the percentage of space to leave for future improvements.
        private readonly Func<NoDepotCVRProblem, IList<int>, int> _selectSeed; // hold the select seed function.
        // holds the intra-route improvements;
        private readonly List<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>> _intraImprovements; 
        private readonly List<IInterTourImprovementOperator> _interImprovements; // holds the inter-route improvements.
        private readonly Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> _overlaps; // holds the overlap function.
        private readonly float _thresholdPercentage; // the threshold percentage.
        private readonly float _localizationFactor; // the localization factor.

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="selectSeed">The seed selection heuristic.</param>
        /// <param name="overlaps">Function to calculate tour overlaps.</param>
        /// <param name="k">The # of visits to place before apply intra improvements.</param>
        /// <param name="slackPercentage">The percentage of space to leave for future improvements.</param>
        /// <param name="thresholdPercentage">The percentage of unplaced visits to try and place in existing tours.</param>
        /// <param name="localizationFactor">The factor to take into account the weight to the seed visit.</param>
        public SeededLocalizedCheapestInsertionSolver(Func<NoDepotCVRProblem, IList<int>, int> selectSeed, 
            Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> overlaps, int k = 10, float slackPercentage = 5, 
            float thresholdPercentage = 10, float localizationFactor = 0.5f)
        {
            _selectSeed = selectSeed;
            _overlaps = overlaps;
            _k = k;
            _slackPercentage = slackPercentage / 100f;
            _thresholdPercentage = thresholdPercentage / 100f;
            _localizationFactor = localizationFactor;

            // register the default intra improvement operators.
            _intraImprovements = new List<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>>(1);
            _intraImprovements.Add(new TSP.Solvers.HillClimbing3OptSolver());

            // register the default inter improvement operators.
            _interImprovements = new List<IInterTourImprovementOperator>(4);
            _interImprovements.Add(new Operators.ExchangeInterImprovementOperator());
            _interImprovements.Add(new Operators.RelocateImprovementOperator());
            _interImprovements.Add(new Operators.RelocateExchangeInterImprovementOperator(5));
            _interImprovements.Add(new Operators.CrossExchangeInterImprovementOperator(5));
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="selectSeed">The seed selection heuristic.</param>
        /// <param name="intraImprovements">The tour improvement operators.</param>
        /// <param name="interImprovements">The tour exhange improvement operators.</param>
        /// <param name="overlaps">Function to calculate tour overlaps.</param>
        /// <param name="k">The # of visits to place before apply intra improvements.</param>
        /// <param name="slackPercentage">The percentage of space to leave for future improvements.</param>
        /// <param name="thresholdPercentage">The percentage of unplaced visits to try and place in existing tours.</param>
        /// <param name="localizationFactor">The factor to take into account the weight to the seed visit.</param>
        public SeededLocalizedCheapestInsertionSolver(Func<NoDepotCVRProblem, IList<int>, int> selectSeed, 
            IEnumerable<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>> intraImprovements,
            IEnumerable<IInterTourImprovementOperator> interImprovements,
            Delegates.OverlapsFunc<NoDepotCVRProblem, ITour> overlaps, int k = 10, float slackPercentage = 5, 
            float thresholdPercentage = 10, float localizationFactor = 0.75f)
        {
            _selectSeed = selectSeed;
            _intraImprovements = new List<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>>(intraImprovements);
            _interImprovements = new List<IInterTourImprovementOperator>(_interImprovements);
            _overlaps = overlaps;
            _k = k;
            _slackPercentage = slackPercentage / 100f;
            _thresholdPercentage = thresholdPercentage / 100f;
            _localizationFactor = localizationFactor;
        }

        /// <summary>
        /// Gets the name of this solver.
        /// </summary>
        public override string Name => "SLCI";

        /// <summary>
        /// Solvers the given problem using the given objective.
        /// </summary>
        public override NoDepotCVRPSolution Solve(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, out float fitness)
        {
            // create the solution.
            var solution = new NoDepotCVRPSolution(problem.Weights.Length);

            // create a list of visits to place.
            var visits = new List<int>(System.Linq.Enumerable.Range(0, problem.Weights.Length));

            // create a new problem definition, taking into account the slack percentage.
            problem = new NoDepotCVRProblem()
            {
                Capacity = problem.Capacity.Scale(1 - _slackPercentage),
                Weights = problem.Weights
            };

            // keep placing visit until none are left.
            while (visits.Count > 0)
            {
                // try and distribute the remaining visits if there are only a few left.
                if (visits.Count < problem.Weights.Length * _thresholdPercentage)
                {
                    this.TryPlaceRemaining(problem, objective, solution, visits);
                }
                
                // select a visit using some heuristic.
                if (visits.Count > 0)
                {
                    // select a visit according to the given seed strategy.
                    var seed = _selectSeed(problem, visits);
                    visits.Remove(seed);
                    
                    Func<int, float> lambdaCostFunc = null;
                    if (_localizationFactor != 0)
                    { // create a function to add the localized effect (relative to the seed) to the CI algorithm
                       // if lambda is set.
                        lambdaCostFunc = (v) => problem.Weights[seed][v] + 
                                problem.Weights[v][seed];
                    }

                    // start a route r.
                    var currentTour = solution.Add(seed, seed);
                    var content = problem.Capacity.Empty();
                    problem.Capacity.Add(content, seed);
                    solution.Contents.Add(content);

                    while (visits.Count > 0)
                    {
                        // calculate the cheapest visit to insert.
                        Pair location;
                        int visit;
                        var increase = currentTour.CalculateCheapestAny(problem.Weights, visits, 
                            out location, out visit, lambdaCostFunc);

                        // calculate the actual increase if an extra cost was added.
                        if (lambdaCostFunc != null)
                        { // use the seed cost; the cost to the seed visit.
                            increase -= lambdaCostFunc(visit);
                        }

                        // calculate the new weight.
                        var potentialWeight = content.Weight + increase;
                        // cram as many visits into one route as possible.
                        if (problem.Capacity.UpdateAndCheckCosts(content, potentialWeight, visit))
                        {
                            // insert the visit, it is possible to add it. 
                            visits.Remove(visit);
                            currentTour.InsertAfter(location.From, visit);

                            // update the cost of the route.
                            content.Weight = potentialWeight;

                            // improve if needed.
                            if (((problem.Weights.Length - visits.Count) % _k) == 0)
                            { // an improvement is decided.
                                // apply the inter-route improvements.
                                content.Weight = this.ImproveIntraRoute(problem.Weights, currentTour,
                                    content.Weight);

                                // also to the inter-improvements.
                                this.Improve(problem, objective, solution, solution.Count - 1);
                                currentTour = solution.Tour(solution.Count - 1);
                            }
                        }
                        else
                        {// ok we are done!
                            // run the inter-improvements one last time.
                            this.Improve(problem, objective, solution, solution.Count - 1);

                            // break the route.
                            break;
                        }
                    }
                }
            }

            fitness = objective.Calculate(problem, solution);
            return solution;
        }
        
        /// <summary>
        /// Runs the inter-tour improvements on the new tour and the existing tours.
        /// </summary>
        private void Improve(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, int newTourIdx)
        {
            // the current route.
            var currentTour = solution.Tour(newTourIdx);

            for (int tourIdx = 0; tourIdx < solution.Count; tourIdx++)
            { // apply the intra-route heurstic between the new and all existing routes.
                if (tourIdx != newTourIdx && _overlaps(problem, solution.Tour(tourIdx),
                    solution.Tour(newTourIdx)))
                { // only check routes that overlap.
                    if (this.ImproveInterRoute(problem, objective, solution, tourIdx, newTourIdx))
                    { // if there was an improvement, run intra-improvements also.
                        this.ImproveIntraRoute(problem.Weights, solution.Tour(tourIdx), 
                            objective.Calculate(problem, solution, tourIdx));
                        this.ImproveIntraRoute(problem.Weights, solution.Tour(newTourIdx), 
                            objective.Calculate(problem, solution, newTourIdx));
                    }
                }
            }
        }

        /// <summary>
        /// Try placing the remaining visits in the existing tours.
        /// </summary>
        private void TryPlaceRemaining(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, List<int> visits)
        {
            var succes = true;
            Func<int, float> lambdaCostFunc = null;

            // keep placing visits until it's not possible anymore.
            while (succes && visits.Count > 0)
            {
                succes = false;
                var bestIncrease = float.MaxValue;
                Pair? bestLocation = null;
                int bestTourIdx = -1;
                int bestVisit = -1;
                for (int tourIdx = 0; tourIdx < solution.Count; tourIdx++)
                {
                    var tour = solution.Tour(tourIdx);
                    var seed = tour.First;
                    Pair location;
                    int visit;
                                    
                    lambdaCostFunc = null;
                    if (_localizationFactor != 0)
                    { // create a function to add the localized effect (relative to the seed) to the CI algorithm
                        // if lambda is set.
                        lambdaCostFunc = (v) => problem.Weights[seed][v] + 
                        problem.Weights[v][seed];
                    }

                    // run CI algorithm.
                    var increase = tour.CalculateCheapestAny(problem.Weights, visits, out location, out visit,
                        lambdaCostFunc);
                    if (increase < bestIncrease)
                    {
                        bestIncrease = increase;
                        bestVisit = visit;
                        bestLocation = location;
                        bestTourIdx = tourIdx;
                    }
                }

                // calculate the actual increase if an extra cost was added.
                var bestTour = solution.Tour(bestTourIdx);
                var actualIncrease = bestIncrease;
                if (_localizationFactor != 0)
                { // create a function to add the localized effect (relative to the seed) to the CI algorithm
                    // if lambda is set.
                    lambdaCostFunc = (v) => problem.Weights[bestTour.First][v] + 
                    problem.Weights[v][bestTour.First];
                    actualIncrease -= lambdaCostFunc(bestVisit);
                }

                // try to do the actual insert
                var tourTime = objective.Calculate(problem, solution, bestTourIdx);
                if (problem.Capacity.UpdateAndCheckCosts(solution.Contents[bestTourIdx], tourTime + actualIncrease, bestVisit))
                { // insert the visit.
                    bestTour.InsertAfter(bestLocation.Value.From, bestVisit);
                    visits.Remove(bestVisit);

                    this.Improve(problem, objective, solution, bestTourIdx);
                    
                    succes = true;
                }
            }
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
                        Itinero.Logging.Logger.Log("SeededLocalizedCheapestInsertionSolver", Itinero.Logging.TraceEventType.Information,
                            "Intra-improvement found {0} {1}->{2}",
                                improvementOperator.Name, newWeight, newWeight - delta);
                                
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
        private bool ImproveInterRoute(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, 
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

                    var tour1Weight = objective.Calculate(problem, solution, tour1Idx);
                    var tour2Weight = objective.Calculate(problem, solution, tour2Idx);
                    var totalBefore =  tour1Weight + tour2Weight;

                    float delta;
                    if(improvementOperation.Apply(problem, objective, solution, tour1Idx, tour2Idx, out delta))
                    { // there was an improvement.
                        improvement = true;
                        globalImprovement = true;

                        tour1Weight = objective.Calculate(problem, solution, tour1Idx);
                        tour2Weight = objective.Calculate(problem, solution, tour2Idx);
                        var totalAfter =  tour1Weight + tour2Weight;
                        
                        Itinero.Logging.Logger.Log("SeededLocalizedCheapestInsertionSolver", Itinero.Logging.TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2} ({3}->{4})",
                                tour1Idx, tour2Idx, improvementOperation.Name, totalBefore, totalAfter);

                        //break;
                    }
                    else if (!improvementOperation.IsSymmetric &&
                        improvementOperation.Apply(problem, objective, solution, tour2Idx, tour1Idx, out delta))
                    { // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        globalImprovement = true;

                        Itinero.Logging.Logger.Log("SeededLocalizedCheapestInsertionSolver", Itinero.Logging.TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2}",
                            tour1Idx, tour2Idx, improvementOperation.Name);

                        //break;
                    }
                }
            }
            return globalImprovement;
        }
    }
}