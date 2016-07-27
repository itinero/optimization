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

namespace Itinero.Logistics.Fitness
{
    /// <summary>
    /// Contains extension methods for the fitness handler.
    /// </summary>
    public static class FitnessHandlerExtensions
    {

        /// <summary>
        /// Returns true if fitness1 is better than the fitness2.
        /// </summary>
        public static bool IsBetterThan<TFitness>(this FitnessHandler<TFitness> handler, TFitness fitness1, TFitness fitness2)
        {
            return handler.CompareTo(fitness1, fitness2) > 0;
        }

        /// <summary>
        /// Adds the given fitnesses together.
        /// </summary>
        /// <typeparam name="TFitness"></typeparam>
        public static TFitness Add<TFitness>(this FitnessHandler<TFitness> handler, TFitness fitness1, TFitness fitness2, TFitness fitness3)
        {
            var fitness = handler.Add(fitness1, fitness2);
            return handler.Add(fitness, fitness3);
        }
    }
}