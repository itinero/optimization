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

using System.Collections.Generic;
using System.Linq;
using Itinero.Optimization.Solvers.Shared.Sequences;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.Tours.Sequences;

namespace Itinero.Optimization.Solvers.CVRP
{
    /// <summary>
    /// Represents a CVRP candidate solution.
    /// </summary>
    internal class CVRPCandidate : Candidate<CVRProblem, CVRPSolution>
    {
        private readonly List<(float weight, (string metric, float value)[] constraints)> _tourData = 
            new List<(float weight, (string metric, float value)[] constraints)>();
        
        /// <summary>
        /// Gets the number of tours.
        /// </summary>
        public int Count => this.Solution.Count;

        /// <summary>
        /// Adds a new tour based on a first visit.
        /// </summary>
        /// <returns>A new tour with the departure and arrival set.</returns>
        public int AddNew()
        {
            // add a new tour.
            var t = this.Solution.AddNew(this.Problem.Departure, this.Problem.Arrival);
            var tour = this.Solution.Tour(t);
            
            // add data for the tour and calculate initial costs.
            _tourData.Add((0, new (string metric, float value)[this.Problem.CapacityConstraints.Length]));
            var tourData = _tourData[_tourData.Count - 1];
            foreach (var pair in tour.Pairs())
            {
                tourData.weight += this.Problem.TravelWeight(pair.From, pair.To);
            }
            for (var c = 0; c < this.Problem.CapacityConstraints.Length; c++)
            {
                var constraint = this.Problem.CapacityConstraints[c];

                var cost = constraint.costs[this.Problem.Departure];
                if (this.Problem.Arrival.HasValue)
                {
                    cost += constraint.costs[this.Problem.Arrival.Value];
                }
                
                tourData.constraints[c] = (constraint.metric, cost);
            }
            _tourData[_tourData.Count - 1] = tourData;
            
            // update total fitness.
            this.Fitness += tourData.weight;
            
            return t;
        }

        /// <summary>
        /// Removes the tour with the given index.
        /// </summary>
        /// <param name="t">The tour index.</param>
        public void Remove(int t)
        {
            this.Solution.Remove(t);
            var tourData = _tourData[t];
            _tourData.RemoveAt(t);
            
            // update total fitness.
            this.Fitness -= tourData.weight;
        }

        /// <summary>
        /// Gets the tour.
        /// </summary>
        /// <param name="t">The tour index.</param>
        /// <returns>The tour.</returns>
        public Tour Tour(int t)
        {
            return this.Solution.Tour(t);
        }

        /// <summary>
        /// Gets the tour data.
        /// </summary>
        /// <param name="t">The tour index.</param>
        /// <returns>Data about the tour.</returns>
        public (float weight, (string metric, float value)[] constraints) TourData(int t)
        {
            return _tourData[t];
        }

        /// <summary>
        /// Returns true if the given visit can be inserted into the given tour with the given travel cost increase.
        /// </summary>
        /// <param name="t">The tour.</param>
        /// <param name="visit">The visit.</param>
        /// <param name="travelWeightIncrease">The extra travel weight if inserted.</param>
        /// <returns>True if it's possible without violating any constraints.</returns>
        public bool CanInsert(int t, int visit, float travelWeightIncrease)
        {
            var tourData = _tourData[t];
            
            // check travel weight.
            if (tourData.weight + travelWeightIncrease > this.Problem.MaxWeight)
            { // too much travel weight.
                return false;
            }
            
            // calculate constraints.
            for (var c = 0; c < this.Problem.CapacityConstraints.Length; c++)
            {
                var constraint = this.Problem.CapacityConstraints[c];
                var cost = constraint.costs[visit] + tourData.constraints[c].value;
                if (cost > constraint.max)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given visit can be inserted in the given tour at the given location.
        /// </summary>
        /// <param name="t">The tour index.</param>
        /// <param name="before">The visit to insert after.</param>
        /// <param name="visit">The visit to insert.</param>
        public bool CanInsertAfter(int t, int before, int visit)
        {
            // calculate travel weight increase.
            var tour = this.Tour(t);
            var after = tour.GetNeigbour(before);
            var diff = this.Problem.TravelWeight(before, visit);
            if (after != Tours.Tour.NOT_SET)
            {
                diff += this.Problem.TravelWeight(visit, after);
                diff -= this.Problem.TravelWeight(before, after);
            }

            return this.CanInsert(t, before, diff);
        }

        /// <summary>
        /// Inserts the given visit in the given tour at the given location.
        /// </summary>
        /// <param name="t">The tour index.</param>
        /// <param name="before">The visit to insert after.</param>
        /// <param name="visit">The visit to insert.</param>
        public void InsertAfter(int t, int before, int visit)
        {
            // update the tour.
            var tour = this.Tour(t);
            var after = tour.GetNeigbour(before);
            tour.InsertAfter(before, visit);
            
            // update tour data and weight.
            var tourData = _tourData[t];
            for (var c = 0; c < this.Problem.CapacityConstraints.Length; c++)
            {
                var constraint = this.Problem.CapacityConstraints[c];
                tourData.constraints[c] = (constraint.metric, 
                    constraint.costs[visit] + tourData.constraints[c].value);
            }
            var diff = this.Problem.TravelWeight(before, visit);
            if (after != Tours.Tour.NOT_SET)
            {
                diff += this.Problem.TravelWeight(visit, after);
                diff -= this.Problem.TravelWeight(before, after);
            }
            tourData.weight = tourData.weight + diff;
            _tourData[t] = tourData;
            
            // update total fitness.
            this.Fitness += diff;
        }

        /// <summary>
        /// Simulates a swap of the given sequences.
        /// </summary>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="s1">The sequence from the first tour.</param>
        /// <param name="s2">The sequence from the second tour.</param>
        /// <returns>True and a delta if the swap is possible, false otherwise.</returns>
        public (bool success, float fitnessDelta) SimulateSwap(int t1, int t2, Seq s1, Seq s2)
        {
            const float e = 0.01f;

            var pair1 = new Pair(s1[0], s1[s1.Length - 1]);
            var pair1Between = new Pair(s1[1], s1[s1.Length - 2]);
            var pair2 = new Pair(s2[0], s2[s2.Length - 1]);
            var pair2Between = new Pair(s2[1], s2[s2.Length - 2]);

            var tour1Current = s1.TotalOriginal;
            var tour1Future = this.Problem.TravelWeight(pair1.From,pair2Between.From) +
                              this.Problem.TravelWeight(pair2Between.To, pair1.To) + s2.Between;

            var tour2Current = s2.TotalOriginal;
            var tour2Future = this.Problem.TravelWeight(pair2.From, pair1Between.From) +
                this.Problem.TravelWeight(pair1Between.To, pair2.To) + s1.Between;

            var difference = tour1Current - tour1Future +
                             tour2Current - tour2Future;

            if (difference <= e)
            {
                // new weights are not better.
                return (false, difference);
            }

            var tour1Data = this.TourData(t1);
            var tour1FutureComplete = tour1Data.weight - tour1Current + tour1Future;
            if (tour1FutureComplete > this.Problem.MaxWeight)
            {
                // constraint violated.
                return (false, 0);
            }

            var tour2Data = this.TourData(t2);
            var tour2FutureComplete = tour2Data.weight - tour2Current + tour2Future;
            if (tour2FutureComplete > this.Problem.MaxWeight)
            {
                // constraint violated.
                return (false, 0);
            }

            var exchange1 = this.TryExchange(tour1Data.constraints, s1, s2);
            var exchange2 = this.TryExchange(tour2Data.constraints, s2, s1);
            if (!exchange1.success || !exchange2.success)
            {
                // constraint violated.
                return (false, 0);
            }
            
            // don't do the swap.
            return (true, difference);
        }

        /// <summary>
        /// Returns true if by constraints the given sequence exchange is possible.
        /// </summary>
        /// <param name="contents">The content in this tour.</param>
        /// <param name="sToRemove">The sequence to remove, the visits between first and last.</param>
        /// <param name="sToAdd">The sequence to add, the visits between first and last.</param>
        /// <returns>True when the exchange is possible and an updated content array.</returns>
        private (bool success, (string metric, float value)[] contents) TryExchange((string metric, float value)[] contents, Seq sToRemove,
            Seq sToAdd)
        {
            var newContents = contents;
            for (var c = 0; c < contents.Length; c++)
            {
                var constraint = this.Problem.CapacityConstraints[c];
                var content = contents[c];
                var value = content.value;

                for (var i = 1; i < sToRemove.Length - 1; i++)
                {
                    value -= constraint.costs[sToRemove[i]];
                }

                for (var i = 1; i < sToAdd.Length - 1; i++)
                {
                    value += constraint.costs[sToAdd[i]];
                }

                if (constraint.max < value)
                {
                    return (false, null);
                }

                newContents[c] = (content.metric, value);
            }

            return (true, newContents);
        }
        
        /// <summary>
        /// Tries to swap the given sequences.
        /// </summary>
        /// <param name="t1">The first tour.</param>
        /// <param name="t2">The second tour.</param>
        /// <param name="s1">The sequence from the first tour.</param>
        /// <param name="s2">The sequence from the second tour.</param>
        /// <returns>True and a delta if the swap was executed, false otherwise.</returns>
        public (bool success, float fitnessDelta) TrySwap(int t1, int t2, Seq s1, Seq s2)
        {
            const float e = 0.01f;

            var pair1 = new Pair(s1[0], s1[s1.Length - 1]);
            var pair1Between = new Pair(s1[1], s1[s1.Length - 2]);
            var pair2 = new Pair(s2[0], s2[s2.Length - 1]);
            var pair2Between = new Pair(s2[1], s2[s2.Length - 2]);

            var tour1Current = s1.TotalOriginal;
            var tour1Future = this.Problem.TravelWeight(pair1.From,pair2Between.From) +
                              this.Problem.TravelWeight(pair2Between.To, pair1.To) + s2.Between;

            var tour2Current = s2.TotalOriginal;
            var tour2Future = this.Problem.TravelWeight(pair2.From, pair1Between.From) +
                              this.Problem.TravelWeight(pair1Between.To, pair2.To) + s1.Between;

            var difference = tour1Current - tour1Future +
                             tour2Current - tour2Future;

            if (difference <= e)
            {
                // new weights are not better.
                return (false, difference);
            }

            var tour1Data = this.TourData(t1);
            var tour1FutureComplete = tour1Data.weight - tour1Current + tour1Future;
            if (tour1FutureComplete > this.Problem.MaxWeight)
            {
                // constraint violated.
                return (false, 0);
            }

            var tour2Data = this.TourData(t2);
            var tour2FutureComplete = tour2Data.weight - tour2Current + tour2Future;
            if (tour2FutureComplete > this.Problem.MaxWeight)
            {
                // constraint violated.
                return (false, 0);
            }

            var exchange1 = this.TryExchange(tour1Data.constraints, s1, s2);
            var exchange2 = this.TryExchange(tour2Data.constraints, s2, s1);
            if (!exchange1.success || !exchange2.success)
            {
                // constraint violated.
                return (false, 0);
            }
            
            var tour1 = this.Tour(t1);
            if (s1.Wraps)
            { // move first.
                this.Solution.UpdateFirst(t1, s1[s1.Length - 1]);
                tour1 = this.Tour(t1);
            }

            var tour2 = this.Tour(t2);
            if (s2.Wraps)
            { // move first.
                this.Solution.UpdateFirst(t2, s2[s2.Length - 1]);
                tour1 = this.Tour(t2);
            }
            
            // do the swap.
            // s2 -> tour1
            var previous = pair1.From;
            for (var v = 1; v < s2.Length - 1; v++)
            {
                var visit = s2[v];
                tour1.ReplaceEdgeFrom(previous, visit);
                previous = visit;
            }
            tour1.ReplaceEdgeFrom(previous, pair1.To);

            // s1 -> tour2
            previous = pair2.From;
            for (var v = 1; v < s1.Length - 1; v++)
            {
                var visit = s1[v];
                tour2.ReplaceEdgeFrom(previous, visit);
                previous = visit;
            }
            tour2.ReplaceEdgeFrom(previous, pair2.To);
            
            // update content.
            _tourData[t1] = (tour1FutureComplete, exchange1.contents);
            _tourData[t2] = (tour2FutureComplete, exchange2.contents);
            
            // update total fitness.
            this.Fitness += difference;
            
            // don't do the swap.
            return (true, difference);
        }

        /// <summary>
        /// Enumerates all sequences of sizes in the given range.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public IEnumerable<Seq> SeqAndSmaller(int t, int minSize, int maxSize)
        {
            var tour = this.Solution.Tour(t);
            var problem = this.Problem;
            
            var visits = tour.ToArray();
            if (maxSize > visits.Length)
            {
                maxSize = visits.Length;
            }

            if (minSize > visits.Length)
            {
                // make sure not to enumerate anything.
                maxSize = -1;
            }

            var tourSequence = new Sequence(visits);
            for (var length = maxSize; length >= minSize; length--)
            {
                float? between = null;
                float end = 0;
                float betweenReversed = 0;
                var visitCost = 0f;
                foreach (var s in tourSequence.SubSequences(length, length, false))
                {
                    float start = 0;
                    if (between == null)
                    {
                        // first sequence of this length.
                        var betweenSeq = s.SubSequence(1, s.Length - 2);
                        between = betweenSeq.Weight(problem.TravelWeight);
                        start = problem.TravelWeight(s[0], s[1]);
                        end = problem.TravelWeight(s[s.Length - 2], s[s.Length - 1]);
                        betweenReversed = betweenSeq.WeightReversed(problem.TravelWeight);
                        visitCost = betweenSeq.Cost(problem.VisitWeight);
                    }
                    else
                    {
                        // move weights along.
                        var newStart = problem.TravelWeight(s[0], s[1]);
                        var newEnd = problem.TravelWeight(s[s.Length - 2], s[s.Length - 1]);
                        between -= newStart;
                        between += end;
                        betweenReversed -= problem.TravelWeight(s[1], s[0]);
                        betweenReversed += problem.TravelWeight(s[s.Length - 2], s[s.Length - 3]);
                        start = newStart;
                        end = newEnd;
                        visitCost -= problem.VisitWeight(s[0]);
                        visitCost += problem.VisitWeight(s[s.Length - 2]);
                    }

                    if (between + start + end < problem.TravelWeight(s[0], s[s.Length - 1]) * 2f)
                    {
                        continue;
                    }

                    yield return new Seq(s)
                    {
                        BetweenTravelCost = between.Value,
                        BetweenTravelCostReversed = betweenReversed,
                        BetweenVisitCost = visitCost,
                        TotalOriginal = between.Value + end + start + visitCost
                    };
                }
            }
        }

        /// <summary>
        /// Gets all unplaced visits.
        /// </summary>
        /// <returns>A hashset of visits that can still be place in the solution.</returns>
        public HashSet<int> GetUnplacedVisits()
        {
            var visits = new HashSet<int>(this.Problem.Visits);
            for (var t = 0; t < this.Solution.Count; t++)
            {
                foreach (var visit in this.Solution.Tour(t))
                {
                    visits.Remove(visit);
                }
            }
            
            // remove start and end locations.
            visits.Remove(this.Problem.Departure);
            if (this.Problem.Arrival.HasValue) visits.Remove(this.Problem.Arrival.Value);

            return visits;
        }
    }
}