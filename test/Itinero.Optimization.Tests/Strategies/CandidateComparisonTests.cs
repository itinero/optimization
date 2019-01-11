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
using Xunit;

namespace Itinero.Optimization.Tests.Strategies
{
    /// <summary>
    /// Tests candidate comparison.
    /// </summary>
    public class CandidateComparisonTests
    {
        /// <summary>
        /// Tests candicate comparison using a generic IComparable interface.
        /// </summary>
        [Fact]
        public void CandidateComparison_ShouldHandleGenericIComparable()
        {
            Assert.Equal(0, Itinero.Optimization.Strategies.CandidateComparison.Compare(
                new CandidateGenericIComparable(0), new CandidateGenericIComparable(0)));
            Assert.Equal(1, Itinero.Optimization.Strategies.CandidateComparison.Compare(
                new CandidateGenericIComparable(1), new CandidateGenericIComparable(0)));
            Assert.Equal(-1, Itinero.Optimization.Strategies.CandidateComparison.Compare(
                new CandidateGenericIComparable(0), new CandidateGenericIComparable(1)));
        }
        struct CandidateGenericIComparable : IComparable<CandidateGenericIComparable>
        {
            public CandidateGenericIComparable(int value)
            {
                this.Value = value;
            }

            public int Value { get; set; }

            public int CompareTo(CandidateGenericIComparable other)
            {
                return this.Value.CompareTo(other.Value);
            }
        }

        /// <summary>
        /// Tests candicate comparison using a generic IComparable interface.
        /// </summary>
        [Fact]
        public void CandidateComparison_ShouldHandleIComparable()
        {
            Assert.Equal(0, Itinero.Optimization.Strategies.CandidateComparison.Compare(
                new CandidateIComparable(0), new CandidateIComparable(0)));
            Assert.Equal(1, Itinero.Optimization.Strategies.CandidateComparison.Compare(
                new CandidateIComparable(1), new CandidateIComparable(0)));
            Assert.Equal(-1, Itinero.Optimization.Strategies.CandidateComparison.Compare(
                new CandidateIComparable(0), new CandidateIComparable(1)));
        }
        struct CandidateIComparable : IComparable
        {
            public CandidateIComparable(int value)
            {
                this.Value = value;
            }

            public int Value { get; set; }

            public int CompareTo(object other)
            {
                if (other is CandidateIComparable)
                {
                    return this.Value.CompareTo(((CandidateIComparable)other).Value);
                }
                throw new Exception("Cannot compare two different types.");
            }
        }
    }
}