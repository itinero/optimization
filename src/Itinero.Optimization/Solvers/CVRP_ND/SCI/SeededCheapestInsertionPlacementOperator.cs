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
using System.Diagnostics;
using System.Threading;
using Itinero.Optimization.Solvers.CVRP_ND.Operators;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Shared.Seeds;

namespace Itinero.Optimization.Solvers.CVRP_ND.SCI
{
    /// <summary>
    /// A placement operator using a seeded cheapest insertion construction heuristic.
    /// </summary>
    internal class SeededCheapestInsertionPlacementOperator : PlacementOperator<CVRPNDCandidate>
    {
        private readonly PlacementOperator<CVRPNDCandidate> _seedOperator;
        private readonly PlacementOperator<CVRPNDCandidate> _afterSeedPlacementOperator;
        private readonly PlacementOperator<CVRPNDCandidate> _beforeSeedPlacementOperator;
        private readonly PlacementOperator<CVRPNDCandidate> _remainingPlacementOperator;
        private readonly float _remainingThreshold; // the place remaining threshold percentage.

        /// <summary>
        /// Creates a new SCI strategy.
        /// </summary>
        /// <param name="seedOperator">The seed operator, expected is an operator that adds new tours.</param>
        /// <param name="beforeSeedPlacementOperator">The placement operator, expected is an operator that places visits in the existing tours before a new tour was seeded.</param>
        /// <param name="afterSeedPlacementOperator">The placement operator, expected is an operator that places visits in the existing tours after a new tour was seeded.</param>
        /// <param name="remainingThreshold">The remaining threshold parameter.</param>
        public SeededCheapestInsertionPlacementOperator(PlacementOperator<CVRPNDCandidate> seedOperator = null,
            PlacementOperator<CVRPNDCandidate> beforeSeedPlacementOperator = null, PlacementOperator<CVRPNDCandidate> afterSeedPlacementOperator = null, 
            float remainingThreshold = 0.03f)
        {
            _seedOperator = seedOperator ?? new SeedPlacementOperator((_, visits) => SeedHeuristics.GetSeedRandom(visits));
            _beforeSeedPlacementOperator = beforeSeedPlacementOperator;
            _afterSeedPlacementOperator = afterSeedPlacementOperator ?? CheapestInsertionPlacementOperator.DefaultLastOnly;
            _remainingThreshold = remainingThreshold;
            
            _remainingPlacementOperator = CheapestInsertionPlacementOperator.Default;
        }
        
        public override string Name => "SCI_PLACE";

        public override bool Apply(CVRPNDCandidate candidate)
        {
            var visits = candidate.GetUnplacedVisits();
            return this.Apply(candidate, visits);
        }

        public override bool Apply(CVRPNDCandidate candidate, ICollection<int> visits)
        {
            _beforeSeedPlacementOperator?.Apply(candidate, visits);
            
            var remainingThreshold = (int) (_remainingThreshold * candidate.Problem.Count);
            while (visits.Count > 0)
            {
                if (!_seedOperator.Apply(candidate, visits))
                {
                    break;
                }

                if (visits.Count <= remainingThreshold)
                {
                    _remainingPlacementOperator.Apply(candidate, visits);
                }
                
                _afterSeedPlacementOperator.Apply(candidate, visits);
                //Debug.Assert(candidate.IsFeasible());
            }

            return false;
        }
        
        private static readonly ThreadLocal<SeededCheapestInsertionPlacementOperator> DefaultLastOnlyLazy = new ThreadLocal<SeededCheapestInsertionPlacementOperator>(
            () => new SeededCheapestInsertionPlacementOperator());
        public static SeededCheapestInsertionPlacementOperator DefaultLastOnly => DefaultLastOnlyLazy.Value;
        
        private static readonly ThreadLocal<SeededCheapestInsertionPlacementOperator> DefaultLazy = new ThreadLocal<SeededCheapestInsertionPlacementOperator>(() => 
            new SeededCheapestInsertionPlacementOperator(beforeSeedPlacementOperator: CheapestInsertionPlacementOperator.Default, 
                afterSeedPlacementOperator: CheapestInsertionPlacementOperator.DefaultLastOnly));
        public static SeededCheapestInsertionPlacementOperator Default => DefaultLazy.Value;
    }
}