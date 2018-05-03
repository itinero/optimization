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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Itinero.Optimization.Tests")]
[assembly: InternalsVisibleTo("Itinero.Optimization.Tests.Benchmarks")]
namespace Itinero.Optimization.Strategies
{
    /// <summary>
    /// Contains functionality to compare candidates.
    /// </summary>
    internal static class CandidateComparison
    {
        /// <summary>
        /// Compares two candidates.
        /// </summary>
        /// <param name="candidate1">The first candidate.</param>
        /// <param name="candidate2">The second candidate.</param>
        /// <param name="comparison">A custom comparison function, if any.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. 
        ///  The return value has these meanings: 
        ///  Value Meaning Less than zero This instance precedes other in the sort order. 
        ///  Zero This instance occurs in the same position in the sort order as other. 
        ///  Greater than zero This instance follows other in the sort order.</returns>
        internal static int Compare<TCandidate>(TCandidate candidate1, TCandidate candidate2, Comparison<TCandidate> comparison = null)
        {
            // first try comparison function.
            if (comparison != null)
            {
                return comparison(candidate1, candidate2);
            }

            // try generic comparable.
            var candidate1ComparableGen = candidate1 as IComparable<TCandidate>;
            if (candidate1ComparableGen != null)
            {
                return candidate1ComparableGen.CompareTo(candidate2);
            }

            // try comparable.
            var candidate1Comparable = candidate1 as IComparable;
            if (candidate1Comparable != null)
            {
                return candidate1Comparable.CompareTo(candidate2);
            }

            throw new InvalidOperationException("The two given candidates cannot be compared, no method of comparison found.");
        }
    }
}