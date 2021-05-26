using System;
using System.Collections.Generic;
using Itinero.Algorithms.Weights;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;

namespace Itinero.Optimization.Solvers.Shared.LocalSearch.Local2Opt
{
    internal static class Local2OptDirectedOperation
    {
        /// <summary>
        /// Gets all possible unique pairs of edges, but returned as quadruplets to be able to take into account turning weights.
        ///
        /// Each pair of edges occurs only ones, if (edge1, edge2) occurs, (edge2, edge1) won't.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <returns>An enumerable of edge pairs.</returns>
        public static IEnumerable<(Quad edge1, Quad edge2)> GetDisjointEdgePairsDirected(this Tour tour)
        {
            var seqOuter = 0;
            foreach (var edge1 in tour.Quadruplets(true, true))
            {
                var seqInner = 0;
                foreach (var edge2 in tour.Quadruplets(true, true))
                {
                    if (seqInner <= seqOuter)
                    {
                        seqInner++;
                        continue;
                    }
                    seqInner++;
                    
                    if (edge1.Visit2 == Tour.NOT_SET) continue;
                    if (edge1.Visit3 == Tour.NOT_SET) continue;
                    if (edge2.Visit2 == Tour.NOT_SET) continue;
                    if (edge2.Visit3 == Tour.NOT_SET) continue;

                    if (edge1.Visit3 == edge2.Visit2) continue;
                    if (edge1.Visit3 == edge2.Visit3) continue;
                    if (edge1.Visit2 == edge2.Visit3) continue;
                    if (edge1.Visit2 == edge2.Visit2) continue;

                    yield return (edge1, edge2);
                }

                seqOuter++;
            }
        }

        /// <summary>
        /// Enumerates all 2-Opt moves that lead to an improvement in the form of quadruplets. The turns are optimized at visit2 and visit3.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="weightFunc">The weight function. Accepts a quadruplet (visit1 -> visit2 -> visit3 -> visit4) and returns
        /// the weight between ]visit1 -> visit2 -> visit3 -> visit4[ (excluding the weights of the turns at visit1 and visit4).</param>
        /// <returns>A 2-Opt move, if any.</returns>
        public static IEnumerable<((Quad edge1, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge1Turns, Quad edge2, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge2Turns) move, float weightDecrease)>
            Get2OptImprovementsDirected(this Tour tour,
                Func<Quad, float> weightFunc)
        {
            foreach (var (q1, q2) in tour.GetDisjointEdgePairsDirected())
            {
                var currentWeight = weightFunc(q1) +
                                    weightFunc(q2);
                
                // consider edge1 -> edge2.
                var bestWeight = float.MaxValue;
                (TurnEnum visit2Turn, TurnEnum visit3Turn) best1Turns = default;
                (TurnEnum visit2Turn, TurnEnum visit3Turn) best2Turns = default;
                foreach (var q1T2 in TurnEnumExtensions.All)
                foreach (var q1T3 in TurnEnumExtensions.All)
                foreach (var q2T2 in TurnEnumExtensions.All)
                foreach (var q2T3 in TurnEnumExtensions.All)
                {
                    var newSeq1 = new Quad(q1.Visit1, 
                        DirectedHelper.OverwriteTurn(q1.Visit2, q1T2), 
                        DirectedHelper.OverwriteTurn(q2.Visit2, q2T2), q2.Visit1);
                    var newSeq2 = new Quad(q1.Visit4, 
                        DirectedHelper.OverwriteTurn(q1.Visit3, q1T3),
                        DirectedHelper.OverwriteTurn(q2.Visit3, q2T3), q2.Visit4);
                    
                    var weight = weightFunc(newSeq1) +
                                   weightFunc(newSeq2);
                    if (!(weight < bestWeight)) continue;
                    
                    bestWeight = weight;
                    best1Turns = (q1T2, q1T3);
                    best2Turns = (q2T2, q2T3);
                }

                if (bestWeight < currentWeight)
                { // an improvement was found.
                    yield return ((q1, best1Turns, q2, best2Turns), 
                        currentWeight - bestWeight);
                }

                // consider edge2 -> edge1.
                bestWeight = float.MaxValue;
                best1Turns = default;
                best2Turns = default;
                foreach (var q1T2 in TurnEnumExtensions.All)
                foreach (var q1T3 in TurnEnumExtensions.All)
                foreach (var q2T2 in TurnEnumExtensions.All)
                foreach (var q2T3 in TurnEnumExtensions.All)
                {
                    var newSeq1 = new Quad(q2.Visit1, 
                        DirectedHelper.OverwriteTurn(q2.Visit2, q2T2), 
                        DirectedHelper.OverwriteTurn(q1.Visit2, q1T2), q1.Visit1);
                    var newSeq2 = new Quad(q2.Visit4, 
                        DirectedHelper.OverwriteTurn(q2.Visit3, q2T3),
                        DirectedHelper.OverwriteTurn(q1.Visit3, q1T3), q1.Visit4);
                    
                    var edge1To2 = weightFunc(newSeq1) +
                                   weightFunc(newSeq2);
                    if (!(edge1To2 < bestWeight)) continue;
                    
                    bestWeight = edge1To2;
                    best1Turns = (q1T2, q1T3);
                    best2Turns = (q2T2, q2T3);
                }

                if (bestWeight < currentWeight)
                { // an improvement was found.
                    // possibly the turn of visit1 in q1 has changed.
                    yield return ((q2, best2Turns, q1, best1Turns), 
                        currentWeight - bestWeight);
                }
            }
        }

        /// <summary>
        /// Selects the best move, the one with the highest decrease in weight, from the given sequence.
        /// </summary>
        /// <param name="moves">The moves.</param>
        /// <returns>The best move, if any.</returns>
        public static ((Quad edge1, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge1Turns, Quad edge2, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge2Turns) move, float weightDecrease)? Best(
            this IEnumerable<((Quad edge1, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge1Turns, Quad edge2, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge2Turns) move, float weightDecrease)> moves)
        {
            ((Quad edge1, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge1Turns, Quad edge2, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge2Turns) move, float weightDecrease)? bestMove = null;

            foreach (var move in moves)
            {
                if (bestMove == null ||
                    move.weightDecrease > bestMove.Value.weightDecrease)
                {
                    bestMove = move;
                }
            }

            return bestMove;
        }

        /// <summary>
        /// Applies the given 2-opt move to the current tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <param name="move">The move.</param>
        public static void Apply2OptDirected(this Tour tour, 
            (Quad edge1, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge1Turns, Quad edge2, (TurnEnum visit2Turn, TurnEnum visit3Turn) edge2Turns) move)
        {
            var edge1 = move.edge1;
            var edge2 = move.edge2;

            var x0 = edge1.Visit1;
            var x1 = edge1.Visit2;
            var x1NewTurn = move.edge1Turns.visit2Turn;
            var x1Visit = DirectedHelper.ExtractVisit(x1);
            var x2 = edge1.Visit3;
            var x2NewTurn = move.edge1Turns.visit3Turn;
            var x2Visit = DirectedHelper.ExtractVisit(x2);
            var x3 = edge1.Visit4;

            var y0 = edge2.Visit1;
            var y1 = edge2.Visit2;
            var y1NewTurn = move.edge2Turns.visit2Turn;
            var y1Visit = DirectedHelper.ExtractVisit(y1);
            var y2 = edge2.Visit3;
            var y2NewTurn = move.edge2Turns.visit3Turn;
            var y2Visit = DirectedHelper.ExtractVisit(y2);
            var y3 = edge2.Visit4;
            
            // build new turn aware visits.
            var newX0 = x0;
            var x0Visit = DirectedHelper.ExtractVisit(x0);
            if (x0Visit == y1Visit) newX0 = DirectedHelper.BuildVisit(x0Visit, y1NewTurn);
            if (x0Visit == y2Visit) newX0 = DirectedHelper.BuildVisit(x0Visit, y2NewTurn);
            var newX1 = DirectedHelper.OverwriteTurn(x1, move.edge1Turns.visit2Turn);
            var newX2 = DirectedHelper.OverwriteTurn(x2, move.edge1Turns.visit3Turn);
            var newX3 = x3;
            var x3Visit = DirectedHelper.ExtractVisit(x3);
            if (x3Visit == y1Visit) newX3 = DirectedHelper.BuildVisit(x3Visit, y1NewTurn);
            if (x3Visit == y2Visit) newX3 = DirectedHelper.BuildVisit(x3Visit, y2NewTurn);
            
            var newY0 = y0;
            var y0Visit = DirectedHelper.ExtractVisit(y0);
            if (y0Visit == x1Visit) newY0 = DirectedHelper.BuildVisit(y0Visit, x1NewTurn);
            if (y0Visit == x2Visit) newY0 = DirectedHelper.BuildVisit(y0Visit, x2NewTurn);
            var newY1 = DirectedHelper.OverwriteTurn(y1, move.edge2Turns.visit2Turn);
            var newY2 = DirectedHelper.OverwriteTurn(y2, move.edge2Turns.visit3Turn);
            var newY3 = y3;
            var y3Visit = DirectedHelper.ExtractVisit(y3);
            if (y3Visit == x1Visit) newY3 = DirectedHelper.BuildVisit(y3Visit, x1NewTurn);
            if (y3Visit == x2Visit) newY3 = DirectedHelper.BuildVisit(y3Visit, x2NewTurn);

            var clonedTour = tour.Clone();
            
            // a normal 2-opt move in this format means:
            // 1. replaces x1->x2 and y1->y2 with x0->)x1->y1(->y0 and x3->)x2->y2(->y3
            // 2. inverses order of visits between x2->(visits)->y1 to y1->(reversed visits)->x2
            
            // collect visits between.
            var visitsBetween = new List<int>();
            var next = tour.Next(x2);
            if (next == Tour.END && tour.IsClosedDirected())
            {
                next = tour.First;
            }
            while (next != y1)
            {
                visitsBetween.Add(next);
                next = tour.Next(next);
                if (next == Tour.END && tour.IsClosedDirected())
                {
                    next = tour.First;
                }
            }
            
            // replace visits with their turn optimized versions.
            tour.Replace(x0, newX0);
            tour.Replace(x1, newX1);
            tour.Replace(x2, newX2);
            tour.Replace(x3, newX3);
            tour.Replace(y0, newY0);
            tour.Replace(y1, newY1);
            tour.Replace(y2, newY2);
            tour.Replace(y3, newY3);
            
            // replace edges
            // we replace more than strictly needed because x1, x2, y1 and y2 could have different turn date.
            tour.ReplaceEdgeFrom(newX0, newX1);
            tour.ReplaceEdgeFrom(newX1, newY1);
            tour.ReplaceEdgeFrom(newY1, newY0);
            
            tour.ReplaceEdgeFrom(newX3, newX2);
            tour.ReplaceEdgeFrom(newX2, newY2);
            tour.ReplaceEdgeFrom(newY2, newY3);
            
            // add the reversed part.
            visitsBetween.Reverse();
            for (var i = 1; i < visitsBetween.Count; i++)
            {
                tour.ReplaceEdgeFrom(visitsBetween[i-1], visitsBetween[i]);
            }

            if (tour.Count != clonedTour.Count) throw new Exception("Loss of visits during 2-opt.");
        }
    }
}