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
        public int PopulationSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the maximum generations.
        /// </summary>
        public int MaxGenerations { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the stagnation count.
        /// </summary>
        public int StagnationCount { get; set; } = 10;

        /// <summary>
        /// Gets or sets the elitism in percentage.
        /// </summary>
        public float ElitismPercentage { get; set; } = 3;

        /// <summary>
        /// Gets or sets the crossover percentage.
        /// </summary>
        public float CrossOverPercentage { get; set; } = 30;

        /// <summary>
        /// Gets or sets the mutation percentage.
        /// </summary>
        public float MutationPercentage { get; set; } = 10;

        /// <summary>
        /// Gets or sets the improvement percentage.
        /// </summary>
        public float ImprovementPercentage { get; set; } = 1;

        /// <summary>
        /// Returns default GA settings.
        /// </summary>
        public static GASettings Default => new GASettings();
    }
}