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
using Itinero.Optimization.Abstract.Tours.Operations;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Abstract.Tours.Operators
{
    /// <summary>
    /// Contains tests for shift after operations.
    /// </summary>
    [TestFixture]
    public class ShiftAfterTests
    { 
        /// <summary>
        /// Tests the shift after tour.
        /// </summary>
        [Test]
        public void TestShiftAfterTour()
        {
            var tour = new Tour(new int[] { 0, 2, 3, 4, 1 });
            var shiftAfterTour = tour.GetShiftedAfter(1, 0);
            Assert.AreEqual(shiftAfterTour, new int[] { 0, 1, 2, 3, 4 });

            tour = new Tour(new int[] { 0, 1, 2, 3, 4 });
            shiftAfterTour = tour.GetShiftedAfter(1, 4);
            Assert.AreEqual(shiftAfterTour, new int[] { 0, 2, 3, 4, 1 });

            tour = new Tour(new int[] { 0, 1, 2, 3, 4 });
            shiftAfterTour = tour.GetShiftedAfter(1, 2);
            Assert.AreEqual(shiftAfterTour, new int[] { 0, 2, 1, 3, 4 });
        }
    }
}