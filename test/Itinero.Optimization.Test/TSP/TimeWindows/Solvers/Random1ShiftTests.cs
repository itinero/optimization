// Itinero.Optimization - Route optimization for .NET
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
using Itinero.Optimization.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows;
using Itinero.Optimization.TSP.TimeWindows.Solvers.Operators;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Random
{
    /// <summary>
    /// Containts tests for the Random1Shift perturber.
    /// </summary>
    [TestFixture]
    public class Random1ShiftTests
    {
        /// <summary>
        /// Tests the actual perturbations.
        /// </summary>
        [Test]
        public void TestPerturbation()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the perturber.
            var perturber = new Random1Shift<TSPTWObjective>();

            // create a problem.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 10);
            var objective = new TSPTWObjective();

            // execute random shifts.
            for (int i = 0; i < 1000; i++)
            {
                // create solution.
                var solution = new Optimization.Routes.Route(new int[] { 0, 1, 2, 3, 4 });
                var fitnessBefore = objective.Calculate(problem, solution);

                // shift one customer.
                float difference;
                perturber.Apply(problem, objective, solution, out difference);

                // check if valid solution.
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);

                // test if first is still first.
                Assert.AreEqual(problem.First, solution.First);

                // calculate expected difference.
                var fitnessAfter = objective.Calculate(problem, solution);

                Assert.AreEqual(fitnessAfter - fitnessBefore, difference, 0.0000001);
            }
        }

        /// <summary>
        /// Tests the actual perturbations but with time-windows that make sure no perturbation is possible.
        /// </summary>
        [Test]
        public void TestPerturbationViolations()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the perturber.
            var perturber = new Random1Shift<TSPTWObjective>();

            // create a problem where any move would violate windows.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 2);
            problem.Windows[0] = new TimeWindow()
            {
                Min = 0,
                Max = 1
            };
            problem.Windows[1] = new TimeWindow()
            {
                Min = 2,
                Max = 3
            };
            problem.Windows[2] = new TimeWindow()
            {
                Min = 4,
                Max = 5
            };
            problem.Windows[3] = new TimeWindow()
            {
                Min = 6,
                Max = 7
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 8,
                Max = 9
            };
            var objective = new TSPTWObjective();

            // execute random shifts.
            for (int i = 0; i < 1000; i++)
            {
                // create solution.
                var solution = new Optimization.Routes.Route(new int[] { 0, 1, 2, 3, 4 });
                var fitnessBefore = objective.Calculate(problem, solution);

                // shift one customer.
                float difference;
                perturber.Apply(problem, objective, solution, out difference);

                // check if valid solution.
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);

                // test if first is still first.
                Assert.AreEqual(problem.First, solution.First);

                // calculate expected difference.
                var fitnessAfter = objective.Calculate(problem, solution);

                Assert.AreEqual(fitnessAfter - fitnessBefore, difference, 0.0000001);
            }
        }

        /// <summary>
        /// Tests the actual perturbations but with time-windows with some perturbation possible.
        /// </summary>
        [Test]
        public void TestPerturbationSomeViolations()
        {
            // set the seed manually.
            RandomGeneratorExtensions.GetGetNewRandom = () => new RandomGenerator(4541247);

            // create the perturber.
            var perturber = new Random1Shift<TSPTWObjective>();

            // create a problem where any move would violate windows.
            var problem = TSPTWHelper.CreateTSPTW(0, 0, 5, 2);
            problem.Windows[2] = new TimeWindow()
            {
                Min = 4,
                Max = 5
            };
            problem.Windows[3] = new TimeWindow()
            {
                Min = 6,
                Max = 7
            };
            var objective = new TSPTWObjective();

            // execute random shifts.
            for (int i = 0; i < 1000; i++)
            {
                // create solution.
                var solution = new Optimization.Routes.Route(new int[] { 0, 1, 2, 3, 4 });
                var fitnessBefore = objective.Calculate(problem, solution);

                // shift one customer.
                float difference;
                perturber.Apply(problem, objective, solution, out difference);

                // check if valid solution.
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);

                // test if first is still first.
                Assert.AreEqual(problem.First, solution.First);

                // calculate expected difference.
                var fitnessAfter = objective.Calculate(problem, solution);

                Assert.AreEqual(fitnessAfter - fitnessBefore, -difference, 0.0000001);
            }
        }
    }
}