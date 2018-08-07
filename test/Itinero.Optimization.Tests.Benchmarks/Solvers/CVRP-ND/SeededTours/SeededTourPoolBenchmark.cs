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

using System.IO;
using BenchmarkDotNet.Attributes;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Solvers.CVRP_ND;
using Itinero.Optimization.Solvers.CVRP_ND.TourSeeded;

namespace Itinero.Optimization.Tests.Benchmarks.Solvers.CVRP_ND.SeededTours
{
    public class SeededTourPoolBenchmark
    {
        private readonly SeededTourPool _pool;

        public SeededTourPoolBenchmark()
        {
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = Newtonsoft.Json.JsonConvert.SerializeObject;
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = Newtonsoft.Json.JsonConvert.DeserializeObject;

            using (var stream = typeof(SeededTourPoolBenchmark).Assembly.GetManifestResourceStream("Itinero.Optimization.Tests.Benchmarks.data.models.model-spijkenisse-5400.json"))
            using (var textReader = new StreamReader(stream))
            {
                var json = textReader.ReadToEnd();
                var model = MappedModel.FromJson(json);
                var problem = model.TryToCVRPND().Value;
                _pool = new SeededTourPool(problem, 50);
            }
        }

        [Benchmark]
        public object BuildTourPoolForModel1Spijkenisse5400()
        {
            _pool.Build();
            return false;
        }
    }
}