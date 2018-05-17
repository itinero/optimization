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

using Xunit;
using Itinero.Optimization.Strategies;
using System;
using Itinero.Optimization.Tests.Strategies.TestProblems.MinimizeInteger;

namespace Itinero.Optimization.Tests.Strategies
{
    /// <summary>
    /// Tests for the strategy extensions.
    /// </summary>
    public class IStrategyExtensionsTests
    {
        /// <summary>
        /// Tests if the iteration strategy iterates.
        /// </summary>
        [Fact]
        public void IStrategyExtensions_FuncIterateShouldIterateExactNumber()
        {
            var i = 0;
            Func<int, IntegerCandidate> strategy = (p) => 
            {
                i++;
                return new IntegerCandidate(p - i); 
            };

            strategy = strategy.Iterate(3);

            var candidate = strategy(5);
            Assert.NotNull(candidate);
            Assert.Equal(2, candidate.Value);
        }

        /// <summary>
        /// Tests if the iteration strategy iterates.
        /// </summary>
        [Fact]
        public void IStrategyExtensions_IterateShouldIterateExactNumber()
        {
            var i = 0;
            Strategy<int, IntegerCandidate> strategy = new FuncStrategy<int, IntegerCandidate>(
                (p) => 
                {
                    i++;
                    return new IntegerCandidate(p - i); 
                });

            strategy = strategy.Iterate(3);

            var candidate = strategy.Search(5);
            Assert.NotNull(candidate);
            Assert.Equal(2, candidate.Value);
        }
    }
}