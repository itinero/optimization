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

using System.Collections.Generic;

namespace Itinero.Optimization.Solvers.Shared.Collections
{
    /// <summary>
    /// A readonly set interface (Why isn't this in .NET BCL's?).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IReadOnlySet<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Checks if this is a subset of other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a subset of other; false if not</returns>
        bool IsSubsetOf(IEnumerable<T> other);
        
        /// <summary>
        /// Checks if this is a superset of other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a superset of other; false if not</returns>
        bool IsSupersetOf(IEnumerable<T> other);
        
        /// <summary>
        /// Checks if this is a proper superset of other (i.e. other strictly contained in this).
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a proper superset of other; false if not</returns>
        bool IsProperSupersetOf(IEnumerable<T> other);
        
        /// <summary>
        /// Checks if this is a proper subset of other (i.e. strictly contained in).
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a proper subset of other; false if not</returns>
        bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Checks if this set overlaps other (i.e. they share at least one item).
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if these have at least one common element; false if disjoint</returns>
        bool Overlaps(IEnumerable<T> other);
        
        /// <summary>
        /// Checks if this set contains the item
        /// </summary>
        /// <param name="item">item to check for containment</param>
        /// <returns>true if item contained; false if not</returns>
        bool Contains(T item);
    }
}