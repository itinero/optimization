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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSP;

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// Extension methods for the ISTSP.
    /// </summary>
    public static class ISTSPExtensions
    {
        /// <summary>
        /// Converts to an equivalent TSP.
        /// </summary>
        public static ITSP ToTSP(this ISTSP itsp)
        {
            var weights = new double[itsp.Weights.Length][];
            for (var i = 0; i < itsp.Weights.Length; i++)
            {
                weights[i] = itsp.Weights[i].Clone() as double[];
            }
            if (itsp.Last.HasValue)
            {
                return new TSPProblem(itsp.First, itsp.Last.Value, itsp.Weights);
            }
            return new TSPProblem(itsp.First, itsp.Weights);
        }

        /// <summary>
        /// Converts to an STSP.
        /// </summary>
        public static ISTSP ToSTSP(this ITSP tsp, float max = float.MaxValue)
        {
            var weights = new double[tsp.Weights.Length][];
            for (var i = 0; i < tsp.Weights.Length; i++)
            {
                weights[i] = tsp.Weights[i].Clone() as double[];
            }
            if (tsp.Last.HasValue)
            {
                return new STSPProblem(tsp.First, tsp.Last.Value, tsp.Weights, max);
            }
            return new STSPProblem(tsp.First, tsp.Weights, max);
        }

        /// <summary>
        /// Generates an emtpy route.
        /// </summary>
        public static Route EmptyRoute(this ISTSP problem)
        {
            if (problem.Last.HasValue)
            {
                if (problem.First == problem.Last)
                {
                    return new Logistics.Routes.Route(new int[] { problem.First }, problem.Last);
                }
                return new Logistics.Routes.Route(new int[] { problem.First, problem.Last.Value }, problem.Last);
            }
            else
            {
                return new Logistics.Routes.Route(new int[] { problem.First });
            }
        }
    }
}