using System;
using BenchmarkDotNet.Running;

namespace Itinero.Optimization.Tests.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Strategies.CandidateComparisonBenchmark>();
            BenchmarkRunner.Run<Solvers.TSP.EAX.EAXSolverBenchmark>();
        }
    }
}
