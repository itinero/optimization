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