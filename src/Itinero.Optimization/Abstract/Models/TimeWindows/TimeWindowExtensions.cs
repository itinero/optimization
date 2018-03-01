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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.Abstract.Models.TimeWindows
{
    /// <summary>
    /// Contains extension methods related to time windows.
    /// </summary>
    public static class TimeWindowExtensions
    {
        /// <summary>
        /// Calculates the total minimum diff.
        /// </summary>
        public static float TotalMinDiff<T>(this Tour tour, float[][] times, TimeWindow[] windows)
            where T : struct
        {
            var total = 0f;
            var seconds = 0f;
            var previous = Constants.NOT_SET;
            var enumerator = tour.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (previous != Constants.NOT_SET)
                { // keep track of time.
                    seconds += times[previous][current];
                }
                total += windows[current].MinDiff(seconds);
                previous = current;
            }
            return total;
        }
    }
}