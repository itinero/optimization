// Itinero.Optimization - Route optimization for .NET
// Copyright (C) 2017 Abelshausen Ben
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

using System;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.Sequence.Directed.Solver
{
    /// <summary>
    /// A heuristic to construct a decent solution to the best turns in a directed sequence.
    /// </summary>
    public sealed class ConstructionSolver : SolverBase<float, SequenceDirectedProblem, SequenceDirectedObjective, Tour, float>
    {
        private readonly SequenceDirectedProblem _problem;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public ConstructionSolver(SequenceDirectedProblem problem)
        {
            _problem = problem;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get { return "CONST"; }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override Tour Solve(SequenceDirectedProblem problem, SequenceDirectedObjective objective, out float fitness)
        {
            var tour = new Tour(problem.Sequence, problem.Sequence[problem.Sequence.Length - 1]);
            fitness = objective.Calculate(problem, tour);


        }
    }
}