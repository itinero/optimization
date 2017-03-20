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

using Itinero.Optimization.Algorithms.Solvers.GA;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.Solvers.GA
{
    /// <summary>
    /// Contains tests for the GASolver.
    /// </summary>
    [TestFixture]
    public class GASolverTests
    {
        /// <summary>
        /// Tests the name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create the solver.
            var solver = new GASolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(new ObjectiveMock(),
                new GeneratorMock(), new CrossOverMock(),
                new SelectionMock(), new LocalSearchMock());

            Assert.AreEqual("GA_[MOCK_GENERATOR_MOCK_LOCALSEARCH_MOCK_CROSSOVER_MOCK_SELECTION]", solver.Name);
        }

        /// <summary>
        /// Tests the GA solver.
        /// </summary>
        [Test]
        public void Test()
        {
            // create the solver.
            var solver = new GASolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(new ObjectiveMock(),
                new GeneratorMock(), new CrossOverMock(),
                new SelectionMock(), new LocalSearchMock(),
                new GASettings()
                {
                    MaxGenerations = 1000,
                    PopulationSize = 400,
                    StagnationCount = 200,
                    CrossOverPercentage = 10,
                    ElitismPercentage = 5,
                    MutationPercentage = 10
                });

            // execute and test result.
            var solutionFitness = 0.0f;
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 100
            }, new ObjectiveMock(), out solutionFitness);

            Assert.AreEqual(0, solution.Value, 1);
        }
    }
}