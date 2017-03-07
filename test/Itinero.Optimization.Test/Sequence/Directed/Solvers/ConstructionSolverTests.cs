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

using Itinero.Optimization.Sequence.Directed;
using Itinero.Optimization.Sequence.Directed.Solver;
using Itinero.Optimization.Tours;
using NUnit.Framework;
using System.Linq;

namespace Itinero.Optimization.Test.Sequence.Directed.Solvers
{
    /// <summary>
    /// Contains construction solver tests.
    /// </summary>
    [TestFixture]
    public class ConstructionSolverTests
    {
        /// <summary>
        /// Tests with a fixed sequence.
        /// </summary>
        [Test]
        public void TestFixed()
        {
            var problem = SequenceProblemHelper.Create(new Tour(new int[] { 0, 1, 2, 3 }, 3), 4, 60, 360);

            var solver = new ConstructionSolver();
            var result = solver.Solve(problem, new SequenceDirectedObjective());
            
            Assert.IsNotNull(result);
            var arr = result.ToArray();
            Assert.AreEqual(arr.Length, 4);
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(4, arr[1]);
            Assert.AreEqual(8, arr[2]);
            Assert.AreEqual(12, arr[3]);
        }

        /// <summary>
        /// Tests with a closed sequence.
        /// </summary>
        [Test]
        public void TestClosed()
        {
            var problem = SequenceProblemHelper.Create(new Tour(new int[] { 0, 1, 2, 3 }, 0), 4, 60, 360);

            var solver = new ConstructionSolver();
            var result = solver.Solve(problem, new SequenceDirectedObjective());

            Assert.IsNotNull(result);
            var arr = result.ToArray();
            Assert.AreEqual(arr.Length, 4);
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(4, arr[1]);
            Assert.AreEqual(8, arr[2]);
            Assert.AreEqual(12, arr[3]);
        }
    }
}