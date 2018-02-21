/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;
using System.IO;
using Itinero.LocalGeo;
using Itinero.Optimization.Tours;
using Itinero.Optimization.VRP.NoDepot.Capacitated;
using NetTopologySuite.Features;

namespace Itinero.Optimization.Test.Staging.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// A few helper function to help with staging.
    /// </summary>
    public static class StagingHelper
    {
        /// <summary>
        /// Builds a problem from the given geojson embedded resource.
        /// </summary>
        /// <param name="embeddedResourcePath">The embedded resource to use.</param>
        /// <param name="solution">The solution, if already represented in the file.</param>
        /// <returns>A problem based on the points in the file.</returns>
        public static NoDepotCVRProblem BuildProblem(this string embeddedResourcePath,
            out NoDepotCVRPSolution solution, out List<Coordinate> locations)
        {
            var features = embeddedResourcePath.GetFeatureCollection();
            
            // builds the weight matrix.
            List<ITour> tours;
            IAttributesTable attributes;
            var weights = features.BuildMatrix(out tours, out locations, out attributes);

            // try get the max property.
            int max;
            if (!attributes.TryGetValueInt32("max", out max))
            {
                max = -1;
            }

            // build the problem.
            var problem = new NoDepotCVRProblem()
            {
                Capacity = new Capacity()
                {
                    Max = max
                },
                Weights = weights
            };
            var objective = new NoDepotCVRPObjective();

            // generate random extra constraint that's always satisfied.
            var values = new float[weights.Length];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = Itinero.Optimization.Algorithms.Random.RandomGeneratorExtensions.GetRandom().Generate(100) + 20;
            }
            problem.Capacity.Constraints = new MaxCapacityConstraint[]
            {
                new MaxCapacityConstraint()
                {
                    Max = 120 * weights.Length + 100,
                    Name = "something",
                    Values = values
                }
            };

            // build a solution if any.
            solution = null;
            if (tours != null &&
                tours.Count > 0)
            {
                solution = new NoDepotCVRPSolution(weights.Length);
                foreach (var tour in tours)
                {
                    solution.Add(tour);
                    solution.Contents.Add(new Optimization.VRP.NoDepot.Capacitated.Solvers.CapacityExtensions.Content()
                    {
                        Weight = objective.Calculate(problem, solution, solution.Count - 1)
                    });
                }
            }

            return problem;
        }
    }
}