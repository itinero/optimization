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

using Itinero.Optimization.Tours;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Sequence.Directed.Solvers.Operators
{
    /// <summary>
    /// Contains tests for the direction local search operator.
    /// </summary>
    [TestFixture]
    public class DirectionLocalSearchOperatorTests
    {
        /// <summary>
        /// Tests the operator on a closed tour.
        /// </summary>
        [Test]
        public void TestClosed()
        {
            var problem = SequenceProblemHelper.Create(new Tour(new int[] { 0, 1, 2, 3 }, 0), 4, 60, 360);
            var tour = new Tour(new int[] { 0, 4, 8, 12 }, 0);

            var op = new Itinero.Optimization.Sequence.Directed.Solver.Operators.DirectionLocalSearchOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(0, delta);

            tour = new Tour(new int[] { 0, 5, 8, 12 }, 12);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 9, 12 }, 12);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 8, 13 }, 13);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);
        }

        /// <summary>
        /// Tests the operator on a fixed tour.
        /// </summary>
        [Test]
        public void TestFixed()
        {
            var problem = SequenceProblemHelper.Create(new Tour(new int[] { 0, 1, 2, 3 }, 3), 4, 60, 360);
            var tour = new Tour(new int[] { 0, 4, 8, 12 }, 12);

            var op = new Itinero.Optimization.Sequence.Directed.Solver.Operators.DirectionLocalSearchOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(0, delta);

            tour = new Tour(new int[] { 0, 5, 8, 12 }, 12);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 9, 12 }, 12);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 8, 14 }, 14);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequence.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);
        }
    }
}
