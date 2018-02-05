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
using Itinero.Optimization.Routing;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// A router that calculates real-world solutions to the No-depot CVRP.
    /// </summary>
    public class NoDepotCVRPRouter : AlgorithmBase
    {
        private readonly IWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;
        private readonly float _max;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        public NoDepotCVRPRouter(IWeightMatrixAlgorithm<float> weightMatrixAlgorithm, float max,
            ISolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> solver = null)
        {
            _max = max;
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
            _solver = solver;
        }

        private NoDepotCVRPSolution _solution = null;
        private ISolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> _solver;

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

            // build problem.
            var problem = new NoDepotCVRProblem()
            {
                Max = _max,
                Weights = _weightMatrixAlgorithm.Weights
            };

            // solve.
            if (_solver == null)
            {
                _solution = problem.Solve((p, tour1, tour2) => 
                {
                    return _weightMatrixAlgorithm.ToursOverlap(tour1, tour2);
                });
            }
            else
            {
                _solution = problem.Solve(_solver);
            }

            this.HasSucceeded = true;
        }

        /// <summary>
        /// Gets the weight matrix.
        /// </summary>
        public IWeightMatrixAlgorithm<float> WeightMatrix
        {
            get
            {
                return _weightMatrixAlgorithm;
            }
        }

        /// <summary>
        /// Gets the raw solution.
        /// </summary>
        public NoDepotCVRPSolution RawSolution
        {
            get
            {
                return _solution;
            }
        }        
    }
}