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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.STSP.Directed;
using Itinero.Optimization.STSP.Directed.Solvers.Operators;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.STSP.Directed.Solvers.Operators
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
            var problem = STSPHelper.CreateDirectedSTSP(0, 5, 10, 1, 40);
            problem.Weights.SetWeight(0, 1, 1);
            problem.Weights.SetWeight(1, 2, 1);
            problem.Weights.SetWeight(2, 3, 1);
            problem.Weights.SetWeight(3, 4, 1);
            problem.Weights.SetWeight(4, 0, 1);

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 8, 12, 16 }, null);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new CheapestInsertionOperator(4, 10);
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }

        /// <summary>
        /// Tests an 'open' fixed route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestOpenFixed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateDirectedSTSP(0, 4, 5, 10, 1, 40);
            problem.Weights.SetWeight(0, 1, 1);
            problem.Weights.SetWeight(1, 2, 1);
            problem.Weights.SetWeight(2, 3, 1);
            problem.Weights.SetWeight(3, 4, 1);
            problem.Weights.SetWeight(4, 0, 1);

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 12, 4, 16 }, 16);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new CheapestInsertionOperator(4, 10);
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }

        /// <summary>
        /// Tests a 'closed' route where cheapest insertion can help.
        /// </summary>
        [Test]
        public void TestClosed()
        {
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the problem and make sure 0->1->2->3->4 is the solution.
            var problem = STSPHelper.CreateDirectedSTSP(0, 0, 5, 10, 1, 40);
            problem.Weights.SetWeight(0, 1, 1);
            problem.Weights.SetWeight(1, 2, 1);
            problem.Weights.SetWeight(2, 3, 1);
            problem.Weights.SetWeight(3, 4, 1);
            problem.Weights.SetWeight(4, 0, 1);

            // create a route with one shift.
            var route = new Optimization.Tours.Tour(new int[] { 0, 12, 4, 16 }, 16);

            // apply the 1-shift local search, it should find the customer to replocate.
            var localSearch = new CheapestInsertionOperator(4, 10);
            STSPFitness delta;
            Assert.IsTrue(localSearch.Apply(problem, new STSPObjective(), route, out delta));
        }
    }
}
