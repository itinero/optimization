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

namespace Itinero.Optimization.Algorithms.Solvers
{
    /// <summary>
    /// Abstract representation of a heuristic/solver operator that is applied to a single instance and may lead to better/worse solution.
    /// </summary>
    public interface IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TWeight : struct
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Returns true if the given object is supported.
        /// </summary>
        /// <returns></returns>
        bool Supports(TObjective objective);

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="delta">The difference between the fitness value before and after the operation. The new fitness value can be calculated by subtracting the delta value from the old fitness value. This means a delta > 0 means an improvement in fitness when lower is better.</param>
        /// <returns></returns>
        bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta);
    }
}