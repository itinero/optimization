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

using OsmSharp.Logistics.Solvers.GA;
using System;
using System.Collections.Generic;

namespace OsmSharp.Logistics.Tests.Solvers.GA
{
    /// <summary>
    /// A mockup of a selection operator.
    /// </summary>
    class SelectionMock : ISelectionOperator<ProblemMock, SolutionMock>
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
        public int Select(ProblemMock problem, Individual<SolutionMock>[] population, ISet<int> exclude)
        {
            // try two and select the best one.
            var selected = -1;
            do 
            {
                var individual1 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(population.Length);
                var individual2 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(population.Length - 1);
                if (individual1 <= individual2)
                { // make sure they are different.
                    individual2++;
                }

                selected = individual2;
                if(population[individual1].Fitness < population[individual2].Fitness)
                {
                    selected = individual1;
                }
            } while (exclude.Contains(selected));
            return selected;
        }
    }
}