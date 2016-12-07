// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
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