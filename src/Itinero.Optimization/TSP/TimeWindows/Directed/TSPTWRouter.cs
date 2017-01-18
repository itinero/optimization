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
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.TimeWindows.Directed
{
    /// <summary>
    /// An algorithm to calculate TSP-TW solutions with u-turn prevention.
    /// </summary>
    public sealed class TSPTWRouter : AlgorithmBase
    {
        private readonly TimeWindow[] _windows;
        private readonly int _first;
        private readonly int? _last;
        private readonly float _turnPenalty;
        private readonly Itinero.Optimization.Algorithms.Solvers.ISolver<float, TSPTWProblem, TSPTWObjective, Tour, float> _solver;
        private readonly IDirectedWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public TSPTWRouter(IDirectedWeightMatrixAlgorithm<float> weightMatrixAlgorithm, TimeWindow[] windows, float turnPenalty, int first, int? last = null,
            SolverBase<float, TSPTWProblem, TSPTWObjective, Itinero.Optimization.Tours.Tour, float> solver = null)
        {
            _turnPenalty = turnPenalty;
            _windows = windows;
            _first = first;
            _last = last;
            _solver = solver;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }

        private Tour _tour = null;
        
        /// <summary>
        /// Executes the actual algorithm.
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

            // map the windows if needed.
            var windows = _windows;
            if (_weightMatrixAlgorithm.Errors.Count > 0 ||
                _weightMatrixAlgorithm.MassResolver.Errors.Count > 0)
            {
                var newWindows = new List<TimeWindow>();
                for (var i = 0; i < _windows.Length; i++)
                {
                    if (_weightMatrixAlgorithm.MassResolver.Errors.ContainsKey(i))
                    {
                        continue;
                    }
                    var resolvedIndex = _weightMatrixAlgorithm.MassResolver.IndexOf(i);
                    if (_weightMatrixAlgorithm.Errors.ContainsKey(resolvedIndex))
                    {
                        continue;
                    }
                    newWindows.Add(_windows[i]);
                }
                windows = newWindows.ToArray();
            }

            // build problem.
            var first = _first;
            TSPTWProblem problem = null;
            if (_last.HasValue)
            { // the last customer was set.
                if (_weightMatrixAlgorithm.MassResolver.Errors.TryGetValue(_last.Value, out error))
                { // if the last location is set and it could not be resolved everything fails.
                    this.ErrorMessage = string.Format("Could resolve last location: {0}",
                        error);
                    return;
                }

                problem = new TSPTWProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.IndexOf(_last.Value),
                    _weightMatrixAlgorithm.Weights, windows, _turnPenalty);
            }
            else
            { // the last customer was not set.
                problem = new TSPTWProblem(_weightMatrixAlgorithm.IndexOf(first), _weightMatrixAlgorithm.Weights, windows, _turnPenalty);
            }

            // execute the solver.
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
        public IDirectedWeightMatrixAlgorithm<float> WeightMatrix
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