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
using Itinero.Logging;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Strategies.VNS
{
    /// <summary>
    /// Variable neighbourhood search (VNS) strategy.
    /// </summary>
    /// <typeparam name="TProblem">The problem type.</typeparam>
    /// <typeparam name="TCandidate">The candidate type.</typeparam>
    internal sealed class VNSStrategy<TProblem, TCandidate> : Strategy<TProblem, TCandidate>
        where TCandidate : class
    {
        private readonly Strategy<TProblem, TCandidate> _generator;
        private readonly Perturber<TCandidate> _perturber;
        private readonly Operator<TCandidate> _localSearch;
        private readonly Delegates.StopConditionDelegate<TCandidate> _stopCondition;

        /// <summary>
        /// Creates a new VNS strategy.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="perturber">The perturber.</param>
        /// <param name="localSearch">The local search operator.</param>
        /// <param name="stopCondition">The stop condition.</param>
        public VNSStrategy(Func<TProblem, TCandidate> generator, Func<TCandidate, int, bool> perturber,
            Func<TCandidate, bool> localSearch, Delegates.StopConditionDelegate<TCandidate> stopCondition = null)
            : this(generator.ToStrategy(), perturber, localSearch, stopCondition)
        {
            
        }

        /// <summary>
        /// Creates a new VNS strategy.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="perturber">The perturber.</param>
        /// <param name="localSearch">The local search operator.</param>
        /// <param name="stopCondition">The stop condition.</param>
        public VNSStrategy(Strategy<TProblem, TCandidate> generator, Perturber<TCandidate> perturber,
            Operator<TCandidate> localSearch, Delegates.StopConditionDelegate<TCandidate> stopCondition = null)
        {
            _generator = generator;
            _perturber = perturber;
            _localSearch = localSearch;
            _stopCondition = stopCondition;
        }

        /// <inheritdoc />
        public override string Name => $"VNS_[{_generator.Name}_{_perturber.Name}_{_localSearch.Name}]";

        /// <inheritdoc />
        public override TCandidate Search(TProblem problem)
        {
            Logger.Log($"{typeof(VNSStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                $"{this.Name}: Started generating initial solution...");
            var globalBest = _generator.Search(problem);
            if (!this.ReportIntermidiateResult(globalBest)) { return globalBest; }

            if (_localSearch.Apply(globalBest))
            {
                Logger.Log($"{typeof(VNSStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                    $"{this.Name}: Improvement found by local search: {globalBest}");
                if (!this.ReportIntermidiateResult(globalBest)) { return globalBest; }
            }

            var iteration = 0;
            var level = 0;
            while (true)
            {
                // check stopping conditions.
                if (_stopCondition != null &&
                    _stopCondition.Invoke(globalBest, iteration, level))
                {
                    Logger.Log($"{typeof(VNSStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                        $"{this.Name}: Stop condition reached, best is: {globalBest}");
                    return globalBest;
                }
                
                // make a copy of the existing solution.
                var current = globalBest.Clone();
                
                // run the perturber, shake things up!
                _perturber.Apply(current, level);
                
                // run the local search.
                _localSearch.Apply(current);

                level++;
                iteration++;
                if (CandidateComparison.Compare(globalBest, current) > 0)
                { // we found a new best.
                    Logger.Log($"{typeof(VNSStrategy<TProblem, TCandidate>)}.{nameof(Search)}", TraceEventType.Verbose,
                        $"{this.Name}: Improvement found at iteration {iteration} and level {level}: {globalBest}");
                    globalBest = current;
                    level = 0;
                }
            }
        }
    }
}