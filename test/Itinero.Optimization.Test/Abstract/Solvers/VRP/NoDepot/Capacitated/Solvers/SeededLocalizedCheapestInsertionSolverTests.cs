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
using Itinero.LocalGeo;
using Itinero.Optimization.General;
using Itinero.Optimization.Test.Staging;
using Itinero.Optimization.Test.Staging.VRP.NoDepot.Capacitated;
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated;
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers;
using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers.Operators;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Abstract.Solvers.VRP.NoDepot.Capacitated.Solvers
{
    /// <summary>
    /// Contains tests for the seeded localized cheapest insertion helper.
    /// </summary>
    [TestFixture]
    public class SeededLocalizedCheapestInsertionSolverTests
    {
        /// <summary>
        /// Tests problem1.
        /// </summary>
        [Test]
        public void TestProblem1()
        {
            var problem = "data.geometric.problem1.geojson".BuildProblem(out var solution, out var locations);

            var solver = new SeededLocalizedCheapestInsertionSolver(problem.SelectSeedHeuristic, 
                (p, t1, t2) => {
                    return locations.ToursOverlap(t1, t2);
                });
            var result = solver.Solve(problem, new NoDepotCVRPObjective(), out var fitness);
            var json = result.BuildFeatures(locations).ToGeoJson();
        }

        /// <summary>
        /// Tests problem2.
        /// </summary>
        [Test]
        public void TestProblem2()
        {
            var problem = "data.geometric.problem2.geojson".BuildProblem(out var solution, out var locations);

            var solver = new SeededLocalizedCheapestInsertionSolver(problem.SelectSeedHeuristic, 
                (p, t1, t2) => {
                    return locations.ToursOverlap(t1, t2);
                });
            var result = solver.Solve(problem, new NoDepotCVRPObjective(), out var fitness);
            var json = result.BuildFeatures(locations).ToGeoJson();
        }
    }
}