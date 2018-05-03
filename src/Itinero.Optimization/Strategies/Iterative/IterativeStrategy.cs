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

namespace Itinero.Optimization.Strategies.Iterative
{
    /// <summary>
    /// A strategy that generates a candidate and then applies an operator one or more times.
    /// </summary>
    public class IterativeStrategy<TProblem, TCandidate> : Strategy<TProblem, TCandidate>
    {
        private readonly IStrategy<TProblem, TCandidate> _generator;
        private readonly IOperator<TCandidate> _op;
        private readonly int _n;
        private readonly Delegates.NewCandidate<TCandidate> _stopCondition;
        private readonly Comparison<TCandidate> _comparer = null;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="op">The operator.</param>
        /// <param name="n">The number of iterations.</param>
        /// <param name="stopCondition">The stop condition, if any.</param>
        /// <param name="comparer">The comparer, if any.</param>
        public IterativeStrategy(IStrategy<TProblem, TCandidate> generator, IOperator<TCandidate> op, int n = 1,
            Delegates.NewCandidate<TCandidate> stopCondition = null, Comparison<TCandidate> comparer = null)
        {
            _n = n;
            _generator =generator;
            _stopCondition = stopCondition;
            _comparer = comparer;
            _op = op;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override string Name
        {
            get
            {
                return string.Format("IT[{0}x{1}]", _n, _generator.Name);
            }
        }

        /// <summary>
        /// Runs this strategy on the given problem and returns the best candidate.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <returns>A candidate.</returns>
        public override TCandidate Search(TProblem problem)
        {
            var i = 0;
            var best = default(TCandidate);
            while (i < _n &&
                (_stopCondition == null || best == null || _stopCondition.Invoke(best)))
            {
                Itinero.Logging.Logger.Log("IterativeSolver", Itinero.Logging.TraceEventType.Verbose,
                    "Started iteration {0}:{1}", i, best);

                // execute solver.
                var nextSolution = _generator.Search(problem);
                if (best == null ||
                    CandidateComparison.Compare(nextSolution, best, _comparer) > 0)
                { // yep, found a better solution!
                    if (best != null)
                    {
                        Itinero.Logging.Logger.Log("IterativeSolver", Itinero.Logging.TraceEventType.Verbose,
                            "Found a better solution at iteration {0}: {1} -> {2}", i, best, nextSolution);
                    }

                    best = nextSolution;

                    // report new solution.
                    this.ReportIntermidiateResult(best);
                }

                // apply operators if any.
                if (_op != null)
                {
                    if (_op.Apply(nextSolution))
                    { // an improvement was found, check if better.
                        if (best == null ||
                            CandidateComparison.Compare(nextSolution, best, _comparer) > 0)
                        {
                            if (best != null)
                            {
                                Itinero.Logging.Logger.Log("IterativeSolver", Itinero.Logging.TraceEventType.Verbose,
                                    "Found a better solution at iteration {0}: {1} -> {2}", i, best, nextSolution);
                            }

                            best = nextSolution;

                            // report new solution.
                            this.ReportIntermidiateResult(best);
                        }
                    }
                }

                i++;
            }
            return best;
        }
    }
}