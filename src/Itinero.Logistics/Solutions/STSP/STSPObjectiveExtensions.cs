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

using System;
using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSP;
using Itinero.Logistics.Weights;

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// Extension methods for the STSPObjective.
    /// </summary>
    public static class STSPObjectiveExtensions
    {
        /// <summary>
        /// Converts the ISTSPObjective to an equivalent TSPObjective.
        /// </summary>
        public static TSPObjective<T> ToTSPObjective<T>(this STSPObjective<T> istspObjective)
            where T : struct
        {
            return new STSPObjectiveAsTSPObjective<T>(istspObjective);
        }
        
        private class STSPObjectiveAsTSPObjective<T> : TSPObjective<T>
            where T : struct
        {
            private readonly STSPObjective<T> _o;

            public STSPObjectiveAsTSPObjective(STSPObjective<T> o)
            {
                _o = o;
            }

            public override string Name
            {
                get
                {
                    return _o.Name;
                }
            }

            public override float Calculate(ITSP<T> problem, IRoute solution)
            {
                return _o.CalculateWeight(problem.ToSTSP(), solution);
            }

            public override float IfShiftAfter(ITSP<T> problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
            {
                return _o.IfShiftAfter(problem.ToSTSP(), route, customer, before, oldBefore, oldAfter, newAfter);
            }

            public override bool ShiftAfter(ITSP<T> problem, IRoute route, int customer, int before, out float difference)
            {
                return _o.ShiftAfter(problem.ToSTSP(), route, customer, before, out difference);
            }
        }
    }
}