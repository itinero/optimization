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
using Xunit;

namespace Itinero.Optimization.Tests
{
    /// <summary>
    /// A class containing extra asserts.
    /// </summary>
    public static class ExtraAssert
    {
        /// <summary>
        ///  Verifies that all items in the two given enumerables are equal. If they are not, then an NUnit.Framework.AssertionException is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ItemsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected == null &&
                actual == null)
            { // both null, nothing to do.
                return;
            }
            if (actual == null)
            {
                throw new Exception("Actual enumerable is null, expected a non-null enumerable.");
            }
            if (expected == null)
            {
                throw new Exception("Expected enumerable is null, expected a non-null enumerable.");
            }
            using (var expectedEnumerator = expected.GetEnumerator())
            using (var actualEnumerator = actual.GetEnumerator())
            {
                while (expectedEnumerator.MoveNext())
                {
                    if (!actualEnumerator.MoveNext())
                    {
                        throw new Exception("Actual enumerable less elements than expected.");
                    }

                    Assert.Equal(expectedEnumerator.Current, actualEnumerator.Current);
                }

                if (actualEnumerator.MoveNext())
                {
                    throw new Exception("Actual enumerable more elements than expected.");
                }
            }
        }
    }
}