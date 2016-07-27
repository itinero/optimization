// Itinero.Logistics - Route optimization for .NET
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
using Itinero.Logistics.Solvers.GA;

namespace Itinero.Logistics.Tests.Solvers.GA
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
                new SelectionMock(), new LocalSearchMock());

            // execute and test result.
            var solutionFitness = 0.0f;
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, new ObjectiveMock(), out solutionFitness);

            Assert.AreEqual(0, solution.Value);
        }
    }
}