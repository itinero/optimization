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

namespace Itinero.Optimization.Algorithms.Solvers.GA
{
    /// <summary>
    /// Represents an individual in a GA population.
    /// </summary>
    public struct Individual<TSolution, TFitness>
    {
        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        public TFitness Fitness { get; set; }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        public TSolution Solution { get; set; }

        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0} - ({1})]", this.Fitness, this.Solution);
        }
    }
}