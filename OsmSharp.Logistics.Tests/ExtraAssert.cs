// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests
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
        /// <param name="actual"></param>
        /// <param name="expected"></param>
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
