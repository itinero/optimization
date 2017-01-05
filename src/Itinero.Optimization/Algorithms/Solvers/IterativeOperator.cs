// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Optimization.Algorithms.Solvers.Objective;
using System;

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
            return _operator.Supports(objective);
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
            for(var i = 0; i < _n; i++)
            {
                TFitness localDelta;
                if (_operator.Apply(problem, objective, solution, out localDelta))
                {
                    delta = objective.Add(problem, delta, localDelta);
                    bestFitness = objective.Subtract(problem, bestFitness, localDelta);
                    success = true;
                }
                else if(_stopAtFail)
                { // stop at first fail.
                    break;
                }
            }
            return success;
        }
    }
}
