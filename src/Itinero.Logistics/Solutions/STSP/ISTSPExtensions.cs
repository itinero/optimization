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
        public static ITSP<T> ToTSP<T>(this ISTSP<T> itsp)
            where T : struct
        {
            var weights = new T[itsp.Weights.Length][];
            for (var i = 0; i < itsp.Weights.Length; i++)
            {
                weights[i] = itsp.Weights[i].Clone() as T[];
            }
            if (itsp.Last.HasValue)
            {
                return new TSPProblem<T>(itsp.WeightHandler, itsp.First, itsp.Last.Value, weights);
            }
            return new TSPProblem<T>(itsp.WeightHandler, itsp.First, weights);
        }

        /// <summary>
        /// Converts to an STSP.
        /// </summary>
        public static ISTSP<T> ToSTSP<T>(this ITSP<T> tsp)
            where T : struct
        {
            return tsp.ToSTSP(tsp.WeightHandler.Infinity);
        }

        /// <summary>
        /// Converts to an STSP.
        /// </summary>
        public static ISTSP<T> ToSTSP<T>(this ITSP<T> tsp, T max)
            where T : struct
        {
            var weights = new T[tsp.Weights.Length][];
            for (var i = 0; i < tsp.Weights.Length; i++)
            {
                weights[i] = tsp.Weights[i].Clone() as T[];
            }
            if (tsp.Last.HasValue)
            {
                return new STSPProblem<T>(tsp.WeightHandler, tsp.First, tsp.Last.Value, weights, max);
            }
            return new STSPProblem<T>(tsp.WeightHandler, tsp.First, weights, max);
        }

        /// <summary>
        /// Generates an emtpy route.
        /// </summary>
        public static Route EmptyRoute<T>(this ISTSP<T> problem)
            where T : struct
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
                return new Logistics.Routes.Route(new int[] { problem.First }, null);
            }
        }
    }
}