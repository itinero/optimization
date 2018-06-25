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
    public class PerturberTests
    {
        [Fact]
        public void Perturber_Level0ShouldBeDefault()
        {
            var perturber = new PertuberMock();
            Assert.True(perturber.Apply(new IntegerCandidate(10)));
        }

        private class PertuberMock : Perturber<IntegerCandidate>
        {
            public override string Name => "MOCK";
            
            public override bool Apply(IntegerCandidate candidate, int level)
            {
                Assert.Equal(0, level);
                return true;
            }
        }
    }
}