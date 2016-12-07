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
using System.Linq;

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// A mockup of a solver that generates solution to the mockup problem of reducing a number to zero.
    /// </summary>
    class GeneratorMock : SolverBase<float, ProblemMock, ObjectiveMock, SolutionMock, float>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "MOCK_GENERATOR"; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override SolutionMock Solve(ProblemMock problem, ObjectiveMock objective, out float fitness)
        {
            var solution = new SolutionMock()
            {
                Value = RandomGeneratorExtensions.GetRandom().Generate(problem.Max)
            };
            fitness = solution.Value;
            return solution;
        }

        /// <summary>
        /// Stops execution.
        /// </summary>
        public override void Stop()
        {

        }
    }
}
