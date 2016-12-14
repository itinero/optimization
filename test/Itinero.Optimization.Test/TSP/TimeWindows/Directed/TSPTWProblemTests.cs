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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.TimeWindows;
using NUnit.Framework;

namespace Itinero.Optimization.Test.TSP.TimeWindows.Directed
{
    /// <summary>
    /// Contains tests for the TSPTW problem.
    /// </summary>
    [TestFixture]
    public class TSPTWProblemTests
    {
        /// <summary>
        /// Tests time and violations.
        /// </summary>
        [Test]
        public void TestTimeAndViolationsSuccess()
        {
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 5, 100, new TimeWindow[] {
                new TimeWindow()
                {
                    Min = 0,
                    Max = 5
                },
                new TimeWindow()
                {
                    Min = 5,
                    Max = 15
                },
                new TimeWindow()
                {
                    Min = 15,
                    Max = 25
                },
                new TimeWindow()
                {
                    Min = 25,
                    Max = 35
                },
                new TimeWindow()
                {
                    Min = 35,
                    Max = 45
                }
            }, 1);
            var w = 10;
            problem.Times.SetWeight(0, 1, w, w, w, w);
            problem.Times.SetWeight(1, 2, w, w, w, w);
            problem.Times.SetWeight(2, 3, w, w, w, w);
            problem.Times.SetWeight(3, 4, w, w, w, w);
            problem.Times.SetWeight(4, 0, w, w, w, w);
            
            float time, waitTime, violatedTime;
            var validFlags = new bool[5];
            var invalids = problem.TimeAndViolations(new Optimization.Tours.Tour(new int[] { 0, 4, 8, 12, 16 }), out time, out waitTime, out violatedTime, ref validFlags);
            Assert.AreEqual(0, invalids);
            Assert.AreEqual(50, time);
            Assert.AreEqual(0, waitTime);
            Assert.AreEqual(0, violatedTime);
            Assert.IsTrue(validFlags[0]);
            Assert.IsTrue(validFlags[1]);
            Assert.IsTrue(validFlags[2]);
            Assert.IsTrue(validFlags[3]);
            Assert.IsTrue(validFlags[4]);
        }

        /// <summary>
        /// Tests time and violations.
        /// </summary>
        [Test]
        public void TestTimeAndViolationsTooLate()
        {
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 5, 100, new TimeWindow[] {
                new TimeWindow()
                {
                    Min = 0,
                    Max = 5
                },
                new TimeWindow()
                {
                    Min = 5,
                    Max = 15
                },
                new TimeWindow()
                {
                    Min = 15,
                    Max = 25
                },
                new TimeWindow()
                {
                    Min = 25,
                    Max = 35
                },
                new TimeWindow()
                {
                    Min = 5,
                    Max = 15
                },
            }, 1);
            var w = 10;
            problem.Times.SetWeight(0, 1, w, w, w, w);
            problem.Times.SetWeight(1, 2, w, w, w, w);
            problem.Times.SetWeight(2, 3, w, w, w, w);
            problem.Times.SetWeight(3, 4, w, w, w, w);
            problem.Times.SetWeight(4, 0, w, w, w, w);

            float time, waitTime, violatedTime;
            var validFlags = new bool[5];
            var invalids = problem.TimeAndViolations(new Optimization.Tours.Tour(new int[] { 0, 4, 8, 12, 16 }), out time, out waitTime, out violatedTime, ref validFlags);
            Assert.AreEqual(1, invalids);
            Assert.AreEqual(50, time);
            Assert.AreEqual(0, waitTime);
            Assert.AreEqual(25, violatedTime);
            Assert.IsTrue(validFlags[0]);
            Assert.IsTrue(validFlags[1]);
            Assert.IsTrue(validFlags[2]);
            Assert.IsTrue(validFlags[3]);
            Assert.IsFalse(validFlags[4]);

            problem.Windows[1] = new TimeWindow()
            {
                Min = 0,
                Max = 5
            };
            problem.Windows[4] = new TimeWindow()
            {
                Min = 35,
                Max = 45
            };
            invalids = problem.TimeAndViolations(new Optimization.Tours.Tour(new int[] { 0, 4, 8, 12, 16 }), out time, out waitTime, out violatedTime, ref validFlags);
            Assert.AreEqual(1, invalids);
            Assert.AreEqual(50, time);
            Assert.AreEqual(0, waitTime);
            Assert.AreEqual(5, violatedTime);
            Assert.IsTrue(validFlags[0]);
            Assert.IsFalse(validFlags[1]);
            Assert.IsTrue(validFlags[2]);
            Assert.IsTrue(validFlags[3]);
            Assert.IsTrue(validFlags[4]);
        }

        /// <summary>
        /// Tests time and violations.
        /// </summary>
        [Test]
        public void TestTimeAndViolationsEarly()
        {
            var problem = TSPTWHelper.CreateDirectedTSPTW(0, 5, 100, new TimeWindow[] {
                new TimeWindow()
                {
                    Min = 0,
                    Max = 5
                },
                new TimeWindow()
                {
                    Min = 5,
                    Max = 15
                },
                new TimeWindow()
                {
                    Min = 15,
                    Max = 25
                },
                new TimeWindow()
                {
                    Min = 25,
                    Max = 35
                },
                new TimeWindow()
                {
                    Min = 45,
                    Max = 55
                }
            }, 1);
            var w = 10;
            problem.Times.SetWeight(0, 1, w, w, w, w);
            problem.Times.SetWeight(1, 2, w, w, w, w);
            problem.Times.SetWeight(2, 3, w, w, w, w);
            problem.Times.SetWeight(3, 4, w, w, w, w);
            problem.Times.SetWeight(4, 0, w, w, w, w);

            float time, waitTime, violatedTime;
            var validFlags = new bool[5];
            var invalids = problem.TimeAndViolations(new Optimization.Tours.Tour(new int[] { 0, 4, 8, 12, 16 }), out time, out waitTime, out violatedTime, ref validFlags);
            Assert.AreEqual(0, invalids);
            Assert.AreEqual(50, time);
            Assert.AreEqual(5, waitTime);
            Assert.AreEqual(0, violatedTime);
            Assert.IsTrue(validFlags[0]);
            Assert.IsTrue(validFlags[1]);
            Assert.IsTrue(validFlags[2]);
            Assert.IsTrue(validFlags[3]);
            Assert.IsTrue(validFlags[4]);
        }
    }
}