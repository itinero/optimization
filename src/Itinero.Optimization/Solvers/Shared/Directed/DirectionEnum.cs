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

namespace Itinero.Optimization.Solvers.Shared.Directed
{
    /// <summary>
    /// Enumerates possible directions at a visit.
    /// </summary>
    public enum DirectionEnum : byte
    {
        /// <summary>
        /// Backward arrival or departure.
        /// </summary>
        Backward = 0,
        /// <summary>
        /// Forward arrival or departure.
        /// </summary>
        Forward = 1,
    }

    /// <summary>
    /// Contains extensions methods for the direction enum.
    /// </summary>
    public static class DirectionEnumExtensions
    {
        /// <summary>
        /// Builds the weight id for the given visit and given direction.
        /// </summary>
        /// <param name="visit">The visit.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The weight id.</returns>
        public static int WeightId(this DirectionEnum direction, int visit)
        {
            return (visit * 2) + (byte) direction;
        }
    }
}