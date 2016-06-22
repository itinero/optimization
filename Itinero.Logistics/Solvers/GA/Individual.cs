// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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

namespace Itinero.Logistics.Solvers.GA
{
    /// <summary>
    /// Represents an individual in a GA population.
    /// </summary>
    /// <typeparam name="TSolution"></typeparam>
    public struct Individual<TSolution>
    {
        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        public float Fitness { get; set; }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        public TSolution Solution { get; set; }

        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0} - ({1})]", this.Fitness, this.Solution);
        }
    }
}