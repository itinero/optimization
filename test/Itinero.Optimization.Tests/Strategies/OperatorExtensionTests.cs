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
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Tests.Strategies.TestProblems.MinimizeInteger;
using Xunit;

namespace Itinero.Optimization.Tests.Strategies
{
    /// <summary>
    /// Tests for the operator extension methods.
    /// </summary>
    public class IOperatorExtensionTests
    {
        /// <summary>
        /// Test the apply until extension method.
        /// </summary>
        [Fact]
        public void IOperatorExtension_FuncApplyUntilShouldApplyUntil()
        {
            Func<IntegerCandidate, bool> oper = (c) => 
            {
                c.Value--;
                return c.Value > 0;
            };

            oper = oper.ApplyUntil();

            var can = new IntegerCandidate(1000);
            Assert.True(oper(can));
            Assert.Equal(0, can.Value);
        }

        /// <summary>
        /// Test the apply until extension method.
        /// </summary>
        [Fact]
        public void IOperatorExtension_FuncApplyUntilWithMaxShouldStopAtMax()
        {
            Func<IntegerCandidate, bool> oper = (c) => 
            {
                c.Value--;
                return c.Value > 0;
            };

            oper = oper.ApplyUntil(100);

            var can = new IntegerCandidate(1000);
            Assert.True(oper(can));
            Assert.Equal(900, can.Value);
        }

        /// <summary>
        /// Test the iteration extension method.
        /// </summary>
        [Fact]
        public void IOperatorExtension_FuncIterateShouldRepeat()
        {
            Func<IntegerCandidate, bool> oper = (c) => 
            {
                c.Value--;
                return true;
            };

            oper = oper.Iterate(100);

            var can = new IntegerCandidate(1000);
            Assert.True(oper(can));
            Assert.Equal(900, can.Value);
        }

        /// <summary>
        /// Test the apply until extension method.
        /// </summary>
        [Fact]
        public void IOperatorExtension_ApplyUntilShouldApplyUntil()
        {
            Operator<IntegerCandidate> oper = new FuncOperator<IntegerCandidate>((c) => 
            {
                c.Value--;
                return c.Value > 0;
            });

            oper = oper.ApplyUntil();

            var can = new IntegerCandidate(1000);
            Assert.True(oper.Apply(can));
            Assert.Equal(0, can.Value);
        }

        /// <summary>
        /// Test the apply until extension method.
        /// </summary>
        [Fact]
        public void IOperatorExtension_ApplyUntilWithMaxShouldStopAtMax()
        {
            Operator<IntegerCandidate> oper = new FuncOperator<IntegerCandidate>((c) => 
            {
                c.Value--;
                return c.Value > 0;
            });

            oper = oper.ApplyUntil(100);

            var can = new IntegerCandidate(1000);
            Assert.True(oper.Apply(can));
            Assert.Equal(900, can.Value);
        }

        /// <summary>
        /// Test the iteration extension method.
        /// </summary>
        [Fact]
        public void IOperatorExtension_IterateShouldRepeat()
        {
            Operator<IntegerCandidate> oper = new FuncOperator<IntegerCandidate>((c) => 
            {
                c.Value--;
                return true;
            });

            oper = oper.Iterate(100);

            var can = new IntegerCandidate(1000);
            Assert.True(oper.Apply(can));
            Assert.Equal(900, can.Value);
        }
    }
}