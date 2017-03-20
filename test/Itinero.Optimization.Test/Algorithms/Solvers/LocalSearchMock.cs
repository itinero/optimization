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
    /// A mockup of a local search procedure for a very simple problem, reduce a number to zero.
    /// </summary>
    class LocalSearchMock : IOperator<float, ProblemMock, ObjectiveMock, SolutionMock, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "MOCK_LOCALSEARCH"; }
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
            var fitnessBefore = solution.Value;
            var reduction = RandomGeneratorExtensions.GetRandom().Generate(problem.Max / 50);
            if (reduction < problem.Max / 1000)
            { // mock the operator failing to find better solution.
                delta = float.MaxValue;
                return false;
            }
            if (reduction > fitnessBefore)
            { // ok reduce to zero, problem solved.
                delta = fitnessBefore;
                solution.Value = 0;
                return true;
            }
            solution.Value = solution.Value - reduction;
            delta = solution.Value - fitnessBefore; // when improvement, new is lower, delta < 0
            return delta < 0;
        }
    }
}