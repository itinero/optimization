// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

namespace OsmSharp.Logistics.Solvers.VNS
{
    /// <summary>
    /// A Variable Neighbourhood Search (VNS) solver.
    /// </summary>
    public class VNSSolver<TProblem, TObjective, TSolution> : SolverBase<TProblem, TObjective, TSolution>
        where TSolution : ICloneable
    {
        private readonly SolverDelegates.StopConditionWithLevelDelegate<TProblem, TObjective, TSolution> _stopCondition;
        private readonly ISolver<TProblem, TObjective, TSolution> _generator;
        private readonly IPerturber<TProblem, TObjective, TSolution> _perturber;
        private readonly IOperator<TProblem, TObjective, TSolution> _localSearch;

        /// <summary>
        /// Creates a new VNS solver.
        /// </summary>
        /// <param name="generator">The solver that generates initial solutions.</param>
        /// <param name="perturber">The perturber that varies neighbourhood.</param>
        /// <param name="localSearch">The local search that improves solutions.</param>
        public VNSSolver(ISolver<TProblem, TObjective, TSolution> generator, IPerturber<TProblem, TObjective, TSolution> perturber,
            IOperator<TProblem, TObjective, TSolution> localSearch)
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
        public VNSSolver(ISolver<TProblem, TObjective, TSolution> generator, IPerturber<TProblem, TObjective, TSolution> perturber,
            IOperator<TProblem, TObjective, TSolution> localSearch, SolverDelegates.StopConditionWithLevelDelegate<TProblem, TObjective, TSolution> stopCondition)
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
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override TSolution Solve(TProblem problem, TObjective objective, out double fitness)
        {
            var globalBestFitness = double.MaxValue;
            var globalBest = _generator.Solve(problem, objective, out globalBestFitness);

            // report new solution.
            this.ReportIntermidiateResult(globalBest);

            var difference = 0.0;
            if (_localSearch.Apply(problem, objective, globalBest, out difference))
            { // localsearch leads to better solution, adjust the fitness.
                globalBestFitness = globalBestFitness - difference;

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
                var perturbedDifference = 0.0;
                _perturber.Apply(problem, objective, perturbedSolution, level, out perturbedDifference);

                // improve things by using a local search procedure.
                var localSearchDifference = 0.0;
                var localSearchStepDifference = 0.0;
                while (_localSearch.Apply(problem, objective, perturbedSolution, out localSearchStepDifference))
                { // keep the local search going until no more improvements are found!
                    localSearchDifference += localSearchStepDifference;
                }

                if (localSearchDifference + perturbedDifference > 0)
                { // there was an improvement, keep new solution as global.
                    globalBestFitness = globalBestFitness - perturbedDifference - localSearchDifference;
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
            fitness = globalBestFitness;
            return globalBest;
        }
    }
}