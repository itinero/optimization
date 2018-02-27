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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Models.TimeWindows;
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