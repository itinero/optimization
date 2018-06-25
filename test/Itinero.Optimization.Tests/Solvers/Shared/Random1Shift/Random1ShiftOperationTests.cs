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

using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.Shared.Random1Shift;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.Random1Shift
{
    public class Random1ShiftOperationTests
    {
        /// <summary>
        /// Randomly shifting should not change count of a tour.
        /// </summary>
        [Fact]
        public void Random1ShiftOperation_ShiftShouldNotChangeCount()
        {
            var tour = new Tour(new[] {0, 1, 2, 3, 4}, 0);
            var count = tour.Count;
            
            for (var i = 0; i < 100; i++)
            {
                tour.DoRandom1Shift();
                Assert.Equal(count, tour.Count);
            }
        }
        
        /// <summary>
        /// Randomly shifting should not change visits in a tour.
        /// </summary>
        [Fact]
        public void Random1ShiftOperation_ShiftShouldNotChangeContent()
        {
            var tour = new Tour(new[] {0, 1, 2, 3, 4}, 0);
            var count = tour.Count;
            
            for (var i = 0; i < 100; i++)
            {
                tour.DoRandom1Shift();
                var content = new HashSet<int>(tour);
                Assert.Contains(0, content);
                Assert.Contains(1, content);
                Assert.Contains(2, content);
                Assert.Contains(3, content);
                Assert.Contains(4, content);
            }
        }
    }
}