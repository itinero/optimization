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

namespace Itinero.Optimization.Algorithms.Solvers.Objective
{
    /// <summary>
    /// Contains extension methods for the objective class.
    /// </summary>
    public static class ObjectiveBaseExtensions
    {
        /// <summary>
        /// Returns true if fitness1 is better than the fitness2.
        /// </summary>
        public static bool IsBetterThan<TProblem, TSolution, TFitness>(this ObjectiveBase<TProblem, TSolution, TFitness> objective, TProblem problem, TFitness fitness1, TFitness fitness2)
        {
            return objective.CompareTo(problem, fitness1, fitness2) < 0;
        }

        /// <summary>
        /// Adds the given fitnesses together.
        /// </summary>
        public static TFitness Add<TProblem, TSolution, TFitness>(this ObjectiveBase<TProblem, TSolution, TFitness> objective, TProblem problem, TFitness fitness1, TFitness fitness2, TFitness fitness3)
        {
            var fitness = objective.Add(problem, fitness1, fitness2);
            return objective.Add(problem, fitness, fitness3);
        }
    }
}