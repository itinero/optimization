// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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
