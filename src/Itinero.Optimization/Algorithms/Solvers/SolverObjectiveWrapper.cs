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

using Itinero.Optimization.Algorithms.Solvers.Objective;
using System;

namespace Itinero.Optimization.Algorithms.Solvers
{
    /// <summary>
    /// A wrapper for a solver, replacing the objective with another objective on each call.
    /// </summary>
    public class SolverObjectiveWrapper<TWeight, TProblem, TObjective, TObjectiveUsed, TSolution, TFitness> : ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
        where TObjectiveUsed : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        private readonly ISolver<TWeight, TProblem, TObjectiveUsed, TSolution, TFitness> _solver;
        private readonly TObjectiveUsed _objective;
        private readonly Func<TProblem, TObjective, TSolution, TFitness> _calculateFitness;

        /// <summary>
        /// Creates a new solver objective wrapper.
        /// </summary>
        public SolverObjectiveWrapper(ISolver<TWeight, TProblem, TObjectiveUsed, TSolution, TFitness> solver, TObjectiveUsed objective, Func<TProblem, TObjective, TSolution, TFitness> calculateFitness)
        {
            _solver = solver;
            _solver.IntermidiateResult += _solver_IntermidiateResult;
            _objective = objective;
            _calculateFitness = calculateFitness;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return _solver.Name; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public TSolution Solve(TProblem problem, TObjective objective)
        {
            return _solver.Solve(problem, _objective);
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public TSolution Solve(TProblem problem, TObjective objective, out TFitness fitness)
        {
            var solution = _solver.Solve(problem, _objective, out fitness);
            fitness = _calculateFitness(problem, objective, solution);
            return solution;
        }

        /// <summary>
        /// Stops the executing of the solving process.
        /// </summary>
        public void Stop()
        {
            _solver.Stop();
        }

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate<TSolution> IntermidiateResult;

        /// <summary>
        /// Called when an intermediate solution is available.
        /// </summary>
        /// <param name="result"></param>
        private void _solver_IntermidiateResult(TSolution result)
        {
            if (this.IntermidiateResult != null)
            { // yes, there is a listener that cares!
                this.IntermidiateResult(result);
            }
        }
    }
}
