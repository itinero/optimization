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

using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Abstract.Tours;
using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Abstract.Tours.TurningWeights;

namespace Itinero.Optimization.Sequences.Directed.Solver.Operators
{
    /// <summary>
    /// A local search operator trying to improve the solution by switching directions.
    /// </summary>
    public class DirectionLocalSearchOperator : IOperator<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float>
    {
        /// <summary>
        /// Gets the name of this operator.
        /// </summary>
        public string Name
        {
            get
            {
                return "LOC_DIR";
            }
        }

        /// <summary>
        /// Returns true if the given objective is supported.
        /// </summary>
        public bool Supports(SequenceDirectedObjective objective)
        {
            return true;
        }
        
        /// <summary>
        /// Applies this operator.
        /// </summary>
        public bool Apply(SequenceDirectedProblem problem, SequenceDirectedObjective objective, Tour solution, out float delta)
        {
            var weights = problem.Weights;
            delta = objective.Zero;

            var fitnessBefore = objective.Calculate(problem, solution);

            var enumerator = solution.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var previous = enumerator.Current;
                var previousDepartureId = DirectedHelper.ExtractDepartureId(previous);
                var current = solution.GetNeigbour(previous);
                if (current == Constants.NOT_SET)
                { // last of open.
                    continue;
                }
                var next = solution.GetNeigbour(current);
                var nextArrivalId = Constants.NOT_SET;
                if (next != Constants.NOT_SET)
                {
                    nextArrivalId = DirectedHelper.ExtractArrivalId(next);
                }
                int currentTurn, currentId, currentArrivalId, currentDepartureId;
                DirectedHelper.ExtractAll(current, out currentArrivalId, out currentDepartureId, out currentId, out currentTurn);

                var weightBefore = weights[previousDepartureId][currentArrivalId];
                var weightAfter = float.MaxValue;
                var newDirectedId = Constants.NOT_SET;
                if (next == Constants.NOT_SET)
                { // there is no next, only one option here:
                    // 0 or 1: arrival id offset = 0 OR
                    // 2 or 3: arrival id offset = 1
                    if (currentArrivalId % 2 == 0)
                    { // check turn = 2.
                        weightAfter = weights[previousDepartureId][currentArrivalId + 1];
                        if (weightAfter < weightBefore)
                        { // do the switch.
                            newDirectedId = DirectedHelper.BuildDirectedId(currentId, 2);
                        }
                    }
                    else
                    { // check turn = 0
                        weightAfter = weights[previousDepartureId][currentArrivalId - 1];
                        if (weightAfter < weightBefore)
                        { // do the switch.
                            newDirectedId = DirectedHelper.BuildDirectedId(currentId, 0);
                        }
                    }
                }
                else
                { // three options left, excluding the current one of 4.
                    weightBefore += weights[currentDepartureId][nextArrivalId];
                    weightBefore += problem.TurnPenalties[currentTurn];

                    currentArrivalId = currentArrivalId - (currentArrivalId % 2);
                    currentDepartureId = currentDepartureId - (currentDepartureId % 2);
                    for (var i = 0; i < 4; i++)
                    {
                        if (i == currentTurn)
                        {
                            continue;
                        }

                        int arrivalOffset, departureOffset;
                        DirectedHelper.ExtractOffset(i, out arrivalOffset, out departureOffset);
                        var newWeightAfter = weights[previousDepartureId][currentArrivalId + arrivalOffset]
                            + weights[currentDepartureId + departureOffset][nextArrivalId]
                            + problem.TurnPenalties[i];
                        if (newWeightAfter < weightAfter &&
                            newWeightAfter < weightBefore)
                        {
                            newDirectedId = DirectedHelper.BuildDirectedId(currentId, i);
                            weightAfter = newWeightAfter;
                        }
                    }
                }

                // switch if a new directed if was found.
                if (newDirectedId != Constants.NOT_SET)
                {
                    solution.Replace(current, newDirectedId);
                }
            }

            var fitnessAfter = objective.Calculate(problem, solution);

            delta = objective.Subtract(problem, fitnessBefore, fitnessAfter);

            return objective.CompareTo(problem, delta, objective.Zero) > 0;
        }
    }
}
