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

using Itinero.Logistics.Logging;
using Itinero.Logistics.Routes;
using Itinero.Logistics.Solutions.TSP.GA.Operators;
using Itinero.Logistics.Solutions.TSP.LocalSearch;
using Itinero.Logistics.Solvers;
using Itinero.Logistics.Solvers.GA;

namespace Itinero.Logistics.Solutions.TSP.GA.EAX
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    public class EAXSolver<T> : GASolver<T, ITSP<T>, TSPObjective<T>, IRoute, float>
        where T : struct
    {
        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(GASettings settings)
            : base(new MinimumWeightObjective<T>(), new HillClimbing3OptSolver<T>(), new EAXOperator<T>(30, EAXOperator<T>.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom, true),
            new TournamentSelectionOperator<ITSP<T>, IRoute, TSPObjective<T>, float>(10, 0.5), new EmptyOperator<T, ITSP<T>, TSPObjective<T>, IRoute, float>(), settings)
        {

        }

        /// <summary>
        /// Creates a new EAX-solver.
        /// </summary>
        public EAXSolver(GASettings settings, IOperator<T, ITSP<T>, TSPObjective<T>, IRoute, float> mutation)
            : base(new MinimumWeightObjective<T>(), new HillClimbing3OptSolver<T>(), new EAXOperator<T>(30, EAXOperator<T>.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom, true),
            new TournamentSelectionOperator<ITSP<T>, IRoute, TSPObjective<T>, float>(10, 0.5), mutation, settings)
        {

        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSP<T> problem, TSPObjective<T> objective, out float fitness)
        {
            if(problem.Weights.Length < 5)
            { // use brute-force solver when problem is very small.
                var bruteForceSolver = new BruteForceSolver<T>();
                return bruteForceSolver.Solve(problem, objective, out fitness);
            }

            if (problem.Last == null)
            { // the problem is 'open', we need to convert the problem and then solution.
                Logging.Logger.Log("EAXSolver.Solve", Logging.TraceEventType.Warning,
                    string.Format("EAX cannot solver 'open' TSP's: converting problem to a closed equivalent."));

                var convertedProblem = problem.ToClosed();
                var solution = base.Solve(convertedProblem, objective, out fitness);
                var route = new Logistics.Routes.Route(solution, null);
                fitness = 0;
                foreach(var pair in route.Pairs())
                {
                    fitness = fitness + problem.WeightHandler.GetTime(problem.Weights[pair.From][pair.To]);
                }
                return route;
            }
            else if(problem.First != problem.Last)
            { // the problem is 'closed' but has a fixed end point.
                Logger.Log("EAXSolver.Solve", Logging.TraceEventType.Warning,
                    string.Format("EAX cannot solver 'closed' TSP's with a fixed endpoint: converting problem to a closed equivalent."));

                var convertedProblem = problem.ToClosed();
                var convertedRoute = base.Solve(convertedProblem, objective, out fitness);
                convertedRoute.InsertAfter(System.Linq.Enumerable.Last(convertedRoute), problem.Last.Value);
                var route = new Logistics.Routes.Route(convertedRoute, problem.Last.Value);
                fitness = 0;
                foreach (var pair in route.Pairs())
                {
                    fitness = fitness + problem.WeightHandler.GetTime(problem.Weights[pair.From][pair.To]);
                }
                return route;
            }
            return base.Solve(problem, objective, out fitness);
        }
    }
}