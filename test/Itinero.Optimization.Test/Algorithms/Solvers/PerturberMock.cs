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

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// A mockup of a perturber procedure for a very simple problem, reduce a number to zero.
    /// </summary>
    class PerturberMock : IPerturber<float, ProblemMock, ObjectiveMock, SolutionMock, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "MOCK_PERTURBER"; }
        }

        /// <summary>
        /// Returns true if the given object is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(ObjectiveMock objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ProblemMock problem, ObjectiveMock objective, SolutionMock solution, out float delta)
        {
            return this.Apply(problem, objective, solution, 1, out delta);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ProblemMock problem, ObjectiveMock objective, SolutionMock solution, int level, out float delta)
        {
            var before = solution.Value;
            var after = RandomGeneratorExtensions.GetRandom().Generate(problem.Max);
            solution.Value = after;
            delta = before - after; // when improvement, after is lower, delta > 0
            return delta > 0;
        }
    }
}
