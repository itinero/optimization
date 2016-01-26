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

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solvers;
using OsmSharp.Math.Random;
using System;

namespace OsmSharp.Logistics.Solutions.TSP.Random
{
    /// <summary>
    /// An operator to execute n random 1-shift* relocations.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere.</remarks>
    public class Random1Shift : IPerturber<ITSP, ITSPObjective, IRoute>
    {
        private readonly IRandomGenerator _random = OsmSharp.Logistics.Extensions.GetNewRandom();

        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "RAN_1SHFT"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(ITSPObjective objective)
        {
            return objective.Name == MinimumWeightObjective.MinimumWeightObjectiveName;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSP problem, ITSPObjective objective, IRoute route, out double difference)
        {
            return this.Apply(problem, objective, route, 1, out difference);
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(ITSP problem, ITSPObjective objective, IRoute route, int level, out double difference)
        {
            difference = 0;
            while (level > 0)
            {
                // remove random customer after another random customer.
                var customer = _random.Generate(problem.Weights.Length);
                var insert = _random.Generate(problem.Weights.Length - 1);
                if (insert >= customer)
                { // customer is the same of after.
                    insert++;
                }

                double shiftDiff;
                if (!objective.ShiftAfter(problem, route, customer, insert, out shiftDiff))
                {
                    throw new Exception(
                        string.Format("Failed to shift customer {0} after {1} in route {2}.", customer, insert, route.ToInvariantString()));
                }
                difference += shiftDiff;

                // decrease level.
                level--;
            }
            return difference < 0;
        }
    }
}