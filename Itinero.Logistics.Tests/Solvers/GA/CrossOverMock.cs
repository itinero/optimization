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

using Itinero.Logistics.Solvers.GA;
using System;

namespace Itinero.Logistics.Tests.Solvers.GA
{
    /// <summary>
    /// A mockup of a crossover operator.
    /// </summary>
    class CrossOverMock : ICrossOverOperator<float, ProblemMock, ObjectiveMock, SolutionMock>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return "MOCK_CROSSOVER"; }
        }

        /// <summary>
        /// Applies this operator using the given solutions and produces a new solution.
        /// </summary>
        /// <returns></returns>
        public SolutionMock Apply(ProblemMock problem, ObjectiveMock objective, SolutionMock solution1, SolutionMock solution2, out float fitness)
        {
            if (solution1.Value < solution2.Value)
            {
                fitness = solution1.Value + Algorithms.RandomGeneratorExtensions.GetRandom().Generate(
                    solution2.Value - solution1.Value);
            }
            else
            {
                fitness = solution2.Value + Algorithms.RandomGeneratorExtensions.GetRandom().Generate(
                    solution1.Value - solution2.Value);
            }
            return new SolutionMock()
            {
                Value = fitness
            };
        }
    }
}