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
using Itinero.Optimization.Strategies.GA;
using Itinero.Optimization.Strategies.Tools.Selectors;
using Itinero.Optimization.Tests.Strategies.TestProblems.MinimizeInteger;
using Xunit;

namespace Itinero.Optimization.Tests.Strategies.GA
{
    /// <summary>
    /// Contains tests for the tournament selector.
    /// </summary>
    public class TournamentSelectorTests
    {
        /// <summary>
        /// Tests if the tournament selector does what it's advertising to do.
        /// </summary>
        [Fact]
        public void TTournamentSelector_ShouldSelectUsingATournament()
        {
            var random = new NotSoRandomGenerator(
                new [] {0.6f, 0.2f, 0.8f}, new [] {0, 2, 3});
            
            // create population and selector.
            var population = new []
            {
                new IntegerCandidate(10),
                new IntegerCandidate(1),
                new IntegerCandidate(3),
                new IntegerCandidate(4), 
            };
            var selector = new TournamentSelector<IntegerCandidate>(50, 0.5f, random);

            Assert.Equal(0, selector.Select(population, null));
            Assert.Equal(-1, selector.Select(population, null));
            Assert.Equal(2, selector.Select(population, null));
            Assert.Equal(-1, selector.Select(population, null));
            Assert.Equal(3, selector.Select(population, null));
            Assert.Equal(-1, selector.Select(population, null));
        }
    }
}