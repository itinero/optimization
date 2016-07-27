// Itinero.Logistics - Route optimization for .NET
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

using Itinero.Logistics.Objective;

namespace Itinero.Logistics.Solvers
{
    /// <summary>
    /// Contains extension methods for solvers and operators.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Apply the operator until no more improvements can be found.
        /// </summary>
        /// <returns></returns>
        public static bool ApplyUntil<TWeight, TProblem, TObjective, TSolution, TFitness>(this IOperator<TWeight, TProblem, TObjective, TSolution, TFitness> oper, 
            TProblem problem, TObjective objective, TSolution solution, out TFitness delta)
            where TObjective : ObjectiveBase<TFitness>
            where TWeight : struct
        {
            var fitnessHandler = objective.FitnessHandler;
            delta = fitnessHandler.Zero;
            var localDelta = delta;
            while (oper.Apply(problem, objective, solution, out localDelta))
            {
                delta = fitnessHandler.Add(delta, localDelta);
            }
            return !fitnessHandler.IsZero(delta);
        }
    }
}