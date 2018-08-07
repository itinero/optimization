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

using Itinero.Optimization.Solvers.CVRP_ND;
using System;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.CVRP_ND
{
    public class CVRPNDCandidateTests
    {
        [Fact]
        public void CVRPCandidate_AddNewShouldCalculateTourData()
        {
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Fitness = 0,
                Problem = new CVRPNDProblem(new []
                {
                    new [] { 0f, 1f, 2f }, 
                    new [] { 3f, 4f, 5f }, 
                    new [] { 6f, 7f, 8f }
                })
            };
            var tourData = candidate.TourData(candidate.AddNew(0));
            Assert.Equal(0, tourData.weight);
            Assert.NotNull(tourData.constraints);
            Assert.Empty(tourData.constraints);
            
            candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Fitness = 0,
                Problem = new CVRPNDProblem(new []
                {
                    new [] { 0f, 1f, 2f }, 
                    new [] { 3f, 4f, 5f }, 
                    new [] { 6f, 7f, 8f }
                }, new []{ 0f, 0f, 0f }, 100, new (string metric, float max, float[] costs)[]
                {
                    ("weight", 100, new [] { 10f, 10f, 10f })
                })
            };
            tourData = candidate.TourData(candidate.AddNew(0));
            Assert.Equal(0, tourData.weight);
            Assert.NotNull(tourData.constraints);
            Assert.Single(tourData.constraints);
            Assert.Equal("weight", tourData.constraints[0].metric);
            Assert.Equal(10, tourData.constraints[0].value);
        }

        [Fact]
        public void CVRPCandidate_RemoveShouldRemoveTourAndDecreaseCount()
        {
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Fitness = 0,
                Problem = new CVRPNDProblem(new []
                {
                    new [] { 0f, 1f, 2f }, 
                    new [] { 3f, 4f, 5f }, 
                    new [] { 6f, 7f, 8f }
                })
            };
            
            var tour = candidate.Tour(candidate.AddNew(10));
            tour.InsertAfter(tour.First, 0);
            tour = candidate.Tour(candidate.AddNew(10));
            tour.InsertAfter(tour.First, 1);
            tour = candidate.Tour(candidate.AddNew(10));
            tour.InsertAfter(tour.First, 2);
            
            Assert.Equal(3, candidate.Count);
            tour = candidate.Tour(0);
            Assert.Equal(new [] { 10, 0 }, tour);
            tour = candidate.Tour(1);
            Assert.Equal(new [] { 10, 1 }, tour);
            tour = candidate.Tour(2);
            Assert.Equal(new [] { 10, 2 }, tour);
            
            candidate.Remove(1);
            Assert.Equal(2, candidate.Count);
            tour = candidate.Tour(0);
            Assert.Equal(new [] { 10, 0 }, tour);
            tour = candidate.Tour(1);
            Assert.Equal(new [] { 10, 2 }, tour);
            
            candidate.Remove(0);
            Assert.Equal(1, candidate.Count);
            tour = candidate.Tour(0);
            Assert.Equal(new [] { 10, 2 }, tour);
            
            candidate.Remove(0);
            Assert.Equal(0, candidate.Count);
        }

        [Fact]
        public void CVRPCandidate_SeqAndSmallerWith2ShouldEnumeratePairs()
        {
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Fitness = 0,
                Problem = new CVRPNDProblem(new[]
                {
                    new [] { 0f, 1f, 2f },
                    new [] { 3f, 4f, 5f },
                    new [] { 6f, 7f, 8f }
                })
            };

            var tour = candidate.Tour(candidate.AddNew(0));
            tour.InsertAfter(0, 1);
            tour.InsertAfter(1, 2);

            var seqs = candidate.SeqAndSmaller(0, 2, 2, false);
            foreach (var s in seqs)
            {
                Assert.Equal(2, s.Length);
                Assert.Equal(0, s.Between);
                Assert.Equal(0, s.BetweenTravelCost);
                Assert.Equal(0, s.BetweenTravelCostReversed);
                Assert.Equal(0, s.BetweenVisitCost);
                Assert.Equal(candidate.Problem.TravelWeight(s[0], s[1]),
                    s.TotalOriginal);
            }
        }

        [Fact]
        public void CVRPCandidate_SeqAndSmallerWith3ShouldEnumerateTriples()
        {
            var candidate = new CVRPNDCandidate()
            {
                Solution = new CVRPNDSolution(),
                Fitness = 0,
                Problem = new CVRPNDProblem(new[]
                {
                    new [] { 0f, 1f, 2f },
                    new [] { 3f, 4f, 5f },
                    new [] { 6f, 7f, 8f }
                })
            };

            var tour = candidate.Tour(candidate.AddNew(0));
            tour.InsertAfter(0, 1);
            tour.InsertAfter(1, 2);

            var seqs = candidate.SeqAndSmaller(0, 3, 3, false);
            foreach (var s in seqs)
            {
                Assert.Equal(3, s.Length);
                Assert.Equal(0, s.Between);
                Assert.Equal(0, s.BetweenTravelCost);
                Assert.Equal(0, s.BetweenTravelCostReversed);
                Assert.Equal(0, s.BetweenVisitCost);
                Assert.Equal(candidate.Problem.TravelWeight(s[0], s[1]) + candidate.Problem.TravelWeight(s[1], s[2]),
                    s.TotalOriginal);
            }
        }
    }
}