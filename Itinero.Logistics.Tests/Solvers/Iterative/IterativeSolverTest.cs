// Itinero - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using Itinero.Logistics.Solvers.Iterative;
using Itinero.Logistics.Algorithms;

namespace Itinero.Logistics.Tests.Solvers.Iterative
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
            var solver = new IterativeSolver<ProblemMock, ObjectiveMock, SolutionMock>(
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
            var solver = new IterativeSolver<ProblemMock, ObjectiveMock, SolutionMock>(
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
            IterativeSolver<ProblemMock, ObjectiveMock, SolutionMock> solver = null;
            solver = new IterativeSolver<ProblemMock, ObjectiveMock, SolutionMock>(
                new GeneratorMock(), 10, (i, p, o, s) =>
            {
                if(s != null && best.Value > s.Value)
                { // keep best solution.
                    best = s;
                }
                if(i > 5)
                { // stop solver.
                    solver.Stop();
                }
                if(i > 6)
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
            IterativeSolver<ProblemMock, ObjectiveMock, SolutionMock> solver = null;
            solver = new IterativeSolver<ProblemMock, ObjectiveMock, SolutionMock>(
                new GeneratorMock(), 10, (i, p, o, s) =>
                {
                    if (s != null && best.Value > s.Value)
                    { // keep best solution.
                        best = s;
                        if(best.Value < 100)
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