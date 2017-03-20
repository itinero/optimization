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
using Itinero.Optimization.Algorithms.Solvers.VNS;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.Solvers.VNS
{
    /// <summary>
    /// Contains tests for the VNSSolver using mockup problem(s) and solution(s).
    /// </summary>
    [TestFixture]
    public class VNSSolverTests
    {
        /// <summary>
        /// Tests the name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create solver.
            var solver = new VNSSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), new PerturberMock(), new LocalSearchMock());

            Assert.AreEqual("VNS_[MOCK_GENERATOR_MOCK_PERTURBER_MOCK_LOCALSEARCH]", solver.Name);
        }

        /// <summary>
        /// Tests the VNSSolver without a stop condition but with a call to stop after a while.
        /// </summary>
        [Test]
        public void TestStop()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create solver.
            var solver = new VNSSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), new PerturberMock(), new LocalSearchMock());

            // run solver but stop when after a few reported improvements.
            var intermediateCount = 0;
            var best = new SolutionMock() { Value = 1000 };
            solver.IntermidiateResult += (s) =>
            {
                intermediateCount++;
                if (intermediateCount > 5)
                { // this should never ever happen!
                    Assert.Fail("Solver has not stopped!");
                }
                if (intermediateCount == 5)
                { // stop solver.
                    solver.Stop();
                    best = s;
                }
            };
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());

            Assert.AreEqual(best.Value, solution.Value);
        }

        /// <summary>
        /// Tests the VNSSolver with a stop condition.
        /// </summary>
        [Test]
        public void TestStopCondition()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create solver.
            var solver = new VNSSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), new PerturberMock(), new LocalSearchMock(), (i, l, p, o, s) =>
                {
                    return s.Value < 100;
                });

            // run solver but stop when after a few reported improvements.
            var best = new SolutionMock() { Value = 1000 };
            solver.IntermidiateResult += (s) =>
            {
                best = s;
            };
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());

            Assert.AreEqual(best.Value, solution.Value);
            Assert.IsTrue(solution.Value < 100);
        }
    }
}
