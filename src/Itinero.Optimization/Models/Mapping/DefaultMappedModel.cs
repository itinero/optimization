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

using System;
using Itinero.Algorithms.Matrices;
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents a default mapped model.
    /// </summary>
    public class DefaultMappedModel : MappedModel
    {
        private readonly IWeightMatrixAlgorithm<float> _weightMatrix;

        /// <summary>
        /// Creates a new mapped model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="weightMatrix"></param>
        public DefaultMappedModel(Model model, IWeightMatrixAlgorithm<float> weightMatrix)
        {
            this.Model = model;
            _weightMatrix = weightMatrix;
        }

        /// <summary>
        /// Builds the abstract model.
        /// </summary>
        /// <returns>The complete abstract model.</returns>
        public override AbstractModel BuildAbstract()
        {
            // run calculations if needed.
            if (!_weightMatrix.HasRun)
            {
                _weightMatrix.Run();
            }

            // build travel cost matrix.
            var travelCosts = _weightMatrix.BuildTravelCostMatrix();

            // adjust timewindows.
            var timeWindows = this.Model.TimeWindows.ToAbstract(_weightMatrix);

            // adjust costs.
            var visitCosts = this.Model.VisitCosts.ToAbstract(_weightMatrix);

            // adjust vehicle pool.
            var vehiclePool = this.Model.VehiclePool.ToAbstract(_weightMatrix);

            return new Abstract.Models.AbstractModel()
            {
                VisitCosts = visitCosts,
                TimeWindows = timeWindows,
                VehiclePool = vehiclePool,
                TravelCosts = new Abstract.Models.Costs.TravelCostMatrix[]
                {
                    travelCosts
                }
            };
        }

        /// <summary>
        /// Builds a real-world route representing the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        public override Route BuildRoute(ITour tour)
        {
            return _weightMatrix.BuildRoute(tour);
        }
    }
}