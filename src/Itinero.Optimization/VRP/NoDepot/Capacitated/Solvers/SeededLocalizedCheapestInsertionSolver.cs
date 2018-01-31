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
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers
{
    /// <summary>
    /// A seeded localized cheapest insertion solver.
    /// </summary>
    public class SeededLocalizedCheapestInsertionSolver : SolverBase<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>
    {
        // the amount of customers to place before applying local improvements.
        private readonly int _k; 
        // the percentage bound of space to leave for future improvements.
        private readonly float _slackPercentage; 
        // hold the select seed function.
        private readonly Func<NoDepotCVRProblem, NoDepotCVRPSolution, int> _selectSeed; 
        // holds the intra-route improvements;
        private List<IOperator<float, TSP.ITSProblem, TSP.TSPObjective, ITour, float>> _intraImprovements; 
        private List<IInterTourImprovementOperator> _interImprovements; // Holds the inter-route improvements.
        private bool _use_seed_cost; // Flag to configure seed costs.
        private bool _use_seed; // Flag to use seeding heuristic or not.
        private float _thresholdPercentage; // The threshold percentage.
        private float _lambda; // The lambda.

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public SeededLocalizedCheapestInsertionSolver(Func<NoDepotCVRProblem, NoDepotCVRPSolution, int> selectSeed)
        {
            _selectSeed = selectSeed;
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

            // create a list of customers to place.
            var customers = new List<int>(System.Linq.Enumerable.Range(0, problem.Weights.Length));

            // create the local max, taking into account the slack percentage.
            var max = problem.Max - (problem.Max * _slackPercentage);

            // keep placing customer until none are left.
            // keep a list of cheapest insertions.
            IInsertionCosts costs = new BinaryHeapInsertionCosts();
            while (customers.Count > 0)
            {
                // try and distribute the remaining customers if there are only a few left.
                if (customers.Count < problem.Weights.Length * _thresholdPercentage)
                {
                    bool succes = true;
                    while (succes && customers.Count > 0)
                    {
                        succes = false;
                        CheapestInsertionResult best = new CheapestInsertionResult();
                        best.Increase = float.MaxValue;
                        int best_idx = -1;
                        for (int route_idx = 0; route_idx < solution.Count; route_idx++)
                        {
                            IRoute route = solution.Route(route_idx);
                            CheapestInsertionResult result =
                                CheapestInsertionHelper.CalculateBestPlacement(problem, route, customers);
                            if (best.Increase > result.Increase)
                            {
                                best = result;
                                best_idx = route_idx;
                            }
                        }

                        IRoute best_route = solution.Route(best_idx);
                        double route_time = problem.Time(best_route);
                        if (route_time + best.Increase < max)
                        { // insert the customer.
                            best_route.InsertAfter(best.CustomerBefore, best.Customer);
                            customers.Remove(best.Customer);

                            this.Improve(problem, solution, max, best_idx);

                            succes = true;
                        }
                    }
                }
                
                // select a customer using some heuristic.
                if (customers.Count > 0)
                {
                    // select a customer according to the given seed strategy.
                    var seed = _selectSeed(problem, solution);
                    customers.Remove(seed);
                    
                    Func<int, float> lambdaCostFunc = null;
                    if (_lambda != 0)
                    { // create a function to add the localized effect (relative to the seed) to the CI algorithm
                       // if lambda is set.
                        lambdaCostFunc = (v) => problem.Weights[seed][v] + 
                                problem.Weights[v][seed];
                    }

                    // start a route r.
                    var currentTour = solution.Add(seed, seed);
                    var currentWeight = 0f;
                    //solution[solution.Count - 1] = 0;

                    while (customers.Count > 0)
                    {
                        // calculate the cheapest visit to insert.
                        Pair location;
                        int visit;
                        var increase = currentTour.CalculateCheapestAny(problem.Weights, customers, 
                            out location, out visit, lambdaCostFunc);

                        // calculate the actual increase if an extra cost was added.
                        if (lambdaCostFunc != null)
                        { // use the seed cost; the cost to the seed customer.
                            increase -= lambdaCostFunc(visit);
                        }

                        // calculate the new weight.
                        var potentialWeight = currentWeight + increase;
                        // cram as many customers into one route as possible.
                        if (potentialWeight < max)
                        {
                            // insert the customer, it is 
                            customers.Remove(visit);
                            currentTour.InsertAfter(location.From, visit);

                            // // free some memory in the costs list.
                            // costs.Remove(result.CustomerBefore, result.CustomerAfter);

                            // update the cost of the route.
                            currentWeight = potentialWeight;
                            //solution[solution.Count - 1] = potentialWeight;

                            // improve if needed.
                            if (((problem.Weights.Length - customers.Count) % _k) == 0)
                            { // an improvement is decided.
                                // apply the inter-route improvements.
                                currentWeight = this.ImproveIntraRoute(problem.Weights, currentTour, 
                                    currentWeight);

                                // also to the inter-improvements.
                                currentTour = this.Improve(problem, solution, max, solution.Count - 1);
                            }
                        }
                        else
                        {// ok we are done!

                            // run the inter-improvements one last time.
                            this.Improve(problem, solution, max, solution.Count - 1);

                            // break the route.
                            break;
                        }
                    }
                }
            }

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
            int tour1Idx, int tour2Idx, double max)
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
                    if(improvementOperation.Apply(problem, solution, tour1Idx, tour2Idx, max))
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
                        improvementOperation.Improve(problem, solution, tour2Idx, tour1Idx, max))
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