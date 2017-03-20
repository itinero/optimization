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
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Algorithms.Solvers.GA
{
    /// <summary>
    /// A mockup of a selection operator.
    /// </summary>
    class SelectionMock : ISelectionOperator<ProblemMock, SolutionMock, ObjectiveMock, float>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return "MOCK_SELECTION"; }
        }

        /// <summary>
        /// Selects a new solution for reproduction.
        /// </summary>
        /// <returns></returns>
        public int Select(ProblemMock problem, ObjectiveMock objective, Individual<SolutionMock, float>[] population, ISet<int> exclude)
        {
            // try two and select the best one.
            var selected = -1;
            do
            {
                var individual1 = RandomGeneratorExtensions.GetRandom().Generate(population.Length);
                var individual2 = RandomGeneratorExtensions.GetRandom().Generate(population.Length - 1);
                if (individual1 <= individual2)
                { // make sure they are different.
                    individual2++;
                }

                selected = individual2;
                if (population[individual1].Fitness < population[individual2].Fitness)
                {
                    selected = individual1;
                }
            } while (exclude.Contains(selected));
            return selected;
        }
    }
}