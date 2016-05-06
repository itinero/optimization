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

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// Extension methods for the ISTSPObjective.
    /// </summary>
    public static class ISTSPObjectiveExtensions
    {
        /// <summary>
        /// Converts the ISTSPObjective to an equivalent TSPObjective.
        /// </summary>
        public static ITSPObjective ToTSPObjective(this ISTSPObjective istspObjective)
        {
            return new STSPObjectiveAsTSPObjective(istspObjective);
        }
        
        private class STSPObjectiveAsTSPObjective : ITSPObjective
        {
            private readonly ISTSPObjective _o;

            public STSPObjectiveAsTSPObjective(ISTSPObjective o)
            {
                _o = o;
            }

            public string Name
            {
                get
                {
                    return _o.Name;
                }
            }

            public double Calculate(ITSP problem, IRoute solution)
            {
                return _o.Calculate(problem.ToSTSP(), solution);
            }

            public double IfShiftAfter(ITSP problem, IRoute route, int customer, int before, int oldBefore, int oldAfter, int newAfter)
            {
                return _o.IfShiftAfter(problem.ToSTSP(), route, customer, before, oldBefore, oldAfter, newAfter);
            }

            public bool ShiftAfter(ITSP problem, IRoute route, int customer, int before, out double difference)
            {
                return _o.ShiftAfter(problem.ToSTSP(), route, customer, before, out difference);
            }
        }
    }
}