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

using NUnit.Framework;
using OsmSharp.Logistics.Solvers.GA;
using OsmSharp.Math.Random;

namespace OsmSharp.Logistics.Tests.Solvers.GA
{
    /// <summary>
    /// Tests the tournament selection operator.
    /// </summary>
    [TestFixture]
    public class TournamentSelectionOperatorTests
    {
        /// <summary>
        /// Basic selection test.
        /// </summary>
        [Test]
        public void Test1()
        {
            // set a randomizer to control the result of the tournament selector.
            StaticRandomGenerator.Set(new NotSoRandomGenerator(
                new double[] { 0.6, 0.2, 0.8 }, new int[] { 0, 2, 3 }));

            // create population and selector.
            var population = new Individual<int>[] { 
                new Individual<int>() { Fitness = 10, Solution = 10 },
                new Individual<int>() { Fitness = 1, Solution = 1 },
                new Individual<int>() { Fitness = 3, Solution = 3 },
                new Individual<int>() { Fitness = 4, Solution = 4 } };
            var selector = new TournamentSelectionOperator<int, int>(50, 0.5);

            Assert.AreEqual(2, selector.Select(0, population, null));
            Assert.AreEqual(-1, selector.Select(0, population, null));
            Assert.AreEqual(3, selector.Select(0, population, null));
            Assert.AreEqual(-1, selector.Select(0, population, null));
            Assert.AreEqual(0, selector.Select(0, population, null));
            Assert.AreEqual(-1, selector.Select(0, population, null));

            // reset the randomizer.
            StaticRandomGenerator.Reset();
        }
    }
}