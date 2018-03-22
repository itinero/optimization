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

using System;

namespace Itinero.Optimization.Algorithms.Solvers.Objective
{
    /// <summary>
    /// Represents an objective for an algoritm to work towards and is responsible for handling fitness calculations.
    /// </summary>
    public abstract class ObjectiveBase<TProblem, TSolution, TFitness>
    {
        /// <summary>
        /// Gets the name of this objective.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the non-continous flag, affects using deltas.
        /// </summary>
        public abstract bool IsNonContinuous
        {
            get;
        }

        /// <summary>
        /// Calculates the fitness of the given solution.
        /// </summary>
        public abstract TFitness Calculate(TProblem problem, TSolution solution);

        /// <summary>
        /// Gets a fitness value that represent zero.
        /// </summary>
        public abstract TFitness Zero
        {
            get;
        }

        /// <summary>
        /// Gets a fitness value that represent the highest possible value.
        /// </summary>
        public abstract TFitness Infinite
        {
            get;
        }

        /// <summary>
        /// Compares fitness1 to fitness2 and returns 1 if fitness1 is better, 0 if equal and -1 if fitness2 is better.
        /// </summary>
        public abstract int CompareTo(TProblem problem, TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Adds two fitness value.
        /// </summary>
        public abstract TFitness Add(TProblem problem, TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Substracts a fitness value from another.
        /// </summary>
        public abstract TFitness Subtract(TProblem problem, TFitness fitness1, TFitness fitness2);

        /// <summary>
        /// Returns true if the given fitness value is zero.
        /// </summary>
        public abstract bool IsZero(TProblem problem, TFitness fitness);
    }
}
