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

using BenchmarkDotNet.Attributes;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Solvers.Tours;
using System;
using System.Collections.Generic;
using Itinero.Optimization.Solvers.CVRP_ND;
using System.IO;
using System.Text;
using Itinero.Optimization.Solvers.CVRP_ND.SCI;

namespace Itinero.Optimization.Tests.Benchmarks.Solvers.CVRP_ND.SCI
{
    public class SeededCheapestInsertionBenchmark
    {
        private readonly CVRPNDProblem _problem;

        public SeededCheapestInsertionBenchmark()
        {
            Itinero.Optimization.IO.Json.JsonSerializer.ToJsonFunc = Newtonsoft.Json.JsonConvert.SerializeObject;
            Itinero.Optimization.IO.Json.JsonSerializer.FromJsonFunc = Newtonsoft.Json.JsonConvert.DeserializeObject;

            using (var stream = typeof(SeededCheapestInsertionBenchmark).Assembly.GetManifestResourceStream("Itinero.Optimization.Tests.Benchmarks.data.models.model-spijkenisse-5400.json"))
            using (var textReader = new StreamReader(stream))
            {
                var json = textReader.ReadToEnd();
                var model = MappedModel.FromJson(json);
                _problem = model.TryToCVRPND().Value;
            }
        }

        [Benchmark]
        public object SolveModel1Spijkenisse5400()
        {
            return SeededCheapestInsertionStrategy.Default.Search(_problem);
        }
    }
}
