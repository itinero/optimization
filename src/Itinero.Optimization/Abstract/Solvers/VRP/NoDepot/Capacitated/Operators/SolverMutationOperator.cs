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

using Itinero.Optimization.Algorithms.Solvers;

namespace Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Operators
{
    /// <summary>
    /// A mutation operator that uses a solver to generate a completely new solution.
    /// </summary>
    public class SolverMutationOperator : IOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>
    {
        private readonly ISolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> _solver;

        /// <summary>
        /// Creates a new solver mutation operator.
        /// </summary>
        /// <param name="solver">The solver.</param>
        public SolverMutationOperator(
            ISolver<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> solver)
        {
            _solver = solver;
        }
        
        /// <inheritdoc />
        public string Name { get; } = "SLCI";
        
        /// <inheritdoc />
        public bool Supports(NoDepotCVRPObjective objective)
        {
            return true;
        }

        /// <inheritdoc />
        public bool Apply(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, out float delta)
        {
            var before = objective.Calculate(problem, solution);
            var newSolution = _solver.Solve(problem, objective, out var _);
            solution.CopyFrom(newSolution);
            var after = objective.Calculate(problem, solution);
            delta = before - after;
            return true;
        }
    }
}