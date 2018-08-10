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

using Itinero.Optimization.Solvers;
using Itinero.Optimization.Solvers.Shared.Directed;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Solvers.TSP_D;
using Itinero.Optimization.Solvers.TSP_D.Operators;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.TSP_D.Operators
{
    public class CheapestReinsertionOperatorTests
    {
        [Fact]
        public void CheapestReinsertionOperator_ShouldImproveOrDoNothingOnOpen()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            var sequence = new[] {2, 9, 5, 13, 17};
            for (var t = 0; t < 100; t++)
            {
                var tour = new Tour(sequence, null);
                var candidate = new Candidate<TSPDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };

                var fitnessBefore = candidate.Fitness;
                if (CheapestReinsertionOperator.Default.Apply(candidate))
                {
                    Assert.True(fitnessBefore > candidate.Fitness);
                }
                else
                {
                    Assert.Equal(sequence, tour);
                }
            }
        }
        
        [Fact]
        public void CheapestReinsertionOperator_ShouldPreserveVisitsOnOpen()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            var sequence = new[] {2, 9, 5, 13, 17};
            for (var t = 0; t < 100; t++)
            {
                var tour = new Tour(sequence, null);
                var candidate = new Candidate<TSPDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };

                if (CheapestReinsertionOperator.Default.Apply(candidate))
                {
                    Assert.Equal(sequence.Length, tour.Count);
                }
            }
        }
        
        [Fact]
        public void CheapestReinsertionOperator_ShouldImproveOrDoNothingOnClosed()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            var sequence = new[] {2, 9, 5, 13, 17 };
            for (var t = 0; t < 100; t++)
            {
                var tour = new Tour(sequence, sequence[0]);
                var candidate = new Candidate<TSPDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };

                var fitnessBefore = candidate.Fitness;
                if (CheapestReinsertionOperator.Default.Apply(candidate))
                {
                    Assert.True(fitnessBefore > candidate.Fitness);
                }
                else
                {
                    Assert.Equal(sequence, tour);
                }
            }
        }
        
        [Fact]
        public void CheapestReinsertionOperator_ShouldPreserveVisitsOnClosed()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            var sequence = new[] {2, 9, 5, 13, 17};
            for (var t = 0; t < 100; t++)
            {
                var tour = new Tour(sequence, sequence[0]);
                var candidate = new Candidate<TSPDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };

                if (CheapestReinsertionOperator.Default.Apply(candidate))
                {
                    Assert.Equal(sequence.Length, tour.Count);
                }
            }
        }
        
        [Fact]
        public void CheapestReinsertionOperator_ShouldImproveOrDoNothingOnFixed()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            var sequence = new[] {2, 9, 5, 13, 17 };
            for (var t = 0; t < 100; t++)
            {
                var tour = new Tour(sequence, sequence[sequence.Length - 1]);
                var candidate = new Candidate<TSPDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };

                var fitnessBefore = candidate.Fitness;
                if (CheapestReinsertionOperator.Default.Apply(candidate))
                {
                    Assert.True(fitnessBefore > candidate.Fitness);
                }
                else
                {
                    Assert.Equal(sequence, tour);
                }
            }
        }
        
        [Fact]
        public void CheapestReinsertionOperator_ShouldPreserveVisitsOnFixed()
        {
            // create problem.
            var problem = new TSPDProblem(0, null, WeightMatrixHelpers.BuildDirected(5, 10), 2);

            var sequence = new[] {2, 9, 5, 13, 17};
            for (var t = 0; t < 100; t++)
            {
                var tour = new Tour(sequence, sequence[sequence.Length - 1]);
                var candidate = new Candidate<TSPDProblem, Tour>()
                {
                    Solution = tour,
                    Problem = problem,
                    Fitness = tour.WeightDirected(problem.Weight, problem.TurnPenalty)
                };

                if (CheapestReinsertionOperator.Default.Apply(candidate))
                {
                    Assert.Equal(sequence.Length, tour.Count);
                }
            }
        }
    }
}