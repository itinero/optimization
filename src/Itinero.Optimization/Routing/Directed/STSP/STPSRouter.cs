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
using Itinero.Optimization.STSP;

namespace Itinero.Optimization.Routing.Directed.STSP
{
    /// <summary>
    /// An algorithm to calculate STSP solutions.
    /// </summary>
    public class STSPRouter : AlgorithmBase
    {
        private readonly DirectedWeightMatrixAlgorithm _weightMatrixAlgorithm;
        private readonly int _first;
        private readonly int? _last;
        private readonly float _max;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public STSPRouter(DirectedWeightMatrixAlgorithm weightMatrixAlgorithm, float max, int first = 0, int? last = null,
            SolverBase<float, STSProblem, STSPObjective, Itinero.Optimization.Tours.Tour, STSPFitness> solver = null)
        {
            _first = first;
            _last = last;
            _max = max;

            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _solver = solver;
        }

        private Itinero.Optimization.Tours.Tour _route = null;
        private SolverBase<float, STSProblem, STSPObjective, Itinero.Optimization.Tours.Tour, STSPFitness> _solver;

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
            STSProblem problem = null;
            if (_last.HasValue)
            { // the last customer was set.
                if (_weightMatrixAlgorithm.MassResolver.Errors.TryGetValue(_last.Value, out error))
                { // if the last location is set and it could not be resolved everything fails.
                    this.ErrorMessage = string.Format("Could resolve last location: {0}",
                        error);
                    return;
                }

                problem = new STSProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.IndexOf(_last.Value),
                    _weightMatrixAlgorithm.Weights, _max);
            }
            else
            { // the last customer was not set.
                problem = new STSProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.Weights, _max);
            }

            // solve.
            if (_solver == null)
            {
                _route = problem.Solve();
            }
            else
            {
                _route = problem.Solve(_solver);
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the weight matrix.
        /// </summary>
        public DirectedWeightMatrixAlgorithm WeightMatrix
        {
            get
            {
                return _weightMatrixAlgorithm;
            }
        }

        /// <summary>
        /// Gets the raw route representing the order of the locations.
        /// </summary>
        public Itinero.Optimization.Tours.ITour RawRoute
        {
            get
            {
                return _route;
            }
        }
    }
}