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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers.GA;
using System.Collections.Generic;

namespace Itinero.Optimization.Test.Algorithms.Solvers.GA
{
    /// <summary>
    /// A mockup of a selection operator.
    /// </summary>
    class SelectionMock : ISelectionOperator<ProblemMock, SolutionMock, ObjectiveMock, float>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return "MOCK_SELECTION"; }
        }

        /// <summary>
        /// Selects a new solution for reproduction.
        /// </summary>
        /// <returns></returns>
        public int Select(ProblemMock problem, ObjectiveMock objective, Individual<SolutionMock, float>[] population, ISet<int> exclude)
        {
            // try two and select the best one.
            var selected = -1;
            do
            {
                var individual1 = RandomGeneratorExtensions.GetRandom().Generate(population.Length);
                var individual2 = RandomGeneratorExtensions.GetRandom().Generate(population.Length - 1);
                if (individual1 <= individual2)
                { // make sure they are different.
                    individual2++;
                }

                selected = individual2;
                if (population[individual1].Fitness < population[individual2].Fitness)
                {
                    selected = individual1;
                }
            } while (exclude.Contains(selected));
            return selected;
        }
    }
}