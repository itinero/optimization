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

namespace Itinero.Optimization.Sequence.Directed
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

            // check if an entry in the sequence was not found.
            foreach(var c in _sequence)
            {
                LocationError locationError;
                RouterPointError routerPointError;
                if (_weightMatrixAlgorithm.TryGetError(c, out locationError, out routerPointError))
                {
                    if (locationError != null)
                    {
                        this.ErrorMessage = string.Format("The location at index {0} is in error: {1}", c,
                            locationError.ToInvariantString());
                    }
                    else if (routerPointError != null)
                    {
                        this.ErrorMessage = string.Format("The location at index {0} is in error: {1}", c,
                            routerPointError.ToInvariantString());
                    }
                    else
                    {
                        this.ErrorMessage = string.Format("The location at index {0} is in error.", c);
                    }
                    this.HasSucceeded = false;
                    return;
                }
            }

            // build problem.
            var problem = new SequenceDirectedProblem(_sequence, _weightMatrixAlgorithm.Weights, _turnPenalty);

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