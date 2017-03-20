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

using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Test.Algorithms.Solvers
{
    /// <summary>
    /// A mockup of an objective.
    /// </summary>
    class ObjectiveMock : ObjectiveBase<ProblemMock, SolutionMock, float>
    {
        /// <summary>
        /// Gets a fitness value that represent the highest possible value.
        /// </summary>
        public override float Infinite
        {
            get
            {
                return float.MaxValue;
            }
        }

        /// <summary>
        /// Gets the non-lineair flag, affects using deltas.
        /// </summary>
        public override bool IsNonContinuous
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public override string Name
        {
            get
            {
                return "MOCK";
            }
        }

        /// <summary>
        /// Gets a fitness value that represent zero.
        /// </summary>
        public override float Zero
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds two fitness value.
        /// </summary>
        public override float Add(ProblemMock problem, float fitness1, float fitness2)
        {
            return fitness1 + fitness2;
        }

        /// <summary>
        /// Calculates the fitness of the given solution.
        /// </summary>
        public override float Calculate(ProblemMock problem, SolutionMock solution)
        {
            return solution.Value;
        }

        /// <summary>
        /// Compares fitness1 to fitness2 and returns -1 if fitness1 is better, 0 if equal and 1 if fitness2 is better.
        /// </summary>
        public override int CompareTo(ProblemMock problem, float fitness1, float fitness2)
        {
            return fitness1.CompareTo(fitness2);
        }

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public override bool IsZero(ProblemMock problem, float fitness)
        {
            return fitness == 0;
        }

        /// <summary>
        /// Substracts a fitness value from another.
        /// </summary>
        public override float Subtract(ProblemMock problem, float fitness1, float fitness2)
        {
            return fitness1 - fitness2;
        }
    }
}
