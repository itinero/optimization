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

using Itinero.Optimization.Tours;
using Itinero.Optimization.Tours.Operations;
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Tours.Operators
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