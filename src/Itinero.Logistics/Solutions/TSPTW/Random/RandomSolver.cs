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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solvers;

namespace Itinero.Logistics.Solutions.TSPTW.Random
{
    class RandomSolver<T> : TSP.Random.RandomSolver<T>, ISolver<T, ITSPTW<T>, TSPTWObjective<T>, IRoute, float>
        where T : struct
    {
        public IRoute Solve(ITSPTW<T> problem, TSPTWObjective<T>  objective)
        {
            return base.Solve(problem, new TSPObjectiveWrapper<T>(objective));
        }

        public IRoute Solve(ITSPTW<T> problem, TSPTWObjective<T>  objective, out float fitness)
        {
            var solution = base.Solve(problem, new TSPObjectiveWrapper<T>(objective));
            fitness = objective.Calculate(problem, solution);
            return solution;
        }
    }
}
