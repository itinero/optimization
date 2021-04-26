// using Itinero.Optimization.Solvers;
// using Itinero.Optimization.Solvers.Shared;
// using Itinero.Optimization.Solvers.Shared.Directed;
// using Itinero.Optimization.Solvers.Tours;
// using Itinero.Optimization.Solvers.TSP_TW_D;
// using Itinero.Optimization.Solvers.TSP_TW_D.Operators;
// using Xunit;
//
// namespace Itinero.Optimization.Tests.Solvers.TSP_TW_D.Operators
// {
//     public class Local2OptOperatorTests
//     {
//         [Fact]
//         public void Local2OptOperator_ShouldExecutePossibleMove()
//         {
//             var weights = WeightMatrixHelpers.BuildDirected(5, 10);
//             weights.SetDirectedWeights(0,1,1);
//             weights.SetDirectedWeights(1,2,1);
//             weights.SetDirectedWeights(2,3,1);
//             weights.SetDirectedWeights(3,4,1);
//             weights.SetDirectedWeights(4,0,1);
//             weights.SetDirectedWeights(3,1,100);
//             var windows = new TimeWindow[5];
//             var problem = new TSPTWDProblem(0, 0, weights, 2, windows);
//             
//             var tour = new Tour(new int[] { 0, 3, 2, 1, 4 }, 0);
//
//             // apply operator, should detect possible move.
//             var fitness = tour.Fitness(problem);
//             var candidate = new Candidate<TSPTWDProblem, Tour>()
//             {
//                 Problem = problem,
//                 Solution = tour,
//                 Fitness = fitness
//             };
//             Assert.True(Local2OptOperator.Default.Apply(candidate));
//
//             // test result.
//             Assert.Equal(5, candidate.Fitness);
//             Assert.Equal(new int[] { 0, 1, 2, 3, 4 }, tour);
//         }
//     }
// }