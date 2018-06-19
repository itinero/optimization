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
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;
using Xunit;

namespace Itinero.Optimization.Tests.Models
{
    public class ModelValidationTests
    {
        /// <summary>
        /// Tests is a model is indeed invalid without visits.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithoutVisits()
        {
            var model = new Model {
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
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("No visits defined.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid without a vehicle pool.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithoutAVehiclePool()
        {
            var model = new Model {Visits = new Visit[0]};

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("No vehicle pool defined.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid with a null-visit.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithNullVisit()
        {
            var model = new Model
            {
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
                    null
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid visit: Visit at index 0 is null.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid with an invalid time window.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithInvalidTimeWindow()
        {
            var model = new Model
            {
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
                        Longitude = 2,
                        TimeWindow = new TimeWindow()
                        {
                            Min = 100,
                            Max = 10
                        }
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid visit: Time window for visit at index 0 invalid: Max has to be >= min: [100, 10]", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid with a null visit cost.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithNullVisitCost()
        {
            var model = new Model
            {
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
                        Longitude = 2,
                        TimeWindow = new TimeWindow()
                        {
                            Min = 10,
                            Max = 100
                        },
                        VisitCosts = new VisitCost[]
                        {
                            null
                        }
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid visit: Visit at index 0 has a visit cost at index 0 that is null.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid with an invalid visit cost.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithInvalidVisitCost()
        {
            var model = new Model
            {
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
                        Longitude = 2,
                        TimeWindow = new TimeWindow()
                        {
                            Min = 10,
                            Max = 100
                        },
                        VisitCosts = new VisitCost[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = -1f
                            }
                        }
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid visit: Visit at index 0 has a visit cost at index 0 that has a value < 0.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid with a duplicate visit cost.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithDuplicateVisitCost()
        {
            var model = new Model
            {
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
                        Longitude = 2,
                        TimeWindow = new TimeWindow()
                        {
                            Min = 10,
                            Max = 100
                        },
                        VisitCosts = new VisitCost[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 100f
                            },
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 100f
                            }
                        }
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid visit: Visit at index 0 has a visit cost at index 1 that has a duplicate metric time.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid without any vehicles in the pool.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithoutVehiclesInPool()
        {
            var model = new Model
            {
                VehiclePool = new VehiclePool(),
                Visits = new Visit[]
                {
                    new Visit()
                    {
                        Latitude = 1,
                        Longitude = 2,
                        TimeWindow = new TimeWindow()
                        {
                            Min = 10,
                            Max = 100
                        },
                        VisitCosts = new VisitCost[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 100f
                            }
                        }
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid vehicle pool: No vehicles defined in vehicle pool, at least one vehicle is required.", reason);
        }

        /// <summary>
        /// Tests if a model is indeed invalid with a vehicle without a metric.
        /// </summary>
        [Fact]
        public void Model_ShouldBeInvalidWithVehicleWithoutMetric()
        {
            var model = new Model
            {
                VehiclePool = new VehiclePool()
                {
                    Reusable = true,
                    Vehicles = new []
                    {
                        new Vehicle()
                        {
                            Profile = "Car"
                        }
                    }
                },
                Visits = new Visit[]
                {
                    new Visit()
                    {
                        Latitude = 1,
                        Longitude = 2,
                        TimeWindow = new TimeWindow()
                        {
                            Min = 10,
                            Max = 100
                        },
                        VisitCosts = new VisitCost[]
                        {
                            new VisitCost()
                            {
                                Metric = Metrics.Time,
                                Value = 100f
                            }
                        }
                    }
                }
            };

            Assert.False(model.IsValid(out var reason));
            Assert.Equal("Invalid vehicle pool: Vehicle at index 0 is invalid: Vehicle doesn't have a metric.", reason);
        }
    }
}