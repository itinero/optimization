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

using Itinero.Optimization.TSP.Directed.Solvers;
using Itinero.Optimization.TSP.Directed.Solvers.Operators;
using NUnit.Framework;

namespace Itinero.Optimization.Test.TSP.Directed.Solvers.Operators
{
    /// <summary>
    /// Contains tests for the direction local search operator.
    /// </summary>
    [TestFixture]
    public class DirectionLocalSearchTest
    {
        /// <summary>
        /// Tests on an open route.
        /// </summary>
        [Test]
        public void TestOpen()
        {
            var problem = TSPHelper.CreateDirectedTSP(0, 10, 100, 5);
            var random = new RandomSolver();
            var randomRoute = random.Solve(problem, new Optimization.TSP.Directed.TSPObjective());

            var op = new DirectionLocalSearch();
            float delta;
            var optimized = op.Apply(problem, new Optimization.TSP.Directed.TSPObjective(), randomRoute, out delta);


        }
    }
}