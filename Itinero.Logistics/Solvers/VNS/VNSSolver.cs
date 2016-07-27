// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Logistics.Fitness;
using Itinero.Logistics.Objective;

namespace Itinero.Logistics.Solvers.VNS
{
    /// <summary>
    /// A Variable Neighbourhood Search (VNS) solver.
    /// </summary>
    public class VNSSolver<TWeight, TProblem, TObjective, TSolution, TFitness> : SolverBase<TWeight, TProblem, TObjective, TSolution, TFitness>, 
        IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TSolution : ISolution
        where TObjective : ObjectiveBase<TFitness>
        where TWeight : struct
    {
        private static Itinero.Logistics.Logging.Logger _log = new Logging.Logger("VNSSolver");
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
            get { return string.Format("VNS_[{0}_{1}_{2}]", 
                _generator.Name, _perturber.Name, _localSearch.Name); }
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
            var fitnessHandler = objective.FitnessHandler;

            _log.Log(Logging.TraceEventType.Information, "Started generating initial solution...");

            TFitness globalBestFitness;
            var globalBest = _generator.Solve(problem, objective, out globalBestFitness);

            _log.Log(Logging.TraceEventType.Information, "Initial solution generated: {0}.", globalBestFitness);

            // report new solution.
            this.ReportIntermidiateResult(globalBest);

            var difference = fitnessHandler.Zero;
            if (_localSearch.Apply(problem, objective, globalBest, out difference))
            { // localsearch leads to better solution, adjust the fitness.
                globalBestFitness = fitnessHandler.Subtract(globalBestFitness, difference);

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
                var perturbedDifference = fitnessHandler.Zero;
                _perturber.Apply(problem, objective, perturbedSolution, level, out perturbedDifference);

                // improve things by using a local search procedure.
                var localSearchDifference = fitnessHandler.Zero;
                _localSearch.ApplyUntil(problem, objective, perturbedSolution, out localSearchDifference);

                var diff = fitnessHandler.Add(localSearchDifference, perturbedDifference);
                if (!fitnessHandler.IsZero(diff))
                { // there was an improvement, keep new solution as global.
                    globalBestFitness = fitnessHandler.Subtract(globalBestFitness, perturbedDifference);
                    globalBestFitness = fitnessHandler.Subtract(globalBestFitness, localSearchDifference);
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
            var fitnessHandler = objective.FitnessHandler;

            delta = fitnessHandler.Zero;
            var globalBest = (TSolution)solution.Clone();

            var i = 0;
            var level = 1;
            while (!this.IsStopped &&
                (_stopCondition == null || !_stopCondition.Invoke(i, level, problem, objective, globalBest)))
            { // keep running until stop condition is true or this solver is stopped.
                // shake things up a bit, or in other word change neighbourhood.
                var perturbedSolution = (TSolution)globalBest.Clone();
                var perturbedDifference = fitnessHandler.Zero;
                _perturber.Apply(problem, objective, perturbedSolution, level, out perturbedDifference);

                // improve things by using a local search procedure.
                var localSearchDifference = fitnessHandler.Zero;
                _localSearch.ApplyUntil(problem, objective, perturbedSolution, out localSearchDifference);

                var diff = fitnessHandler.Add(localSearchDifference, perturbedDifference);
                if (!fitnessHandler.IsZero(diff))
                { // there was an improvement, keep new solution as global.
                    delta = fitnessHandler.Add(delta, perturbedDifference, localSearchDifference);
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

            if (!fitnessHandler.IsZero(delta))
            {
                solution.CopyFrom(globalBest);
                return true;
            }
            return false;
        }
    }
}