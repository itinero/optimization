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

using Itinero.Optimization.Strategies.VNS;
using Itinero.Optimization.Tests.Strategies.TestProblems.MinimizeInteger;
using Xunit;

namespace Itinero.Optimization.Tests.Strategies.VNS
{
    /// <summary>
    /// Contains tests for the VNS strategy.
    /// </summary>
    public class VNSStrategyTests
    {
        /// <summary>
        /// Tests the name property.
        /// </summary>
        [Fact]
        public void VNSStrategy_ShouldBeVNSAndGeneratorPerturberAndLocalSearchNames()
        {
            var strategy = new VNSStrategy<int, IntegerCandidate>(
                (p) => new IntegerCandidate(p),
                (c, l) =>
                {
                    c.Value += 1;
                    return false;
                },
                TestProblems.MinimizeInteger.Operators.SubtractRandom,
                (c, i, l) => c.Value == 0);
            Assert.Equal($"VNS_[{Itinero.Optimization.Strategies.Constants.ANONYMOUS}_" +
                         $"{Itinero.Optimization.Strategies.Constants.ANONYMOUS}_" +
                         $"{Itinero.Optimization.Strategies.Constants.ANONYMOUS}]", strategy.Name);
        }
        
        /// <summary>
        /// Tests the vns strategy on the integer problem.
        /// </summary>
        [Fact]
        public void VNSStrategy_ShouldSolverIntegerProblemToPerfection()
        {
            var strategy = new VNSStrategy<int, IntegerCandidate>(
                (p) => new IntegerCandidate(p),
                (c, l) =>
                {
                    c.Value += 1;
                    return false;
                },
                TestProblems.MinimizeInteger.Operators.SubtractRandom,
                (c, i, l) => c.Value == 0);
            var solution = strategy.Search(100);
            Assert.NotNull(solution);
            Assert.Equal(0, solution.Value);
        }
    }
}