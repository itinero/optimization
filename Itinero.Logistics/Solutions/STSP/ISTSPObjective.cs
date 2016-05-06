// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Routes;

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// Abstract representation of a basic STSP-objective.
    /// </summary>
    public interface ISTSPObjective
    {
        /// <summary>
        /// Returns the name of this objective.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Calculates the fitness of a STSP solution.
        /// </summary>
        /// <returns></returns>
        double Calculate(ISTSP problem, IRoute solution);

        /// <summary>
        /// Executes the shift-after and returns the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        bool ShiftAfter(ISTSP problem, IRoute route, int customer, int before, out double difference);

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        double IfShiftAfter(ISTSP problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter);
    }
}