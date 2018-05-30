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
    /// A mutation operator that uses an opertor to refill a multilated new solution.
    /// </summary>
    public class SLCIRedoMutationOperator : IOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>
    {
        private readonly IOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> _op;

        /// <summary>
        /// Creates a new solver mutation operator.
        /// </summary>
        /// <param name="op">The operator.</param>
        public SLCIRedoMutationOperator(
            IOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float> op)
        {
            _op = op;
        }
        
        /// <inheritdoc />
        public string Name { get; } = "SLCI-REFILL";
        
        /// <inheritdoc />
        public bool Supports(NoDepotCVRPObjective objective)
        {
            return true;
        }

        /// <inheritdoc />
        public bool Apply(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, out float delta)
        {
            var before = objective.Calculate(problem, solution);
            var originalCount = solution.Count;
            var removeCount = 2;
            while (solution.Count > originalCount - removeCount &&
                   solution.Count > 0)
            {
                var t = Itinero.Optimization.Algorithms.Random.RandomGeneratorExtensions.GetRandom()
                    .Generate(solution.Count);
                solution.RemoveTour(t);
            }
            
            var newSolution = _op.Apply(problem, objective, solution, out var _);
            var after = objective.Calculate(problem, solution);
            delta = before - after;
            return true;
        }
    }
}