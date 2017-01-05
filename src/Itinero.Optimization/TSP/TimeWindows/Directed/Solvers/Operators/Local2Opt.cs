// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;
using System.Collections.Generic;

namespace Itinero.Optimization.TSP.TimeWindows.Directed.Solvers.Operators
{
    /// <summary>
    /// A local 2-Opt* search for the TSP-TW.
    /// </summary>
    /// <remarks>* 2-Opt: Removes two edges and reconnects the two resulting paths in a different way to obtain a new tour.</remarks>
    public class Local2Opt : IOperator<float, TSPTWProblem, TSPTWObjective, Tour, float>
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "2OPT"; }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        /// <returns></returns>
        public bool Supports(TSPTWObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool Apply(TSPTWProblem problem, TSPTWObjective objective, Tour tour, out float delta)
        {
            delta = 0;

            var fitness = objective.Calculate(problem, tour);

            var customers = new List<int>(problem.Times.Length + 1);
            customers.AddRange(tour);
            if (tour.Last == tour.First)
            { // add last customer at the end if it's the same as the first one.
                customers.Add(tour.Last.Value);
            }

            // select two edges with at least one edge between them at both sides of the tour.
            var weightBefore = 0f;
            if (problem.Windows[DirectedHelper.ExtractId(customers[0])].Min > weightBefore)
            { // wait here!
                weightBefore = problem.Windows[DirectedHelper.ExtractId(customers[0])].Min;
            }
            for (var edge1 = 0; edge1 < customers.Count - 3; edge1++)
            { // iterate over all from-edges.
                var edge11 = customers[edge1 + 0];
                var edge12 = customers[edge1 + 1];

                // extract directional information.
                int edge11arrivalId, edge11departureId, edge11id, edge11turn;
                DirectedHelper.ExtractAll(edge11, out edge11arrivalId, out edge11departureId, out edge11id, out edge11turn);
                int edge12arrivalId, edge12departureId, edge12id, edge12turn;
                DirectedHelper.ExtractAll(edge12, out edge12arrivalId, out edge12departureId, out edge12id, out edge12turn);

                for (var edge2 = edge1 + 2; edge2 < customers.Count - 1; edge2++)
                { // iterate over all possible to-edges given the from-edge.
                    var edge21 = customers[edge2 + 0];
                    var edge22 = customers[edge2 + 1];

                    // at this point we have two edges 11->12->...->21->22->...
                    // attempt to reverse the part in between.
                    // 1: calculate the reverse part and check if feasible.
                    // 2: if not feasible then stop.
                    // 3: if feasible then calculate the forward part.
                    // 4: if better then accept the result.

                    // calculate het best possible reverse sequence by switching turns and departure and arrivel id's.
                    // First start by creating a sequence with all turns and arrival and departure id's equal to zero.
                    var part = new List<int>();
                    part.Add(DirectedHelper.UpdateDepartureOffset(edge11, 0));
                    for (var b = edge2; b > edge1; b--)
                    {
                        part.Add(DirectedHelper.BuildDirectedId(DirectedHelper.ExtractId(customers[b]), 0));
                    }
                    part.Add(DirectedHelper.UpdateArrivalOffset(edge22, 0));

                    var localDelta = this.OptimizePart(problem, part);
                    while (localDelta > 0.1)
                    {
                        localDelta = this.OptimizePart(problem, part);
                    }

                    int violated;
                    float violatedTime, waitTime;
                    var partTime = objective.CalculateTimeForPart(problem, part, weightBefore, out violated, out violatedTime, out waitTime);

                    if (violated == 0)
                    { // new part is feasible, this should be fine.
                        var existingPart = new List<int>();
                        existingPart.Add(edge11);
                        for (var c = edge1 + 1; c < edge2; c++)
                        {
                            existingPart.Add(customers[c]);
                        }
                        existingPart.Add(edge22);
                        
                        var existingPartTime = objective.CalculateTimeForPart(problem, existingPart, weightBefore, out violated, out violatedTime, out waitTime);

                        if (existingPartTime > partTime)
                        { // an improvment, add the new part.
                            tour.Replace(edge11, part[0]);
                            tour.Replace(edge22, part[part.Count - 1]);

                            for (var c = 1; c < part.Count; c++)
                            {
                                tour.ReplaceEdgeFrom(part[c - 1], part[c]);
                            }
                            var newFitness = objective.Calculate(problem, tour);

                            delta = fitness - newFitness;
                            return true;
                        }
                    }
                }

                // update weight before.
                weightBefore += problem.Times[edge11arrivalId][edge12arrivalId];
                weightBefore += problem.TurnPenalties[edge11turn];
                if (problem.Windows[DirectedHelper.ExtractId(customers[edge1])].Min > weightBefore)
                { // wait here!
                    weightBefore = problem.Windows[DirectedHelper.ExtractId(customers[edge1])].Min;
                }
            }

            return false;
        }

        /// <summary>
        /// Optimizes the given part of the tour by choosing the best improvements in either the departureOffset or arrivalOffset of the first/last customers or the turns at any of the intermediate ones.
        /// </summary>
        private float OptimizePart(TSPTWProblem problem, List<int> part)
        {
            int turn1, arrivalId1, departureId1, id1;
            int turn2, arrivalId2, departureId2, id2;
            int turn3, arrivalId3, departureId3, id3;

            var delta = 0f; // the positive difference.
            var arrivalIdChange = -1;
            var departureIdChange = -1;
            var customerIdx = -1;
            var customerTurn = -1;

            // try changing the departureId.
            DirectedHelper.ExtractAll(part[0], out arrivalId1, out departureId1, out id1, out turn1);
            DirectedHelper.ExtractAll(part[1], out arrivalId2, out departureId2, out id2, out turn2);
            var weight = problem.TurnPenalties[turn1];
            weight += problem.Times[departureId1][arrivalId2];
            var new0 = DirectedHelper.SwitchDepartureOffset(part[0]);
            DirectedHelper.ExtractAll(new0, out arrivalId3, out departureId3, out id3, out turn3);
            var newWeight = problem.TurnPenalties[turn3];
            newWeight += problem.Times[departureId3][arrivalId2];
            if (newWeight < weight)
            { // there was an improvement found in changing the departure id.
                departureIdChange = DirectedHelper.ExtractDepartureId(new0);
                delta = newWeight - weight;
            }

            // try changing the arrivalId.
            DirectedHelper.ExtractAll(part[part.Count - 2], out arrivalId1, out departureId1, out id1, out turn1);
            DirectedHelper.ExtractAll(part[part.Count - 1], out arrivalId2, out departureId2, out id2, out turn2);
            weight = problem.TurnPenalties[turn2];
            weight += problem.Times[departureId1][arrivalId2];
            var newLast = DirectedHelper.SwitchArrivalOffset(part[part.Count - 1]);
            DirectedHelper.ExtractAll(newLast, out arrivalId3, out departureId3, out id3, out turn3);
            newWeight = problem.TurnPenalties[turn3];
            newWeight += problem.Times[departureId1][arrivalId3];
            if (newWeight < weight && 
                delta < (newWeight - weight))
            { // there was an improvement found in changing the arrival id.
                arrivalIdChange = DirectedHelper.ExtractDepartureId(newLast);
                departureIdChange = -1;
                delta = newWeight - weight;
            }

            for (var c = 1; c < part.Count - 1; c++)
            {
                var perviousDepartureId = DirectedHelper.ExtractDepartureId(part[c - 1]);
                DirectedHelper.ExtractAll(part[c], out arrivalId1, out departureId1, out id1, out turn1);
                var nextArrivalid = DirectedHelper.ExtractArrivalId(part[c + 1]);
                weight = problem.Times[perviousDepartureId][arrivalId1];
                weight += problem.TurnPenalties[turn1];
                weight += problem.Times[departureId1][nextArrivalid];

                for (var t = 0;  t < 3; t++)
                {
                    if (t == turn1)
                    {
                        continue;
                    }

                    var newDirectedId = DirectedHelper.BuildDirectedId(id1, t);
                    DirectedHelper.ExtractAll(newDirectedId, out arrivalId2, out departureId2, out id2, out turn2);
                    newWeight = problem.Times[perviousDepartureId][arrivalId2];
                    newWeight += problem.TurnPenalties[turn2];
                    newWeight += problem.Times[departureId2][nextArrivalid];

                    if (newWeight < weight &&
                        delta < (newWeight - weight))
                    { // there was an improvement found in changing the turn.
                        arrivalIdChange = -1;
                        departureIdChange = -1;
                        customerIdx = c;
                        customerTurn = t;
                        delta = newWeight - weight;
                    }
                }
            }

            if (delta > 0)
            {
                if (departureIdChange != -1)
                {
                    part[0] = DirectedHelper.SwitchDepartureOffset(part[0]);
                }
                else if (arrivalIdChange != -1)
                {
                    part[part.Count - 1] = DirectedHelper.SwitchArrivalOffset(part[part.Count - 1]);
                }
                else if (customerIdx != -1)
                {
                    part[customerIdx] = DirectedHelper.BuildDirectedId(
                        DirectedHelper.ExtractId(part[customerIdx]), customerTurn);
                }
            }
            return delta;
        }
    }
}