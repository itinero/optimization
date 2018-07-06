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

using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Solvers.CVRP.Operators;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Shared.Seeds;

namespace Itinero.Optimization.Solvers.CVRP.SCI
{
    /// <summary>
    /// A placement operator using a seeded cheapest insertion construction heuristic.
    /// </summary>
    internal class SeededCheapestInsertionPlacementOperator : PlacementOperator<CVRPCandidate>
    {
        private readonly float _improvementsThreshold; // the threshold for when the apply inter-tour improvements.
        private readonly PlacementOperator<CVRPCandidate> _seedOperator;
        private readonly PlacementOperator<CVRPCandidate> _placementOperator;
        private readonly float _remainingThreshold; // the place remaining threshold percentage.

        /// <summary>
        /// Creates a new SCI strategy.
        /// </summary>
        /// <param name="seedOperator">The seed operator, expected is an operator that adds new tours.</param>
        /// <param name="placementOperator">The placement operator, expected is an operator that places visits in the existing tours.</param>
        /// <param name="remainingThreshold">The remaining threshold parameter.</param>
        /// <param name="improvementsThreshold">The improvements threshold parameter.</param>
        public SeededCheapestInsertionPlacementOperator(PlacementOperator<CVRPCandidate> seedOperator = null,
            PlacementOperator<CVRPCandidate> placementOperator = null, float remainingThreshold = 0.03f, 
                float improvementsThreshold = 0.25f)
        {
            _seedOperator = seedOperator ?? new SeedPlacementOperator((_, visits) => SeedHeuristics.GetSeedRandom(visits));
            _placementOperator = placementOperator ?? new CheapestInsertionPlacementOperator();
            _remainingThreshold = remainingThreshold;
            _improvementsThreshold = improvementsThreshold;
        }
        
        public override string Name => "SCI_PLACE";

        public override bool Apply(CVRPCandidate candidate)
        {
            var visits = candidate.GetUnplacedVisits();
            return this.Apply(candidate, visits);
        }

        public override bool Apply(CVRPCandidate candidate, ICollection<int> visits)
        {
            while (_seedOperator.Apply(candidate, visits))
            {
                _placementOperator.Apply(candidate, visits);
            }

            return false;
        }
        
        private static readonly ThreadLocal<SeededCheapestInsertionPlacementOperator> DefaultLazy = new ThreadLocal<SeededCheapestInsertionPlacementOperator>(() => new SeededCheapestInsertionPlacementOperator());
        public static SeededCheapestInsertionPlacementOperator Default => DefaultLazy.Value;
    }
}