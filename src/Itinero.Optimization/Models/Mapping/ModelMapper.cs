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

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// The model mapper, maps a model to the routing network.
    /// </summary>
    public static class ModelMapper
    {
        /// <summary>
        /// Maps the model to the routing network using the given router.
        /// </summary>
        public static DefaultMappedModel Map(this Model model, RouterBase router)
        {
            var profileName = model.VehiclePool.Vehicles[0].Profile;
            var profile = router.Db.GetSupportedProfile(profileName);

            var weightMatrixAlgorithm = new WeightMatrixAlgorithm(router, profile, 
                new MassResolvingAlgorithm(router, new Profiles.IProfileInstance[] { profile }, 
                    model.Visits));
            
            return new DefaultMappedModel(model, weightMatrixAlgorithm);
        }
    }
}