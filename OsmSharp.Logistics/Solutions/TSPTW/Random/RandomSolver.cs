using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Logistics.Solutions.TSPTW.Random
{
    class RandomSolver : TSP.Random.RandomSolver, ISolver<ITSPTW, ITSPTWObjective, IRoute>
    {
        public IRoute Solve(ITSPTW problem, ITSPTWObjective objective)
        {
            return base.Solve(problem, objective);
        }

        public IRoute Solve(ITSPTW problem, ITSPTWObjective objective, out double fitness)
        {
            return base.Solve(problem, objective, out fitness);
        }
    }
}
