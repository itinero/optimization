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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using System.Linq;

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// A mockup of a solver that generates solution to the mockup problem of reducing a number to zero.
    /// </summary>
    class GeneratorMock : SolverBase<float, ProblemMock, ObjectiveMock, SolutionMock, float>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "MOCK_GENERATOR"; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override SolutionMock Solve(ProblemMock problem, ObjectiveMock objective, out float fitness)
        {
            var solution = new SolutionMock()
            {
                Value = RandomGeneratorExtensions.GetRandom().Generate(problem.Max)
            };
            fitness = solution.Value;
            return solution;
        }

        /// <summary>
        /// Stops execution.
        /// </summary>
        public override void Stop()
        {

        }
    }
}
