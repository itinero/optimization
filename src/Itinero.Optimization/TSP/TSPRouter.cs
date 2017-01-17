// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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

using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Routing.Matrices;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.TSP
{
    /// <summary>
    /// An algorithm to calculate TSP solutions.
    /// </summary>
    public sealed class TSPRouter : AlgorithmBase
    {
        private readonly IWeightMatrixAlgorithm _weightMatrixAlgorithm;
        private readonly int _first;
        private readonly int? _last;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public TSPRouter(IWeightMatrixAlgorithm weightMatrixAlgorithm, int first = 0, int? last = null, 
            SolverBase<float, TSProblem, TSPObjective, Itinero.Optimization.Tours.Tour, float> solver = null)
        {
            _first = first;
            _last = last;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _solver = solver;
        }

        private Tour _tour = null;
        private SolverBase<float, TSProblem, TSPObjective, Itinero.Optimization.Tours.Tour, float> _solver;

        /// <summary>
        /// Excutes the actual algorithm.
        /// </summary>
        protected override void DoRun()
        {
            // calculate weight matrix.
            if (!_weightMatrixAlgorithm.HasRun)
            { // only run if it has not been run yet.
                _weightMatrixAlgorithm.Run();
            }
            if (!_weightMatrixAlgorithm.HasSucceeded)
            { // algorithm has not succeeded.
                this.ErrorMessage = string.Format("Could not calculate weight matrix: {0}",
                    _weightMatrixAlgorithm.ErrorMessage);
                return;
            }

            LocationError error;
            if (_weightMatrixAlgorithm.MassResolver.Errors.TryGetValue(_first, out error))
            { // if the first location could not be resolved everything fails.
                this.ErrorMessage = string.Format("Could resolve first location: {0}",
                    error);
                return;
            }

            // build problem.
            var first = _first;
            TSProblem problem = null;
            if (_last.HasValue)
            { // the last customer was set.
                if (_weightMatrixAlgorithm.MassResolver.Errors.TryGetValue(_last.Value, out error))
                { // if the last location is set and it could not be resolved everything fails.
                    this.ErrorMessage = string.Format("Could resolve last location: {0}",
                        error);
                    return;
                }

                problem = new TSProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.IndexOf(_last.Value),
                    _weightMatrixAlgorithm.Weights);
            }
            else
            { // the last customer was not set.
                problem = new TSProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.Weights);
            }

            // solve.
            if (_solver == null)
            {
                _tour = problem.Solve();
            }
            else
            {
                _tour = problem.Solve(_solver);
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the weight matrix.
        /// </summary>
        public IWeightMatrixAlgorithm WeightMatrix
        {
            get
            {
                return _weightMatrixAlgorithm;
            }
        }

        /// <summary>
        /// Gets the tour.
        /// </summary>
        public Tour Tour
        {
            get
            {
                return _tour;
            }
        }
    }
}