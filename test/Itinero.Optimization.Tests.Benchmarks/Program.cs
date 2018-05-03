using System;
using BenchmarkDotNet.Running;

namespace Itinero.Optimization.Tests.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Strategies.CandidateComparisonBenchmark>();
        }
    }
}
