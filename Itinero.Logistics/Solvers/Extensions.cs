// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
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
        public static bool ApplyUntil<TProblem, TObjective, TSolution>(this IOperator<TProblem, TObjective, TSolution> oper, 
            TProblem problem, TObjective objective, TSolution solution, out double delta)
        {
            delta = 0;
            var localDelta = 0.0;
            while (oper.Apply(problem, objective, solution, out localDelta))
            {
                delta += localDelta;
            }
            return delta > 0;
        }
    }
}