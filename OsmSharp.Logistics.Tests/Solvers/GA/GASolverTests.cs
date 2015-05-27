// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Logistics.Solvers.GA;

namespace OsmSharp.Logistics.Tests.Solvers.GA
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
            var solver = new GASolver<ProblemMock, SolutionMock>(
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
            var solver = new GASolver<ProblemMock, SolutionMock>(
                new GeneratorMock(), new CrossOverMock(),
                new SelectionMock(), new LocalSearchMock());

            // execute and test result.
            var solutionFitness = 0.0;
            var solution = solver.Solve(new ProblemMock()
            {
                Max = 1000
            }, out solutionFitness);

            Assert.AreEqual(0, solution.Value);
        }
    }
}