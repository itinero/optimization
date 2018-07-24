/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

namespace Itinero.Optimization.Strategies.GA
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
        public static GASettings Default => new GASettings()
        {
            MaxGenerations = 1000,
            PopulationSize = 100,
            StagnationCount = 10,
            ElitismPercentage = 3,
            MutationPercentage = 10,
            CrossOverPercentage = 30
        };
    }
}