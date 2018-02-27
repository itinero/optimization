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

using Itinero.Optimization.Algorithms.Directed;
using Itinero.Optimization.Algorithms.NearestNeighbour;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Tours;
using Itinero.Optimization.Tours.Operations;
using System.Collections.Generic;

namespace Itinero.Optimization.Solutions.TSP.TimeWindows.Directed
{
    /// <summary>
    /// The default TSP-TW profile definition.
    /// </summary>
    public sealed class TSPTWProblem
    {
        /// <summary>
        /// An empty constructor used just to clone stuff.
        /// </summary>
        private TSPTWProblem()
        {

        }

        /// <summary>
        /// Creates a new 'open' TSP-TW with only a start customer.
        /// </summary>
        public TSPTWProblem(int first, float[][] times, TimeWindow[] windows, float turnPenaltyInSeconds)
        {
            this.First = first;
            this.Last = null;
            this.Times = times;
            this.Windows = windows;

            this.TurnPenalties = new float[] {
                0,
                turnPenaltyInSeconds,
                turnPenaltyInSeconds,
                0
            };

            for (var x = 0; x < this.Times.Length; x++)
            {
                this.Times[x][first] = 0;
            }

            for (var x = 0; x < this.Times.Length; x++)
            {
                this.Times[x][first * 2 + 0] = 0;
                this.Times[x][first * 2 + 1] = 0;
            }
        }

        /// <summary>
        /// Creates a new TSP-TW, 'closed' when first equals last.
        /// </summary>
        public TSPTWProblem(int first, int last, float[][] weights, TimeWindow[] windows, float turnPenaltyInSeconds)
        {
            this.First = first;
            this.Last = last;
            this.Times = weights;
            this.Windows = windows;
            
            this.TurnPenalties = new float[] {
                0,
                turnPenaltyInSeconds,
                turnPenaltyInSeconds,
                0
            };

            this.Times[first * 2 + 0][last * 2 + 0] = 0;
            this.Times[first * 2 + 1][last * 2 + 0] = 0;
            this.Times[first * 2 + 0][last * 2 + 1] = 0;
            this.Times[first * 2 + 1][last * 2 + 1] = 0;
        }

        /// <summary>
        /// Gets the windows.
        /// </summary>
        public TimeWindow[] Windows { get; set; }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Times { get; set; }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First { get; set; }

        /// <summary>
        /// Gets the last customer if the problem is closed.
        /// </summary>
        public int? Last { get; set; }

        /// <summary>
        /// Gets or sets the turn penalties per type of turn.
        /// 0: forward, forward.
        /// 1: forward, backward.
        /// 2: backward, forward.
        /// 3: backward, backward.
        /// </summary>
        public float[] TurnPenalties { get; set; }
        
        /// <summary>
        /// Creates a deep-copy of this problem.
        /// </summary>
        /// <returns></returns>
        public TSPTWProblem Clone()
        {
            var weights = new float[this.Times.Length][];
            for (var i = 0; i < this.Times.Length; i++)
            {
                weights[i] = this.Times[i].Clone() as float[];
            }
            var clone = new TSPTWProblem();
            clone.First = this.First;
            clone.Last = this.Last;
            clone.Times = this.Times;
            clone.Windows = this.Windows;
            clone.TurnPenalties = this.TurnPenalties;
            return clone;
        }

        /// <summary>
        /// Solves this problim using a default solver.
        /// </summary>
        /// <returns></returns>
        public Tour Solve()
        {
            return this.Solve(new Solvers.HillClimbing3OptSolver());
        }

        /// <summary>
        /// Solvers this problem using the given solver.
        /// </summary>
        public Tour Solve(ISolver<float, TSPTWProblem, TSPTWObjective, Tour, float> solver)
        {
            return solver.Solve(this, new TSPTWObjective());
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, NearestNeighbours[]> _forwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbours GetNNearestNeighboursForward(int n, int id)
        {
            if (_forwardNearestNeighbours == null)
            { // not there yet, create.
                _forwardNearestNeighbours = new Dictionary<int, NearestNeighbours[]>();
            }
            NearestNeighbours[] nearestNeighbours = null;
            if (!_forwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new NearestNeighbours[this.Times.Length];
                _forwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighbours.ForwardDirected(this.Times, n, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, NearestNeighbours[]> _backwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public NearestNeighbours GetNNearestNeighboursBackward(int n, int id)
        {
            if (_backwardNearestNeighbours == null)
            { // not there yet, create.
                _backwardNearestNeighbours = new Dictionary<int, NearestNeighbours[]>();
            }
            NearestNeighbours[] nearestNeighbours = null;
            if (!_backwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new NearestNeighbours[this.Times.Length];
                _backwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighbours.BackwardDirected(this.Times, n, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<float, SortedNearestNeighbours[]> _forwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public SortedNearestNeighbours GetNearestNeighboursForward(float weight, int id)
        {
            if (_forwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _forwardSortedNearestNeighbours = new Dictionary<float, SortedNearestNeighbours[]>();
            }
            SortedNearestNeighbours[] nearestNeighbours = null;
            if (!_forwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new SortedNearestNeighbours[this.Times.Length];
                _forwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = SortedNearestNeighbours.ForwardDirected(this.Times, weight, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<float, SortedNearestNeighbours[]> _backwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public SortedNearestNeighbours GetNearestNeighboursBackward(float weight, int id)
        {
            if (_backwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _backwardSortedNearestNeighbours = new Dictionary<float, SortedNearestNeighbours[]>();
            }
            SortedNearestNeighbours[] nearestNeighbours = null;
            if (!_backwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new SortedNearestNeighbours[this.Times.Length];
                _backwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[id];
            if (result == null)
            { // not found, calculate.
                result = SortedNearestNeighbours.BackwardDirected(this.Times, weight, id);
                nearestNeighbours[id] = result;
            }
            return result;
        }
        
        /// <summary>
        /// Calculates total time and all violations.
        /// </summary>
        /// <param name="time">The total travel time without waittime.</param>
        /// <param name="tour">The tour to calculate for.</param>
        /// <param name="validFlags">Flags for each customer, a customer window is violated when false.</param>
        /// <param name="violatedTime">The total time of violations.</param>
        /// <param name="waitTime">The total waiting time.</param>
        /// <returns>The number of invalid customers.</returns>
        public int TimeAndViolations(Tour tour, out float time, out float waitTime, out float violatedTime, ref bool[] validFlags)
        {
            return TimeAndViolations(tour as IEnumerable<int>, out time, out waitTime, out violatedTime, ref validFlags);
        }

        /// <summary>
        /// Calculates total time and violations if the given id was moved after 'before'. Assumes the tour is closed.
        /// </summary>
        public int TimeAndViolations(IEnumerable<int> tour, out float time, out float waitTime, out float violatedTime, ref bool[] validFlags)
        {
            var times = this.Times;
            var windows = this.Windows;
            var turnPenalties = this.TurnPenalties;

            time = 0f;
            violatedTime = 0f;
            waitTime = 0f;

            var violated = 0;
            var previousFrom = int.MaxValue;
            var firstTo = int.MaxValue;
            var first = Constants.NOT_SET;
            foreach (var directedId in tour)
            {
                if (first == Constants.NOT_SET)
                {
                    first = directedId;
                }

                // extract turns and stuff from directed id.
                int arrivalId, departureId, id, turn;
                DirectedHelper.ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);

                // add the weight from the previous customer to the current one.
                var turnPenalty = 0f;
                if (previousFrom != int.MaxValue)
                { // there is a previous, add the travel time.
                    time = time + times[previousFrom][arrivalId];

                    // and keep the turn penalty.
                    turnPenalty += turnPenalties[turn];
                }
                else
                { // first customer.
                    firstTo = arrivalId;
                }

                // check the windows (before turn-penalties).
                var window = windows[id];
                validFlags[id] = true;
                if (window.Max < (time + waitTime))
                { // ok, unfeasible.
                    violatedTime += (time + waitTime) - window.Max;
                    validFlags[id] = false;
                    violated++;
                }
                if (window.Min > (time + waitTime))
                { // wait here!
                    waitTime += (window.Min - (time + waitTime));
                }

                // add the turn penalty.
                time += turnPenalty;

                previousFrom = departureId;
            }

            // add the weight between last and first.
            if (previousFrom != int.MaxValue)
            {
                time = time + times[previousFrom][firstTo];

                // add turn penalty at first customer.
                time += turnPenalties[DirectedHelper.ExtractTurn(first)];
            }

            return violated;
        }

        /// <summary>
        /// Calculates the weight for a part of the tour including the turns at first and last position.
        /// </summary>
        /// <param name="part">The part to calculate for.</param>
        /// <param name="timeBefore">The time when arriving at the first customer, excluding the turn at that first customer.</param>
        public int TimeAndViolationsForPart(IEnumerable<int> part, float timeBefore, out float time, out float waitTime, out float violatedTime, ref bool[] validFlags)
        {
            var times = this.Times;
            var windows = this.Windows;
            var turnPenalties = this.TurnPenalties;

            time = timeBefore;
            violatedTime = 0f;
            waitTime = 0f;

            var violated = 0;
            var previousFrom = int.MaxValue;
            foreach (var directedId in part)
            {
                // extract turns and stuff from directed id.
                int arrivalId, departureId, id, turn;
                DirectedHelper.ExtractAll(directedId, out arrivalId, out departureId, out id, out turn);

                // add the weight from the previous customer to the current one.
                var turnPenalty = 0f;
                if (previousFrom != int.MaxValue)
                { // there is a previous, add the travel time.
                    time = time + times[previousFrom][arrivalId];

                    // and keep the turn penalty.
                    turnPenalty += turnPenalties[turn];
                }
                else
                { // first customer.
                    // add turn penalty at first customer.
                    time += turnPenalties[DirectedHelper.ExtractTurn(directedId)];
                }

                // check the windows (before turn-penalties).
                var window = windows[id];
                validFlags[id] = true;
                if (window.Max < (time + waitTime))
                { // ok, unfeasible.
                    violatedTime += (time + waitTime) - window.Max;
                    validFlags[id] = false;
                    violated++;
                }
                if (window.Min > (time + waitTime))
                { // wait here!
                    waitTime += (window.Min - (time + waitTime));
                }

                // add the turn penalty.
                time += turnPenalty;

                previousFrom = departureId;
            }

            time = time - timeBefore; // correct the time again, the time returned is the time of only this part.

            return violated;
        }

        /// <summary>
        /// Creates an initial empty tour, add fixed first and/or last customer.
        /// </summary>
        /// <returns></returns>
        public Tour CreateEmptyTour()
        {
            var firstDirectedId = DirectedHelper.BuildDirectedId(this.First, 0);
            if (!this.Last.HasValue)
            {
                return new Tours.Tour(new int[] { firstDirectedId }, null);
            }
            else
            {
                if (this.Last == this.First)
                {
                    return new Tours.Tour(new int[] { firstDirectedId }, firstDirectedId);
                }
                var lastDirectedId = DirectedHelper.BuildDirectedId(this.Last.Value, 0);
                return new Tour(new int[] { firstDirectedId, lastDirectedId }, lastDirectedId);
            }
        }
    }
}