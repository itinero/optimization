// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;

namespace OsmSharp.Logistics.Solvers
{
    /// <summary>
    /// A wrapper for an operator, replacing the objective with another objective on each call.
    /// </summary>
    public class OperatorAndObjective<TProblem, TObjective, TSolution> : IOperator<TProblem, TObjective, TSolution>
    {
        private readonly IOperator<TProblem, TObjective, TSolution> _operator;
        private readonly TObjective _objective;

        /// <summary>
        /// Creates a new operator and objective wrapper.
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="objective"></param>
        public OperatorAndObjective(IOperator<TProblem, TObjective, TSolution> oper, TObjective objective)
        {
            _operator = oper;
            _objective = objective;
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
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="objective">The objective.</param>
        /// <param name="solution">The solution.</param>
        /// <param name="delta">The difference in fitness, when > 0 there was an improvement and a reduction in fitness.</param>
        /// <returns></returns>
        public bool Apply(TProblem problem, TObjective objective, TSolution solution, out double delta)
        {
            return _operator.Apply(problem, _objective, solution, out delta);
        }
    }
}
