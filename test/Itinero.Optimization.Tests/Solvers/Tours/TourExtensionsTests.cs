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
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.Random;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours
{
    /// <summary>
    /// Contains tests for the extensions related to tours.
    /// </summary>
    public class TourExtensionsTests
    {
        /// <summary>
        /// The from extension method should enumerate everything until from when the tour is closed.
        /// </summary>
        [Fact]
        public void TourExtensions_FromShouldEnumerateUntilFromWhenClosed()
        {
            var tour = new Tour(new [] { 0, 1, 2, 3, 4 }, 0);

            Assert.Equal(new[] {0, 1, 2, 3, 4}, tour.From(0));
            Assert.Equal(new[] {1, 2, 3, 4, 0}, tour.From(1));
            Assert.Equal(new[] {2, 3, 4, 0, 1}, tour.From(2));
            Assert.Equal(new[] {3, 4, 0, 1, 2}, tour.From(3));
            Assert.Equal(new[] {4, 0, 1, 2, 3}, tour.From(4));
        }
    }
}