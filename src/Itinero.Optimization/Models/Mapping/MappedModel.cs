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
using Itinero.Algorithms.Search;
using Itinero.Optimization.Abstract.Models;
using Itinero.Optimization.Abstract.Tours;
using System.Linq;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents a default mapped model.
    /// </summary>
    public class MappedModel
    {
        private readonly RouterBase _router;

        /// <summary>
        /// Creates a new mapped model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="router"></param>
        public MappedModel(Model model, RouterBase router)
        {
            this.Model = model;
            _router = router;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns></returns>
        public Model Model { get; private set; }

        private WeightMatrixBase _weightMatrix;
        private AbstractModel _abstractModel = null;

        /// <summary>
        /// Builds the abstract model.
        /// </summary>
        /// <returns>The complete abstract model.</returns>
        public AbstractModel BuildAbstract()
        {
            if (_abstractModel != null)
            {
                return _abstractModel;
            }

            var profileName = this.Model.VehiclePool.Vehicles[0].Profile;
            var profile = _router.Db.GetSupportedProfile(profileName);

            _weightMatrix = null;
            if (this.Model.VehiclePool.Vehicles[0].TurnPentalty == 0)
            {
                var weightMatrixAlgorithm = new WeightMatrixAlgorithm(_router, profile, 
                    new MassResolvingAlgorithm(_router, new Profiles.IProfileInstance[] { profile }, 
                        this.Model.Visits));
                weightMatrixAlgorithm.Run();

                _weightMatrix = new WeightMatrix(weightMatrixAlgorithm);
            }
            else
            {
                var weightMatrixAlgorithm = new DirectedWeightMatrixAlgorithm(_router, profile, 
                    new MassResolvingAlgorithm(_router, new Profiles.IProfileInstance[] { profile }, 
                        this.Model.Visits));
                weightMatrixAlgorithm.Run();
                
                _weightMatrix = new WeightMatrixDirected(weightMatrixAlgorithm);
            }

            // build travel cost matrix.
            var travelCosts = _weightMatrix.BuildTravelCostMatrices();

            // adjust timewindows.
            var timeWindows = this.Model.TimeWindows.ToAbstract(_weightMatrix);

            // adjust costs.
            var visitCosts = this.Model.VisitCosts.ToAbstract(_weightMatrix);

            // adjust vehicle pool.
            var vehiclePool = this.Model.VehiclePool.ToAbstract(_weightMatrix);

            _abstractModel = new Abstract.Models.AbstractModel()
            {
                VisitCosts = visitCosts,
                TimeWindows = timeWindows,
                VehiclePool = vehiclePool,
                TravelCosts = travelCosts
            };
            return _abstractModel;
        }

        /// <summary>
        /// Builds a real-world route representing the given tour.
        /// </summary>
        /// <param name="tour">The tour.</param>
        /// <returns></returns>
        public Route BuildRoute(ITour tour)
        {
            if (this.Model.VisitCosts != null &&
                this.Model.VisitCosts.Length > 0)
            { // there are visit costs, perhaps needed in route construction.
                var timeCosts = this.Model.VisitCosts.FirstOrDefault(x => x.Name == Metrics.Time);

                return _weightMatrix.BuildRoute(tour, (v, a) =>
                {
                    foreach (var visitCost in this.Model.VisitCosts)
                    {
                        a.AddOrReplace("cost_" + visitCost.Name, visitCost.Costs[v].ToInvariantString());
                    }

                    if (timeCosts != null)
                    {
                        return timeCosts.Costs[v];
                    }
                    return 0;
                });
            }
            return _weightMatrix.BuildRoute(tour, (v, a) => 0);
        }

        /// <summary>
        /// Returns true if the two given tours overlap.
        /// </summary>
        /// <param name="tour1">The first tour.</param>
        /// <param name="tour2">The second tour.</param>
        /// <returns></returns>
        public bool Overlaps(ITour tour1, ITour tour2)
        {
            return _weightMatrix.Overlaps(tour1, tour2);
        }
    }
}