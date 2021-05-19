// using System;
// using Itinero.Optimization.Solvers.Shared.Directed;
// using Itinero.Optimization.Solvers.Tours;
//
// namespace Itinero.Optimization.Solvers.Shared.TimeWindows.Local2Opt.Directed
// {
//     internal static class Local2OptOperation
//     {
//         /// <summary>
//         /// Runs a local 2-opt local search in a first improvement strategy.
//         /// </summary>
//         /// <param name="tour">The tour.</param>
//         /// <param name="weightFunc">The function to get weights.</param>
//         /// <param name="turnPenaltyFunc">The function to get turn penalties.</param>
//         /// <param name="windows">The time windows.</param>
//         /// <returns>True if the operation succeeded and an improvement was found.</returns>
//         /// <remarks>* 2-opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
//         public static bool Do2Opt(this Tour tour, Func<int, int, float> weightFunc, Func<TurnEnum, float> turnPenaltyFunc, TimeWindow[] windows)
//         {
//             // iterate over all pairs and see if they can be exchanged.
//             
//             // calculate for each:
//             //     q1: v1->v2->v3->v4
//             //     q2: v5->v6->v7->v8
//             // exchanges:
//             //         v1->v6->v7->v4
//             //         v5->v2->v3->v8
//
//             float QuadWeight(Quad q)
//             {
//                 return SequenceWeight(q.Visit1, q.Visit2, q.Visit3, q.Visit4);
//             }
//
//             float SequenceWeight(int v1, int v2, int v3, int v4)
//             {
//                 return weightFunc(DirectedHelper.WeightIdDeparture(v1),
//                     DirectedHelper.WeightIdArrival(v2)) +
//                        weightFunc(DirectedHelper.WeightIdDeparture(v2),
//                     DirectedHelper.WeightIdArrival(v3)) + 
//                        weightFunc(DirectedHelper.WeightIdDeparture(v3),
//                     DirectedHelper.WeightIdArrival(v4)) + 
//                        turnPenaltyFunc(DirectedHelper.ExtractTurn(v2)) + 
//                        turnPenaltyFunc(DirectedHelper.ExtractTurn(v3));
//             }
//
//             foreach (var q1 in tour.Quadruplets(true, true))
//             {
//                 if (q1.Visit2 == Tour.NOT_SET && q1.Visit3 == Tour.NOT_SET) continue;
//
//                 var q1Weight = QuadWeight(q1);
//
//                 var q1V2 = DirectedHelper.ExtractVisit(q1.Visit2);
//                 var q1V3 = DirectedHelper.ExtractVisit(q1.Visit3);
//
//                 foreach (var q2 in tour.Quadruplets(true, true))
//                 {
//                     if (q1.Equals(q2)) continue;
//                     if (q2.Visit2 == Tour.NOT_SET && q2.Visit3 == Tour.NOT_SET) continue;
//
//                     var q2Weight = QuadWeight(q2);
//                     
//                     var q2V2 = DirectedHelper.ExtractVisit(q2.Visit2);
//                     var q2V3 = DirectedHelper.ExtractVisit(q2.Visit3);
//
//                     var costBefore = q1Weight + q2Weight;
//
//                     // calculate cost after
//                     var leastCost = float.MaxValue;
//
//                     Quad? bestQ1 = null;
//                     Quad? bestQ2 = null;
//                     
//                     foreach (var q1T2 in TurnEnumExtensions.All)
//                     foreach (var q1T3 in TurnEnumExtensions.All)
//                     foreach (var q2T2 in TurnEnumExtensions.All)
//                     foreach (var q2T3 in TurnEnumExtensions.All)
//                     {
//                         var q1New = new Quad(q1.Visit1, DirectedHelper.BuildVisit(q2V2, q2T2),
//                             DirectedHelper.BuildVisit(q2V3, q2T3), q1.Visit4);
//                         var q2New = new Quad(q2.Visit1, DirectedHelper.BuildVisit(q1V2, q1T2),
//                             DirectedHelper.BuildVisit(q1V3, q1T3), q2.Visit4);
//                         
//                         var costAfter = QuadWeight(q1New) + QuadWeight(q2New);
//
//                         if (!(costAfter < leastCost)) continue;
//                         
//                         leastCost = costAfter;
//                         bestQ1 = q1New;
//                         bestQ2 = q2New;
//                     }
//                     
//                     if (costBefore <= leastCost) continue;
//                     
//                     // take action!
//                     
//
//                     return true;
//                 }
//             }
//             
//             return false;
//         }
//     }
// }