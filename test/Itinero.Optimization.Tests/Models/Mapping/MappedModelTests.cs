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
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Vehicles.Constraints;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Xunit;

namespace Itinero.Optimization.Tests.Models.Mapping
{
    public class MappedModelTests
    {
        /// <summary>
        /// Tests the solver name.
        /// </summary>
        [Fact]
        public void MappedModel_ToJsonShouldReturnDefaultJson()
        {
            // setup json stuff.
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = Newtonsoft.Json.JsonConvert.SerializeObject;
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = Newtonsoft.Json.JsonConvert.DeserializeObject;

            var model = new MappedModel()
            {
                Visits = new[]
                {
                    new Visit()
                    {
                        Latitude = 1.01f,
                        Longitude = 1.02f,
                        TimeWindow = new TimeWindow(),
                        VisitCosts = new[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 100
                            }
                        }
                    },
                    new Visit()
                    {
                        Latitude = 2.01f,
                        Longitude = 2.02f,
                        TimeWindow = new TimeWindow(),
                        VisitCosts = new[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 200
                            }
                        }
                    }
                },
                VehiclePool = new VehiclePool()
                {
                    Vehicles = new[]
                    {
                        new Vehicle()
                        {
                            Arrival = null,
                            Departure = null,
                            Profile = "car",
                            CapacityConstraints = new CapacityConstraint[]
                            {
                                new CapacityConstraint()
                                {
                                    Capacity = 100,
                                    Metric = Metrics.Weight
                                }
                            },
                            Metric = Metrics.Time,
                            TurnPentalty = 0
                        }
                    }
                },
                TravelCosts = new[]
                {
                    new TravelCostMatrix()
                    {
                        Costs = new[]
                        {
                            new[] {0f, 1f},
                            new[] {2f, 3f}
                        }
                    }
                }
            };

            var json = model.ToJson();
            Assert.Equal("{\"TravelCosts\":[{\"Metric\":\"time\",\"Costs\":[[0.0,1.0],[2.0,3.0]],\"Directed\":false}],\"Visits\":[{\"Latitude\":1.01,\"Longitude\":1.02,\"TimeWindow\":[],\"VisitCosts\":[{\"Metric\":\"time\",\"Value\":100.0}]},{\"Latitude\":2.01,\"Longitude\":2.02,\"TimeWindow\":[],\"VisitCosts\":[{\"Metric\":\"time\",\"Value\":200.0}]}],\"VehiclePool\":{\"Vehicles\":[{\"Metric\":\"time\",\"Profile\":\"car\",\"Departure\":null,\"Arrival\":null,\"TurnPentalty\":0.0,\"CapacityConstraints\":[{\"Metric\":\"weight\",\"Capacity\":100.0}]}],\"Reusable\":false}}",
                json);
        }
    }
}