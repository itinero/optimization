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
using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Strategies.Random;

namespace Itinero.Optimization.Strategies.Tools.Selectors
{
    /// <summary>
    /// A tournament selector.
    /// </summary>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    public class TournamentSelector<TCandidate> : ISelector<TCandidate>
    {
        private readonly float _sizePercentage;
        private readonly float _probability;
        private readonly ThreadLocal<List<int>> _candidatesInTournament = 
            new ThreadLocal<List<int>>(() => new List<int>());
        private readonly Comparison<TCandidate> _comparison = null;
        private readonly RandomGenerator _random = null;

        /// <summary>
        /// Creates a new tournament selector.
        /// </summary>
        /// <param name="sizePercentage">The percentage of the population that will be randomly chosen to compete.</param>
        /// <param name="probability">The probability that and individual is chosen.</param>
        /// <param name="random">The random generator to use.</param>
        /// <param name="comparison">The comparison function to use to determine order.</param>
        public TournamentSelector(float sizePercentage = 10, 
            float probability = 0.5f, RandomGenerator random = null, Comparison<TCandidate> comparison = null)
        {
            _sizePercentage = sizePercentage;
            _probability = probability;
            _comparison = comparison;
            _random = random ?? RandomGenerator.Default;
        }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; } = "TOURN";

        /// <summary>
        /// Selects a candidate from the given population.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <param name="exclude">A function to ignore individuals.</param>
        /// <returns>The index of the selected candidate.</returns>
        public int Select(TCandidate[] population, Func<int, bool> exclude = null)
        {
            var scaledSize = (int)System.Math.Ceiling(((_sizePercentage / 100) * population.Length));
            var tempPop = _candidatesInTournament.Value;
            tempPop.Clear();

            while (tempPop.Count < scaledSize)
            { // keep looping until enough individuals are selected or until no more are available.
                var i = _random.Generate(population.Length);
                if (exclude == null || !exclude(i))
                { // do not tournament excluded solutions.
                    tempPop.Add(i);
                }
            }

            // sort the population.
            tempPop.Sort((x, y) => CandidateComparison.Compare(
                population[x], population[y], _comparison));

            // choose a candidate.
            foreach (var t in tempPop)
            {
                // choose a candidate.
                if (_random.Generate(1.0f) < _probability)
                { // candidate chosen!
                    return t;
                }
            }
            return -1;
        }
    }
}