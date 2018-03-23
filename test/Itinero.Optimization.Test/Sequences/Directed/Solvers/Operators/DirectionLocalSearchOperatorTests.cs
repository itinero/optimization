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

using Itinero.Optimization.Abstract.Tours;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Sequences.Directed.Solvers.Operators
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

            var op = new Itinero.Optimization.Sequences.Directed.Solver.Operators.DirectionLocalSearchOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(0, delta);

            tour = new Tour(new int[] { 0, 5, 8, 12 }, 0);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 9, 12 }, 0);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 8, 13 }, 0);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
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

            var op = new Itinero.Optimization.Sequences.Directed.Solver.Operators.DirectionLocalSearchOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(0, delta);

            tour = new Tour(new int[] { 0, 5, 8, 12 }, 12);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 9, 12 }, 12);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);

            tour = new Tour(new int[] { 0, 4, 8, 14 }, 14);
            Assert.IsTrue(op.Apply(problem, new Optimization.Sequences.Directed.SequenceDirectedObjective(), tour, out delta));
            Assert.AreEqual(96, delta);
        }
    }
}
