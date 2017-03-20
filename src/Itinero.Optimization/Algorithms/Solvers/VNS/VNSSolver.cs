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

using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Algorithms.Solvers.VNS
{
    /// <summary>
    /// A Variable Neighbourhood Search (VNS) solver.
    /// </summary>
    public class VNSSolver<TWeight, TProblem, TObjective, TSolution, TFitness> : SolverBase<TWeight, TProblem, TObjective, TSolution, TFitness>,
        IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TSolution : ISolution
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
        where TWeight : struct
    {
        private static Itinero.Optimization.Logging.Logger _log = new Logging.Logger("VNSSolver");
        private readonly SolverDelegates.StopConditionWithLevelDelegate<TProblem, TObjective, TSolution> _stopCondition;
        private readonly ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> _generator;
        private readonly IPerturber<TWeight, TProblem, TObjective, TSolution, TFitness> _perturber;
        private readonly IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> _localSearch;

        /// <summary>
        /// Creates a new VNS solver.
        /// </summary>
        /// <param name="generator">The solver that generates initial solutions.</param>
        /// <param name="perturber">The perturber that varies neighbourhood.</param>
        /// <param name="localSearch">The local search that improves solutions.</param>
        public VNSSolver(ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> generator, IPerturber<TWeight, TProblem, TObjective, TSolution, TFitness> perturber,
            IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> localSearch)
        {
            _generator = generator;
            _perturber = perturber;
            _localSearch = localSearch;
        }

        /// <summary>
        /// Creates a new VNS solver.
        /// </summary>
        /// <param name="generator">The solver that generates initial solutions.</param>
        /// <param name="perturber">The perturber that varies neighbourhood.</param>
        /// <param name="localSearch">The local search that improves solutions.</param>
        /// <param name="stopCondition">The stop condition to control the number of iterations.</param>
        public VNSSolver(ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> generator, IPerturber<TWeight, TProblem, TObjective, TSolution, TFitness> perturber,
            IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> localSearch, SolverDelegates.StopConditionWithLevelDelegate<TProblem, TObjective, TSolution> stopCondition)
        {
            _generator = generator;
            _perturber = perturber;
            _localSearch = localSearch;
            _stopCondition = stopCondition;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return string.Format("VNS_[{0}_{1}_{2}]",
              _generator.Name, _perturber.Name, _localSearch.Name);
            }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public virtual bool Supports(TObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override TSolution Solve(TProblem problem, TObjective objective, out TFitness fitness)
        {
            var zero = objective.Zero;

            _log.Log(Logging.TraceEventType.Information, "Started generating initial solution...");

            TFitness globalBestFitness;
            var globalBest = _generator.Solve(problem, objective, out globalBestFitness);

            _log.Log(Logging.TraceEventType.Information, "Initial solution generated: {0}.", globalBestFitness);

            // report new solution.
            this.ReportIntermidiateResult(globalBest);

            var difference = objective.Zero;
            if (_localSearch.Apply(problem, objective, globalBest, out difference))
            { // localsearch leads to better solution, adjust the fitness.
                globalBestFitness = objective.Subtract(problem, globalBestFitness, difference);

                _log.Log(Logging.TraceEventType.Information, "Improvement found by local search: {0}.", globalBestFitness);

                // report new solution.
                this.ReportIntermidiateResult(globalBest);
            }

            var i = 0;
            var level = 1;
            while (!this.IsStopped &&
                (_stopCondition == null || !_stopCondition.Invoke(i, level, problem, objective, globalBest)))
            { // keep running until stop condition is true or this solver is stopped.
                // shake things up a bit, or in other word change neighbourhood.
                var perturbedSolution = (TSolution)globalBest.Clone();
                var perturbedDifference = objective.Zero;
                _perturber.Apply(problem, objective, perturbedSolution, level, out perturbedDifference);

                // improve things by using a local search procedure.
                var localSearchDifference = objective.Zero;
                _localSearch.ApplyUntil(problem, objective, perturbedSolution, out localSearchDifference);

                // calculate new fitness and compare.
                TFitness newFitness = default(TFitness);
                if (!objective.IsNonContinuous)
                { // solution fitness can be updated by adding the differences.
                    newFitness = objective.Add(problem, globalBestFitness, perturbedDifference);
                    newFitness = objective.Add(problem, newFitness, localSearchDifference);
                }
                else
                { // solution fitness needs to updated every time.
                    newFitness = objective.Calculate(problem, perturbedSolution);
                }

                if (objective.IsBetterThan(problem, newFitness, globalBestFitness))
                { // there was an improvement, keep new solution as global.
                    globalBestFitness = newFitness;
                    globalBest = perturbedSolution;
                    level = 1; // reset level.

                    _log.Log(Logging.TraceEventType.Information, "Improvement found by perturber and local search: {0}.", globalBestFitness);

                    // report new solution.
                    this.ReportIntermidiateResult(globalBest);
                }
                else
                {
                    level = level + 1;
                }
            }

            _log.Log(Logging.TraceEventType.Information, "Stop condition reached, best: {0}.", globalBestFitness);

            fitness = globalBestFitness;
            return globalBest;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
        {
            var zero = objective.Zero;
            var globalBestFitness = objective.Calculate(problem, solution);

            delta = objective.Zero;
            var globalBest = (TSolution)solution.Clone();

            var i = 0;
            var level = 1;
            while (!this.IsStopped &&
                (_stopCondition == null || !_stopCondition.Invoke(i, level, problem, objective, globalBest)))
            { // keep running until stop condition is true or this solver is stopped.
                // shake things up a bit, or in other word change neighbourhood.
                var perturbedSolution = (TSolution)globalBest.Clone();
                var perturbedDifference = objective.Zero;
                _perturber.Apply(problem, objective, perturbedSolution, level, out perturbedDifference);

                // improve things by using a local search procedure.
                var localSearchDifference = objective.Zero;
                _localSearch.ApplyUntil(problem, objective, perturbedSolution, out localSearchDifference);

                // calculate new fitness and compare.
                var newFitness = objective.Subtract(problem, globalBestFitness, perturbedDifference);
                newFitness = objective.Subtract(problem, newFitness, localSearchDifference);
                if (objective.IsBetterThan(problem, newFitness, globalBestFitness))
                { // there was an improvement, keep new solution as global.
                    delta = objective.Add(problem, delta, perturbedDifference, localSearchDifference);
                    globalBest = perturbedSolution;
                    level = 1; // reset level.

                    // report new solution.
                    this.ReportIntermidiateResult(globalBest);
                }
                else
                {
                    level = level + 1;
                }
            }

            if (!objective.IsZero(problem, delta))
            {
                solution.CopyFrom(globalBest);
                return true;
            }
            return false;
        }
    }
}
