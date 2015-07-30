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
using OsmSharp.Logistics.Solutions.TSPTW.Objectives;
using OsmSharp.Logistics.Solutions.TSPTW.VNS;
using OsmSharp.Logistics.Solvers.GA;
using OsmSharp.Math.Random;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Solutions.TSPTW.VNS
{
    /// <summary>
    /// Containts tests for the VND operator.
    /// </summary>
    [TestFixture]
    public class VNDOperatorTests
    {
        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutions5ClosedNotFixed()
        {
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new OsmSharp.WinForms.UI.Logging.DebugTraceListener());

            StaticRandomGenerator.Set(4541247);

            // create problem.
            var problem = TSPTWHelper.CreateTSP(0, 0, 5, 10);
            problem.Windows[1] = new Logistics.Solutions.TimeWindow()
            {
                Min = 5,
                Max = 15
            };
            problem.Windows[2] = new Logistics.Solutions.TimeWindow()
            {
                Min = 15,
                Max = 25
            };
            problem.Windows[3] = new Logistics.Solutions.TimeWindow()
            {
                Min = 25,
                Max = 35
            };
            problem.Windows[4] = new Logistics.Solutions.TimeWindow()
            {
                Min = 35,
                Max = 45
            };

            // create the solver.
            var solver = new VNDOperator();
            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(0, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

    }
}
