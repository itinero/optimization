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
using System.Collections.Generic;
using System.Threading;

namespace Itinero.Optimization.Sequences.Directed
{
    /// <summary>
    /// An algorithm to calculate u-turn aware sequences solutions.
    /// </summary>
    public sealed class SequenceDirectedRouter : AlgorithmBase
    {
        private readonly IDirectedWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;
        private readonly Tour _sequence;
        private readonly float _turnPenalty;
        private readonly SolverBase<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float> _solver;

        /// <summary>
        /// Creates a new router.
        /// </summary>
        public SequenceDirectedRouter(IDirectedWeightMatrixAlgorithm<float> weightMatrixAlgorithm, float turnPenalty, Tour sequence, 
            SolverBase<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float> solver = null)
        {
            _turnPenalty = turnPenalty;
            _sequence = sequence;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _solver = solver;
        }

        private Tour _tour = null;

        /// <summary>
        /// Executes the actual algorithm.
        /// </summary>
        protected override void DoRun(CancellationToken token)
        {
            // calculate weight matrix.
            if (!_weightMatrixAlgorithm.HasRun)
            { // only run if it has not been run yet.
                _weightMatrixAlgorithm.Run(token);
            }
            if (!_weightMatrixAlgorithm.HasSucceeded)
            { // algorithm has not succeeded.
                this.ErrorMessage = string.Format("Could not calculate weight matrix: {0}",
                    _weightMatrixAlgorithm.ErrorMessage);
                return;
            }

            // check if an entry in the sequence was not found.
            this.ErrorMessage = string.Empty;
            var toRemove = new HashSet<int>();
            foreach (var c in _sequence)
            {
                LocationError locationError;
                RouterPointError routerPointError;
                if (_weightMatrixAlgorithm.TryGetError(c, out locationError, out routerPointError))
                {
                    if (locationError != null)
                    {
                        this.ErrorMessage += string.Format("The location at index {0} is in error: {1}", c,
                            locationError.ToInvariantString());
                    }
                    else if (routerPointError != null)
                    {
                        this.ErrorMessage += string.Format("The location at index {0} is in error: {1}", c,
                            routerPointError.ToInvariantString());
                    }
                    else
                    {
                        this.ErrorMessage += string.Format("The location at index {0} is in error.", c);
                    }
                    
                    toRemove.Add(c);
                }
            }

            // remove all invalids.
            var validRoute = _sequence.Clone() as Tour;
            foreach (var invalid in toRemove)
            {
                validRoute.Remove(invalid);
            }

            // build problem.
            var problem = new SequenceDirectedProblem(validRoute, _weightMatrixAlgorithm.Weights, _turnPenalty);

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