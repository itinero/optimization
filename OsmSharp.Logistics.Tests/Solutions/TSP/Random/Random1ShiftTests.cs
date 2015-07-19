// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSP;
using OsmSharp.Logistics.Solutions.TSP.Random;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Solutions.TSP.Random
{
    /// <summary>
    /// Containts tests for the Random1Shift perturber.
    /// </summary>
    [TestFixture]
    public class Random1ShiftTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Test]
        public void TestName()
        {
            // create the perturber.
            var perturber = new Random1Shift();

            Assert.AreEqual("RAN_1SHFT", perturber.Name);
        }

        /// <summary>
        /// Tests the actual perturbations.
        /// </summary>
        [Test]
        public void TestPerturbation()
        {
            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            // create the perturber.
            var perturber = new Random1Shift();

            // create a problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);

            // execute random shifts.
            for(int i = 0; i < 1000; i++)
            {
                // create solution.
                var solution = new Route(new int[] { 0, 1, 2, 3, 4 });
                var fitnessBefore = 0.0;
                foreach (var pair in solution.Pairs())
                {
                    fitnessBefore = fitnessBefore +
                        problem.Weights[pair.From][pair.To];
                }

                // shift one customer.
                double difference;
                perturber.Apply(problem, new MinimumWeightObjective(), solution, out difference);

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
                var fitnessAfter = 0.0;
                foreach(var pair in solution.Pairs())
                {
                    fitnessAfter = fitnessAfter +
                        problem.Weights[pair.From][pair.To];
                }

                Assert.AreEqual(fitnessAfter - fitnessBefore, difference, 0.0000001);
            }
        }
    }
}