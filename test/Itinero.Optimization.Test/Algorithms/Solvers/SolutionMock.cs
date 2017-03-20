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

using Itinero.Optimization.Algorithms;

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// A mockup of a solution that consists of a single double.
    /// </summary>
    class SolutionMock : ISolution
    {
        /// <summary>
        /// Creates a new solution mock.
        /// </summary>
        public SolutionMock()
        {

        }

        /// <summary>
        /// Creates a new mock solution.
        /// </summary>
        /// <param name="value"></param>
        public SolutionMock(float value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Clones this solution.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new SolutionMock()
            {
                Value = this.Value
            };
        }

        /// <summary>
        /// Copies the solution from the given solution to this one.
        /// </summary>
        /// <param name="solution"></param>
        public void CopyFrom(ISolution solution)
        {
            this.Value = (solution as SolutionMock).Value;
        }
    }
}
