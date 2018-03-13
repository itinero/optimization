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

using Itinero.Optimization.Abstract.Solvers.TSP;

namespace Itinero.Optimization.Abstract.Tours.Operations
{
    /// <summary>
    /// Contains general shiftafter operations.
    /// </summary>
    public static class ShiftAfter
    {
        /// <summary>
        /// Shifts the given customer after the given 'before' customer and returns the difference in fitness between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public static bool Do(float[][] weights, Tour tour, int customer, int before, out float difference)
        {
            // shift after and keep all info.
            int oldBefore, oldAfter, newAfter;
            if (!tour.ShiftAfter(customer, before, out oldBefore, out oldAfter, out newAfter))
            { // shift did not succeed.
                difference = 0;
                return false;
            }

            if (oldAfter == Constants.END)
            {
                oldAfter = tour.First;
            }
            if (newAfter == Constants.END)
            {
                newAfter = tour.First;
            }

            difference = -weights[oldBefore][customer]
                    - weights[customer][oldAfter]
                    + weights[oldBefore][oldAfter]
                    - weights[before][newAfter]
                    + weights[before][customer]
                    + weights[customer][newAfter];
            return true;
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public static float If(ITSProblem problem, Tours.ITour tour, int customer, int before, int oldBefore, int oldAfter, int newAfter)
        {
            if (oldAfter == Constants.END)
            {
                oldAfter = tour.First;
            }
            if (newAfter == Constants.END)
            {
                newAfter = tour.First;
            }

            return -problem.Weight(oldBefore, customer)
                    - problem.Weight(customer,oldAfter)
                    + problem.Weight(oldBefore,oldAfter)
                    - problem.Weight(before,newAfter)
                    + problem.Weight(before,customer)
                    + problem.Weight(customer,newAfter);
        }

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public static float If(float[][] weights, Tours.ITour tour, int customer, int before, int oldBefore, int oldAfter, int newAfter)
        {
            if (oldAfter == Constants.END)
            {
                oldAfter = tour.First;
            }
            if (newAfter == Constants.END)
            {
                newAfter = tour.First;
            }

            return -weights[oldBefore][customer]
                    - weights[customer][oldAfter]
                    + weights[oldBefore][oldAfter]
                    - weights[before][newAfter]
                    + weights[before][customer]
                    + weights[customer][newAfter];
        }

        /// <summary>
        /// Pretends to shift the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="tour">The tour to shift in.</param>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <returns>The enumerable that represents the new route.</returns>
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     customer:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        /// </remarks>
        /// <exception cref="System.ArgumentException">When customer equals before.</exception>
        public static ShiftedAfterTour GetShiftedAfter(this ITour tour, int customer, int before)
        {
            return new ShiftedAfterTour(tour, customer, before);
        }
    }
}