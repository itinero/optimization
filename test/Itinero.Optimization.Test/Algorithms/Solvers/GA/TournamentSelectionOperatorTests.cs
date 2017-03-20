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
using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.Solvers.GA
{
    /// <summary>
    /// Tests the tournament selection operator.
    /// </summary>
    [TestFixture]
    public class TournamentSelectionOperatorTests
    {
        /// <summary>
        /// Setup of these tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            RandomGeneratorExtensions.Reset();
            RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new NotSoRandomGenerator(
                    new float[] { 0.6f, 0.2f, 0.8f }, new int[] { 0, 2, 3 });
            };
        }

        /// <summary>
        /// Basic selection test.
        /// </summary>
        [Test]
        public void Test1()
        {

            // create population and selector.
            var population = new Individual<SolutionMock, float>[] {
                new Individual<SolutionMock, float>() { Fitness = 10, Solution = new SolutionMock(10) },
                new Individual<SolutionMock, float>() { Fitness = 1, Solution = new SolutionMock(1) },
                new Individual<SolutionMock, float>() { Fitness = 3, Solution = new SolutionMock(3) },
                new Individual<SolutionMock, float>() { Fitness = 4, Solution = new SolutionMock(4) } };
            var selector = new TournamentSelectionOperator<ProblemMock, SolutionMock, ObjectiveMock, float>(50, 0.5);

            var objective = new ObjectiveMock();
            Assert.AreEqual(2, selector.Select(new ProblemMock(), objective, population, null));
            Assert.AreEqual(-1, selector.Select(new ProblemMock(), objective, population, null));
            Assert.AreEqual(3, selector.Select(new ProblemMock(), objective, population, null));
            Assert.AreEqual(-1, selector.Select(new ProblemMock(), objective, population, null));
            Assert.AreEqual(0, selector.Select(new ProblemMock(), objective, population, null));
            Assert.AreEqual(-1, selector.Select(new ProblemMock(), objective, population, null));
        }

        /// <summary>
        /// Teardown of these tests.
        /// </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            RandomGeneratorExtensions.Reset();
        }
    }
}