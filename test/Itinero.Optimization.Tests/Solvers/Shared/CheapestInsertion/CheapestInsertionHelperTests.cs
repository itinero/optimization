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

using Itinero.Optimization.Solvers.Shared.CheapestInsertion;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.CheapestInsertion
{
    public class CheapestInsertionHelperTests
    {
        [Fact]
        public void CheapestInsertionHelper_ShouldInsertAtCheapestLocation()
        {
            var matrix = new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 3, 0, 1, 2 },
                new float[] { 2, 3, 0, 1 },
                new float[] { 1, 2, 3, 0 }};

            var tour = new Tour(new[] { 0, 2, 3 }, 0);
            var result = tour.CalculateCheapest((x, y) => matrix[x][y], 1, null, null);
            Assert.Equal(0, result.location.From);
            Assert.Equal(2, result.location.To);
            Assert.Equal(0, result.cost);

            tour = new Tour(new[] { 0, 1, 3 }, 0);
            result = tour.CalculateCheapest((x, y) => matrix[x][y], 2, null, null);
            Assert.Equal(1, result.location.From);
            Assert.Equal(3, result.location.To);
            Assert.Equal(0, result.cost);
            
            tour = new Tour(new[] { 0, 1, 2 }, 0);
            result = tour.CalculateCheapest((x, y) => matrix[x][y], 3, null, null);
            Assert.Equal(2, result.location.From);
            Assert.Equal(0, result.location.To);
            Assert.Equal(0, result.cost);
        }

        [Fact]
        public void CheapestInsertionHelper_ShouldInsertWhenCanPlace()
        {
            var matrix = new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 3, 0, 1, 2 },
                new float[] { 2, 3, 0, 1 },
                new float[] { 1, 2, 3, 0 }};

            var tour = new Tour(new[] { 0, 3 }, 0);
            var result = tour.CalculateCheapest((x, y) => matrix[x][y], new [] { 1, 2 }, null, (w, v) => v == 1);
            Assert.Equal(0, result.location.From);
            Assert.Equal(3, result.location.To);
            Assert.Equal(0, result.cost);
            Assert.Equal(1, result.visit);

            tour = new Tour(new[] { 0, 3 }, 0);
            result = tour.CalculateCheapest((x, y) => matrix[x][y], new [] { 1, 2 }, null, (w, v) => v == 2);
            Assert.Equal(0, result.location.From);
            Assert.Equal(3, result.location.To);
            Assert.Equal(0, result.cost);
            Assert.Equal(2, result.visit);
        }
    }
}