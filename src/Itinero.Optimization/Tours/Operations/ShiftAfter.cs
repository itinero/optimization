// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

namespace Itinero.Optimization.Tours.Operations
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
    }
}