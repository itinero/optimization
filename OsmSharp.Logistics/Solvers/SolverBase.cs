// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

namespace OsmSharp.Logistics.Solvers
{
    /// <summary>
    /// A base implementation for a solver.
    /// </summary>
    public abstract class SolverBase<TProblem, TObjective, TSolution> : ISolver<TProblem, TObjective, TSolution>
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
            double fitness;
            return this.Solve(problem, objective, out fitness);
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public abstract TSolution Solve(TProblem problem, TObjective objective, out double fitness);

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