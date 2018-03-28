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

using System;
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Abstract.Models.Costs;
using Itinero.Optimization.Abstract.Models.TimeWindows;
using Itinero.Optimization.Abstract.Models.Vehicles;
using Itinero.Optimization.Abstract.Models.Vehicles.Constraints;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Abstract.Models
{
    /// <summary>
    /// Contains tests related to the model.
    /// </summary>
    [TestFixture]
    public class ModelTests
    {
        /// <summary>
        /// Tests converting a model to json.
        /// </summary>
        [Test]
        public void TestToJson()
        {
            // setup json stuff.
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = o =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(o);
            };
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = (o, t) =>
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(o, t);
            };

            var model = new AbstractModel()
            {
                TimeWindows = new TimeWindow[]
                {
                    new TimeWindow()
                    {
                        Min = 1,
                        Max = 10
                    },
                    new TimeWindow()
                    {
                        Min = 1,
                        Max = 10
                    },
                    new TimeWindow()
                    {
                        Min = 1,
                        Max = 10
                    }
                },
                VehiclePool = new VehiclePool()
                {
                    Vehicles = new Vehicle[]
                    {
                        new Vehicle()
                        {
                            Arrival = null,
                            Departure = null,
                            CapacityConstraints = new CapacityConstraint[]
                            {
                                new CapacityConstraint()
                                {
                                    Capacity = 100,
                                    Name = "weight"
                                }
                            },
                            Metric = "time",
                            TurnPentalty = 0
                        }
                    }
                },
                VisitCosts = new VisitCosts[]
                {
                    new VisitCosts()
                    {
                        Name = "weight",
                        Costs = new float[]
                        {
                            10,
                            10,
                            10
                        }
                    }
                },
                TravelCosts = new TravelCostMatrix[]
                {
                    new TravelCostMatrix()
                    {
                        Directed = false,
                        Name = "time",
                        Costs = new float[][]
                        {
                            new float[]
                            {
                                0,
                                10,
                                10
                            },
                            new float[]
                            {
                                10,
                                0,
                                10
                            },
                            new float[]
                            {
                                10,
                                10,
                                0
                            }
                        }
                    }
                }
            };

            var json = model.ToJson();
            Assert.AreEqual("{\"TravelCosts\":[{\"Name\":\"time\",\"Costs\":[[0.0,10.0,10.0],[10.0,0.0,10.0],[10.0,10.0,0.0]],\"Directed\":false}],\"TimeWindows\":[{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0}],\"VisitCosts\":[{\"Name\":\"weight\",\"Costs\":[10.0,10.0,10.0]}],\"VehiclePool\":{\"Vehicles\":[{\"Metric\":\"time\",\"CapacityConstraints\":[{\"Name\":\"weight\",\"Capacity\":100.0}],\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0}],\"Reusable\":false}}",
                json);
        }

        /// <summary>
        /// Tests loading a model from json.
        /// </summary>
        [Test]
        public void TestFromJson()
        {
            // setup json stuff.
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = o =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(o);
            };
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = (o, t) =>
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(o, t);
            };

            var json = "{\"TravelCosts\":[{\"Name\":\"time\",\"Costs\":[[0.0,10.0,10.0],[10.0,0.0,10.0],[10.0,10.0,0.0]],\"Directed\":false}],\"TimeWindows\":[{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0}],\"VisitCosts\":[{\"Name\":\"weight\",\"Costs\":[10.0,10.0,10.0]}],\"VehiclePool\":{\"Vehicles\":[{\"Metric\":\"time\",\"CapacityConstraints\":[{\"Name\":\"weight\",\"Capacity\":100.0}],\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0}],\"Reusable\":false}}";
            var model = AbstractModel.FromJson(json);

            Assert.IsNotNull(model);
        }


        [Test]
        public void TestInsaneRequirements()
        {
            // setup json stuff.
            Console.WriteLine("HELLO WORLD");
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = o =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(o);
            };
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = (o, t) =>
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(o, t);
            };


            var json = "{\"TravelCosts\":[{\"Name\":\"time\",\"Costs\":[[0.0,10.0,10.0],[10.0,0.0,10.0],[10.0,10.0,0.0]],\"Directed\":false}],\"TimeWindows\":[{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0}],\"VisitCosts\":[{\"Name\":\"weight\",\"Costs\":[10.0,10.0,10.0]}],\"VehiclePool\":{\"Vehicles\":[{\"Metric\":\"time\",\"CapacityConstraints\":[{\"Name\":\"weight\",\"Capacity\":100.0}],\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0}],\"Reusable\":false}}";
            var model = AbstractModel.FromJson(json);

            string reason;
            int[] ids;
            bool res = model.SanityCheck(out reason, out ids);



        }
    }
}