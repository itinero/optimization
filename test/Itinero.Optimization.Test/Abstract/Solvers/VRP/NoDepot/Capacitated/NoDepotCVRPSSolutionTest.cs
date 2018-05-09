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

using Newtonsoft.Json;
using Itinero.Optimization.Abstract.Models;

namespace Itinero.Optimization.Test.Abstract.Solvers.VRP.NoDepot.Capacitated
{

    /// <summary>
    /// Tests related to the no-depot CVRP solution.
    /// </summary>
    [TestFixture]
    public class NoDepotCVRPSolutionTests
    {
        float capacityMax = 100f;

        public NoDepotCVRProblem createProblem(int? depot = null)
        {


            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = o =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(o);
            };
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = (o, t) =>
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(o, t);
            };

            var json = "{\"TravelCosts\":[{\"Name\":" +
                "\"time\",\"Costs\":[" +
                    "[0.0 , 10.0, 5.0 , 40.0]," +
                    "[10.0, 0.0 , 10.0, 20.0]," +
                    "[5.0 , 10.0, 0.0 , 20.0]," +
                    "[40.0, 20.0, 20.0, 0.0 ]" +

                "],\"Directed\":false}]," +
                "\"VisitCosts\":[{\"Name\":\"weight\",\"Costs\":[0.0,10.0,10.0, 10.0]}]," +
                "\"VehiclePool\":{\"Vehicles\":[{\"Metric\":\"time\",\"CapacityConstraints\":[{\"Name\":\"weight\",\"Capacity\":100.0}],\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0}],\"Reusable\":false}}";
            var model = AbstractModel.FromJson(json);
            log(model.ToJson());
            var capacity = new Capacity()
            {
                Max = capacityMax
            };
            var ndcvrp = new NoDepotCVRProblem(capacity, model.TravelCosts[0].Costs, model.VisitCosts[0].Costs, depot);
            return ndcvrp;
        }

        public NoDepotCVRPObjective createObjective(NoDepotCVRProblem ndpr)
        {
            Func<NoDepotCVRProblem, IList<int>, int> seedFunc = (prob, visits) => visits[0];
            var obj = new NoDepotCVRPObjective(seedFunc, null);
            return obj;
        }

        public NoDepotCVRPSolution createSolution()
        {
            return new NoDepotCVRPSolution(0);
        }

        [Test]
        public void TestCapacityConstraints()
        {

            clearLog();

            var p = createProblem(null);
            Assert.AreEqual(capacityMax, p.Capacity.Max);

            p = createProblem(0); // visit cost of 0 is 0 
            Assert.AreEqual(capacityMax, p.Capacity.Max);

            p = createProblem(1); // visit cost of 1 is 10 
            Assert.AreEqual(capacityMax - 10, p.Capacity.Max);

        }


        [Test]
        public void TestWeightOf()
        {
            clearLog();

            var problem = createProblem(0);
            var obj = createObjective(problem);
            var sol = createSolution();

            Assert.Zero(sol.CalculateTotalWeight());

            sol.Add(problem, 3);
            var t0 = sol.Tour(0);

            t0.InsertAfter(3, 2);
            Assert.AreEqual(60, sol.CalculateWeightOf(problem, 0));

            int pos = sol.CalculateDepotPosition(problem, 0, out float cost, null);
            log("Cheapest pos: " + pos.ToString() + ", cost: " + cost);

            Assert.AreEqual(3, pos);



            t0.InsertAfter(2, 1);
            Assert.AreEqual(80, sol.CalculateWeightOf(problem, 0));


            log(sol.Tour(0).ToInvariantString());
            log(sol.CalculateWeightOf(problem, 0).ToString());

            Assert.AreEqual(sol.Tour(0).First, 3);


            pos = sol.CalculateDepotPosition(problem, 0, out cost, null);
            log("Cheapest pos: " + pos.ToString() + ", cost: " + cost);

            Assert.AreEqual(2, pos);
            Assert.AreEqual(5.0, cost);

            // And I thought java could have it bad...
            var removed = new Optimization.Abstract.Solvers.VRP.Operators.Seq(new Optimization.Abstract.Tours.Sequences.Sequence(new[] { 3, 2, 1 }));
            pos = sol.CalculateDepotPosition(problem, 0, out cost, removed);
            log("Cheapest pos: " + pos.ToString() + ", cost: " + cost);

            Assert.AreNotEqual(2, pos);

            t0.ReplaceEdgeFrom(3, 1); // cut out 2

            cost = sol.SimulateDepotCost(problem, out pos, 0, placedVisit: 2, after: 3);
            log("Simulated Cheapest pos: " + pos.ToString() + ", cost: " + cost);
            Assert.AreEqual(2, pos);
            Assert.AreEqual(5, cost);

            cost = sol.SimulateDepotCost(problem, out pos, 0, placedVisit: 2, after: 1);
            log("Simulated Cheapest pos: " + pos.ToString() + ", cost: " + cost);

            log("[Done]");
        }


        var path = "/home/pietervdvn/Desktop/TestLog.txt";
        private void log(String msg)
        {
            try
            {

                File.AppendAllText(path, msg + "\n");
            }
            catch (Exception e) { }
        }

        private void clearLog()
        {
            try
            {

                File.WriteAllText(path, "/home/pietervdvn/Desktop/TestLog.txt", "Testlog\n-------\n");
            }
            catch (Exception e)
            {

            }
        }

    }
}