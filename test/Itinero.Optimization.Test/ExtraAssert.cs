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

using NUnit.Framework;
using System.Collections.Generic;

namespace Itinero.Optimization.Test
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
        public static void ItemsAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected == null &&
                actual == null)
            { // both null, nothing to do.
                return;
            }
            if (actual == null)
            {
                throw new NUnit.Framework.AssertionException("Actual enumerable is null, expected a non-null enumerable.");
            }
            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();
            while (expectedEnumerator.MoveNext())
            {
                if (!actualEnumerator.MoveNext())
                {
                    throw new NUnit.Framework.AssertionException("Actual enumerable less elements than expected.");
                }
                Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current,
                    "At least one element in the actual enumerator is different from the element at the same position in the expected enumerator.");
            }
            if (actualEnumerator.MoveNext())
            {
                throw new NUnit.Framework.AssertionException("Actual enumerable more elements than expected.");
            }
        }
    }
}