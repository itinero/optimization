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

using Itinero.Logistics.Objective;
using Itinero.Logistics.Routes;

namespace Itinero.Logistics.Solutions.TSPTW
{
    /// <summary>
    /// Abstract representation of a basic TSPTW-objective.
    /// </summary>
    public abstract class TSPTWObjective<T> : ObjectiveBase<ITSPTW<T>, IRoute, float>
        where T : struct
    {
        /// <summary>
        /// Calculates the fitness of a TSPTW solution.
        /// </summary>
        /// <returns></returns>
        public override abstract float Calculate(ITSPTW<T> problem, IRoute solution);

        /// <summary>
        /// Calculates the fitness of a TSP solution as if the given TSPTW problem was just a TSP instance.
        /// </summary>
        /// <returns></returns>
        public abstract float CalculateTSP(TSP.ITSP<T> problem, IRoute solution);

        /// <summary>
        /// Executes the shift-after and returns the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public abstract bool ShiftAfter(ITSPTW<T> problem, IRoute route, int customer, int before, out float difference);

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public abstract float IfShiftAfter(ITSPTW<T> problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter);

        /// <summary>
        /// Executes the shift-after and returns the difference between the solution before the shift and after the shift.
        /// </summary>
        /// <returns></returns>
        public abstract bool ShiftAfterTSP(TSP.ITSP<T> problem, IRoute route, int customer, int before, out float difference);

        /// <summary>
        /// Returns the difference in fitness 'if' the shift-after would be executed with the given settings.
        /// </summary>
        /// <returns></returns>
        public abstract float IfShiftAfterTSP(TSP.ITSP<T> problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter);

        /// <summary>
        /// Gets a fitness value that represent the highest possible value.
        /// </summary>
        public override sealed float Infinite
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Gets a fitness value that represent zero.
        /// </summary>
        public override sealed float Zero
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds two fitness value.
        /// </summary>
        public override sealed float Add(ITSPTW<T> problem, float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }

        /// <summary>
        /// Compares fitness1 to fitness2 and returns -1 if fitness1 is better, 0 if equal and 1 if fitness2 is better.
        /// </summary>
        public override sealed int CompareTo(ITSPTW<T> problem, float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public override sealed bool IsZero(ITSPTW<T> problem, float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Substracts a fitness value from another.
        /// </summary>
        public override sealed float Subtract(ITSPTW<T> problem, float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }
    }
}