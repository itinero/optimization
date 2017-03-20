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
using Itinero.Optimization.Algorithms.Solvers.GA;

namespace Itinero.Optimization.Test.Algorithms.Solvers.GA
{
    /// <summary>
    /// A mockup of a crossover operator.
    /// </summary>
    class CrossOverMock : ICrossOverOperator<float, ProblemMock, ObjectiveMock, SolutionMock, float>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return "MOCK_CROSSOVER"; }
        }

        /// <summary>
        /// Applies this operator using the given solutions and produces a new solution.
        /// </summary>
        /// <returns></returns>
        public SolutionMock Apply(ProblemMock problem, ObjectiveMock objective, SolutionMock solution1, SolutionMock solution2, out float fitness)
        {
            if (solution1.Value < solution2.Value)
            {
                fitness = solution1.Value - RandomGeneratorExtensions.GetRandom().Generate(
                    solution2.Value - solution1.Value);
            }
            else
            {
                fitness = solution2.Value - RandomGeneratorExtensions.GetRandom().Generate(
                    solution1.Value - solution2.Value);
            }
            return new SolutionMock()
            {
                Value = fitness
            };
        }
    }
}