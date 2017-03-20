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