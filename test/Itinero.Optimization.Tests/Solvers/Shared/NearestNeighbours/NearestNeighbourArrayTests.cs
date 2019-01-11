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

using Itinero.Optimization.Solvers.Shared.NearestNeighbours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Shared.NearestNeighbours
{
    public class NearestNeighbourArrayTests
    {
        /// <summary>
        /// Tests a 1-nearest neighbour in a 4x4 matrix.
        /// </summary>
        [Fact]
        public void NearestNeighbourArray_ShouldBeTheNearestWhenN1()
        {
            var matrix = new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 3, 0, 1, 2 },
                new float[] { 2, 3, 0, 1 },
                new float[] { 1, 2, 3, 0 }};

            var nearest = new NearestNeighbourArray((x ,y) => matrix[x][y], 4, 1);
            Assert.NotNull(nearest);
            Assert.Equal(1, nearest.N);

            for (var i = 0; i < matrix.Length; i++)
            {
                var nn = nearest[i];
                Assert.NotNull(nn);
                Assert.Single(nn);
                var n = i + 1;
                if (n >= matrix.Length) n =  matrix.Length - n;
                Assert.Equal(n, nn[0]);
            }
        }
        
        /// <summary>
        /// Tests a 2-nearest neighbour in a 4x4 matrix.
        /// </summary>
        [Fact]
        public void NearestNeighbourArray_ShouldBeTwoNearestWhenN2()
        {
            var matrix = new [] {
                new float[] { 0, 1, 2, 3 },
                new float[] { 3, 0, 1, 2 },
                new float[] { 2, 3, 0, 1 },
                new float[] { 1, 2, 3, 0 }};

            var nearest = new NearestNeighbourArray((x ,y) => matrix[x][y], 4, 2);
            Assert.NotNull(nearest);
            Assert.Equal(2, nearest.N);

            for (var i = 0; i < matrix.Length; i++)
            {
                var nn = nearest[i];
                Assert.NotNull(nn);
                Assert.Equal(2, nn.Length);
                var n1 = i + 1;
                if (n1 >= matrix.Length) n1 =  matrix.Length - n1;
                var n2 = n1 + 1;
                if (n2 >= matrix.Length) n2 =  matrix.Length - n2;
                Assert.Equal(n1, nn[0]);
                Assert.Equal(n2, nn[1]);
            }
        }
    }
}