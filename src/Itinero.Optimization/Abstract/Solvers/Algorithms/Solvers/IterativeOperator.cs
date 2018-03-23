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
using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Algorithms.Solvers
{
    /// <summary>
    /// An iterative operator, executes on operator n-times in a row on the best solution and keeps the best solution around.
    /// </summary>
    public class IterativeOperator<TWeight, TProblem, TObjective, TSolution, TFitness> : IOperator<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TWeight : struct
    where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        private readonly IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> _operator;
        private readonly int _n;
        private readonly bool _stopAtFail;

        /// <summary>
        /// Creates a new iterative operator.
        /// </summary>
        public IterativeOperator(IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> op, int n, bool stopAtFail = false)
        {
            _operator = op;
            _n = n;
            _stopAtFail = stopAtFail;
        }

        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("{0}x{1}", _n, _operator.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Supports(TObjective objective)
        {
            return _operator .Supports(objective);
        }

        /// <summary>
        /// Applies this operation.
        /// </summary>param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
        {
            var best = solution;
            var bestFitness = objective.Calculate(problem, solution);
            delta = objective.Zero;
            var success = false;
            for (var i = 0; i < _n; i++)
            {
                TFitness localDelta;
                if (_operator .Apply(problem, objective, solution, out localDelta))
                {
                    delta = objective.Add(problem, delta, localDelta);
                    bestFitness = objective.Subtract(problem, bestFitness, localDelta);
                    success = true;

                    Itinero.Logging.Logger.Log(this.Name, Itinero.Logging.TraceEventType.Verbose,
                        "Improvement found {0}/{1}: {2}", i + 1, _n, bestFitness);
                }
                else if (_stopAtFail)
                { // stop at first fail.
                    break;
                }
            }
            return success;
        }
    }
}