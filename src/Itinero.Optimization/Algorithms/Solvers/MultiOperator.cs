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
using System;
using System.Text;

namespace Itinero.Optimization.Algorithms.Solvers
{
    /// <summary>
    /// Combines multiple operators into one by executing them sequentially.
    /// </summary>
    public class MultiOperator<TWeight, TProblem, TObjective, TSolution, TFitness> : IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TWeight : struct
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        private readonly IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>[] _operators;

        /// <summary>
        /// Creates a new multi operator.
        /// </summary>
        public MultiOperator(params IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>[] operators)
        {
            _operators = operators;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                var names = new StringBuilder();
                names.Append("MULTI{");
                for(var i = 0; i < _operators.Length; i++)
                {
                    if (i > 0)
                    {
                        names.Append(',');
                    }
                    names.Append(_operators[i].Name);
                }
                names.Append('}');
                return names.ToInvariantString();
            }
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
        {
            var success = false;
            delta = objective.Zero;
            for (var i = 0; i < _operators.Length; i++)
            {
                TFitness localDelta;
                if (_operators[i].Apply(problem, objective, solution, out localDelta))
                {
                    delta = objective.Add(problem, localDelta, delta);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        public bool Supports(TObjective objective)
        {
            for (var i = 0; i < _operators.Length; i++)
            {
                if (!_operators[i].Supports(objective))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
