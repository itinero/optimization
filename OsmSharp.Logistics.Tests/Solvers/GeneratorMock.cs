// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logistics.Solvers;

namespace OsmSharp.Logistics.Tests.Solvers
{
    /// <summary>
    /// A mockup of a solver that generates solution to the mockup problem of reducing a number to zero.
    /// </summary>
    public class GeneratorMock : SolverBase<ProblemMock, SolutionMock>
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
        public override SolutionMock Solve(ProblemMock problem, out double fitness)
        {
            var solution = new SolutionMock()
            {
                Value = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(problem.Max)
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