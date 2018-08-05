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
