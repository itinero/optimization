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

using System.Diagnostics;
using System.Threading;
using Itinero.Optimization.Solvers.CVRP_ND.SCI;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Strategies;

namespace Itinero.Optimization.Solvers.CVRP_ND.Operators
{
    /// <summary>
    /// An operator that removes a few tours and redoes placement.
    /// </summary>
    internal class RedoPlacementOperator : Operator<CVRPNDCandidate>
    {
        private readonly PlacementOperator<CVRPNDCandidate> _placementOperator;
        private readonly int _removeCount;

        public RedoPlacementOperator(PlacementOperator<CVRPNDCandidate> placementOperator = null, int tourRemoveCount = 2)
        {
            _removeCount = tourRemoveCount;
            _placementOperator = placementOperator ?? new SeededCheapestInsertionPlacementOperator(
                                     beforeSeedPlacementOperator: new CheapestInsertionPlacementOperator());
        }

        public override string Name => $"REDO_{_placementOperator.Name}";

        public override bool Apply(CVRPNDCandidate candidate)
        {
            var before = candidate.Fitness;
            var originalCount = candidate.Count;
            while (candidate.Count > originalCount - _removeCount &&
                   candidate.Count > 0)
            {
                var t = Strategies.Random.RandomGenerator.Default.Generate(candidate.Count);
                candidate.Remove(t);
            }

            _placementOperator.Apply(candidate);
            
            Debug.Assert(candidate.GetUnplacedVisits().Count == 0);

            return candidate.Fitness < before;
        }
        
        private static readonly ThreadLocal<RedoPlacementOperator> DefaultLazy = new ThreadLocal<RedoPlacementOperator>(() => new RedoPlacementOperator());
        public static RedoPlacementOperator Default => DefaultLazy.Value;
    }
}