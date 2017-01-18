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
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.STSP.Directed
{
    /// <summary>
    /// An algorithm to calculate u-turn aware TSP solutions.
    /// </summary>
    public sealed class STSPRouter : AlgorithmBase
    {
        private readonly IDirectedWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;
        private readonly int _first;
        private readonly int? _last;
        private readonly float _max;
        private readonly float _turnPenalty;
        private readonly SolverBase<float, STSProblem, STSPObjective, Tour, STSPFitness> _solver;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public STSPRouter(IDirectedWeightMatrixAlgorithm<float> weightMatrixAlgorithm, float turnPenalty, float max, int first, int? last = null,
            SolverBase<float, STSProblem, STSPObjective, Tour, STSPFitness> solver = null)
        {
            _turnPenalty = turnPenalty;
            _max = max;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _first = first;
            _last = last;
            _solver = solver;
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

            LocationError le;
            RouterPointError rpe;
            if (_weightMatrixAlgorithm.TryGetError(_first, out le, out rpe))
            { // if the last location is set and it could not be resolved everything fails.
                if (le != null)
                {
                    this.ErrorMessage = string.Format("Could resolve first location: {0}",
                        le);
                }
                else if (rpe != null)
                {
                    this.ErrorMessage = string.Format("Could route to/from first location: {0}",
                        rpe);
                }
                else
                {
                    this.ErrorMessage = string.Format("First location was in error list.");
                }
                return;
            }

            // build problem.
            var first = _first;
            STSProblem problem = null;
            if (_last.HasValue)
            { // the last customer was set.
                if (_weightMatrixAlgorithm.TryGetError(_last.Value, out le, out rpe))
                { // if the last location is set and it could not be resolved everything fails.
                    if (le != null)
                    {
                        this.ErrorMessage = string.Format("Could resolve last location: {0}",
                            le);
                    }
                    else if (rpe != null)
                    {
                        this.ErrorMessage = string.Format("Could route to/from last location: {0}",
                            rpe);
                    }
                    else
                    {
                        this.ErrorMessage = string.Format("Last location was in error list.");
                    }
                    return;
                }

                problem = new STSProblem(_weightMatrixAlgorithm.WeightIndex(first), _weightMatrixAlgorithm.WeightIndex(_last.Value),
                    _weightMatrixAlgorithm.Weights, _turnPenalty, _max);
            }
            else
            { // the last customer was not set.
                problem = new STSProblem(_weightMatrixAlgorithm.WeightIndex(first), _weightMatrixAlgorithm.Weights, _turnPenalty,
                    _max);
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