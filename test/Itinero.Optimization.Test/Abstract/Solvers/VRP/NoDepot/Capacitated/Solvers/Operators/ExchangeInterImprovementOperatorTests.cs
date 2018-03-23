// /*
//  *  Licensed to SharpSoftware under one or more contributor
//  *  license agreements. See the NOTICE file distributed with this work for 
//  *  additional information regarding copyright ownership.
//  * 
//  *  SharpSoftware licenses this file to you under the Apache License, 
//  *  Version 2.0 (the "License"); you may not use this file except in 
//  *  compliance with the License. You may obtain a copy of the License at
//  * 
//  *       http://www.apache.org/licenses/LICENSE-2.0
//  * 
//  *  Unless required by applicable law or agreed to in writing, software
//  *  distributed under the License is distributed on an "AS IS" BASIS,
//  *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *  See the License for the specific language governing permissions and
//  *  limitations under the License.
//  */

// using System.Collections.Generic;
// using Itinero.LocalGeo;
// using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated;
// using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers;
// using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers.Operators;
// using Itinero.Optimization.General;
// using Itinero.Optimization.Test.Staging;
// using Itinero.Optimization.Test.Staging.VRP.NoDepot.Capacitated;
// using NUnit.Framework;

// namespace Itinero.Optimization.Test.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers.Operators
// {
//     /// <summary>
//     /// Constains tests for the exchange operator.
//     /// </summary>
//     [TestFixture]
//     public class ExchangeInterImprovementOperatorTests
//     {
//         /// <summary>
//         /// Tests the operator on a solution with no tours.
//         /// </summary>
//         [Test]
//         public void TestNoTours()
//         {
//             // build a real problem but with an empty solution.
//             NoDepotNoDepotCVRPSolution solution = null;
//             List<Coordinate> locations = null;
//             var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
//             var objective = new NoDepotNoDepotCVRPObjective();
//             solution = new NoDepotNoDepotCVRPSolution(problem.Weights.Length);

//             // apply the operator to the empty solution.
//             var op = new ExchangeInterImprovementOperator();
//             float delta;
//             Assert.IsFalse(op.Apply(problem, objective, solution, out delta));
//             Assert.AreEqual(0, delta);
//         }

//         /// <summary>
//         /// Tests the operator on a solution with one tour.
//         /// </summary>
//         [Test]
//         public void TestOneTour()
//         {
//             // build a real problem but with an empty solution.
//             NoDepotNoDepotCVRPSolution solution = null;
//             List<Coordinate> locations = null;
//             var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
//             var objective = new NoDepotNoDepotCVRPObjective();
//             var tour = solution.Tour(0);
//             solution = new NoDepotNoDepotCVRPSolution(problem.Weights.Length);
//             solution.Add(tour);
//             objective.UpdateContent(problem, solution);

//             // apply the operator to the empty solution.
//             var op = new ExchangeInterImprovementOperator();
//             float delta;
//             Assert.IsFalse(op.Apply(problem, objective, solution, out delta));
//             Assert.AreEqual(0, delta);
//         }

//         /// <summary>
//         /// Tests the operator on a solution that's already optimimal.
//         /// </summary>
//         [Test]
//         public void TestOptimalSolution()
//         {
//             // build a real problem but with an empty solution.
//             NoDepotNoDepotCVRPSolution solution = null;
//             List<Coordinate> locations = null;
//             var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
//             var objective = new NoDepotNoDepotCVRPObjective();

//             // apply the operator to the empty solution.
//             var op = new ExchangeInterImprovementOperator();
//             float delta;
//             Assert.IsFalse(op.Apply(problem, objective, solution, out delta));
//             Assert.AreEqual(0, delta);
//         }

//         /// <summary>
//         /// Tests the operator on a solution that has a possible exchange but with the tours given.
//         /// </summary>
//         [Test]
//         public void TestWithExchangeWithFixedTours()
//         {
//             // build a real problem but with an empty solution.
//             NoDepotNoDepotCVRPSolution solution = null;
//             List<Coordinate> locations = null;
//             var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
//             var objective = new NoDepotNoDepotCVRPObjective();

//             // make sure there is a possible exchange.
//             // tour0: replace 3->4->5 with 3->8->5
//             // tour1: replace 7->8->9 with 7->4->9
//             var original = solution.Clone() as NoDepotNoDepotCVRPSolution;
//             var tour0 = solution.Tour(0);
//             var tour1 = solution.Tour(1);
//             tour0.ReplaceEdgeFrom(3, 8);
//             tour0.ReplaceEdgeFrom(8, 5);
//             tour1.ReplaceEdgeFrom(7, 4);
//             tour1.ReplaceEdgeFrom(4, 9);
//             var expectedDelta = problem.Weights.Seq(3, 8, 5) +
//                 problem.Weights.Seq(7, 4, 9) -
//                 problem.Weights.Seq(3, 4, 5) -
//                 problem.Weights.Seq(7, 8, 9);
//             objective.UpdateContent(problem, solution);

//             // apply the operator.
//             var op = new ExchangeInterImprovementOperator();
//             float delta;
//             Assert.IsTrue(op.Apply(problem, objective, solution, 0, 1, out delta));
//             Assert.AreEqual(expectedDelta, delta, TestConstants.E);

//             // apply the operator the other way around.
//             solution = original;
//             tour0 = solution.Tour(0);
//             tour1 = solution.Tour(1);
//             tour0.ReplaceEdgeFrom(3, 8);
//             tour0.ReplaceEdgeFrom(8, 5);
//             tour1.ReplaceEdgeFrom(7, 4);
//             tour1.ReplaceEdgeFrom(4, 9);
//             objective.UpdateContent(problem, solution);

//             // apply the operator.
//             Assert.IsTrue(op.Apply(problem, objective, solution, 1, 0, out delta));
//             Assert.AreEqual(expectedDelta, delta, TestConstants.E);
//         }

//         /// <summary>
//         /// Tests the operator on a solution that has a possible exchange.
//         /// </summary>
//         [Test]
//         public void TestWithExchange()
//         {
//             // build a real problem but with an empty solution.
//             NoDepotNoDepotCVRPSolution solution = null;
//             List<Coordinate> locations = null;
//             var problem = "data.geometric.problem1.geojson".BuildProblem(out solution, out locations);
//             var objective = new NoDepotNoDepotCVRPObjective();

//             // make sure there is a possible exchange.
//             // tour0: replace 3->4->5 with 3->8->5
//             // tour1: replace 7->8->9 with 7->4->9
//             var tour0 = solution.Tour(0);
//             var tour1 = solution.Tour(1);
//             tour0.ReplaceEdgeFrom(3, 8);
//             tour0.ReplaceEdgeFrom(8, 5);
//             tour1.ReplaceEdgeFrom(7, 4);
//             tour1.ReplaceEdgeFrom(4, 9);
//             var expectedDelta = problem.Weights.Seq(3, 8, 5) +
//                 problem.Weights.Seq(7, 4, 9) -
//                 problem.Weights.Seq(3, 4, 5) -
//                 problem.Weights.Seq(7, 8, 9);
//             objective.UpdateContent(problem, solution);

//             // apply the operator.
//             var op = new ExchangeInterImprovementOperator();
//             float delta;
//             Assert.IsTrue(op.Apply(problem, objective, solution, out delta));
//             Assert.AreEqual(expectedDelta, delta, TestConstants.E);
//         }

//         /// <summary>
//         /// Tests the operator on a solution that has a possible exchange but visit costs preventing it.
//         /// </summary>
//         [Test]
//         public void TestWithExchangeWithVisitCosts()
//         {
//             // build a real problem but with an empty solution.
//             NoDepotNoDepotCVRPSolution solution = null;
//             List<Coordinate> locations = null;
//             var problem = "data.geometric.problem5.geojson".BuildProblem(out solution, out locations);
//             var original = solution.Clone() as NoDepotNoDepotCVRPSolution;
//             var objective = new NoDepotNoDepotCVRPObjective();

//             // make sure there is a possible exchange.
//             // tour0: {[0]->1->2->3->[0]}
//             // tour1: {[4]->5->6->7->[4]}
//             // a possible exchange is 3<->5 but not with visit costs.
//             var tour0 = solution.Tour(0);
//             var tour1 = solution.Tour(1);

//             // apply the operator.
//             var op = new ExchangeInterImprovementOperator();
//             float delta;
//             Assert.IsFalse(op.Apply(problem, objective, solution, out delta));

//             // make sure there is a possible exchange.
//             problem.VisitCosts = null;
//             // tour0: {[0]->1->2->3->[0]}
//             // tour1: {[4]->5->6->7->[4]}
//             // a possible exchange is 3<->5.
//             solution = original.Clone() as NoDepotNoDepotCVRPSolution;
//             tour0 = solution.Tour(0);
//             tour1 = solution.Tour(1);

//             // apply the operator.
//             Assert.IsTrue(op.Apply(problem, objective, solution, out delta));
//         }
//     }
// }