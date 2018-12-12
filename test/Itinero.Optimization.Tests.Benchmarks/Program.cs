using System;
using BenchmarkDotNet.Running;

namespace Itinero.Optimization.Tests.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Solvers.Tours.Hull.QuickHullTests>();
            BenchmarkRunner.Run<Strategies.CandidateComparisonBenchmark>();
            BenchmarkRunner.Run<Solvers.TSP.EAX.EAXSolverBenchmark>();
            BenchmarkRunner.Run<Solvers.Tours.Sequences.SequenceEnumerableTests>();
            BenchmarkRunner.Run<Solvers.CVRP_ND.SCI.SeededCheapestInsertionBenchmark>();

            //Console.ReadLine();
        }
    }
}
