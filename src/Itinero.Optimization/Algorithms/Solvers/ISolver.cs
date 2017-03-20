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

namespace Itinero.Optimization.Algorithms.Solvers
{
    /// <summary>
    /// Abstract representation of a solver.
    /// </summary>
    public interface ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        TSolution Solve(TProblem problem, TObjective objective);

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        TSolution Solve(TProblem problem, TObjective objective, out TFitness fitness);

        /// <summary>
        /// Stops the executing of the solving process.
        /// </summary>
        void Stop();

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        event SolverDelegates.IntermidiateDelegate<TSolution> IntermidiateResult;
    }

    /// <summary>
    /// Common delegates for solver implementations.
    /// </summary>
    public static class SolverDelegates
    {
        /// <summary>
        /// A delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate<TSolution>(TSolution result);

        /// <summary>
        /// A delegate for a stop condition.
        /// </summary>
        /// <returns></returns>
        public delegate bool StopConditionDelegate<TProblem, TObjective, TSolution>(int iteration, TProblem problem,
            TObjective objective, TSolution solution);

        /// <summary>
        /// A delegate for a stop condition.
        /// </summary>
        /// <returns></returns>
        public delegate bool StopConditionWithLevelDelegate<TProblem, TObjective, TSolution>(int iteration, int level,
            TProblem problem, TObjective objective, TSolution solution);
    }
}