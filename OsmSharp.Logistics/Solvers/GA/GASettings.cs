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

namespace OsmSharp.Logistics.Solvers.GA
{
    /// <summary>
    /// Represents common settings for a GA.
    /// </summary>
    public class GASettings
    {
        /// <summary>
        /// Gets or sets the population size.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum generations.
        /// </summary>
        public int MaxGenerations { get; set; }

        /// <summary>
        /// Gets or sets the stagnation count.
        /// </summary>
        public int StagnationCount { get; set; }

        /// <summary>
        /// Gets or sets the elitism in percentage.
        /// </summary>
        public int ElitismPercentage { get; set; }

        /// <summary>
        /// Gets or sets the crossover percentage.
        /// </summary>
        public int CrossOverPercentage { get; set; }

        /// <summary>
        /// Gets or sets the mutation percentage.
        /// </summary>
        public int MutationPercentage { get; set; }

        /// <summary>
        /// Returns default GA settings.
        /// </summary>
        public static GASettings Default
        {
            get
            {
                return new GASettings()
                {
                    MaxGenerations = 1000,
                    PopulationSize = 100,
                    StagnationCount = 30,
                    ElitismPercentage = 3,
                    MutationPercentage = 10,
                    CrossOverPercentage = 30
                };
            }
        }
    }
}