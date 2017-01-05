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

using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// A mockup of a perturber procedure for a very simple problem, reduce a number to zero.
    /// </summary>
    class PerturberMock : IPerturber<float, ProblemMock, ObjectiveMock, SolutionMock, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "MOCK_PERTURBER"; }
        }

        /// <summary>
        /// Returns true if the given object is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(ObjectiveMock objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ProblemMock problem, ObjectiveMock objective, SolutionMock solution, out float delta)
        {
            return this.Apply(problem, objective, solution, 1, out delta);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ProblemMock problem, ObjectiveMock objective, SolutionMock solution, int level, out float delta)
        {
            var before = solution.Value;
            var after = RandomGeneratorExtensions.GetRandom().Generate(problem.Max);
            solution.Value = after;
            delta = before - after; // when improvement, after is lower, delta > 0
            return delta > 0;
        }
    }
}
