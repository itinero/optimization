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
    /// A mockup of a local search procedure for a very simple problem, reduce a number to zero.
    /// </summary>
    public class LocalSearchMock : IOperator<ProblemMock, SolutionMock>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "MOCK_LOCALSEARCH"; }
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(ProblemMock problem, SolutionMock solution, out double delta)
        {
            var fitnessBefore = solution.Value;
            var reduction = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(problem.Max / 50);
            if (reduction < problem.Max / 1000)
            { // mock the operator failing to find better solution.
                delta = double.MaxValue;
                return false;
            }
            if(reduction > fitnessBefore)
            { // ok reduce to zero, problem solved.
                delta = fitnessBefore;
                solution.Value = 0;
                return true;
            }
            solution.Value = solution.Value - reduction;
            delta = fitnessBefore - solution.Value;
            return true;
        }
    }
}