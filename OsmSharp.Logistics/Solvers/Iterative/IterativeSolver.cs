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

namespace OsmSharp.Logistics.Solvers.Iterative
{
    /// <summary>
    /// A solver that let's another solver try n-times and keeps the best obtained solution.
    /// </summary>
    public class IterativeSolver<TProblem, TObjective, TSolution> : SolverBase<TProblem, TObjective, TSolution>
    {
        /// <summary>
        /// The number of times to try.
        /// </summary>
        private int _n;

        /// <summary>
        /// The solver to try.
        /// </summary>
        private ISolver<TProblem, TObjective, TSolution> _solver;

        /// <summary>
        /// Holds the stop condition.
        /// </summary>
        private SolverDelegates.StopConditionDelegate<TProblem, TObjective, TSolution> _stopCondition;

        /// <summary>
        /// Creates a new iterative improvement solver.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="n"></param>
        public IterativeSolver(ISolver<TProblem, TObjective, TSolution> solver, int n)
        {
            _solver = solver;
            _n = n;
        }

        /// <summary>
        /// Creates a new iterative improvement solver.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="n"></param>
        /// <param name="stopCondition"></param>
        public IterativeSolver(ISolver<TProblem, TObjective, TSolution> solver, int n, 
            SolverDelegates.StopConditionDelegate<TProblem, TObjective, TSolution> stopCondition)
        {
            _solver = solver;
            _n = n;
            _stopCondition = stopCondition;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return string.Format("ITER_[{0}x{1}]", _n, _solver.Name); }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <param name="problem">The problem to solve.</param>
        /// <param name="objective">The objective to reach.</param>
        /// <param name="fitness">The fitness of the solution found.</param>
        /// <returns></returns>
        public override TSolution Solve(TProblem problem, TObjective objective, out double fitness)
        {
            var i = 0;
            TSolution best = default(TSolution);
            fitness = double.MaxValue;
            while (i < _n && !this.IsStopped &&
                (_stopCondition == null || !_stopCondition.Invoke(i, problem, objective, best)))
            {
                var nextFitness = double.MaxValue;
                var nextRoute = _solver.Solve(problem, objective, out nextFitness);
                if (nextFitness < fitness)
                { // yep, found a better solution!
                    best = nextRoute;
                    fitness = nextFitness;

                    // report new solution.
                    this.ReportIntermidiateResult(best);
                }
                i++;
            }
            return best;
        }
    }
}