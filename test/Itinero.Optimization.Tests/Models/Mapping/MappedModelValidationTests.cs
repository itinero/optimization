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
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Xunit;

namespace Itinero.Optimization.Tests.Models.Mapping
{
    public class MappedModelValidationTests
    {
        /// <summary>
        /// Tests if a model is indeed invalid without travel costs.
        /// </summary>
        [Fact]
        public void MappedModel_ShouldBeInvalidWhenNotTravelCosts()
        {
            var model = new MappedModel {
                VehiclePool = new VehiclePool()
                {
                    Reusable = true,
                    Vehicles = new []
                    {
                        new Vehicle()
                        {
                            Metric = Metrics.Time,
                            Profile = "Car"
                        }
                    }
                },
                Visits = new Visit[]
                {
                    new Visit()
                    {
                        Latitude = 1,
                        Longitude = 2
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("No travel costs found.", reason);
        }
        
        /// <summary>
        /// Tests if a model is indeed invalid with a vehicle without travel costs.
        /// </summary>
        [Fact]
        public void MappedModel_ShouldBeInvalidWithVehicleThatHasNoTravelCosts()
        {
            var model = new MappedModel {
                VehiclePool = new VehiclePool()
                {
                    Reusable = true,
                    Vehicles = new []
                    {
                        new Vehicle()
                        {
                            Metric = Metrics.Time,
                            Profile = "Car"
                        }
                    }
                },
                Visits = new []
                {
                    new Visit()
                    {
                        Latitude = 1,
                        Longitude = 2
                    }
                },
                TravelCosts = new []
                {
                    new TravelCostMatrix()
                    {
                        Costs = new []
                        {
                            new [] { 0f, 1f },
                            new [] { 2f, 3f } 
                        },
                        Metric = Metrics.Distance
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("A vehicle with metric 'time' was found but no travel costs exist for that metric.", reason);
        }
    }
}