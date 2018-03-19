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

using Itinero.Optimization.Models;
using NUnit.Framework;

namespace Itinero.Optimization.Test.Models
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

            var model = new Model()
            {
                TimeWindows = new Optimization.Models.TimeWindows.TimeWindow[]
                {
                    new Optimization.Models.TimeWindows.TimeWindow()
                    {
                        Min = 1,
                        Max = 10
                    },
                    new Optimization.Models.TimeWindows.TimeWindow()
                    {
                        Min = 1,
                        Max = 10
                    },
                    new Optimization.Models.TimeWindows.TimeWindow()
                    {
                        Min = 1,
                        Max = 10
                    }
                },
                VehiclePool = new Optimization.Models.Vehicles.VehiclePool()
                {
                    Vehicles = new Optimization.Models.Vehicles.Vehicle[]
                    {
                        new Optimization.Models.Vehicles.Vehicle()
                        {
                            Arrival = null,
                            Departure = null,
                            CapacityConstraints = new Optimization.Models.Vehicles.Constraints.CapacityConstraint[]
                            {
                                new Optimization.Models.Vehicles.Constraints.CapacityConstraint()
                                {
                                    Capacity = 100,
                                    Name = "weight"
                                }
                            },
                            Profile = "truck",
                            TurnPentalty = 0
                        }
                    }
                },
                VisitCosts = new Optimization.Models.Costs.VisitCosts[]
                {
                    new Optimization.Models.Costs.VisitCosts()
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
                Visits = new LocalGeo.Coordinate[]
                {
                    new LocalGeo.Coordinate()
                    {
                        Latitude = 12.23458f,
                        Longitude = 13.2375f
                    },
                    new LocalGeo.Coordinate()
                    {
                        Latitude = 12.23458f,
                        Longitude = 13.2375f
                    },
                    new LocalGeo.Coordinate()
                    {
                        Latitude = 12.23458f,
                        Longitude = 13.2375f
                    }
                }
            };

            var json = model.ToJson();
            Assert.AreEqual("{\"Visits\":[{\"Latitude\":12.23458,\"Longitude\":13.2375,\"Elevation\":null},{\"Latitude\":12.23458,\"Longitude\":13.2375,\"Elevation\":null},{\"Latitude\":12.23458,\"Longitude\":13.2375,\"Elevation\":null}],\"TimeWindows\":[{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0}],\"VisitCosts\":[{\"Name\":\"weight\",\"Costs\":[10.0,10.0,10.0]}],\"VehiclePool\":{\"Vehicles\":[{\"Profile\":\"truck\",\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0,\"CapacityConstraints\":[{\"Name\":\"weight\",\"Capacity\":100.0}]}],\"Reusable\":false}}",
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

            var json = "{\"Visits\":[{\"Latitude\":12.23458,\"Longitude\":13.2375,\"Elevation\":null},{\"Latitude\":12.23458,\"Longitude\":13.2375,\"Elevation\":null},{\"Latitude\":12.23458,\"Longitude\":13.2375,\"Elevation\":null}],\"TimeWindows\":[{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0},{\"Min\":1.0,\"Max\":10.0}],\"VisitCosts\":[{\"Name\":\"weight\",\"Costs\":[10.0,10.0,10.0]}],\"VehiclePool\":{\"Vehicles\":[{\"Profile\":\"truck\",\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0,\"CapacityConstraints\":[{\"Name\":\"weight\",\"Capacity\":100.0}]}],\"Reusable\":false}}";
            var model = Model.FromJson(json);

            Assert.IsNotNull(model);
        }
    }
}