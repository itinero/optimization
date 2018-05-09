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

using Itinero.Optimization.Abstract.Solvers.VRP.NoDepot.Capacitated;
using Itinero.Optimization.Test.Staging;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using Itinero.Optimization.Models.Costs;
using Itinero.Optimization.Models.Vehicles.Constraints;

namespace Itinero.Optimization.Test.Abstract.Solvers.VRP.NoDepot.Capacitated
{
    /// <summary>
    /// Tests related to the no-depot CVRP solver details.
    /// </summary>
    [TestFixture]
    public class NoDepotCVRPSolverDetailsTests
    {
        /// <summary>
        /// Regression tests for model1, solving this caused the solver to hang.
        /// </summary>
        [Test]
        public void TestRegressionModel1()
        {
            var model = "data.regression.abstract.model1.json".GetAbstractModel();
            var result = NoDepotCVRPSolverDetails.TryToNoDepotCVRP(model);

            var solution = result.Value.Solve();
        }

        /// <summary>
        /// Regression tests for model2, solving this caused the solver to hang.
        /// </summary>
        [Test]
        public void TestRegressionModel2()
        {
            var model = "data.regression.abstract.model2.json".GetAbstractModel();
            var result = NoDepotCVRPSolverDetails.TryToNoDepotCVRP(model);

            var solution = result.Value.Solve();
        }
    }
}