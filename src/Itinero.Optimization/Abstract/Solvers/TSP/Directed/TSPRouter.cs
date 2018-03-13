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

using Itinero.Algorithms;
using Itinero.Algorithms.Matrices;
using Itinero.Algorithms.Search;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Abstract.Tours;

namespace Itinero.Optimization.Abstract.Solvers.TSP.Directed
{
    /// <summary>
    /// An algorithm to calculate u-turn aware TSP solutions.
    /// </summary>
    public sealed class TSPRouter : AlgorithmBase
    {
        private readonly int _first;
        private readonly int? _last;
        private readonly float _turnPenalty;
        private readonly Itinero.Optimization.Algorithms.Solvers.ISolver<float, TSProblem, TSPObjective, Tour, float> _solver;
        private readonly IDirectedWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public TSPRouter(IDirectedWeightMatrixAlgorithm<float> weightMatrixAlgorithm, float turnPenalty, int first, int? last = null,
            SolverBase<float, TSProblem, TSPObjective, Itinero.Optimization.Abstract.Tours.Tour, float> solver = null)
        {
            _turnPenalty = turnPenalty;
            _first = first;
            _last = last;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
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
            TSProblem problem = null;
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

                problem = new TSProblem(_weightMatrixAlgorithm.WeightIndex(first), _weightMatrixAlgorithm.WeightIndex(_last.Value),
                    _weightMatrixAlgorithm.Weights, _turnPenalty);
            }
            else
            { // the last customer was not set.
                problem = new TSProblem(_weightMatrixAlgorithm.WeightIndex(first), _weightMatrixAlgorithm.Weights, _turnPenalty);
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