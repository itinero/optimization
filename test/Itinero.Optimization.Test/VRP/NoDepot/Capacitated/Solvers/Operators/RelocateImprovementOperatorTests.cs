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

using System.Collections.Generic;
using Itinero.LocalGeo;
using Itinero.Optimization.General;
using Itinero.Optimization.Test.Staging;
using Itinero.Optimization.Test.Staging.VRP.NoDepot.Capacitated;
using Itinero.Optimization.VRP.NoDepot.Capacitated;
using Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers.Operators;
using NUnit.Framework;

namespace Itinero.Optimization.Test.VRP.NoDepot.Capacitated.Solvers.Operators
{
    /// <summary>
    /// Contains tests for the relocation improvement operator.
    /// </summary>
    [TestFixture]
    public class RelocateImprovementOperatorTests
    {
        /// <summary>
        /// Tests the operator on a solution with no tours.
        /// </summary>
        [Test]
        public void TestNoTours()
        {
            // build a real problem but with an empty solution.
            NoDepotCVRPSolution solution = null;
            List<Coordinate> locations = null;
            var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
            var objective = new NoDepotCVRPObjective();
            solution = new NoDepotCVRPSolution(problem.Weights.Length);
            objective.UpdateContent(problem, solution);

            // apply the operator to the empty solution.
            var op = new RelocateImprovementOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, objective, solution, out delta));
            Assert.AreEqual(0, delta);
        }

        /// <summary>
        /// Tests the operator on a solution with one tour.
        /// </summary>
        [Test]
        public void TestOneTour()
        {
            // build a real problem but with an empty solution.
            NoDepotCVRPSolution solution = null;
            List<Coordinate> locations = null;
            var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
            var objective = new NoDepotCVRPObjective();
            var tour = solution.Tour(0);
            solution = new NoDepotCVRPSolution(problem.Weights.Length);
            solution.Add(tour);
            objective.UpdateContent(problem, solution);

            // apply the operator to the empty solution.
            var op = new RelocateImprovementOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, objective, solution, out delta));
            Assert.AreEqual(0, delta);
        }

        /// <summary>
        /// Tests the operator on a solution that's already optimimal.
        /// </summary>
        [Test]
        public void TestOptimalSolution()
        {
            // build a real problem but with an empty solution.
            NoDepotCVRPSolution solution = null;
            List<Coordinate> locations = null;
            var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
            var objective = new NoDepotCVRPObjective();
            objective.UpdateContent(problem, solution);

            // apply the operator to the empty solution.
            var op = new RelocateImprovementOperator();
            float delta;
            Assert.IsFalse(op.Apply(problem, objective, solution, out delta));
            Assert.AreEqual(0, delta);
        }

        /// <summary>
        /// Tests the operator on a solution that has a possible exchange but with the tours given.
        /// </summary>
        [Test]
        public void TestWithRelocateWithFixedTours()
        {
            // build a real problem but with an empty solution.
            NoDepotCVRPSolution solution = null;
            List<Coordinate> locations = null;
            var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
            var objective = new NoDepotCVRPObjective();

            // make sure there is a possible relocation.
            // tour0: replace 3->8->4 with 3->4->5
            // tour1: insert 8 into 7->8->9
            var original = solution.Clone() as NoDepotCVRPSolution;
            var tour0 = solution.Tour(0);
            var tour1 = solution.Tour(1);
            tour1.Remove(8);
            tour0.InsertAfter(3, 8);
            var expectedDelta = problem.Weights.Seq(3, 8, 4) -
                problem.Weights.Seq(3, 4) +
                problem.Weights.Seq(7, 9) - 
                problem.Weights.Seq(7, 8, 9);
            objective.UpdateContent(problem, solution);

            // apply the operator.
            var op = new RelocateImprovementOperator();
            float delta;
            Assert.IsTrue(op.Apply(problem, objective, solution, 0, 1, out delta));
            Assert.AreEqual(expectedDelta, delta, TestConstants.E);

            // apply the operator the other way around.
            solution = original;
            tour0 = solution.Tour(0);
            tour1 = solution.Tour(1);
            tour1.Remove(8);
            tour0.InsertAfter(3, 8);
            objective.UpdateContent(problem, solution);

            // apply the operator.
            Assert.IsTrue(op.Apply(problem, objective, solution, 1, 0, out delta));
            Assert.AreEqual(expectedDelta, delta, TestConstants.E);
        }

        /// <summary>
        /// Tests the operator on a solution that has a possible exchange but with the tours given.
        /// </summary>
        [Test]
        public void TestWithRelocate()
        {
            // build a real problem but with an empty solution.
            NoDepotCVRPSolution solution = null;
            List<Coordinate> locations = null;
            var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
            var objective = new NoDepotCVRPObjective();

            // make sure there is a possible relocation.
            // tour0: replace 3->8->4->5 with 3->4->5
            // tour1: insert 8 into 7->8->9
            var original = solution.Clone() as NoDepotCVRPSolution;
            var tour0 = solution.Tour(0);
            var tour1 = solution.Tour(1);
            tour1.Remove(8);
            tour0.InsertAfter(3, 8);
            var expectedDelta = problem.Weights.Seq(3, 8, 4) -
                problem.Weights.Seq(3, 4) +
                problem.Weights.Seq(7, 9) - 
                problem.Weights.Seq(7, 8, 9);
            objective.UpdateContent(problem, solution);

            // apply the operator.
            var op = new RelocateImprovementOperator();
            float delta;
            Assert.IsTrue(op.Apply(problem, objective, solution, out delta));
            Assert.AreEqual(expectedDelta, delta, TestConstants.E);
        }
    }
}