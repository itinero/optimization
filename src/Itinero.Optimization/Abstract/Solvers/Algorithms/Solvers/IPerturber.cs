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
    /// Abstract representation of an operator on the current solution that 'scrables' or changes the solution in some random way depending on a given 'level'.
    /// </summary>
    public interface IPerturber<TWeight, TProblem, TObjective, TSolution, TFitness> : IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TWeight : struct
    {
        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="level">The level.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        bool Apply(TProblem problem, TObjective objective, TSolution solution, int level, out TFitness delta);
    }

    /// <summary>
    /// Wraps an operator and uses it as a perturber, ignoring the level-parameter.
    /// </summary>
    public class OperatorAsPerturber<TWeight, TProblem, TObjective, TSolution, TFitness> : IPerturber<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
        where TWeight : struct
    {
        private readonly IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> _operator;

        /// <summary>
        /// Creates a new operator-as-perturber.
        /// </summary>
        public OperatorAsPerturber(IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> oper)
        {
            _operator = oper;
        }

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return _operator.Name; }
        }

        /// <summary>
        /// Returns true if the given object is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TObjective objective)
        {
            return _operator.Supports(objective);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness, when > 0 there was an improvement and a reduction in fitness.</param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
        {
            return _operator.Apply(problem, objective, solution, out delta);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="level">The level.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, int level, out TFitness delta)
        {
            return this.Apply(problem, objective, solution, out delta);
        }
    }
}