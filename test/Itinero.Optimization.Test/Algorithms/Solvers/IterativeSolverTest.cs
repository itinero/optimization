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
using NUnit.Framework;

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// Contains tests for the IterativeSolver using mockup problem(s) and solution(s).
    /// </summary>
    [TestFixture]
    public class IterativeSolverTest
    {
        /// <summary>
        /// Tests the name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create solver.
            var solver = new IterativeSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), 10);

            Assert.AreEqual("ITER_[10xMOCK_GENERATOR]", solver.Name);
        }

        /// <summary>
        /// Tests the solver without a stop condition.
        /// </summary>
        [Test]
        public void TestNoStopCondition()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create solver.
            var solver = new IterativeSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), 10);

            // run solver.
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());

            // nothing we can say about the state of the solution. 
            // IDEA: improve testability by creating a mock random generator and letting is generate best solutions and then check the result.
            // TODO: when Itinero allows Itinero.Math.Random.StaticRandomGenerator.Set(new DummyGenerator()); improve this part.
        }

        /// <summary>
        /// Tests the solver's stop function.
        /// </summary>
        [Test]
        public void TestStop()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create solver.
            var best = new SolutionMock() { Value = 1000 };
            IterativeSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float> solver = null;
            solver = new IterativeSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), 10, (i, p, o, s) =>
                {
                    if (s != null && best.Value > s.Value)
                    { // keep best solution.
                        best = s;
                    }
                    if (i > 5)
                    { // stop solver.
                        solver.Stop();
                    }
                    if (i > 6)
                    { // solver was not stopped.
                        Assert.Fail("Solver has not stopped!");
                    }
                    // ... but always return false.
                    return false;
                });

            // run solver.
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());
            Assert.AreEqual(best.Value, solution.Value);
        }

        /// <summary>
        /// Tests 
        /// </summary>
        [Test]
        public void TestStopCondition()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create solver.
            var best = new SolutionMock() { Value = 1000 };
            IterativeSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float> solver = null;
            solver = new IterativeSolver<float, ProblemMock, ObjectiveMock, SolutionMock, float>(
                new GeneratorMock(), 10, (i, p, o, s) =>
                {
                    if (s != null && best.Value > s.Value)
                    { // keep best solution.
                        best = s;
                        if (best.Value < 100)
                        {
                            return true;
                        }
                    }
                    return false;
                });

            // run solver.
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock());
            Assert.AreEqual(best.Value, solution.Value);
        }
    }
}
