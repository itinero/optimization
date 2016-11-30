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
using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Algorithms.Random
{
    /// <summary>
    /// Contains tests for the pool.
    /// </summary>
    [TestFixture]
    public class RandomPoolTests
    {
        /// <summary>
        /// Tests a pool with 5 elements.
        /// </summary>
        [Test]
        public void Test5()
        {
            var shuffle = new RandomPool(5);
            var list = new List<int>();
            var current = shuffle.GetNext();
            while(current >= 0)
            {
                list.Add(current);
                current = shuffle.GetNext();
            }

            Assert.AreEqual(5, list.Count);
            Assert.Contains(0, list);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
            Assert.Contains(3, list);
            Assert.Contains(4, list);
        }
    }
}