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

namespace Itinero.Optimization.Test.Abstract.Tours
{
    /// <summary>
    /// Tests for tour.
    /// </summary>
    [TestFixture]
    public class TourExtensionsTests
    {
        /// <summary>
        /// Tests reverse range.
        /// </summary>
        [Test]
        public void TestReverseRange()
        {
            var s = new [] { 0, 1, 2, 3 };
            s.ReverseRange();

            Assert.AreEqual(0, s[0]);
            Assert.AreEqual(2, s[1]);
            Assert.AreEqual(1, s[2]);
            Assert.AreEqual(3, s[3]);

            s = new [] { 0, 1, 2, 3, 4 };
            s.ReverseRange();

            Assert.AreEqual(0, s[0]);
            Assert.AreEqual(3, s[1]);
            Assert.AreEqual(2, s[2]);
            Assert.AreEqual(1, s[3]);
            Assert.AreEqual(4, s[4]);
        }
    }
}