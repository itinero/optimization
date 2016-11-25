// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Logistics.Objective;
using System;

namespace Itinero.Logistics.Solvers
{
    /// <summary>
    /// A wrapper for a solver, replacing the objective with another objective on each call.
    /// </summary>
    public class SolverObjectiveWrapper<TWeight, TProblem, TObjective, TSolution, TFitness> : ISolver<TWeight, TProblem, TObjective, TSolution, TFitness>
        where TObjective : ObjectiveBase<TProblem, TSolution, TFitness>
    {
        private readonly ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> _solver;
        private readonly TObjective _objective;
        private readonly Func<TProblem, TObjective, TSolution, TFitness> _calculateFitness;

        /// <summary>
        /// Creates a new solver objective wrapper.
        /// </summary>
        public SolverObjectiveWrapper(ISolver<TWeight, TProblem, TObjective, TSolution, TFitness> solver, TObjective objective, Func<TProblem, TObjective, TSolution, TFitness> calculateFitness)
        {
            _solver = solver;
            _solver.IntermidiateResult += _solver_IntermidiateResult;
            _objective = objective;
            _calculateFitness = calculateFitness;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return _solver.Name; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public TSolution Solve(TProblem problem, TObjective objective)
        {
            return _solver.Solve(problem, _objective);
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public TSolution Solve(TProblem problem, TObjective objective, out TFitness fitness)
        {
            var solution = _solver.Solve(problem, _objective, out fitness);
            fitness = _calculateFitness(problem, objective, solution);
            return solution;
        }

        /// <summary>
        /// Stops the executing of the solving process.
        /// </summary>
        public void Stop()
        {
            _solver.Stop();
        }

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate<TSolution> IntermidiateResult;

        /// <summary>
        /// Called when an intermediate solution is available.
        /// </summary>
        /// <param name="result"></param>
        private void _solver_IntermidiateResult(TSolution result)
        {
            if (this.IntermidiateResult != null)
            { // yes, there is a listener that cares!
                this.IntermidiateResult(result);
            }
        }
    }
}