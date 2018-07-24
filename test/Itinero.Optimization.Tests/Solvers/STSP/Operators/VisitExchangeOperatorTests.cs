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
using Itinero.Optimization.Solvers.STSP;
using Itinero.Optimization.Solvers.STSP.Operators;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.STSP.Operators
{
    public class VisitExchangeOperatorTests
    {
        [Fact]
        public void VisitExchangeOperator_ShouldCreateNewTourFromGivenTours()
        {
            // create problem.
            var problem = STSPHelper.Create(0, 0, 5, 10, 40);
            var tour1 = new Tour(new[] {0, 1}, 0);
            var tour2 = new Tour(new[] {0, 2}, 0);

            var combinedTour = VisitExchangeOperator.Default.Apply(new Candidate<STSProblem, Tour>()
            {
                Problem = problem,
                Solution = tour1,
                Fitness = tour1.Fitness(problem)
            }, new Candidate<STSProblem, Tour>()
            {
                Problem = problem,
                Solution = tour2,
                Fitness = tour2.Fitness(problem)
            });
            Assert.NotNull(combinedTour);
            Assert.Contains(0, combinedTour.Solution);
            Assert.Contains(1, combinedTour.Solution);
            Assert.Contains(2, combinedTour.Solution);
            Assert.DoesNotContain(3, combinedTour.Solution);
            Assert.DoesNotContain(4, combinedTour.Solution);
        }
    }
}