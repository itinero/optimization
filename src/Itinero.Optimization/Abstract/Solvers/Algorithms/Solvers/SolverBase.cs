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

using Itinero.Optimization.Algorithms.Solvers.Objective;

namespace Itinero.Optimization.Algorithms.Solvers
{
    /// <summary>
    /// A base implementation for a solver.
    /// </summary>
    public abstract class SolverBase<TWeight, TProblem, TObjective, TSolution, TFitness> : ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        /// <summary>
        /// Holds the stopped-flag.
        /// </summary>
        private bool _stopped;

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public TSolution Solve(TProblem problem, TObjective objective)
        {
            TFitness fitness;
            return this.Solve(problem, objective, out fitness);
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public abstract TSolution Solve(TProblem problem, TObjective objective, out TFitness fitness);

        /// <summary>
        /// Returns true if this solver was stopped.
        /// </summary>
        public bool IsStopped
        {
            get
            {
                return _stopped;
            }
        }

        /// <summary>
        /// Stops execution.
        /// </summary>
        public virtual void Stop()
        {
            _stopped = true;
        }

        /// <summary>
        /// Reports an intermediate result if someone is interested.
        /// </summary>
        /// <param name="solution"></param>
        protected void ReportIntermidiateResult(TSolution solution)
        {
            if (this.IntermidiateResult != null)
            { // yes, there is a listener that cares!
                this.IntermidiateResult(solution);
            }
        }

        /// <summary>
        /// Event triggered when an intermediate solution is available.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate<TSolution> IntermidiateResult;
    }
}