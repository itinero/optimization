using System;
using BenchmarkDotNet.Running;

namespace Itinero.Optimization.Tests.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
//            BenchmarkRunner.Run<Strategies.CandidateComparisonBenchmark>();
//            BenchmarkRunner.Run<Solvers.TSP.EAX.EAXSolverBenchmark>();
//            BenchmarkRunner.Run<Solvers.Tours.Sequences.SequenceEnumerableTests>();
//            BenchmarkRunner.Run<Solvers.CVRP_ND.SCI.SeededCheapestInsertionBenchmark>();

            //var sciTests = new Solvers.CVRP_ND.SCI.SeededCheapestInsertionBenchmark();
            //for (var i = 0; i < 100; i++)
            //{
            //    sciTests.SolveModel1Spijkenisse5400();
            //}
//            
            var tourPoolTest = new Solvers.CVRP_ND.SeededTours.SeededTourPoolBenchmark();
            tourPoolTest.BuildTourPoolForModel1Spijkenisse5400();
            
 //           var tourStrategyTest = new Solvers.CVRP_ND.SeededTours.SeededTourStrategyBenchmark();
 //           tourStrategyTest.SolveModel1Spijkenisse5400();

            //Console.ReadLine();
        }
    }
}
