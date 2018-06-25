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

using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Solvers.Shared.Random1Shift
{
    /// <summary>
    /// Implements the random 1 shift operation: Relocates a random visit in a tour by shifting it after another random visit.
    /// </summary>
    internal static class Random1ShiftOperation
    {
        /// <summary>
        /// Shifts a randomly selected visit after another randomly selected visit.
        /// </summary>
        /// <param name="tour">The tour.</param>
        public static void DoRandom1Shift(this Tour tour)
        {
            var count = tour.Count;
            if (tour.Last.HasValue)
            {
                count--;
            }
            
            RandomGenerator.Generate2(count, out var moved, out var newBefore);

            // TODO: perhaps for some problems without a visit list this can be optimized, also getting the two at the same time is faster.
            moved = tour.GetVisitAt(moved);
            newBefore = tour.GetVisitAt(newBefore);

            tour.ShiftAfter(newBefore, moved);
        }

        /// <summary>
        /// Shifts a randomly selected visit after another randomly selected visit.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="moved">The visit that was moved.</param>
        /// <param name="oldBefore">The visit right before the moved one before the move.</param>
        /// <param name="oldAfter">The visit right after the moved one before the move.</param>
        /// <param name="newBefore">The visit right before the moved one after the move.</param>
        /// <param name="newAfter">The visit right after the moved one after the move.</param>
        public static void DoRandom1Shift(this Tour tour, out int moved, out int oldBefore, out int oldAfter, out int newBefore, out int newAfter)
        {
            var count = tour.Count;
            if (tour.Last.HasValue)
            {
                count--;
            }
            
            RandomGenerator.Generate2(count, out moved, out newBefore);

            // TODO: perhaps for some problems without a visit list this can be optimized, also getting the two at the same time is faster.
            moved = tour.GetVisitAt(moved);
            newBefore = tour.GetVisitAt(newBefore);

            tour.ShiftAfter(newBefore, moved, out oldBefore, out oldAfter, out newAfter);
        }
    }
}