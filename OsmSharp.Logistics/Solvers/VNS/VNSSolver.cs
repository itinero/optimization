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
    /// <typeparam name="TSolution"></typeparam>
    /// <typeparam name="TProblem"></typeparam>
    public class VNSSolver<TProblem, TSolution> : SolverBase<TProblem, TSolution>
        where TSolution : ICloneable
    {
        /// <summary>
        /// Holds the stop condition.
        /// </summary>
        private SolverDelegates.StopConditionDelegate<TProblem, TSolution> _stopCondition;

        /// <summary>
        /// Holds the generator.
        /// </summary>
        private ISolver<TProblem, TSolution> _generator;

        /// <summary>
        /// Holds the perturber.
        /// </summary>
        private IPerturber<TProblem, TSolution> _perturber;

        /// <summary>
        /// Holds the local search operator.
        /// </summary>
        private IOperator<TProblem, TSolution> _localSearch;

        /// <summary>
        /// Creates a new VNS solver.
        /// </summary>
        /// <param name="generator">The solver that generates initial solutions.</param>
        /// <param name="perturber">The perturber that varies neighbourhood.</param>
        /// <param name="localSearch">The local search that improves solutions.</param>
        public VNSSolver(ISolver<TProblem, TSolution> generator, IPerturber<TProblem, TSolution> perturber,
            IOperator<TProblem, TSolution> localSearch)
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
        public VNSSolver(ISolver<TProblem, TSolution> generator, IPerturber<TProblem, TSolution> perturber,
            IOperator<TProblem, TSolution> localSearch, SolverDelegates.StopConditionDelegate<TProblem, TSolution> stopCondition)
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
        public override TSolution Solve(TProblem problem, out double fitness)
        {
            var globalBestFitness = double.MaxValue;
            var globalBest = _generator.Solve(problem, out globalBestFitness);

            // report new solution.
            this.ReportIntermidiateResult(globalBest);

            var difference = 0.0;
            if (_localSearch.Apply(problem, globalBest, out difference))
            { // localsearch leads to better solution, adjust the fitness.
                globalBestFitness = globalBestFitness - difference;

                // report new solution.
                this.ReportIntermidiateResult(globalBest);
            }

            var i = 0;
            var level = 1;
            while (!this.IsStopped && 
                (_stopCondition == null || !_stopCondition.Invoke(i, problem, globalBest)))
            { // keep running until stop condition is true or this solver is stopped.
                // shake things up a bit, or in other word change neighbourhood.
                var perturbedSolution = (TSolution)globalBest.Clone();
                var perturbedDifference = 0.0;
                _perturber.Apply(problem, perturbedSolution, level, out perturbedDifference);

                // improve things by using a local search procedure.
                var localSearchDifference = 0.0;
                if (_localSearch.Apply(problem, perturbedSolution, out localSearchDifference))
                { // local search found improvements.
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
            }
            fitness = globalBestFitness;
            return globalBest;
        }
    }
}