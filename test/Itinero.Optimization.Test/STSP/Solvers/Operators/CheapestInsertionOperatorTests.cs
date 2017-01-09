// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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
using Itinero.Optimization.STSP;
using Itinero.Optimization.STSP.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.STSP.Solvers.Operators
{
    /// <summary>
    /// Cheapest insertion operator.
    /// </summary>
    [TestFixture]
    public class CheapestInsertionOperatorTests
    {
        /// <summary>
        /// Tests an 'open' route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestOpen()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateSTSP(0, 5, 10, 40);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 2, 3, 1, 4 }, null);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new CheapestInsertionOperator(n: 50);
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }

        /// <summary>
        /// Tests a 'open' fixed route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestOpenFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateSTSP(0, 4, 5, 10, 40);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 2, 3, 1, 4 }, 4);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new CheapestInsertionOperator(n: 50);
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));

            // test result.
            Assert.AreEqual(-27, delta.Weight);
            Assert.AreEqual(0, delta.Customers);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }

        /// <summary>
        /// Tests a 'closed' route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateSTSP(0, 0, 5, 10, 40);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 2, 3, 1, 4 }, 0);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new CheapestInsertionOperator(n: 50);
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));

            // test result.
            Assert.AreEqual(-27, delta.Weight);
            Assert.AreEqual(0, delta.Customers);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, route.ToArray());
        }
    }
}