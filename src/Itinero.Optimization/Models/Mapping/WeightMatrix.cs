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

using Itinero.Algorithms.Matrices;
using Itinero.Attributes;
using Itinero.Optimization.Abstract.Models.Costs;
using Itinero.Optimization.Abstract.Tours;
using System;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents a standard weight matrix.
    /// </summary>
    public class WeightMatrix : WeightMatrixBase
    {
        private readonly IWeightMatrixAlgorithm<float> _weightMatrixAlgorithm;

        /// <summary>
        /// Creates a new weight matrix.
        /// </summary>
        /// <param name="weightMatrixAlgorithm"></param>
        public WeightMatrix(IWeightMatrixAlgorithm<float> weightMatrixAlgorithm)
        {
            _weightMatrixAlgorithm = weightMatrixAlgorithm;
        }

        /// <summary>
        /// Gets the main metric.
        /// </summary>
        /// <returns></returns>
        public override string Metric => _weightMatrixAlgorithm.Profile.Profile.Metric.ToModelMetric();

        /// <summary>
        /// Adjusts the items to the weight matrix removing invalid or unresolved visits.
        /// </summary>
        /// <param name="array">The array to adjust.</param>
        /// <returns></returns>
        public override T[] AdjustToMatrix<T>(T[] array)
        {
            return _weightMatrixAlgorithm.AdjustToMatrix(array);
        }

        /// <summary>
        /// Builds a real-world route based on the given tour.
        /// </summary>
        public override Route BuildRoute(ITour tour, Func<int, IAttributeCollection, float> customizeVisit)
        {
            return _weightMatrixAlgorithm.BuildRoute(tour, customizeVisit);
        }

        /// <summary>
        /// Builds travel costs matrices based on the data in this weight matrix.
        /// </summary>
        /// <returns></returns>
        public override TravelCostMatrix[] BuildTravelCostMatrices()
        {
            return new TravelCostMatrix[]
            {
                new TravelCostMatrix()
                {
                    Costs = _weightMatrixAlgorithm.Weights,
                    Name = _weightMatrixAlgorithm.Profile.Profile.Metric.ToModelMetric(),
                    Directed = false
                }
            };
        }

        /// <summary>
        /// Gets the index in the matrix of the original location.
        /// </summary>
        /// <param name="original">The index in the original location array.</param>
        /// <returns></returns>
        public override int? WeightIndexNullable(int? original)
        {
            if (original == null)
            {
                return null;
            }
            return _weightMatrixAlgorithm.WeightIndex(original.Value);
        }

        /// <summary>
        /// Returns true if the two tours geographically overlaps.
        /// </summary>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns></returns>
        public override bool Overlaps(ITour tour1, ITour tour2)
        {
            return _weightMatrixAlgorithm.ToursOverlap(tour1, tour2);
        }
    }
}