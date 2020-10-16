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
using Itinero.Optimization.Tests.Strategies.TestProblems.MinimizeInteger;
using Xunit;

namespace Itinero.Optimization.Tests.Strategies.GA
{
    /// <summary>
    /// Contains tests for the GA strategy.
    /// </summary>
    public class GAStrategyTests
    {
        /// <summary>
        /// The genetic algorithm should just do local search when mutation operator is used.
        /// </summary>
        [Fact]
        public void GAStrategy_ShouldDoMutationLocalSearch()
        {
            var settings = GASettings.Default;
            settings.StagnationCount = 100;
            settings.PopulationSize = 100;
            var gaStrategy = new GAStrategy<int, IntegerCandidate>(
                (p) => new IntegerCandidate(p),
                (c1, c2) => c1,
                TestProblems.MinimizeInteger.Operators.SubtractRandom,
                settings);
            var result = gaStrategy.Search(100);
            Assert.NotNull(result);
            Assert.Equal(0, result.Value);
        }

        /// <summary>
        /// The genetic algorithm should just to local search when cross over is local search.
        /// </summary>
        [Fact]
        public void GAStrategy_ShouldDoCrossOverLocalSearch()
        {
            var settings = GASettings.Default;
            settings.StagnationCount = 100;
            settings.PopulationSize = 100;
            var gaStrategy = new GAStrategy<int, IntegerCandidate>(
                (p) => new IntegerCandidate(p),
                (c1, c2) =>
                {
                    var c = new IntegerCandidate(c1.Value);
                    Operators.SubtractRandom(c);
                    return c;
                },
                Operators.Empty,
                settings);
            var result = gaStrategy.Search(100);
            Assert.NotNull(result);
            Assert.Equal(0, result.Value);
        }

        /// <summary>
        /// The genetic algorithm should just to local search when cross over is local search.
        /// </summary>
        [Fact]
        public void GAStrategy_ShouldDoCrossOverAndMutationLocalSearch()
        {
            var settings = GASettings.Default;
            settings.StagnationCount = 100;
            settings.PopulationSize = 100;
            var gaStrategy = new GAStrategy<int, IntegerCandidate>(
                (p) => new IntegerCandidate(p),
                (c1, c2) => new IntegerCandidate((c1.Value + c2.Value) / 2),
                Operators.MutateRandom,
                settings);
            var result = gaStrategy.Search(100);
            Assert.NotNull(result);
            Assert.Equal(0, result.Value);
        }
    }
}